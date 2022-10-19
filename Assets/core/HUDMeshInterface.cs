using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{
    internal interface HUDMeshInterface
    {
        public void Release();
        public void UpdatePos(Vector3 role);
        public void UpdateOffset(Vector2 offset);
        public void RenderTo(CommandBuffer cmdBuffer);
    }
}