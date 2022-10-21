using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{
    internal abstract class HUDMesh : HUDMeshInterface
    {
        protected bool _FollowRole = true;
        public bool FollowRole
        {
            get
            {
                return _FollowRole;
            }
        }
        //原始尺寸
        public Vector2Int Size;
        //记录需要偏移的坐标
        protected Vector2 _offset;
        public int ItemLineGap;
        public Vector2 Offset
        {
            get
            {
                return _offset;
            }
            set
            {

                UpdateOffset(value);

            }
        }
        protected Vector3 _rolePos;
        public Vector3 RolePos
        {
            set
            {
                if (_dirty)
                {
                    _rolePos = value;
                }
                else
                {
                    UpdatePos(value);
                }
            }
            get
            {
                return _rolePos;
            }
        }
        protected Color32 _color;
        public Color32 mColor
        {
            set
            {
                UpdateColor(value);
            }
            get
            {
                return _color;
            }
        }
        public float Scale
        {
            set
            {
                if (_dirty)
                {
                    _Scale = value;
                }
                else
                {
                    UpdateScale(value);
                }
            }
            get
            {
                return _Scale;
            }
        }
        protected float _Scale = 1f;
        protected bool _valid;
        public bool IsValid
        {
            get
            {
                return _valid;
            }
        }
        bool _dirty;
        ///<summary>
        ///Dirty为true会导致顶点 uv offset color 索引全部更新,
        ///如仅须更新顶点坐标,偏移值 就直接修改_rolePos,_offset
        ///</summary>
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


        protected abstract void UpdateOffset(Vector2 offset);


        protected abstract void UpdatePos(Vector3 role);
        protected abstract void UpdateColor(Color32 c);
        protected abstract void UpdateScale(float c);

        public virtual void UpdateLogic()
        {

        }
        public virtual void Rebuild()
        {

        }

    }

}