using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{
    internal interface HUDMeshInterface
    {
        public void Release();
        public void RenderTo(CommandBuffer cmdBuffer);
    }
}