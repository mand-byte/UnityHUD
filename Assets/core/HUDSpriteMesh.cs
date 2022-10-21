using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameHUD
{
    internal sealed class HUDSpriteMesh : HUDMeshSingle
    {
        string str; AlignmentEnum alignmentEnum;
        int width; int height; HUDVector4Int border;
        bool IsSlice;
        public void PushSprite(string str, Vector3 rolepos, Vector2 offset, AlignmentEnum alignmentEnum)
        {
            if (_valid)
            {
                Release();
            }
            this.str = str;
            _valid = true;
            IsSlice = false;
            this.alignmentEnum = alignmentEnum;
            Rebuild();
            _offset = offset;
            _rolePos = rolepos;
        }

        public void PushSliceSprite(string str, Vector3 rolepos, Vector2 offset, int width, int height, HUDVector4Int border, AlignmentEnum alignmentEnum)
        {
            if (_valid)
            {
                Release();
            }
            this.str = str;
            _valid = true;
            this.width = width;
            this.height = height;
            this.border = border;
            this.alignmentEnum = alignmentEnum;
            IsSlice = true;
            for (int i = 0; i < 9; i++)
            {
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
            }
            _offset = offset;
            _rolePos = rolepos;
            Rebuild();
        }
        public override void Rebuild()
        {
            if (IsSlice)
            {
                Size = HUDStringParser.PasreSlicedSprite(m_SpriteVertex, out mMat, str, _offset, width, height, border, alignmentEnum);
            }
            else
            {
                Size = HUDStringParser.PasreSprite(m_SpriteVertex, out mMat, str, _offset, alignmentEnum);
            }
            Dirty = true;
        }

    }
}