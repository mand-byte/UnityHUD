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
        Vector2 uioffset;
        Vector3 role_offset;
        public void PushSprite(string str, Vector3 rolepos, Vector2 roleoffset, Vector2 uioffset, AlignmentEnum alignmentEnum)
        {
            this.str = str;
            IsSlice = false;
            this.alignmentEnum = alignmentEnum;
            _rolePos = rolepos;
            this.role_offset = new Vector3(0, roleoffset.y, 0);
            this.uioffset = uioffset + new Vector2(0, ItemLineGap);
            Rebuild();
        }

        public void PushSliceSprite(string str, Vector3 rolepos, Vector2 roleoffset, Vector2 uioffset, int width, int height, HUDVector4Int border, AlignmentEnum alignmentEnum)
        {

            this.str = str;
            this.width = width;
            this.height = height;
            this.border = border;
            this.alignmentEnum = alignmentEnum;
            IsSlice = true;
            this.role_offset = new Vector3(0, roleoffset.y, 0);
            this.uioffset = uioffset + new Vector2(0, ItemLineGap);
            _rolePos = rolepos;
            Rebuild();
        }
        public override void Rebuild()
        {
            if (_valid)
            {
                Release();
            }
            if (IsSlice)
            {
                for (int i = 0; i < 9; i++)
                {
                    var vertex = ObjectPool<HUDVertex>.Pop();
                    vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                    m_SpriteVertex.Add(vertex);
                }
                Size = HUDStringParser.PasreSlicedSprite(m_SpriteVertex, out MaterialIndex, str, Vector2.zero, width, height, border, alignmentEnum);
            }
            else
            {
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
                Size = HUDStringParser.PasreSprite(m_SpriteVertex, out MaterialIndex, str, Vector2.zero, alignmentEnum);
            }
            Offset = this.uioffset;
            _RoleOffset = this.role_offset;
        }

    }
}