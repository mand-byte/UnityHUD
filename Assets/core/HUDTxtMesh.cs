using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameHUD
{
    internal sealed class HUDTxtMesh : HUDMeshSingle
    {
        string str;
        List<Color32> color;
        Color32 outlineColor;
        float outlineWidth;
        int fontSize;
        FontStyle style;
        int CharGap; int LineGap;
        AlignmentEnum alignment; int widthlimit = 0;
        public void PushText(string str, List<Color32> color, Color32 outlineColor, Vector2 offset, float outlineWidth, int fontSize, int CharGap, int TxtLineGap, FontStyle style, AlignmentEnum alignment, int widthlimit = 0)
        {
            if (_valid)
            {
                Release();
            }
            if (Mat == null)
            {
                Mat = HUDManager.Instance.Mats[0];
            }
            _valid = true;
            this.str = str;
            _offset = offset;
            this.outlineWidth = outlineWidth;
            this.outlineColor = outlineColor;
            this.color = color;
            this.fontSize = fontSize;
            this.CharGap = CharGap;
            this.LineGap = TxtLineGap;
            this.style = style;
            this.alignment = alignment;
            this.widthlimit = widthlimit;

            Rebuild();

        }
        public override void Rebuild()
        {
            if (outlineWidth != 0)
            {
                var temp_offset = _offset;
                temp_offset.Set(_offset.x + outlineWidth, _offset.y + outlineWidth);
                HUDStringParser.ParseText(m_SpriteVertex, str, outlineColor, outlineColor, outlineColor, outlineColor, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
                temp_offset.Set(_offset.x + outlineWidth, _offset.y);
                HUDStringParser.ParseText(m_SpriteVertex, str, outlineColor, outlineColor, outlineColor, outlineColor, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
                temp_offset.Set(_offset.x - outlineWidth, _offset.y);
                HUDStringParser.ParseText(m_SpriteVertex, str, outlineColor, outlineColor, outlineColor, outlineColor, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
                temp_offset.Set(_offset.x - outlineWidth, _offset.y + outlineWidth);
                HUDStringParser.ParseText(m_SpriteVertex, str, outlineColor, outlineColor, outlineColor, outlineColor, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
            }
            if (color.Count == 1)
            {
                Size = HUDStringParser.ParseText(m_SpriteVertex, str, color[0], color[0], color[0], color[0], _offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
            }
            else if (color.Count == 4)
            {
                Size = HUDStringParser.ParseText(m_SpriteVertex, str, color[0], color[1], color[2], color[3], _offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
            }
            else
            {
                Size = HUDStringParser.ParseText(m_SpriteVertex, str, Color.white, Color.white, Color.white, Color.white, _offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
            }
            Dirty = true;
        }
    }
}