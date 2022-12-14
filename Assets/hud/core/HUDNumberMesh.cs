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
            HUDNumberConfig numconfig;
            int number;
            public void PushNumber(int number, int type)
            {
                this.number=number;
                numconfig = Config.NumberTypes[type];
                Rebuild();
            }
            public override void Rebuild()
            {
                if (_valid)
                {
                    Release();
                }
                Size = HUDStringParser.PasrseNumber(m_SpriteVertex, out MaterialIndex, numconfig.Perfixe, numconfig.NumbersGap, number, numconfig.Sign, numconfig.NumbersAlign);
                _valid = true;
            }

        }

        Vector2 _start_offset;
        float _all_endtime;
        HUDNumberConfig numconfig;
        AnimationCurve cCurve, sCurve, pCurve;
        float _cur_time;
        public void PushNumber(int number, int type, Vector3 rolePos)
        {
            _valid = true;
            _FollowRole = false;
            numconfig = Config.NumberTypes[type];
            cCurve = numconfig.ColorCurve;
            sCurve = numconfig.ScaleCurve;
            pCurve = numconfig.MoveCurve;
            _all_endtime = 0;
            if (cCurve.length > 0)
            {
                var Keyframe = cCurve[cCurve.length - 1];
                if (_all_endtime < Keyframe.time)
                {
                    _all_endtime = Keyframe.time;
                }
            }
            if (sCurve.length > 0)
            {
                var Keyframe = sCurve[sCurve.length - 1];
                if (_all_endtime < Keyframe.time)
                {
                    _all_endtime = Keyframe.time;
                }
            }
            if (pCurve.length > 0)
            {
                var Keyframe = pCurve[pCurve.length - 1];
                if (_all_endtime < Keyframe.time)
                {
                    _all_endtime = Keyframe.time;
                }
            }
            _all_endtime += Time.realtimeSinceStartup;
            _cur_time = Time.realtimeSinceStartup;

            var mesh = ObjectPool<HUDNumberMeshBase>.Pop();
            _start_offset = Vector2.zero;
            Meshs.Add(mesh);
            mesh.PushNumber(number, type);
            mesh.RolePos = rolePos;
            mesh.mColor = Color.white;
            mesh.Scale = 1;
            mesh.Offset = _start_offset;
            HUDManager.Instance.Dirty = true;
        }
        public override void UpdateLogic()
        {

            if (_all_endtime < Time.realtimeSinceStartup)
            {
                Release();
                return;
            }
            var d = Time.realtimeSinceStartup - _cur_time;
            var c = cCurve.Evaluate(d);
            Meshs[0].mColor = new Color(1, 1, 1, c);
            var o = pCurve.Evaluate(d);
            Meshs[0].Offset = _start_offset + new Vector2(0, o * HUDManager.PIXELS_PER_UNIT);
            var s = sCurve.Evaluate(d);
            Meshs[0].Scale = s;

        }
        public override void Release()
        {
            base.Release();
            ObjectPool<HUDNumberMeshBase>.Push(Meshs[0] as HUDNumberMeshBase);
            Meshs.Clear();
        }
    }
}