using UnityEngine;
using UnityEngine.Rendering;

namespace GameHUD
{
    internal sealed class HUDTalkMesh : HUDMeshCombine
    {
        public bool ReadyRecycle;
        float _cur_showtime = 0, _cur_vanishedtime = 0, _max_showtime = 0, _max_vanishedtime = 0;
        string str; int idx;

        public void PushTalk(int idx, string content,Vector3 rolepos, Vector2 offset)
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
            Rebuild();
            _offset = new Vector2(offset.x, offset.y + Config.TalkInfoArray[idx].ItemLineGap);
            _rolePos=rolepos;

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
            var txt_offset = _offset + new Vector2(chat_info.ContentSliceValue.Left, chat_info.ContentSliceValue.Bottom);
            txt.PushText(str, chat_info.FontColor, chat_info.NameColorSD,_rolePos, txt_offset, chat_info.OutlineWidth, chat_info.FontSize, chat_info.CharGap, chat_info.LineGap, chat_info.Style, chat_info.Align, chat_info.MaxLineWidth);
            var sp_width = chat_info.ContentSliceValue.Left + txt.Size.x + chat_info.ContentSliceValue.Right;
            var sp_height = chat_info.ContentSliceValue.Top + txt.Size.y + chat_info.ContentSliceValue.Bottom;
            sprite.PushSliceSprite(chat_info.Sprite,_rolePos, _offset, sp_width, sp_height, chat_info.BGSliceValue, chat_info.Align);
            Dirty = true;
            if (chat_info.ItemAlign.Equals(AlignmentEnum.Middle))
            {
                sprite.Offset=new Vector2(sprite.Offset.x - sp_width / 2, sprite.Offset.y);
                txt.Offset=new Vector2(txt.Offset.x - sp_width / 2, txt.Offset.y);
            }
            else if (chat_info.ItemAlign.Equals(AlignmentEnum.Right))
            {
                sprite.Offset=new Vector2(sprite.Offset.x - sp_width, sprite.Offset.y);
                txt.Offset=new Vector2(txt.Offset.x - sp_width, txt.Offset.y);
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