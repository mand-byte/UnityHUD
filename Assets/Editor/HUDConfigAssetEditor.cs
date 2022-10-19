using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.U2D;

#if UNITY_EDITOR
using UnityEditor;
namespace GameHUD
{
    public class HUDConfigAssetEditor
    {
        [MenuItem("Tools/CreateHUDConfigAsset")]
        public static void CreateHUDConfigAsset()
        {
            var fromPath = EditorUtility.SaveFilePanelInProject("select a directory folder for saving the file", "", "asset", "");
            if (!string.IsNullOrEmpty(fromPath))
            {
                var asset = ScriptableObject.CreateInstance<HUDConfigObject>();
                UnityEditor.AssetDatabase.CreateAsset(asset, fromPath);
                UnityEditor.AssetDatabase.Refresh();
            }
        }
        static int _max_size = 2048;
        static int _padding = 1;
        [MenuItem("Tools/CreateAtlas")]
        public static void CreateAtlas()
        {
            var fromPath = EditorUtility.OpenFolderPanel("select a directory folder for reading picture files", "", "");
            if (!string.IsNullOrEmpty(fromPath))
            {
                var files = System.IO.Directory.GetFiles(fromPath, "*.png");
                List<Texture2D> list = new List<Texture2D>();
                for (int i = 0; i < files.Length; i++)
                {

                    var f = files[i].Replace("\\", "/").Replace(Application.dataPath, "");
                    f = "Assets" + f;
                    var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(f);
                    if (tex != null)
                    {
                        list.Add(tex);
                    }
                    else
                    {
                        Debug.LogWarning($"AssetDatabase.LoadAssetAtPath path={f} failed! check it!");
                    }

                }
                var atlasTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                SetTexture(list, true);
                Rect[] aSpriteRect = atlasTex.PackTextures(list.ToArray(), _padding, _max_size, false);
                atlasTex.Apply();
                SetTexture(list, false);
                if (aSpriteRect.Length != list.Count)
                {
                    Debug.LogError($"Atlas size is bigger than {_max_size} , check it !!");
                    return;
                }
                var dir = EditorUtility.SaveFilePanelInProject("select a directory folder for saving the atlas file", "", "png", "");
                if (string.IsNullOrEmpty(dir))
                {
                    return;
                }
                var bytes = atlasTex.EncodeToPNG();
                //生成png图片
                File.WriteAllBytes(dir, bytes);
                //修改png贴图设置
                AssetDatabase.ImportAsset(dir);
                var combintex = AssetImporter.GetAtPath(dir) as TextureImporter;
                combintex.textureType = TextureImporterType.Sprite;
                combintex.mipmapEnabled = true;
                combintex.spritePixelsPerUnit = 1;
                combintex.SaveAndReimport();

                var info = new SpriteInfo[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    info[i] = new SpriteInfo();
                    info[i].Name = list[i].name;
                    info[i].xMax = aSpriteRect[i].xMax;
                    info[i].xMin = aSpriteRect[i].xMin;
                    info[i].yMax = aSpriteRect[i].yMax;
                    info[i].yMin = aSpriteRect[i].yMin;
                    info[i].Width = list[i].width;
                    info[i].Height = list[i].height;
                }
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer = new();
                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                serializer.Serialize(memStream, info);
                memStream.Flush();
                var info_path = Path.ChangeExtension(dir, "txt");
                //生成图集配置文件,材质球暂时在运行时动态生成
                File.WriteAllBytes(info_path, memStream.ToArray());
                UnityEditor.AssetDatabase.Refresh();

                //测试反序列化是否成功
                // var fr= File.OpenRead(info_path);
                // var sis=   serializer.Deserialize(fr) as SpriteInfo[];
                // Debug.LogError(sis.Length);
            }
            //设置贴图是否可读
            void SetTexture(List<Texture2D> list, bool isReadable)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var p = AssetDatabase.GetAssetPath(list[i]);
                    var tex_imp = AssetImporter.GetAtPath(p) as TextureImporter;
                    tex_imp.isReadable = isReadable;
                    tex_imp.SaveAndReimport();
                }
            }
        }
    }
}


#endif
