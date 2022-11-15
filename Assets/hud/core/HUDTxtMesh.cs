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
        Vector2  uioffset ;
        Vector3 role_offset;
        public void PushText(string str, List<Color32> color, Color32 outlineColor, Vector3 rolepos, Vector3 roleoffset, Vector2 uiOffset, float outlineWidth, int fontSize, int CharGap, int TxtLineGap, FontStyle style, AlignmentEnum alignment, int widthlimit = 0)
        {

            MaterialIndex = 0;

            this.str = str;
            this.outlineWidth = outlineWidth;
            this.outlineColor = outlineColor;
            this.color = color;
            this.fontSize = fontSize;
            this.CharGap = CharGap;
            this.LineGap = TxtLineGap;
            this.style = style;
            this.alignment = alignment;
            this.widthlimit = widthlimit;
            this.uioffset = uiOffset + new Vector2(0, ItemLineGap);
            _rolePos = rolepos;
            role_offset =  roleoffset;
            Rebuild();
            

        }
        public override void Rebuild()
        {
            if (_valid)
            {
                Release();
            }
            var temp_offset = Vector2.zero;
            if (outlineWidth != 0)
            {
                temp_offset.Set(outlineWidth, 0);
                HUDStringParser.ParseText(m_SpriteVertex, str, outlineColor, outlineColor, outlineColor, outlineColor, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
                temp_offset.Set(-outlineWidth, 0);
                HUDStringParser.ParseText(m_SpriteVertex, str, outlineColor, outlineColor, outlineColor, outlineColor, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
                temp_offset.Set(0, outlineWidth);
                HUDStringParser.ParseText(m_SpriteVertex, str, outlineColor, outlineColor, outlineColor, outlineColor, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
                temp_offset.Set(0, -outlineWidth);
                HUDStringParser.ParseText(m_SpriteVertex, str, outlineColor, outlineColor, outlineColor, outlineColor, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
            }
            temp_offset = Vector2.zero;
            if (color.Count == 1)
            {
                Size = HUDStringParser.ParseText(m_SpriteVertex, str, color[0], color[0], color[0], color[0], temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
            }
            else if (color.Count == 4)
            {
                Size = HUDStringParser.ParseText(m_SpriteVertex, str, color[0], color[1], color[2], color[3], temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
            }
            else
            {
                Size = HUDStringParser.ParseText(m_SpriteVertex, str, Color.white, Color.white, Color.white, Color.white, temp_offset, fontSize, CharGap, LineGap, style, alignment, widthlimit);
            }
            _RoleOffset=this.role_offset;    
            Offset = this.uioffset;
            _valid = true;
            HUDManager.Instance.Dirty = true;
        }
    }
}