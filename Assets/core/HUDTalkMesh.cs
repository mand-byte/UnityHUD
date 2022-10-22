using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{
    internal sealed class HUDTalkMesh : HUDMeshCombine
    {
        public bool ReadyRecycle;
        float _cur_showtime = 0, _cur_vanishedtime = 0, _max_showtime = 0, _max_vanishedtime = 0;
        string str; int idx;
        public void PushTalk(int idx, string content, Vector3 rolepos, Vector2 offset)
        {
            _FollowRole = true;
            if (_valid)
            {
                Release();
            }
            ReadyRecycle = false;
            _valid = true;
            _cur_showtime = 0;
            _cur_vanishedtime = 0;
            this.idx = idx;
            this.str = content;
            if (Meshs.size == 0)
            {
                var txt = ObjectPool<HUDTxtMesh>.Pop();
                var sprite = ObjectPool<HUDSpriteMesh>.Pop();
                Meshs.Add(sprite);
                Meshs.Add(txt);
            }
            _offset = offset+Config.TalkInfoArray[idx].Offset;
            _rolePos = rolepos;
            Rebuild();

        }
        public override void Rebuild()
        {
            var txt = Meshs[1] as HUDTxtMesh;
            var sprite = Meshs[0] as HUDSpriteMesh;
            if (Config.TalkInfoArray.Length <= this.idx || Config.TalkInfoArray[idx] == null)
            {
                Debug.LogError("聊天没有配置 索引为" + idx);
                return;
            }
            var chat_info = Config.TalkInfoArray[idx];
            _max_showtime = Config.TalkInfoArray[idx].NormalShowTime;
            _max_vanishedtime = Config.TalkInfoArray[idx].VanishedTime;
            txt.PushText(str, chat_info.FontColor, chat_info.NameColorSD, _rolePos, Vector2.zero, chat_info.OutlineWidth, chat_info.FontSize, chat_info.CharGap, chat_info.LineGap, chat_info.Style, AlignmentEnum.Left, chat_info.MaxLineWidth);
            var sp_width = chat_info.ContentSliceValue.Left + txt.Size.x + chat_info.ContentSliceValue.Right;
            var bg_min_width=chat_info.BGSliceValue.Left+chat_info.BGSliceValue.Right;
            sp_width=Mathf.Max(sp_width,bg_min_width);
            var sp_height = chat_info.ContentSliceValue.Top + txt.Size.y + chat_info.ContentSliceValue.Bottom;
            var bg_min_height=chat_info.BGSliceValue.Bottom+chat_info.BGSliceValue.Top;
            sp_height=Mathf.Max(sp_height,bg_min_height);
            sprite.PushSliceSprite(chat_info.Sprite, _rolePos, _offset, sp_width, sp_height, chat_info.BGSliceValue, chat_info.ItemAlign);

            Dirty = true;
            if (chat_info.ItemAlign.Equals(AlignmentEnum.Middle))
            {

                txt.Offset = new Vector2(sprite.Offset.x + chat_info.ContentSliceValue.Left - sp_width / 2, sprite.Offset.y + chat_info.ContentSliceValue.Bottom + txt.Size.y - chat_info.FontSize);
            }
            else if (chat_info.ItemAlign.Equals(AlignmentEnum.Right))
            {
                txt.Offset = new Vector2(sprite.Offset.x + chat_info.ContentSliceValue.Left - sp_width, sprite.Offset.y + chat_info.ContentSliceValue.Bottom + txt.Size.y - chat_info.FontSize);
            }
            else
            {
                txt.Offset = new Vector2(sprite.Offset.x + chat_info.ContentSliceValue.Left, sprite.Offset.y + chat_info.ContentSliceValue.Bottom + txt.Size.y - chat_info.FontSize);
            }

        }

        public override void UpdateLogic()
        {
            if (_cur_showtime >= _max_showtime)
            {
                Release();
                return;
            }
            _cur_showtime += Time.deltaTime;
            if (ReadyRecycle)
            {

                if (_cur_vanishedtime >= _max_vanishedtime)
                {
                    Release();
                    return;
                }
                _cur_vanishedtime += Time.deltaTime;
            }
        }
        public override void Release()
        {
            base.Release();
            ObjectPool<HUDSpriteMesh>.Push(Meshs[0] as HUDSpriteMesh);
            ObjectPool<HUDTxtMesh>.Push(Meshs[1] as HUDTxtMesh);
            Meshs.Clear();
        }

    }
}