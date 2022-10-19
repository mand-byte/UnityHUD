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
        public void PushSprite(string str, Vector2 offset, AlignmentEnum alignmentEnum)
        {
            if (_valid)
            {
                Release();
            }
            this.str=str;
            _valid = true;
            IsSlice = false;
            _offset = offset;
            this.alignmentEnum=alignmentEnum;
            Rebuild();
        }

        public void PushSliceSprite(string str, Vector2 offset, int width, int height, HUDVector4Int border, AlignmentEnum alignmentEnum)
        {
            if (_valid)
            {
                Release();
            }
            this.str=str;
            _valid = true;
            IsSlice = true;
            _offset = offset;
            this.width=width;
            this.height=height;
            this.border=border;
            this.alignmentEnum=alignmentEnum;
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