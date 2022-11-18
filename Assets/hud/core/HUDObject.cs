using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{

    sealed internal class HUDObject
    {
        HUDConfigObject config;
        //存储头顶hud 
        HUDMesh[] _base_hud_mesh = new HUDMesh[(int)HudComponentEnum.Total];
        BetterList<HUDMesh> _dynamical_mesh = new BetterList<HUDMesh>();
        bool _init;
        Transform _trans;
        //初始角色偏移
        Vector3 _role_offset;


        //获取当前需要偏移的高度
        Vector2 GetComponentOffset(HudComponentEnum enume)
        {
            var result = Vector3.zero;
            for (int i = (int)enume - 1; i >= 0; i--)
            {
                var hud_mesh = _base_hud_mesh[i];
                if (hud_mesh != null && hud_mesh.IsValid)
                {
                    result = new Vector2(_role_offset.x, hud_mesh.Size.y + hud_mesh.ItemLineGap + result.y);
                }
            }
            return result;
        }


        public void UpdateHudInfo(HudComponentEnum _type, string name, HUDRelationEnum relationEnum)
        {

            if (string.IsNullOrEmpty(name))
            {
                //如果原先有,现在没有
                MeshHide(_type);
            }
            else
            {
                var mesh = _base_hud_mesh[(int)_type];
                var isupdate = false;
                if (mesh != null && mesh.IsValid)
                {
                    isupdate = true;
                }
                var offset = GetComponentOffset(_type);
                if (_type.Equals(HudComponentEnum.GuildIcon))
                {
                    BuildSprite(_type, name, offset);
                }
                else
                {
                    BuildText(_type, relationEnum, name, offset);
                }
                if (!isupdate)
                    MeshHide(_type, false);
            }
        }
        public void UpdateBloodPercent(float percent)
        {
            var blood_mesh = _base_hud_mesh[(int)HudComponentEnum.Blood];
            if (blood_mesh != null && blood_mesh.IsValid)
            {
                var blood = blood_mesh as HUDBloodMesh;
                blood.PushValue(percent);
                return;
            }
        }
        public void BloodVisable(bool show, HUDRelationEnum relationEnum)
        {
            if (show)
            {
                if (config.BloodRelationArray.Length <= (int)relationEnum)
                {
                    Debug.LogError("blood hud 没有此配置 " + relationEnum.ToString());
                    return;
                }
                var blood_mesh = _base_hud_mesh[(int)HudComponentEnum.Blood];
                if (blood_mesh == null)
                {
                    blood_mesh = ObjectPool<HUDBloodMesh>.Pop();
                    _base_hud_mesh[(int)HudComponentEnum.Blood] = blood_mesh;
                }
                var isupdate = false;
                if (blood_mesh.IsValid)
                {
                    isupdate = true;
                }
                var blood = blood_mesh as HUDBloodMesh;
                blood.Create(config.BloodRelationArray[(int)relationEnum], RolePos, _role_offset);
                if (!isupdate)
                    MeshHide(HudComponentEnum.Blood, false);
            }
            else
            {
                var blood_mesh = _base_hud_mesh[(int)HudComponentEnum.Blood];
                if (blood_mesh != null && blood_mesh.IsValid)
                {
                    MeshHide(HudComponentEnum.Blood);
                }

            }
        }
        //初始偏移


        public void Init(Transform trans, Vector3 offset)
        {
            _trans = trans;
            _role_offset = offset;
            config = HUDManager.Instance.Config;
            _init = true;
        }

        void BuildText(HudComponentEnum enume, HUDRelationEnum relation, string str, Vector2 uiOffset)
        {
            var type_index = (int)enume;
            HUDTxtMesh text_mesh;
            if (_base_hud_mesh[type_index] == null)
            {
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }
                text_mesh = ObjectPool<HUDTxtMesh>.Pop();
                _base_hud_mesh[type_index] = text_mesh;
            }
            else
            {
                text_mesh = _base_hud_mesh[type_index] as HUDTxtMesh;
                if (text_mesh.IsValid)
                    text_mesh.Release();
            }
            FontStyle style = FontStyle.Normal;
            List<Color32> color = null;
            float outlineWidth = 0f;
            var colorOutline = Color.white;
            int fontSize = 0;
            int CharGap = 0;
            int LineGap = 0;
            AlignmentEnum Align = AlignmentEnum.Left;
            int ItemLineGap = 0;
            HUDTXTInfoObject info = new HUDTXTInfoObject();
            switch (enume)
            {
                case HudComponentEnum.Name:
                    if (config.NameRelationArray.Length <= (int)relation)
                    {
                        Debug.LogError("NameRelation 没有此配置 " + relation.ToString());
                        return;
                    }
                    info = config.NameRelationArray[(int)relation];

                    break;
                case HudComponentEnum.Title:
                    if (config.TitleInfoArray.Length <= (int)relation)
                    {
                        Debug.LogError("TitleRelation 没有此配置 " + relation.ToString());
                        return;
                    }
                    info = config.TitleInfoArray[(int)relation];
                    break;
                case HudComponentEnum.GuildName:
                    if (config.GuildRelationArray.Length <= (int)relation)
                    {
                        Debug.LogError("GuildRelation 没有此配置 " + relation.ToString());
                        return;
                    }
                    info = config.GuildRelationArray[(int)relation];
                    break;
            }
            outlineWidth = info.OutlineWidth;
            colorOutline = info.NameColorSD;
            style = info.Style;
            color = info.NameColor;
            fontSize = info.FontSize;
            CharGap = info.CharGap;
            LineGap = info.LineGap;
            Align = info.Align;
            ItemLineGap = info.ItemLineGap;

            text_mesh.ItemLineGap = ItemLineGap;
            text_mesh.PushText(str, color, colorOutline, RolePos, _role_offset, uiOffset, outlineWidth, fontSize, CharGap, LineGap, style, Align, 0);
        }

        public void PushNumber(int number, int type, Vector3 offset)
        {
            if (type>=config.NumberTypes.Count||type<0)
            {
                return;
            }
            var number_mesh = ObjectPool<HUDNumberMesh>.Pop();
            var start_pos = HUDManager.CameraTrans.right * offset.x + HUDManager.CameraTrans.up * offset.y + _trans.position+HUDManager.CameraTrans.forward*offset.z;
            number_mesh.PushNumber(number, type, start_pos);
            _dynamical_mesh.Add(number_mesh);
        }
        public void PushTalk(int idx, string content)
        {
            for (int i = 0; i < _dynamical_mesh.size; i++)
            {
                if (typeof(HUDTalkMesh).IsInstanceOfType(_dynamical_mesh[i]))
                {
                    HUDTalkMesh talk = _dynamical_mesh[i] as HUDTalkMesh;
                    if (!talk.IsValid)
                    {
                        talk.Release();
                    }
                    else
                    {
                        talk.ReadyRecycle = true;
                    }
                    break;
                }
            }
            var v = GetComponentOffset(HudComponentEnum.GuildIcon);
            var chat_mesh = ObjectPool<HUDTalkMesh>.Pop();
            chat_mesh.PushTalk(idx, content, RolePos, _role_offset, v);
            _dynamical_mesh.Add(chat_mesh);
        }
        void BuildSprite(HudComponentEnum enume, string name, Vector2 _offset)
        {
            HUDSpriteMesh sp_mesh;
            int type_index = (int)enume;
            if (_base_hud_mesh[type_index] == null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }
                sp_mesh = ObjectPool<HUDSpriteMesh>.Pop();
                _base_hud_mesh[type_index] = sp_mesh;
            }
            else
            {
                sp_mesh = _base_hud_mesh[type_index] as HUDSpriteMesh;
                if (sp_mesh.IsValid)
                {
                    sp_mesh.Release();
                }
            }
            sp_mesh.ItemLineGap = config.SpriteLineGap;
            sp_mesh.PushSprite(name, RolePos, _role_offset, _offset, AlignmentEnum.Middle);
        }
        Vector3 RolePos = Vector3.zero;

        //目前逻辑所有改变会重新计算uv绘制,后续优化
        public void Update(bool font_mat_rebuild = false)
        {
            if (!_init)
            {
                return;
            }
            if (_trans != null)
            {
                if (font_mat_rebuild)
                {
                    var hud_mesh = _base_hud_mesh[(int)HudComponentEnum.Name];
                    if (hud_mesh != null && hud_mesh.IsValid)
                    {
                        hud_mesh.Rebuild();
                    }
                    hud_mesh = _base_hud_mesh[(int)HudComponentEnum.Title];
                    if (hud_mesh != null && hud_mesh.IsValid)
                    {
                        hud_mesh.Rebuild();
                    }
                    hud_mesh = _base_hud_mesh[(int)HudComponentEnum.GuildName];
                    if (hud_mesh != null && hud_mesh.IsValid)
                    {
                        hud_mesh.Rebuild();
                    }
                    hud_mesh = _base_hud_mesh[(int)HudComponentEnum.GuildIcon];
                    if (hud_mesh != null && hud_mesh.IsValid)
                    {
                        hud_mesh.Rebuild();
                    }
                    for (int i = 0; i < _dynamical_mesh.size; i++)
                    {
                        var mesh = _dynamical_mesh[i];
                        if (mesh != null && mesh.IsValid)
                        {
                            mesh.Rebuild();
                        }
                    }
                }
                CheckPosChange();
                for (int i = 0; i < _base_hud_mesh.Length; i++)
                {
                    var mesh = _base_hud_mesh[i];
                    if (mesh != null && mesh.IsValid)
                    {
                        mesh.UpdateLogic();
                    }
                }
                for (int i = 0; i < _dynamical_mesh.size; i++)
                {
                    var mesh = _dynamical_mesh[i];
                    if (mesh != null && mesh.IsValid)
                    {

                        mesh.UpdateLogic();
                    }
                }
                for (int i = _dynamical_mesh.size - 1; i >= 0; i--)
                {
                    var mesh = _dynamical_mesh[i];
                    if (mesh == null || !mesh.IsValid)
                    {
                        MeshRecyle(mesh);
                        _dynamical_mesh.RemoveAt(i);
                    }
                }

            }
            else
            {
                Debug.LogWarning("hud object TRANSFORM is missing check it out!!");
            }
        }
        void MeshRecyle(HUDMesh mesh)
        {
            if (mesh == null)
            {
                return;
            }
            if (mesh.IsValid)
            {
                mesh.Release();
            }
            //回收对话框
            if (typeof(HUDTalkMesh).IsInstanceOfType(mesh))
            {
                var m = mesh as HUDTalkMesh;
                ObjectPool<HUDTalkMesh>.Push(m);
            }
            else if (typeof(HUDTxtMesh).IsInstanceOfType(mesh))
            {
                var m = mesh as HUDTxtMesh;
                ObjectPool<HUDTxtMesh>.Push(m);
            }
            else if (typeof(HUDSpriteMesh).IsInstanceOfType(mesh))
            {
                var m = mesh as HUDSpriteMesh;
                ObjectPool<HUDSpriteMesh>.Push(m);
            }
            else if (typeof(HUDNumberMesh).IsInstanceOfType(mesh))
            {
                var m = mesh as HUDNumberMesh;
                ObjectPool<HUDNumberMesh>.Push(m);
            }
            else if (typeof(HUDBloodMesh).IsInstanceOfType(mesh))
            {
                var m = mesh as HUDBloodMesh;
                ObjectPool<HUDBloodMesh>.Push(m);
            }

        }
        //检查是否需要更新坐标
        void CheckPosChange()
        {
            if (Vector3.Distance(_trans.position, RolePos) > 0.00001f)
            {
                RolePos = _trans.position;
                for (int i = 0; i < _base_hud_mesh.Length; i++)
                {
                    if (_base_hud_mesh[i] != null && _base_hud_mesh[i].IsValid)
                    {
                        _base_hud_mesh[i].RolePos = RolePos;
                    }
                }
                for (int i = 0; i < _dynamical_mesh.size; i++)
                {
                    if (_dynamical_mesh[i] != null && _dynamical_mesh[i].IsValid && _dynamical_mesh[i].FollowRole)
                    {
                        _dynamical_mesh[i].RolePos = RolePos;
                    }
                }
            }
        }
        //隐藏或显示某个组件
        void MeshHide(HudComponentEnum enume, bool isHide = true)
        {
            //计算当前组件size
            var select_mesh = _base_hud_mesh[(int)enume];
            if (select_mesh == null || select_mesh.IsValid == false)
            {
                return;
            }
            var size = select_mesh.Size;
            var select_linegap = select_mesh.ItemLineGap;
            size.Set(size.x, size.y + select_linegap);
            for (int i = (int)enume + 1; i < _base_hud_mesh.Length; i++)
            {
                var mesh = _base_hud_mesh[i];
                if (mesh != null && mesh.IsValid)
                {
                    var offset = mesh.Offset;
                    if (isHide)
                    {
                        offset.Set(offset.x, offset.y - size.y);
                    }
                    else
                    {
                        offset.Set(offset.x, offset.y + size.y);
                    }
                    mesh.Offset = offset;
                }
            }
            if (isHide)
            {
                select_mesh.Release();
            }
        }
        public void Release()
        {
            HUDManager.Instance.Dirty = true;
            _init = false;
            for (int i = _base_hud_mesh.Length - 1; i >= 0; i--)
            {
                var mesh = _base_hud_mesh[i];
                if (mesh != null)
                {
                    MeshRecyle(mesh);
                    _base_hud_mesh[i] = null;
                }
            }
            for (int i = _dynamical_mesh.size - 1; i >= 0; i--)
            {
                var mesh = _dynamical_mesh[i];
                if (mesh != null)
                {
                    MeshRecyle(mesh);
                    _dynamical_mesh.RemoveAt(i);
                }
            }

        }
        public void FillMeshData(List<MeshData> meshDatas)
        {
            for (int i = 0; i < _base_hud_mesh.Length; i++)
            {
                var meshhud = _base_hud_mesh[i];
                if (meshhud != null && meshhud.IsValid)
                {
                    meshhud.FillMeshData(meshDatas);
                }
            }
            for (int i = 0; i < _dynamical_mesh.size; i++)
            {
                var meshhud = _dynamical_mesh[i];
                if (meshhud != null && meshhud.IsValid)
                {
                    meshhud.FillMeshData(meshDatas);
                }
            }
        }

    }
}