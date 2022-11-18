using UnityEngine;
namespace GameHUD
{

    //用来简化给外部使用的轻量类,与内部使用的HUDObject相对应
    //外部调用者只用维护基本信息.
    public class HUDInfo
    {
        internal long id;
        internal HUDObject Object;
        private Transform _trans;
        public Transform Trans => _trans;
        bool init = false;
        public HUDInfo()
        {

        }
        public bool IsInited=>init;
        public void Init(Transform trans, Vector3 offset)
        {
            if (!init)
            {
                init = true;
                _trans = trans;
                Object.Init(trans, offset);
            }
        }
        ///<summary>
        ///更新头顶ui信息
        ///</summary>
        ///<param name="styleIndex">样式索引</param>
        public void UpdateHudInfo(HudComponentEnum comptype, string str, HUDRelationEnum styleIndex = HUDRelationEnum.Self)
        {
            if (!init) return;
            Object.UpdateHudInfo(comptype, str, styleIndex);
        }
        ///<summary>
        ///聊天气泡框
        ///</summary>
        ///<param name="talkStyleIdx">气泡框样式索引</param>
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
        ///<param name="styleIndex">血条样式索引</param>
        public void BloodVisable(bool show, HUDRelationEnum styleIndex)
        {
            if (!init) return;
            Object.BloodVisable(show, styleIndex);
        }
        ///<summary>
        ///飘字
        ///</summary>
        ///<param name="styleIndex">飘字样式索引</param>
        ///<param name="offset">偏移值</param>
        public void HurtNumber(int styleIndex, int number, Vector3 offset)
        {
            if (!init) return;
            Object.PushNumber(number, styleIndex, offset);
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