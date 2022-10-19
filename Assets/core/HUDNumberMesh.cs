using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameHUD
{
    internal sealed class HUDNumberMesh : HUDMeshSingle
    {
        public void PushNumber(int number, char type, Vector2 offset)
        {
            _offset = offset;
            var size = HUDStringParser.PasrseNumber(m_SpriteVertex, type, number);
        }
        public override void Release()
        {
            base.Release();
            ObjectPool<HUDNumberMesh>.Push(this);
        }

    }
}