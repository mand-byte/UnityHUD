using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{
    internal class HUDMeshSingle : HUDMesh
    {
        protected BetterList<Vector3> mVerts = new BetterList<Vector3>();
        //相对人物偏移坐标 使用uv2
        protected BetterList<Vector2> mOffset = new BetterList<Vector2>();
        protected BetterList<Vector2> mUvs = new BetterList<Vector2>();
        protected BetterList<Color32> mCols = new BetterList<Color32>();
        protected BetterList<int> mIndices = new BetterList<int>();
        protected Material mMat;
        public Material Mat
        {
            get
            {
                return mMat;
            }
            set
            {
                mMat = value;
            }
        }
        protected Mesh mesh;
        public Mesh Mesh
        {
            get
            {
                return mesh;
            }
        }
        protected BetterList<HUDVertex> m_SpriteVertex = new BetterList<HUDVertex>();

        public override void Release()
        {
            mVerts.Clear();
            mUvs.Clear();
            mCols.Clear();
            mIndices.Clear();
            mOffset.Clear();
            mesh?.Clear();
            _offset = Vector2.zero;
            for (int i = 0; i < m_SpriteVertex.size; i++)
            {
                m_SpriteVertex[i].Offset = Vector2.zero;
                ObjectPool<HUDVertex>.Push(m_SpriteVertex[i]);
            }
            m_SpriteVertex.Clear();
            _valid = false;
        }
        public override void UpdateMesh()
        {
            int nOldVertexCount = mVerts.size;
            FillVertex();
            int nLast = mVerts.size - 1;
            int nExSize = mVerts.buffer.Length;
            int nVertexCount = mVerts.size;

            if (mVerts.size < mVerts.buffer.Length)
            {
                //修改buff数据
                Vector3[] vers = mVerts.buffer;
                Vector2[] uv1s = mUvs.buffer;
                Vector2[] offs = mOffset.buffer;
                Color32[] cols = mCols.buffer;
                for (int i = mVerts.size, iMax = mVerts.buffer.Length; i < iMax; ++i)
                {
                    vers[i] = vers[nLast];
                    uv1s[i] = uv1s[nLast];
                    offs[i] = offs[nLast];
                    cols[i] = cols[nLast];
                }
            }
            mVerts.size = nExSize;
            mUvs.size = nExSize;
            mCols.size = nExSize;
            mOffset.size = nExSize;
            bool rebuildIndices = nOldVertexCount != nExSize;
            if (rebuildIndices)
                AdjustIndexes(nVertexCount);
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.hideFlags = HideFlags.DontSave;
                mesh.name = "hud_mesh";
                mesh.MarkDynamic();
            }
            if (rebuildIndices || mesh.vertexCount != mVerts.size)
            {
                mesh.Clear();
            }
            mesh.vertices = mVerts.buffer;
            mesh.uv = mUvs.buffer;
            mesh.uv2 = mOffset.buffer;
            mesh.colors32 = mCols.buffer;
            mesh.triangles = mIndices.buffer;
            HUDManager.Instance.Dirty = true;
            Dirty = false;
        }
        void FillVertex()
        {
            PrepareWrite(m_SpriteVertex.size * 4);
            Vector2 vOffset = Vector2.zero;
            // Vector2 alignOffset = GetTxtAlign();
            for (int i = 0, nSize = m_SpriteVertex.size; i < nSize; ++i)
            {
                HUDVertex v = m_SpriteVertex[i];
                mVerts.Add(RolePos);
                mVerts.Add(RolePos);
                mVerts.Add(RolePos);
                mVerts.Add(RolePos);

                vOffset = v.vecRU;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecRD;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecLD;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecLU;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                mUvs.Add(v.uvRU);
                mUvs.Add(v.uvRD);
                mUvs.Add(v.uvLD);
                mUvs.Add(v.uvLU);
                mCols.Add(v.clrRU);
                mCols.Add(v.clrRD);
                mCols.Add(v.clrLD);
                mCols.Add(v.clrLU);
            }
        }

        void AdjustIndexes(int nVertexCount)
        {
            int nOldSize = mIndices.size;
            int nNewSize = mVerts.size / 4 * 6;
            mIndices.CleanPreWrite(nVertexCount / 4 * 6);
            // 填充多余的
            int nMaxCount = mIndices.buffer.Length;
            int[] Indices = mIndices.buffer;

            int index = 0;
            int i = 0;
            for (; i < nVertexCount; i += 4)
            {
                Indices[index++] = i;
                Indices[index++] = i + 1;
                Indices[index++] = i + 2;

                Indices[index++] = i + 2;
                Indices[index++] = i + 3;
                Indices[index++] = i;
            }
            int nLast = nVertexCount - 1;
            for (; index < nMaxCount;)
            {
                Indices[index++] = nLast;
                Indices[index++] = nLast;
                Indices[index++] = nLast;
                Indices[index++] = nLast;
                Indices[index++] = nLast;
                Indices[index++] = nLast;
            }
            mIndices.size = index;
        }
        protected void PrepareWrite(int nVertexNumb)
        {
            mVerts.CleanPreWrite(nVertexNumb);
            mOffset.CleanPreWrite(nVertexNumb);
            mUvs.CleanPreWrite(nVertexNumb);
            mCols.CleanPreWrite(nVertexNumb);

        }
        //更新角色世界坐标
        protected override void UpdatePos(Vector3 role)
        {
            for (int i = 0; i < mVerts.size; i++)
            {
                mVerts[i] = role;
            }
            _rolePos = role;
            if (mesh != null && mesh.vertexCount == mVerts.buffer.Length)
            {
                mesh.vertices = mVerts.buffer;
            }
        }
        protected override void UpdateColor(Color32 c)
        {
            for (int i = 0; i < mCols.size; i++)
            {
                mCols[i] = c;
            }
            _color = c;
            if (mesh != null && mesh.vertexCount == mCols.buffer.Length)
            {
                mesh.colors32 = mCols.buffer;
            }
        }
        // //更新缩放
        protected override void UpdateScale(float scale)
        {
            if (m_SpriteVertex.size == 0)
            {

                return;
            }
            this._Scale = scale;
            if (mesh == null)
            {
                //还未走 UpdateMesh 直接返回
                return;
            }
            Vector2 vOffset = Vector2.zero;
            mOffset.CleanPreWrite(mesh.vertexCount);
            for (int i = 0, nSize = m_SpriteVertex.size; i < nSize; ++i)
            {
                HUDVertex v = m_SpriteVertex[i];
                vOffset = v.vecRU;
                vOffset.x *= scale;
                vOffset.y *= scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecRD;
                vOffset.x *= scale;
                vOffset.y *= scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecLD;
                vOffset.x *= scale;
                vOffset.y *= scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecLU;
                vOffset.x *= scale;
                vOffset.y *= scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);
            }
            if (mesh != null && mesh.vertexCount == mOffset.buffer.Length)
            {
                mesh.uv2 = mOffset.buffer;
            }
        }
        //更新因其他hud显示或隐藏 要改变此hud的偏移位置
        protected override void UpdateOffset(Vector2 off)
        {
            if (m_SpriteVertex.size == 0 || mVerts.buffer == null)
            {
                return;
            }
            if (mesh == null)
            {
                return;
            }
            var vOffset = Vector2.zero;

            mOffset.CleanPreWrite(mesh.vertexCount);
            for (int i = 0, nSize = m_SpriteVertex.size; i < nSize; ++i)
            {
                HUDVertex v = m_SpriteVertex[i];
                v.Offset -= _offset;
                v.Offset += off;
                vOffset = v.vecRU;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecRD;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecLD;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);

                vOffset = v.vecLU;
                vOffset.x *= Scale;
                vOffset.y *= Scale;
                vOffset += v.Offset;
                mOffset.Add(vOffset);
            }
            if (mesh != null && mesh.vertexCount == mOffset.buffer.Length)
            {
                mesh.uv2 = mOffset.buffer;
            }
            _offset = off;
        }
        public override void RenderTo(CommandBuffer cmdBuffer)
        {
            if (mesh == null)
            {
                return;
            }
            Matrix4x4 matWorld = Matrix4x4.identity;
            cmdBuffer.DrawMesh(mesh, matWorld, Mat);
        }


    }
}