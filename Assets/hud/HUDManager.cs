#define OPEN_PROFILING
#define OPEN_DISTANCE_SORT
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Rendering;

namespace GameHUD
{
    public class HUDManager : MonoBehaviour
    {
        public static readonly int PIXELS_PER_UNIT = 100;
        BetterList<HUDInfo> _cache_info_list = new BetterList<HUDInfo>();
        List<MeshData> _all_meshdata = new List<MeshData>(4);
        private static HUDManager _instance;
        private HUDManager() { }
        public static HUDManager Instance => _instance;
        private HUDConfigObject _configObject;
        public HUDConfigObject Config
        {
            get
            {
                return _configObject;
            }
        }
        CommandBuffer _cmdbuff;
        public CommandBuffer CMDbuff
        {
            get
            {
                return _cmdbuff;
            }

        }
        bool active;
        ///<summary>
        ///是否显示所有hud,用于需要干净的场景时使用,比如过场动画等
        ///</summary>
        public bool Active
        {
            set
            {
                if (active.Equals(value))
                {
                    return;
                }
                if (value)
                {
                    Dirty = true;
                }
                active = value;
            }
            get
            {
                return active;
            }
        }
        Dictionary<string, SpriteInfo> _sprite_dict = new Dictionary<string, SpriteInfo>();
        internal bool Dirty;
        bool ForceRefresh;
        static Transform _cameraTrans;
        public static Transform CameraTrans => _cameraTrans;

        void CombineMeshAndCommit()
        {
            for (int i = 0; i < _all_meshdata.Count; i++)
            {
                _all_meshdata[i].Clear();
            }
#if OPEN_DISTANCE_SORT
            _cache_info_list.Sort((item1, item2) =>
            {
                if (item1.IsInited && item2.IsInited)
                {
                    return Vector3.Distance(_cameraTrans.position, item1.Trans.position) < Vector3.Distance(_cameraTrans.position, item2.Trans.position) ? 1 : -1;
                }
                else
                {
                    return 0;
                }
            });

#endif
            for (int i = 0; i < _cache_info_list.size; i++)
            {
                if (_cache_info_list[i].IsInited)
                    _cache_info_list[i].Object.FillMeshData(_all_meshdata);
            }
            for (int i = _all_meshdata.Count - 1; i >= 0; i--)
            {
                var meshdata = _all_meshdata[i];
                if (meshdata.mVerts.buffer == null || meshdata.mVerts.size == 0)
                {
                    continue;
                }
                int total = meshdata.mVerts.buffer.Length;
                int nLast = meshdata.mVerts.size - 1;
                int vertscount = meshdata.mVerts.size;
                if (meshdata.mVerts.size < meshdata.mVerts.buffer.Length)
                {
                    //修改buff数据
                    Vector3[] vers = meshdata.mVerts.buffer;
                    Vector2[] uv1s = meshdata.mUvs.buffer;
                    Vector2[] offs = meshdata.mOffset.buffer;
                    Vector2[] roleoffs = meshdata.mRole_XZ_Offset.buffer;
                    Color32[] cols = meshdata.mCols.buffer;
                    for (int k = meshdata.mVerts.size; k < total; ++k)
                    {
                        vers[k] = vers[nLast];
                        uv1s[k] = uv1s[nLast];
                        offs[k] = offs[nLast];
                        cols[k] = cols[nLast];
                        roleoffs[k]=roleoffs[nLast];
                    }
                    meshdata.mVerts.size = total;
                    meshdata.mUvs.size = total;
                    meshdata.mCols.size = total;
                    meshdata.mOffset.size = total;
                }
                meshdata.mIndices.CleanPreWrite(vertscount / 4 * 6);
                int j = 0, index = 0;
                int nMaxCount = meshdata.mIndices.buffer.Length;
                for (; j < vertscount; j += 4)
                {
                    meshdata.mIndices[index++] = j;
                    meshdata.mIndices[index++] = j + 1;
                    meshdata.mIndices[index++] = j + 2;

                    meshdata.mIndices[index++] = j + 2;
                    meshdata.mIndices[index++] = j + 3;
                    meshdata.mIndices[index++] = j;
                }
                nLast = vertscount - 1;
                for (; index < nMaxCount;)
                {
                    meshdata.mIndices[index++] = nLast;
                    meshdata.mIndices[index++] = nLast;
                    meshdata.mIndices[index++] = nLast;
                    meshdata.mIndices[index++] = nLast;
                    meshdata.mIndices[index++] = nLast;
                    meshdata.mIndices[index++] = nLast;
                }
                meshdata.mIndices.size = index;
                meshdata.mMesh.vertices = meshdata.mVerts.buffer;
                meshdata.mMesh.uv = meshdata.mUvs.buffer;
                meshdata.mMesh.uv2 = meshdata.mOffset.buffer;
                meshdata.mMesh.uv3 = meshdata.mRole_XZ_Offset.buffer;
                meshdata.mMesh.colors32 = meshdata.mCols.buffer;
                meshdata.mMesh.triangles = meshdata.mIndices.buffer;
                _cmdbuff.DrawMesh(meshdata.mMesh, Matrix4x4.identity, meshdata.mMat);
            }
        }
        void LateUpdate()
        {
            if (!active)
            {
                _cmdbuff.Clear();
                return;
            }
            for (int i = 0; i < _cache_info_list.size; i++)
            {
                _cache_info_list[i].Object.Update(ForceRefresh);
            }
            if (ForceRefresh)
            {
                ForceRefresh = false;
            }
            if (Dirty)
            {
                _cmdbuff.Clear();
#if OPEN_PROFILING
                UnityEngine.Profiling.Profiler.BeginSample("CombinMeshAndCommit");
#endif
                CombineMeshAndCommit();
#if OPEN_PROFILING
                UnityEngine.Profiling.Profiler.EndSample();
#endif
                Dirty = false;
            }
        }
        ///<summary>
        ///初始化hud管理器
        ///</summary>
        /// <param name="config">配置文件</param>
        /// <returns>void</returns>
        public static void Init(HUDConfigObject config)
        {
            if (config != null && _instance == null)
            {
                var go = new GameObject("HUDManager");
                GameObject.DontDestroyOnLoad(go);
                _instance = go.AddComponent<HUDManager>();
                _instance._cmdbuff = new CommandBuffer();
                Font.textureRebuilt += _instance.OnFontTextureChange;
                _instance.active = true;
                _instance.ChangeConfig(config);
            }
        }
        ///<summary>
        ///多套HUD配置互相切换
        ///</summary>
        public void ChangeConfig(HUDConfigObject config)
        {
            if (config != null)
            {
                if (_configObject != null)
                {
                    GameObject.Destroy(_configObject);
                }
                _configObject = config;
                _all_meshdata.Clear();
                for (int i = 0; i < Camera.allCameras.Length; i++)
                {
                    var camera = Camera.allCameras[i];
                    if (camera != null)
                    {
                        var data = camera.gameObject.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                        if (data != null)
                        {
                            if (data.renderType.Equals(UnityEngine.Rendering.Universal.CameraRenderType.Base))
                            {
                                _cameraTrans = camera.transform;
                                break;
                            }
                        }
                    }
                }
                if (_cameraTrans is null)
                {
                    Debug.LogWarning("can not find the main camera!");
                    return;
                }
                InitFont();
                InitAtlas();
                ForceRefresh = true;
            }
        }
        // ///<summary>
        // ///初始化图集
        // ///</summary>
        public void InitAtlas()
        {
            _sprite_dict.Clear();
            int min_count = _configObject.Atlas.Count <= _configObject.AtlasData.Count ? _configObject.Atlas.Count : _configObject.AtlasData.Count;
            BinaryFormatter bf = new BinaryFormatter();
            for (int i = 0; i < min_count; i++)
            {
                if (_configObject.AtlasData[i] == null || _configObject.Atlas[i] == null)
                {
                    Debug.LogError("HUDConfigObject Atlas or AtlasData is missing");
                    continue;
                }
                MemoryStream memoryStream = new MemoryStream(_configObject.AtlasData[i].bytes);
                var atlas = bf.Deserialize(memoryStream) as SpriteInfo[];
                memoryStream.Dispose();
                var tex = _configObject.Atlas[i];
                var m_mat = new Material(Shader.Find("Unlit/HUDSprite"));
                var data = new MeshData();
                data.mMat = m_mat;
                _all_meshdata.Add(data);
                m_mat.SetTexture("_MainTex", _configObject.Atlas[i]);
                m_mat.SetFloat("_UnitPerPixel", 1f / PIXELS_PER_UNIT);
                for (int j = 0; j < atlas.Length; j++)
                {
                    atlas[j].Mat = m_mat;
                    atlas[j].MatIndex = i + 1;
                    _sprite_dict[atlas[j].Name] = atlas[j];
                }
            }
        }
        void OnFontTextureChange(Font f)
        {
            if (Config != null && Config.Font != null && f.name.Equals(Config.Font.name))
            {
                ForceRefresh = true;
                Dirty = true;
            }
        }

        public void InitFont()
        {

            var sh = Shader.Find("Unlit/HUDFont");
            var _font_mat = new Material(sh);
            var data = new MeshData();
            data.mMat = _font_mat;
            _all_meshdata.Add(data);
            _font_mat.SetFloat("_UnitPerPixel", 1f / PIXELS_PER_UNIT);
            _font_mat.mainTexture = _configObject.Font.material.mainTexture;
            _font_mat.mainTextureOffset = _configObject.Font.material.mainTextureOffset;
            _font_mat.mainTextureScale = _configObject.Font.material.mainTextureScale;
        }
        public SpriteInfo GetSprite(string str)
        {
            if (_sprite_dict.TryGetValue(str, out var result))
            {
                return result;
            }
            Debug.LogWarningFormat("GetSprite  str={0} is not exist!!", str);
            return null;
        }
        public HUDInfo CreateHUD(long id)
        {
            for (int i = 0; i < _cache_info_list.size; i++)
            {
                if (_cache_info_list[i].id.Equals(id))
                {
                    Debug.LogWarning("createHUD repeated,check invoked code!");
                    return _cache_info_list[i];
                }
            }
            HUDInfo result = ObjectPool<HUDInfo>.Pop();
            var obj = ObjectPool<HUDObject>.Pop();
            result.Object = obj;
            result.id = id;
            _cache_info_list.Add(result);
            return result;
        }
        internal void DeleteHUD(long id)
        {
            for (int i = 0; i < _cache_info_list.size; i++)
            {
                var info = _cache_info_list[i];
                if (info.id.Equals(id))
                {
                    info.Object.Release();
                    ObjectPool<HUDObject>.Push(info.Object);
                    ObjectPool<HUDInfo>.Push(info);
                    _cache_info_list.RemoveAt(i);
                    break;
                }
            }
        }
    }

}