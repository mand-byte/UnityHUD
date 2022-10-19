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
        public override void RenderTo(CommandBuffer cmdBuffer)
        {

            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].RenderTo(cmdBuffer);
                }
            }
        }

        public override void UpdateMesh()
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].UpdateMesh();
                }
            }
        }


        public override void UpdateOffset(Vector2 offset)
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].UpdateOffset(offset);
                }
            }
        }

        public override void UpdatePos(Vector3 role)
        {
            if (_meshs != null)
            {
                for (int i = 0; i < _meshs.size; i++)
                {
                    _meshs[i].UpdatePos(role);
                }
            }
        }
    }
}