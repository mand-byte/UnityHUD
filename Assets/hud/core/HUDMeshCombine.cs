using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{

    internal class HUDMeshCombine : HUDMesh
    {
        BetterList<HUDMeshSingle> _meshs;
        public BetterList<HUDMeshSingle> Meshs
        {
            get
            {
                if (_meshs == null)
                {
                    _meshs = new BetterList<HUDMeshSingle>();
                }
                return _meshs;
            }
        }
        public override void Release()
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].Release();
                }
            }
            _valid = false;

        }

        protected override void UpdateOffset(Vector2 offset)
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].Offset = offset;
                }
            }
        }

        protected override void UpdatePos(Vector3 role)
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].RolePos = role;
                }
            }
        }
        protected override void UpdateColor(Color32 c)
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].mColor = c;
                }
            }
        }

        protected override void UpdateScale(float c)
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].Scale = c;
                }
            }
        }
        public override void Rebuild()
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].Rebuild();
                }
            }
        }
        public override void FillMeshData(List<MeshData> meshDatas)
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].FillMeshData(meshDatas);
                }
            }
        }
    }
}