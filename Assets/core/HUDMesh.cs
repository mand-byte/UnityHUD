using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{
    internal abstract class HUDMesh : HUDMeshInterface
    {
        //原始尺寸
        public Vector2Int Size;
        //记录需要偏移的坐标
        protected Vector2 _offset;
        public int ItemLineGap;
        public Vector2 Offset => _offset;
        protected Vector3 RolePos;
        public float Scale = 1f;
        protected bool _valid;
        public bool IsValid
        {
            get
            {
                return _valid;
            }
        }
        bool _dirty;
        public bool Dirty
        {
            get
            {
                return _dirty;
            }
            set
            {
                _dirty = value;
            }
        }
        HUDConfigObject _config;
        protected HUDConfigObject Config
        {
            get
            {
                if (_config == null)
                {
                    _config = HUDManager.Instance.Config;
                }
                return _config;
            }
        }
        public virtual void Release()
        {
            _valid = false;
        }
        public abstract void UpdateMesh();


        public abstract void RenderTo(CommandBuffer cmdBuffer);


        public abstract void UpdateOffset(Vector2 offset);


        public abstract void UpdatePos(Vector3 role);
        public virtual void UpdateLogic(){

        }
        public virtual void Rebuild(){
            
        }

    }

}