using System.Text;
using UnityEngine;

namespace GameHUD
{
    internal static class HUDStringParser
    {//解析文字
        public static Vector2Int ParseText(BetterList<HUDVertex> list, string str, Color32 color0, Color32 color1, Color32 color2, Color32 color3, Vector2 offset, int size, int CharGap, int LineGap, FontStyle style, AlignmentEnum alignmentEnum, int widthlimit = 0)
        {
            var config = HUDManager.Instance.Config;
            int total_width = 0;
            int line = 0;
            LineGap = 6;
            int temp_height = 0;
            config.Font.RequestCharactersInTexture(str, size, style);
            for (int i = 0; i < str.Length; i++)
            {
                if (config.Font.GetCharacterInfo(str[i], out var ch, size, style))
                {
                    var vertex = ObjectPool<HUDVertex>.Pop();
                    if (temp_height < ch.glyphHeight)
                    {
                        temp_height = ch.glyphHeight;
                    }
                    vertex.uvRU = ch.uvBottomRight;
                    vertex.uvRD = ch.uvTopRight;
                    vertex.uvLD = ch.uvTopLeft;
                    vertex.uvLU = ch.uvBottomLeft;
                    float fL = ch.minX;
                    float fT = ch.minY;
                    float fR = ch.maxX;
                    float fB = ch.maxY;
                    vertex.vecRU.Set(fR, fT);  // 右上角
                    vertex.vecRD.Set(fR, fB);  // 右下角
                    vertex.vecLD.Set(fL, fB);  // 左下角
                    vertex.vecLU.Set(fL, fT);  // 左上角
                    vertex.clrLD = color1;
                    vertex.clrLU = color0;
                    vertex.clrRU = color3;
                    vertex.clrRD = color2;
                    var temp_width = total_width + ch.advance + CharGap;
                    if (widthlimit > 0 && temp_width >= widthlimit)
                    {
                        line++;
                        total_width = 0;
                    }
                    vertex.Offset.Set(total_width + offset.x, -line * (temp_height + LineGap));
                    total_width += (ch.advance + CharGap);
                    list.Add(vertex);
                }
            }
            total_width = line > 0 ? widthlimit : total_width - CharGap;
            var first_index = list.size - str.Length;
            var offset_y = (size + LineGap) * line;
            var height = (size + LineGap) * (line + 1);
            if (alignmentEnum.Equals(AlignmentEnum.Middle))
            {

                for (int i = 0; i < str.Length; i++)
                {
                    var v = list[first_index + i].Offset;
                    list[first_index + i].Offset = new Vector2(v.x - total_width / 2, v.y + offset_y + LineGap + offset.y);
                }
            }
            else if (alignmentEnum.Equals(AlignmentEnum.Right))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    var v = list[first_index + i].Offset;
                    list[first_index + i].Offset = new Vector2(v.x - total_width, v.y + offset_y + LineGap + offset.y);
                }
            }
            else
            {
                for (int i = 0; i < str.Length; i++)
                {
                    var v = list[first_index + i].Offset;
                    list[first_index + i].Offset = new Vector2(v.x, v.y + offset_y + LineGap + offset.y);
                }
            }
            return new Vector2Int(total_width, height);
        }
        //解析单个图片
        public static Vector2Int PasreSprite(BetterList<HUDVertex> list, out Material mat, string str, Vector2 offset, AlignmentEnum alignmentEnum = AlignmentEnum.Middle, int width = 0, int height = 0)
        {
            var config = HUDManager.Instance.Config;
            var info = HUDManager.Instance.GetSprite(str);
            var vertex = list[list.size - 1];
            mat = info.Mat;
            vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
            width = width == 0 ? info.Width : width;
            float fL = 0.0f;
            float fT = 0.0f;
            float fR = width;
            float fB = height == 0 ? info.Height : height;



            vertex.vecRU.Set(fR, fT);  // 右上角
            vertex.vecRD.Set(fR, fB);  // 右下角
            vertex.vecLD.Set(fL, fB);  // 左下角
            vertex.vecLU.Set(fL, fT);  // 左上角

            float uvR = info.xMax;
            float uvL = info.xMin;
            float uvB = info.yMin;
            float uvT = info.yMax;

            vertex.uvRU.Set(uvR, uvB);
            vertex.uvRD.Set(uvR, uvT);
            vertex.uvLD.Set(uvL, uvT);
            vertex.uvLU.Set(uvL, uvB);

            list.Add(vertex);
            return new Vector2Int(info.Width, info.Height);
        }
        ///<summary>
        ///解析横竖向切割拉伸图填充
        ///</summary>
        /// <param name="offset">偏移坐标</param>
        /// <param name="fillAmount">填充值0到1</param>
        /// <param name="width">最大宽</param>
        /// <param name="height">最大高</param>
        /// <param name="isReverse">是否反转填充,从小到大还是从大到小</param>
        /// <returns>size大小</returns>
        public static Vector2Int PasreSlicedFillSprite(BetterList<HUDVertex> list, out Material mat, string str, Vector2 offset, float fillAmount, int width, int height, bool isReverse, HUDVector4Int slicevalue, SliceTypeEnum slicetype, AlignmentEnum alignmentEnum)
        {

            var config = HUDManager.Instance.Config;
            var info = HUDManager.Instance.GetSprite(str);
            mat = info.Mat;
            if (fillAmount <= 0) fillAmount = 0;
            else if (fillAmount >= 1) fillAmount = 1;
            int xSliceLength = (int)(width * fillAmount);
            int ySliceLength = (int)(height * fillAmount);


            //每像素多少uv坐标点
            var xFactor = (info.xMax - info.xMin) / info.Width;
            var yFactor = (info.yMax - info.yMin) / info.Height;
            var list_count = list.size;
            // x y 轴 三段的像素长度
            int x_length0 = 0, x_length1 = 0, x_length2 = 0, y_length0 = 0, y_length1 = 0, y_length2 = 0;
            if (slicetype == SliceTypeEnum.Horizontal)
            {
                var uv1 = info.xMin + xFactor * slicevalue.Left;
                var uv2 = info.xMax - xFactor * slicevalue.Right;
                if (!isReverse)
                {
                    if (xSliceLength > 0)
                    {
                        x_length0 = slicevalue.Left;
                        if (xSliceLength - slicevalue.Left - slicevalue.Right > 0)
                        {
                            x_length1 = xSliceLength - slicevalue.Left - slicevalue.Right;
                            x_length2 = slicevalue.Right;
                        }
                    }
                    SlicedFill(list[list_count - 3], x_length0, height, offset.x, offset.y, info.xMin, uv1, info.yMin, info.yMax);
                    SlicedFill(list[list_count - 2], x_length1, height, offset.x + x_length0, offset.y, uv1, uv2, info.yMin, info.yMax);
                    SlicedFill(list[list_count - 1], x_length2, height, offset.x + x_length0 + x_length1, offset.y, uv2, info.xMax, info.yMin, info.yMax);
                }
                else
                {
                    if (xSliceLength > 0)
                    {
                        x_length2 = slicevalue.Right;
                        if (xSliceLength - slicevalue.Right - slicevalue.Left > 0)
                        {
                            x_length1 = xSliceLength - slicevalue.Right - slicevalue.Left;
                            x_length0 = slicevalue.Left;
                        }
                    }
                    SlicedFill(list[list_count - 3], x_length0, height, offset.x + width - xSliceLength, offset.y, info.xMin, uv1, info.yMin, info.yMax);
                    SlicedFill(list[list_count - 2], x_length1, height, offset.x + width - xSliceLength + x_length0, offset.y, uv1, uv2, info.yMin, info.yMax);
                    SlicedFill(list[list_count - 1], x_length2, height, offset.x + width - x_length2, offset.y, uv2, info.xMax, info.yMin, info.yMax);
                }

            }
            if (slicetype == SliceTypeEnum.Vertical)
            {

                var uv1 = info.yMin + yFactor * slicevalue.Bottom;
                var uv2 = info.yMax - yFactor * slicevalue.Top;

                if (!isReverse)
                {
                    if (ySliceLength > 0)
                    {
                        y_length0 = (int)slicevalue.Bottom;
                        if (ySliceLength - slicevalue.Bottom - slicevalue.Top > 0)
                        {
                            y_length1 = (int)(ySliceLength - slicevalue.Bottom - slicevalue.Top);
                            y_length2 = (int)slicevalue.Top;
                        }
                    }
                    SlicedFill(list[list_count - 3], width, y_length0, offset.x, offset.y, info.xMin, info.xMax, info.yMin, uv1);
                    SlicedFill(list[list_count - 2], width, y_length1, offset.x, y_length0 + offset.y, info.xMin, info.xMax, uv1, uv2);
                    SlicedFill(list[list_count - 1], width, y_length2, offset.x, ySliceLength - slicevalue.Top + offset.y, info.xMin, info.xMax, uv2, info.yMax);
                }
                else
                {

                    if (ySliceLength > 0)
                    {
                        y_length2 = (int)slicevalue.Top;
                        if (ySliceLength - slicevalue.Top - slicevalue.Bottom > 0)
                        {
                            y_length1 = (int)(ySliceLength - slicevalue.Top - slicevalue.Bottom);
                            y_length0 = (int)slicevalue.Bottom;
                        }
                    }
                    SlicedFill(list[list_count - 3], width, y_length0, offset.x + width, height - ySliceLength + offset.y, info.xMin, info.xMax, info.yMin, uv1);
                    SlicedFill(list[list_count - 2], width, y_length1, offset.x + width, height - ySliceLength + y_length0 + offset.y, info.xMin, info.xMax, uv1, uv2);
                    SlicedFill(list[list_count - 1], width, y_length2, offset.x + width, height - y_length2 + offset.y, info.xMin, info.xMax, uv2, info.yMax);
                }
            }

            return new Vector2Int(width, height);
        }
        ///<summary> 
        ///9宫切图
        ///</summary>
        ///<param name="slicevalue">x left,y right z bottom w top</param>
        public static Vector2Int PasreSlicedSprite(BetterList<HUDVertex> list, out Material mat, string str, Vector2 offset, int width, int height, HUDVector4Int slicevalue, AlignmentEnum alignmentEnum)
        {
            var config = HUDManager.Instance.Config;
            var info = HUDManager.Instance.GetSprite(str);
            mat = info.Mat;
            var xFactor = (info.xMax - info.xMin) / info.Width;
            var yFactor = (info.yMax - info.yMin) / info.Height;
            int slice_width = (int)(width - slicevalue.Left - slicevalue.Right);
            int slice_hight = (int)(height - slicevalue.Bottom - slicevalue.Top);
            var x_uv1 = xFactor * slicevalue.Left + info.xMin;
            var x_uv2 = xFactor * (info.Width - slicevalue.Right) + info.xMin;
            var y_uv1 = yFactor * slicevalue.Bottom + info.yMin;
            var y_uv2 = yFactor * (info.Height - slicevalue.Top) + info.yMin;

            int size = list.size;
            //下左
            SlicedFill(list[size - 9], slicevalue.Left, slicevalue.Bottom, 0, 0, info.xMin, x_uv1, info.yMin, y_uv1);
            //下中
            SlicedFill(list[size - 8], slice_width, slicevalue.Bottom, slicevalue.Left, 0, x_uv1, x_uv2, info.yMin, y_uv1);
            //下右
            SlicedFill(list[size - 7], slicevalue.Right, slicevalue.Bottom, width - slicevalue.Right, 0, x_uv2, info.xMax, info.yMin, y_uv1);

            //中左
            SlicedFill(list[size - 6], slicevalue.Left, slice_hight, 0, slicevalue.Bottom, info.xMin, x_uv1, y_uv1, y_uv2);
            //中中
            SlicedFill(list[size - 5], slice_width, slice_hight, slicevalue.Left, slicevalue.Bottom, x_uv1, x_uv2, y_uv1, y_uv2);
            //中右
            SlicedFill(list[size - 4], slicevalue.Right, slice_hight, width - slicevalue.Right, slicevalue.Bottom, x_uv2, info.xMax, y_uv1, y_uv2);

            //上左
            SlicedFill(list[size - 3], slicevalue.Left, slicevalue.Top, 0, height - slicevalue.Top, info.xMin, x_uv1, y_uv2, info.yMax);
            //上中
            SlicedFill(list[size - 2], slice_width, slicevalue.Top, slicevalue.Left, height - slicevalue.Top, x_uv1, x_uv2, y_uv2, info.yMax);
            //上右
            SlicedFill(list[size - 1], slicevalue.Right, slicevalue.Top, width - slicevalue.Right, height - slicevalue.Top, x_uv2, info.xMax, y_uv2, info.yMax);
            return new Vector2Int(width, height);
        }

        static void SlicedFill(HUDVertex vert, int nWidth, int nHeight, float fOffsetX, float fOffsetY, float xMin, float xMax, float yMin, float yMax)
        {


            float fL = fOffsetX;
            float fB = fOffsetY;
            float fR = fOffsetX + nWidth;
            float fT = fOffsetY + nHeight;
            vert.vecRU.Set(fR, fB);
            vert.vecRD.Set(fR, fT);
            vert.vecLD.Set(fL, fT);
            vert.vecLU.Set(fL, fB);

            vert.uvRU.Set(xMax, yMin);
            vert.uvRD.Set(xMax, yMax);
            vert.uvLD.Set(xMin, yMax);
            vert.uvLU.Set(xMin, yMin);
        }


        static StringBuilder sb = new StringBuilder();
        //解析数字图片
        public static Vector2Int PasrseNumber(BetterList<HUDVertex> list, out Material mat, char Perfixe, int gap, int number, bool sign, AlignmentEnum align)
        {
            var config = HUDManager.Instance.Config;
            sb.Clear();
            sb.Append(Perfixe);
            sb.Append(Perfixe);
            int width = 0;
            int height = 0;
            mat = null;
            string number_str;
            if (sign)
            {
                if (number > 0)
                {
                    number_str = string.Format("+{0}", number);
                }
                else
                {
                    number_str = number.ToString();
                }
            }
            else
            {
                number_str = Mathf.Abs(number).ToString();
            }

            for (int i = 0; i < number_str.Length; i++)
            {
                sb.Remove(1, 1);
                sb.Insert(1, number_str[i]);
                var info = HUDManager.Instance.GetSprite(sb.ToString());
                if (info == null)
                {
                    Debug.LogWarningFormat("Warning ,{0} not in HUDAtlas !", sb.ToString());
                    continue;
                }
                mat = info.Mat;
                var vertex = ObjectPool<HUDVertex>.Pop();
                vertex.clrLD = vertex.clrLU = vertex.clrRD = vertex.clrRU = Color.white;
                float fL = width;
                float fT = 0.0f;
                float fR = info.Width + width;
                float fB = info.Height;

                vertex.vecRU.Set(fR, fT);  // 右上角
                vertex.vecRD.Set(fR, fB);  // 右下角
                vertex.vecLD.Set(fL, fB);  // 左下角
                vertex.vecLU.Set(fL, fT);  // 左上角

                float uvR = info.xMax;
                float uvL = info.xMin;
                float uvB = info.yMin;
                float uvT = info.yMax;

                vertex.uvRU.Set(uvR, uvB);
                vertex.uvRD.Set(uvR, uvT);
                vertex.uvLD.Set(uvL, uvT);
                vertex.uvLU.Set(uvL, uvB);

                list.Add(vertex);
                width += (info.Width + gap);
                if (height < info.Height)
                {
                    height = info.Height;
                }
            }
            width -= gap;
            if (align != AlignmentEnum.Left)
                for (int i = 0; i < list.size; i++)
                {
                    if (align.Equals(AlignmentEnum.Right))
                    {
                        var _off = new Vector2(width, 0);
                        list[i].vecRU -= _off;
                        list[i].vecRD -= _off;
                        list[i].vecLD -= _off;
                        list[i].vecLU -= _off;
                    }
                    else if (align.Equals(AlignmentEnum.Middle))
                    {
                        var _off = new Vector2(width / 2, 0);
                        list[i].vecRU -= _off;
                        list[i].vecRD -= _off;
                        list[i].vecLD -= _off;
                        list[i].vecLU -= _off;
                    }
                }

            return new Vector2Int(width, height);
        }
    }
}