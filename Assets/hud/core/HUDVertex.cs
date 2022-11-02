using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameHUD
{

    internal class HUDVertex
    {

        public Vector2 vecLD; // 左下角
        public Vector2 vecLU; // 左上角
        public Vector2 vecRU; // 右上角
        public Vector2 vecRD; // 右下角
        public Vector2 uvLD;
        public Vector2 uvLU;
        public Vector2 uvRU;
        public Vector2 uvRD;

        public Color32 clrLD;
        public Color32 clrLU;
        public Color32 clrRU;
        public Color32 clrRD;
        
        
        public Vector2 Offset; // ui像素偏移
    }
}

