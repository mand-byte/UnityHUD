using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameHUD
{
    internal sealed class HUDBloodMesh : HUDMeshSingle
    {
        HUDBloodInfoObject info;
        Vector2 bd_offset;
        public void PushValue(float value)
        {
            if (!_valid)
            {
                return;
            }
            HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out MaterialIndex, info.Blood, bd_offset, value, info.BloodWidth, info.BloodHeight, info.Reverse, info.SliceValue, info.SliceType, info.Align);
            HUDManager.Instance.Dirty=true;
        }
        public void Create(HUDBloodInfoObject _info, Vector3 rolepos, Vector3 roleoffset)
        {
            if (_valid)
            {
                Release();
            }
            info = _info;
            _valid = true;
            for (int i = 0; i < 3; i++)
            {
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
            }
            //计算血条背景
            Size = HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out MaterialIndex, info.BloodBg, Vector2.zero, 1f, info.BloodWidthBG, info.BloodHeightBG, info.Reverse, info.SliceBGValue, info.SliceType, info.Align);
          
            for (int i = 0; i < 3; i++)
            {
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                m_SpriteVertex.Add(vertex);
            }
            bd_offset = info.BloodOffset;
            if (info.Align.Equals(AlignmentEnum.Middle))
            {
                bd_offset.Set(bd_offset.x - info.BloodWidthBG / 2 + info.BloodWidth / 2, bd_offset.y);
            }
            else if (info.Align.Equals(AlignmentEnum.Right))
            {
                bd_offset.Set(bd_offset.x - info.BloodWidthBG + info.BloodWidth, bd_offset.y);
            }
            HUDStringParser.PasreSlicedFillSprite(m_SpriteVertex, out MaterialIndex, info.Blood, bd_offset, 1f, info.BloodWidth, info.BloodHeight, info.Reverse, info.SliceValue, info.SliceType, info.Align);

            _RoleOffset = new Vector3(0, roleoffset.y, 0);
            _rolePos = rolepos;
            HUDManager.Instance.Dirty=true;
        }

    }
}