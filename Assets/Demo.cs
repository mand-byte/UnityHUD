using UnityEngine;
using UnityEngine.UIElements;

namespace GameHUD
{
    public class Demo : MonoBehaviour
    {
        public HUDConfigObject config;
        [Tooltip("role")]
        public Transform temp;
        private Transform role;
        public Vector2 offset = new Vector2(0, 1);
        HUDInfo hud;
        string _name, _title, _guide, _icon, _chat_content;
        ushort _name_index, _title_index, _guild_index, _icon_index, _chat_index, _blood_index;
        float _blood_percent;
        int _hurt_number; int _number_type;
        void Start()
        {
            if (config == null || temp == null)
            {
                return;
            }
            HUDManager.Init(config);
            role = GameObject.Instantiate(temp);
            hud = HUDManager.Instance.CreateHUD(1);

            hud.Init(role.transform, offset);
            var doc = GetComponent<UnityEngine.UIElements.UIDocument>();
            var uiasset = doc.rootVisualElement;
            var childCount = uiasset.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var element = uiasset.ElementAt(i);
                if (element.name == "name")
                {
                    var max = element.contentContainer.childCount;
                    for (int k = 0; k < max; k++)
                    {
                        var child = element.contentContainer.ElementAt(k);
                        if (child.name == "str")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                _name = new_str.newValue;
                                var t = child as TextField;
                                t.value = _name;
                            });
                        }
                        else if (child.name == "type")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                ushort.TryParse(new_str.newValue, out _name_index);
                                var t = child as TextField;
                                t.value = _name_index.ToString();
                            });
                        }
                    }
                }
                else if (element.name == "title")
                {
                    var max = element.contentContainer.childCount;
                    for (int k = 0; k < max; k++)
                    {
                        var child = element.contentContainer.ElementAt(k);
                        if (child.name == "str")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                _title = new_str.newValue;
                                var t = child as TextField;
                                t.value = _title;
                            });
                        }
                        else if (child.name == "type")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                ushort.TryParse(new_str.newValue, out _title_index);
                                var t = child as TextField;
                                t.value = _title_index.ToString();
                            });
                        }
                    }
                }
                else if (element.name == "guild")
                {
                    var max = element.contentContainer.childCount;
                    for (int k = 0; k < max; k++)
                    {
                        var child = element.contentContainer.ElementAt(k);
                        if (child.name == "str")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                _guide = new_str.newValue;
                                var t = child as TextField;
                                t.value = _guide;
                            });
                        }
                        else if (child.name == "type")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                ushort.TryParse(new_str.newValue, out _guild_index);
                                var t = child as TextField;
                                t.value = _guild_index.ToString();
                            });
                        }
                    }
                }
                else if (element.name == "icon")
                {
                    var max = element.contentContainer.childCount;
                    for (int k = 0; k < max; k++)
                    {
                        var child = element.contentContainer.ElementAt(k);
                        if (child.name == "str")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                _icon = new_str.newValue;
                                var t = child as TextField;
                                t.value = _icon;
                            });
                        }
                        else if (child.name == "type")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                ushort.TryParse(new_str.newValue, out _icon_index);
                                var t = child as TextField;
                                t.value = _icon_index.ToString();
                            });
                        }
                    }
                }
                else if (element.name == "chat")
                {
                    var max = element.contentContainer.childCount;
                    for (int k = 0; k < max; k++)
                    {
                        var child = element.contentContainer.ElementAt(k);
                        if (child.name == "str")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                _chat_content = new_str.newValue;
                                var t = child as TextField;
                                t.value = _chat_content;
                            });
                        }
                        else if (child.name == "type")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                ushort.TryParse(new_str.newValue, out _chat_index);
                                var t = child as TextField;
                                t.value = _chat_index.ToString();
                            });
                        }
                    }
                }
                else if (element.name == "refresh")
                {
                    var btn = element as Button;
                    btn.RegisterCallback<UnityEngine.UIElements.ClickEvent>((click) =>
                    {
                        hud.BloodVisable(true, (HUDRelationEnum)_blood_index);
                        hud.UpdateBloodPercent(_blood_percent);
                        hud.UpdateHudInfo(HudComponentEnum.Name, _name, (HUDRelationEnum)_name_index);
                        hud.UpdateHudInfo(HudComponentEnum.Title, _title, (HUDRelationEnum)_title_index);
                        hud.UpdateHudInfo(HudComponentEnum.GuildName, _guide, (HUDRelationEnum)_guild_index);
                        hud.UpdateHudInfo(HudComponentEnum.GuildIcon, _icon, (HUDRelationEnum)_icon_index);

                    });
                }
                else if (element.name == "talk")
                {
                    var btn = element as Button;
                    btn.RegisterCallback<UnityEngine.UIElements.ClickEvent>((click) =>
                    {
                        if (!string.IsNullOrEmpty(_chat_content))
                        {
                            hud.Talk(_chat_index, _chat_content);
                        }
                    });
                }
                else if (element.name == "blood")
                {
                    var max = element.contentContainer.childCount;
                    for (int k = 0; k < max; k++)
                    {
                        var child = element.contentContainer.ElementAt(k);
                        if (child.name == "str")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                float.TryParse(new_str.newValue, out _blood_percent);
                                var t = child as TextField;
                                t.value = new_str.newValue;
                            });
                        }
                        else if (child.name == "type")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                ushort.TryParse(new_str.newValue, out _blood_index);
                                var t = child as TextField;
                                t.value = _blood_index.ToString();
                            });
                        }
                    }
                }
                else if (element.name == "number")
                {
                    var max = element.contentContainer.childCount;
                    for (int k = 0; k < max; k++)
                    {
                        var child = element.contentContainer.ElementAt(k);
                        if (child.name == "str")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                var t = child as TextField;
                                int.TryParse(new_str.newValue, out _hurt_number);
                                t.value = _hurt_number.ToString();
                            });
                        }
                        else if (child.name == "type")
                        {
                            child.RegisterCallback<ChangeEvent<string>>((new_str) =>
                            {
                                var t = child as TextField;
                                int.TryParse(new_str.newValue, out _number_type);
                                t.value = _number_type.ToString();
                            });
                        }
                    }
                }
                else if (element.name == "hurt")
                {
                    var btn = element as Button;
                    btn.RegisterCallback<UnityEngine.UIElements.ClickEvent>((click) =>
                    {
                        if (config.NumberTypes != null && config.NumberTypes.Count > 0)
                        {
                            var r = new Unity.Mathematics.Random((uint)System.Guid.NewGuid().GetHashCode());
                            var x = r.NextFloat(-3f, 3f);
                            var pos = new Vector2(x, 0);
                            hud.HurtNumber(_number_type, _hurt_number, pos);
                        }


                    });
                }
            }
        }
    }
}