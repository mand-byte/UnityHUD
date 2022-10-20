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
            _offset = offset;
            this.alignmentEnum = alignmentEnum;
            Rebuild();
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
            IsSlice = true;
            _offset = offset;
            this.width = width;
            this.height = height;
            this.border = border;
            this.alignmentEnum = alignmentEnum;
            if (IsSlice)
            {
                for (int i = 0; i < 9; i++)
                {
                    var vertex = ObjectPool<HUDVertex>.Pop();
                    vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                    m_SpriteVertex.Add(vertex);
                }
            }
            else
            {
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
            }
            Rebuild();
            _rolePos = rolepos;
        }
        public override void Rebuild()
        {
            if (IsSlice)
            {
                int Length = m_SpriteVertex.size;
                for (int i = 9; i > 0; i--)
                {
                    var vertex = m_SpriteVertex[Length - i];
                    if (alignmentEnum.Equals(AlignmentEnum.Middle))
                    {
                        vertex.Offset.Set(_offset.x - width / 2, _offset.y);
                    }
                    else if (alignmentEnum.Equals(AlignmentEnum.Right))
                    {
                        vertex.Offset.Set(_offset.x - width, _offset.y);
                    }
                    else
                    {
                        vertex.Offset.Set(_offset.x, _offset.y);
                    }
                }
                Size = HUDStringParser.PasreSlicedSprite(m_SpriteVertex, out mMat, str, Vector2.zero, width, height, border, alignmentEnum);
            }
            else
            {
                var vertex = m_SpriteVertex[m_SpriteVertex.size - 1];
                if (alignmentEnum.Equals(AlignmentEnum.Middle))
                {
                    vertex.Offset.Set(_offset.x - width / 2, _offset.y);
                }
                else if (alignmentEnum.Equals(AlignmentEnum.Right))
                {
                    vertex.Offset.Set(_offset.x - width, _offset.y);
                }
                else
                {
                    vertex.Offset.Set(_offset.x, _offset.y);
                }
                Size = HUDStringParser.PasreSprite(m_SpriteVertex, out mMat, str, Vector2.zero, alignmentEnum);
            }
            Dirty = true;
        }

    }
}