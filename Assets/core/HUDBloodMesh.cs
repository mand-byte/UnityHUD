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
            HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.Blood, boold_offset, value, info.BloodWidth, info.BloodHeight, info.Reverse, info.SliceValue, info.SliceType,info.Align);
            Dirty = true;
        }
        public void Create(HUDRelationEnum relationEnum, Vector2 offset)
        {
            if (_valid)
            {
                Release();
            }
            _relation = relationEnum;
            _valid = true;
            _offset = offset;
            if (!Config.BloodRelationDict.TryGetValue(relationEnum, out var info))
            {
                Debug.LogWarningFormat("HUDBloodMesh Not have HUDRelationEnum {0} config,check it out!", relationEnum.ToString());
                return;
            }
            //计算血条背景
            Size = HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.BloodBg, offset, 1f, info.BloodWidthBG, info.BloodHeightBG, info.Reverse, info.SliceBGValue, info.SliceType,info.Align);
            
            //计算血条
            boold_offset = offset+info.BloodOffset;
            HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out mMat, info.Blood, boold_offset, 1f, info.BloodWidth, info.BloodHeight, info.Reverse, info.SliceValue, info.SliceType,info.Align);
            Dirty = true;
        }

    }
}