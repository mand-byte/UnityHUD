using UnityEngine;
namespace GameHUD
{

    //用来简化给外部使用的轻量类,与内部使用的HUDObject相对应
    //外部调用者只用维护基本信息.
    public class HUDInfo
    {
        internal long id;
        internal HUDObject Object;
        bool init = false;
        public HUDInfo()
        {

        }
        public void Init(Transform trans, Vector2 offset)
        {
            init = true;
            Object.Init(trans, offset);
        }
        public void UpdateHudInfo(HudComponentEnum comptype, string str, HUDRelationEnum relationtype = HUDRelationEnum.Self)
        {
            if (!init) return;
            Object.UpdateHudInfo(comptype, str, relationtype);
        }
        public void Talk(ushort talkStyleIdx, string str)
        {
            if (!init) return;
            Object.PushTalk(talkStyleIdx, str);
        }
        ///<summary>
        ///更新血条
        ///</summary>
        public void UpdateBloodPercent(float percent)
        {
            if (!init) return;
            Object.UpdateBloodPercent(percent);
        }
        ///<summary>
        ///是否显示血条
        ///</summary>
        public void BloodVisable(bool show, HUDRelationEnum relationtype)
        {
            if (!init) return;
            Object.BloodVisable(show, relationtype);
        }
        public void HurtNumber(int type, int number, Vector2 offset)
        {
            if (!init) return;
            Object.PushNumber(number, type, offset);
        }

        public void Release()
        {
            if (init)
            {
                HUDManager.Instance.DeleteHUD(id);
                init = false;
            }
        }
    }
}