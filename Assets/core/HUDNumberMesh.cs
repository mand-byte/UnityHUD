using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{

    internal sealed class HUDNumberMesh : HUDMeshCombine
    {
        private class HUDNumberMeshBase : HUDMeshSingle
        {
            public void PushNumber(int number, HudNumberType type, Vector2 offset)
            {

                _offset = offset * HUDObject.OFFSETSCALE;
                HUDNumberConfig numconfig = Config.NumberTypeDict[type];
                Size = HUDStringParser.PasrseNumber(m_SpriteVertex, out mMat, numconfig.Perfixe, numconfig.NumbersGap, number,numconfig.Sign);
                for (int i = 0; i < m_SpriteVertex.size; i++)
                {
                    if (numconfig.NumbersAlign.Equals(AlignmentEnum.Left))
                        m_SpriteVertex[i].Offset = new Vector2(_offset.x, _offset.y);
                    else if (numconfig.NumbersAlign.Equals(AlignmentEnum.Middle))
                        m_SpriteVertex[i].Offset = new Vector2(_offset.x - Size.x / 2, _offset.y);
                    else
                        m_SpriteVertex[i].Offset = new Vector2(_offset.x - Size.x, _offset.y);
                }
                _valid = true;
                Dirty = true;
            }
        }

        Vector2 _endposoffset, _start_offset;
        float _all_endtime;
        float _alpha_starttime, _move_endtime;
        HUDNumberConfig numconfig;
        float _temp_move_dtime, _temp_color_dtime;
        public void PushNumber(int number, HudNumberType type, Vector3 rolePos, Vector2 offset)
        {
            _valid = true;
            _FollowRole = false;
            numconfig = Config.NumberTypeDict[type];
            var mesh = ObjectPool<HUDNumberMeshBase>.Pop();
            Meshs.Add(mesh);
            mesh.PushNumber(number, type, offset);
            mesh.RolePos = rolePos;
            _start_offset = offset * HUDObject.OFFSETSCALE;
            _endposoffset = _start_offset + numconfig.MoveVect;
            mesh.mColor = Color.white;
            _alpha_starttime = numconfig.MoveTime + Time.realtimeSinceStartup;
            Dirty = true;
            _move_endtime = Time.realtimeSinceStartup + numconfig.MoveTime;
            _all_endtime = Time.realtimeSinceStartup + numconfig.MoveTime + numconfig.ColorTime;
            _temp_move_dtime = 0;
            _temp_color_dtime = 0;
        }
        public override void UpdateLogic()
        {
            if (Time.realtimeSinceStartup <= _move_endtime)
            {
                _temp_move_dtime += Time.deltaTime;
                Meshs[0].Offset = Vector2.Lerp(_start_offset, _endposoffset, _temp_move_dtime / numconfig.MoveTime);
            }
            else if (numconfig.ColorTime > 0 && _alpha_starttime <= Time.realtimeSinceStartup && Time.realtimeSinceStartup <= _all_endtime)
            {
                _temp_color_dtime += Time.deltaTime;
                Meshs[0].mColor = Color32.Lerp(Color.white, numconfig.Color, _temp_color_dtime / numconfig.ColorTime);
            }
            if (_all_endtime < Time.realtimeSinceStartup)
            {
                Release();
            }
        }
        public override void Release()
        {
            base.Release();
            ObjectPool<HUDNumberMeshBase>.Push(Meshs[0] as HUDNumberMeshBase);
            Meshs.Clear();
        }
    }
}