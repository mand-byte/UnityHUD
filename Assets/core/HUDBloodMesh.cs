using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameHUD
{
    internal sealed class HUDBloodMesh : HUDMeshSingle
    {
        HUDRelationEnum _relation;
        Vector2  bd_offset;
        public void PushValue(float value)
        {
            if (!_valid)
            {
                return;
            }
            Config.BloodRelationDict.TryGetValue(_relation, out var info);
            HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.Blood, bd_offset, value, info.BloodWidth, info.BloodHeight, info.Reverse, info.SliceValue, info.SliceType, info.Align);
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
            _rolePos = rolepos;
            if (!Config.BloodRelationDict.TryGetValue(relationEnum, out var info))
            {
                Debug.LogWarningFormat("HUDBloodMesh Not have HUDRelationEnum {0} config,check it out!", relationEnum.ToString());
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
            }
            //计算血条背景
            Size = HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.BloodBg, _offset, 1f, info.BloodWidthBG, info.BloodHeightBG, info.Reverse, info.SliceBGValue, info.SliceType, info.Align);


            for (int i = 0; i < 3; i++)
            {
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
            }
            bd_offset = _offset + info.BloodOffset;
            if (info.Align.Equals(AlignmentEnum.Middle))
            {
                bd_offset.Set(bd_offset.x - info.BloodWidthBG / 2 + info.BloodWidth / 2, bd_offset.y);
            }
            else if (info.Align.Equals(AlignmentEnum.Right))
            {
                bd_offset.Set(bd_offset.x - info.BloodWidthBG + info.BloodWidth, bd_offset.y);
            }
            HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.Blood, bd_offset, 1f, info.BloodWidth, info.BloodHeight, info.Reverse, info.SliceValue, info.SliceType, info.Align);

        }

    }
}