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
        public int MaterialIndex;
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
                HUDManager.Instance.Dirty = true;
                UpdateOffset(value);

            }
        }
        protected Vector3 _RoleOffset;
        public Vector3 RoleOffset
        {
            get
            {
                return _RoleOffset;
            }
        }
        protected Vector3 _rolePos;
        public Vector3 RolePos
        {
            set
            {
                HUDManager.Instance.Dirty = true;
                UpdatePos(value);

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
                HUDManager.Instance.Dirty = true;
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
                HUDManager.Instance.Dirty = true;
                UpdateScale(value);
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
            ItemLineGap = 0;
            _RoleOffset = Vector3.zero;
            HUDManager.Instance.Dirty=true;
        }

        protected abstract void UpdateOffset(Vector2 offset);


        protected abstract void UpdatePos(Vector3 role);
        protected abstract void UpdateColor(Color32 c);
        protected abstract void UpdateScale(float c);

        public virtual void UpdateLogic()
        {

        }
        //用于font的贴图重建时,需要重新获取uv并绘制
        public virtual void Rebuild()
        {

        }
        public abstract void FillMeshData(List<MeshData> meshDatas);

    }

}