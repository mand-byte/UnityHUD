using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameHUD
{
    internal sealed class HUDBloodMesh : HUDMeshSingle
    {
        Vector2 boold_offset;
        HUDRelationEnum _relation;
        public void PushValue(float value)
        {
            if (!_valid)
            {
                return;
            }
            Config.BloodRelationDict.TryGetValue(_relation, out var info);
            HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.Blood, Vector2.zero, value, info.BloodWidth, info.BloodHeight, info.Reverse, info.SliceValue, info.SliceType, info.Align);
            Dirty = true;
        }
        public void Create(HUDRelationEnum relationEnum, Vector3 rolepos, Vector2 offset)
        {
            if (_valid)
            {
                Release();
            }
            _relation = relationEnum;
            Dirty = true;
            _valid = true;
            _offset = offset;
            _rolePos=rolepos;
            if (!Config.BloodRelationDict.TryGetValue(relationEnum, out var info))
            {
                Debug.LogWarningFormat("HUDBloodMesh Not have HUDRelationEnum {0} config,check it out!", relationEnum.ToString());
                return;
            }
            for (int i = 0; i < 3; i++)
            {

                var vertex = ObjectPool<HUDVertex>.Pop();
                if (info.Align.Equals(AlignmentEnum.Middle))
                {
                    vertex.Offset.Set(offset.x - info.BloodWidthBG / 2, offset.y);
                }
                else if (info.Align.Equals(AlignmentEnum.Right))
                {
                    vertex.Offset.Set(offset.x - info.BloodWidthBG, offset.y);
                }
                else
                {
                    vertex.Offset.Set(offset.x, offset.y);
                }
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
            }
            var list_count = m_SpriteVertex.size;
            //计算血条背景
            Size = HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.BloodBg, Vector2.zero, 1f, info.BloodWidthBG, info.BloodHeightBG, info.Reverse, info.SliceBGValue, info.SliceType, info.Align);

            //计算血条
            boold_offset = m_SpriteVertex[list_count - 1].Offset + info.BloodOffset;
            for (int i = 0; i < 3; i++)
            {
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.Offset = boold_offset;
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
            }
            list_count = m_SpriteVertex.size;
            HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.Blood, Vector2.zero, 1f, info.BloodWidth, info.BloodHeight, info.Reverse, info.SliceValue, info.SliceType, info.Align);

        }

    }
}