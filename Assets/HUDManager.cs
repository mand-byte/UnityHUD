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
        public static readonly int PIXELS_PER_UNIT=100;
        BetterList<HUDInfo> _cache_info_list = new BetterList<HUDInfo>();
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
                if (_cmdbuff == null)
                {
                    _cmdbuff = new CommandBuffer();
                }
                return _cmdbuff;
            }

        }
        BetterList<Material> _mats;
        public Material[] Mats
        {
            get
            {
                return _mats.buffer;
            }
        }
        Dictionary<string, SpriteInfo> _sprite_dict = new Dictionary<string, SpriteInfo>();
        internal bool Dirty;
        bool ForceRefresh;

        void LateUpdate()
        {
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
                CMDbuff.Clear();
                for (int i = 0; i < _cache_info_list.size; i++)
                {
                    _cache_info_list[i].Object.RenderTo(CMDbuff);
                }
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
                Font.textureRebuilt += _instance.OnFontTextureChange;
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
                InitFont();
                InitAtlas();
            }

            _configObject.NameRelationDict?.Clear();
            for (int i = 0; i < _configObject.NameRelationArray.Length; i++)
            {
                var info = _configObject.NameRelationArray[i];
                _configObject.NameRelationDict[info.Relation] = info;
            }
            _configObject.GuildRelationDict.Clear();
            if (_configObject.GuildRelationArray != null)
                for (int i = 0; i < _configObject.GuildRelationArray.Length; i++)
                {
                    var info = _configObject.GuildRelationArray[i];
                    _configObject.GuildRelationDict[info.Relation] = info;
                }
            _configObject.BloodRelationDict?.Clear();
            if (_configObject.BloodRelationArray != null)
                for (int i = 0; i < _configObject.BloodRelationArray.Length; i++)
                {
                    var info = _configObject.BloodRelationArray[i];
                    _configObject.BloodRelationDict[info.Relation] = info;
                }
            _configObject.TitleRelationDict.Clear();
            if (_configObject.TitleInfoArray != null)
                for (int i = 0; i < _configObject.TitleInfoArray.Length; i++)
                {
                    var info = _configObject.TitleInfoArray[i];
                    _configObject.TitleRelationDict[info.Relation] = info;
                }
            ForceRefresh = true;
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
                m_mat.SetTexture("_MainTex", _configObject.Atlas[i]);
                m_mat.SetFloat("_UnitPerPixel",1f/PIXELS_PER_UNIT);
                for (int j = 0; j < atlas.Length; j++)
                {
                    atlas[j].Mat = m_mat;
                    atlas[j].MatIndex = i + 1;
                    _sprite_dict[atlas[j].Name] = atlas[j];
                }
                _mats.Add(m_mat);
            }
        }
        void OnFontTextureChange(Font f)
        {
            if (f.name.Equals(Config.Font.name))
            {
                ForceRefresh = true;
            }
        }

        public void InitFont()
        {
            if (_mats == null)
            {
                _mats = new BetterList<Material>();
            }
            else
            {
                for (int i = 0; i < _mats.size; i++)
                {
                    UnityEngine.Object.Destroy(_mats[i]);
                }
                _mats.Clear();
            }
            var sh = Shader.Find("Unlit/HUDFont");
            var _font_mat = new Material(sh);
            _mats.Add(_font_mat);
            _font_mat.SetFloat("_UnitPerPixel",1f/PIXELS_PER_UNIT);
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