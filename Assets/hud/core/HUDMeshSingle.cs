using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{
    internal class MeshData : System.IDisposable
    {
        public MeshData()
        {
            mMesh = new Mesh();
            mMesh.hideFlags = HideFlags.DontSave;
            mMesh.name = "hud_mesh";
            mMesh.MarkDynamic();
            mMesh.Optimize();
            mVerts = new BetterList<Vector3>();
            mOffset = new BetterList<Vector2>();
            mUvs = new BetterList<Vector2>();
            mCols = new BetterList<Color32>();
            mIndices = new BetterList<int>();
        }
        public BetterList<Vector3> mVerts;
        //ui偏移
        public BetterList<Vector2> mOffset;
        public BetterList<Vector2> mUvs;
        public BetterList<Color32> mCols;
        public BetterList<int> mIndices;
        public Material mMat;
        public Mesh mMesh;
        public void Clear()
        {
            mMesh.Clear();
            mVerts.Clear();
            mUvs.Clear();
            mCols.Clear();
            mIndices.Clear();
            mOffset.Clear();
        }
        public void Dispose()
        {
            Object.Destroy(mMesh);
            Object.Destroy(mMat);
            mMesh = null;
            mMat = null;
            mVerts = null;
            mOffset = null;
            mUvs = null;
            mCols = null;
            mIndices = null;
        }
    }
    internal class HUDMeshSingle : HUDMesh
    {

        protected BetterList<HUDVertex> m_SpriteVertex = new BetterList<HUDVertex>();

        public override void Release()
        {
            base.Release();
            _offset = Vector2.zero;
            for (int i = 0; i < m_SpriteVertex.size; i++)
            {
                m_SpriteVertex[i].Offset = Vector2.zero;
                ObjectPool<HUDVertex>.Push(m_SpriteVertex[i]);
            }
            m_SpriteVertex.Clear();
            _valid = false;
        }

        //更新角色世界坐标
        protected override void UpdatePos(Vector3 role)
        {
            _rolePos = role;
        }
        protected override void UpdateColor(Color32 c)
        {
            for (int i = 0; i < m_SpriteVertex.size; i++)
            {
                m_SpriteVertex[i].clrLD = c;
                m_SpriteVertex[i].clrLU = c;
                m_SpriteVertex[i].clrRD = c;
                m_SpriteVertex[i].clrRU = c;
            }

        }
        // //更新缩放
        protected override void UpdateScale(float scale)
        {
            this._Scale = scale;
        }
        //更新因其他hud显示或隐藏 要改变此hud的偏移位置
        protected override void UpdateOffset(Vector2 off)
        {

            for (int i = 0, nSize = m_SpriteVertex.size; i < nSize; ++i)
            {
                HUDVertex v = m_SpriteVertex[i];
                v.Offset -= _offset;
                v.Offset += off;
            }
            _offset = off;
        }

        public override void FillMeshData(List<MeshData> meshDatas)
        {
            var data = meshDatas[MaterialIndex];
            Vector2 vOffset = Vector2.zero;
            var pos=_rolePos + _RoleOffset;
            for (int i = 0, nSize = m_SpriteVertex.size; i < nSize; ++i)
            {
                HUDVertex v = m_SpriteVertex[i];
                data.mVerts.Add(pos);
                data.mVerts.Add(pos);
                data.mVerts.Add(pos);
                data.mVerts.Add(pos);

                vOffset = v.vecRU;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                data.mOffset.Add(vOffset);

                vOffset = v.vecRD;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                data.mOffset.Add(vOffset);

                vOffset = v.vecLD;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                data.mOffset.Add(vOffset);

                vOffset = v.vecLU;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                data.mOffset.Add(vOffset);

                data.mUvs.Add(v.uvRU);
                data.mUvs.Add(v.uvRD);
                data.mUvs.Add(v.uvLD);
                data.mUvs.Add(v.uvLU);
                data.mCols.Add(v.clrRU);
                data.mCols.Add(v.clrRD);
                data.mCols.Add(v.clrLD);
                data.mCols.Add(v.clrLU);
            }
        }

    }
}