using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
namespace GameHUD
{
    [System.Serializable]
    public struct HUDVector4Int
    {
        [Tooltip("左边切割点 x轴 从图片左边到左切割点的距离")]
        public int Left;
        [Tooltip("右边切割点 x轴 从图片右边到右切割点的距离")]
        public int Right;
        [Tooltip("底部切割点 y轴 从图片下边到下切割点的距离")]
        public int Bottom;
        [Tooltip("上部切割点 y轴 从图片上边到上切割点的距离")]
        public int Top;
    }
    [System.Serializable]
    public struct HUDNumberConfig
    {
        [Tooltip("数字类型")]
        public HudNumberType Type;
        [Tooltip("是否带正负符号")]
        public bool Sign;
        [Tooltip("数字前缀")]
        public char Perfixe;

        [Tooltip("移动曲线")]
        public AnimationCurve MoveCurve;
        [Tooltip("渐变曲线")]
        public AnimationCurve ColorCurve;
        [Tooltip("缩放曲线")]
        public AnimationCurve ScaleCurve;
        [Tooltip("数字之间的间隔")]
        public int NumbersGap;
        [Tooltip("数字的对齐方式")]
        public AlignmentEnum NumbersAlign;

    }
    [System.Serializable]
    public struct HUDTXTInfoObject
    {
        [Tooltip("关系类型")]
        public HUDRelationEnum Relation;
        [Tooltip("名字颜色集合,元素数量只能选1个或4个\n 选1个时为纯色 选4个时,字的颜色显示顺序为左下 左上 右上 右下")]
        public List<Color32> NameColor;
        [Tooltip("名字描边厚度,\n 0为无描边"), Range(0, 2)]
        public float OutlineWidth;
        [Tooltip("字的描边颜色")]
        public Color32 NameColorSD;
        [Tooltip("字体样式")]
        public FontStyle Style;
        [Tooltip("对其方式")]
        public AlignmentEnum Align;
        [Tooltip("字行距")]
        public int LineGap;
        [Tooltip("字间距")]
        public int CharGap;
        [Tooltip("字大小")]
        public ushort FontSize;
        [Tooltip("文字与下层的间距")]
        public int ItemLineGap;

    }
    [System.Serializable]
    public struct HUDBloodInfoObject
    {
        [Tooltip("关系类型")]
        public HUDRelationEnum Relation;
        [Tooltip("血条底图名字")]
        public string BloodBg;
        [Tooltip("血条图名字")]
        public string Blood;
        [Tooltip("对其方式")]
        public AlignmentEnum Align;
        [Tooltip("切割拉伸方向")]
        public SliceTypeEnum SliceType;

        [Tooltip("血条背景宽度"), Range(1, 500)]
        public ushort BloodWidthBG;
        [Tooltip("血条背景高"), Range(1, 500)]
        public ushort BloodHeightBG;
        [Tooltip("血条背景切割坐标点")]
        public HUDVector4Int SliceBGValue;


        [Tooltip("血条宽 \n一定不要大于背景宽度"), Range(1, 500)]
        public ushort BloodWidth;
        [Tooltip("血条高  \n一定不要大于背景高度"), Range(1, 500)]
        public ushort BloodHeight;

        [Tooltip("血条切割切割坐标点")]
        public HUDVector4Int SliceValue;
        [Tooltip("血条是否反向移动")]
        public bool Reverse;
        [Tooltip("血条偏移量")]
        public Vector2Int BloodOffset;
    }

    [System.Serializable]
    public class HUDTalkInfoObject
    {
        [Tooltip("背景底图名字")]
        public string Sprite;
        [Tooltip("背景底图切割点")]
        public HUDVector4Int BGSliceValue;
        [Tooltip("文字显示内容与背景图四边的差值")]
        public HUDVector4Int ContentSliceValue;
        [Tooltip("字体大小")]
        public ushort FontSize;
        [Tooltip("字体颜色 只有1个是单色,4个是渐变色")]
        public List<Color32> FontColor;
        [Tooltip("字体间间隔")]
        public ushort CharGap;
        [Tooltip("字体行间隔")]
        public ushort LineGap;
        [Tooltip("一行字最大宽度  \n宽度计算方式为纯中文情况下 (字体大小+间隔)*字数")]
        public ushort MaxLineWidth;
   
        [Tooltip("字的描边厚度,\n 0为无描边"), Range(0, 2)]
        public float OutlineWidth;
        [Tooltip("字的描边颜色")]
        public Color32 NameColorSD;
        [Tooltip("字体样式")]
        public FontStyle Style;
        [Tooltip("正常情况下,显示时间 单位秒")]
        public float NormalShowTime;
        [Tooltip("当有新聊天出现时,此聊天框消失时间 单位秒")]
        public float VanishedTime;

        [Tooltip("气泡框与下层的间距")]
        public int ItemLineGap;

        [Tooltip("气泡框对其方式")]
        public AlignmentEnum ItemAlign;

    }
    public class HUDConfigObject : ScriptableObject
    {
        [Tooltip("HUD图集  \n图集与图集数据的顺序一定要一一对应")]
        public List<Texture2D> Atlas;
        [Tooltip("HUD图集数据 \n图集与图集数据的顺序一定要一一对应")]
        public List<TextAsset> AtlasData;
        [Tooltip("字体")]
        public Font Font;
        [Tooltip("图间距")]
        public int SpriteGap;

        [Tooltip("图行距")]
        public int SpriteLineGap;

        [System.NonSerialized]
        public Dictionary<HUDRelationEnum, HUDTXTInfoObject> NameRelationDict = new Dictionary<HUDRelationEnum, HUDTXTInfoObject>();
        [Tooltip("名字关系文字配置")]
        public HUDTXTInfoObject[] NameRelationArray;
        [System.NonSerialized]
        public Dictionary<HUDRelationEnum, HUDTXTInfoObject> GuildRelationDict = new Dictionary<HUDRelationEnum, HUDTXTInfoObject>();

        [Tooltip("名字关系文字配置")]
        public HUDTXTInfoObject[] GuildRelationArray;
        [Tooltip("称号设置")]
        public HUDTXTInfoObject[] TitleInfoArray;
        [System.NonSerialized]
        public Dictionary<HUDRelationEnum, HUDTXTInfoObject> TitleRelationDict = new Dictionary<HUDRelationEnum, HUDTXTInfoObject>();

        [System.NonSerialized]
        public Dictionary<HUDRelationEnum, HUDBloodInfoObject> BloodRelationDict = new Dictionary<HUDRelationEnum, HUDBloodInfoObject>();

        [Tooltip("血条关系配置")]
        public HUDBloodInfoObject[] BloodRelationArray;
        [Tooltip("气泡框配置")]
        public HUDTalkInfoObject[] TalkInfoArray;

        [Tooltip("飘字配置")]
        public List<HUDNumberConfig> NumberTypes;
        [HideInInspector, System.NonSerialized]
        public Dictionary<HudNumberType, HUDNumberConfig> NumberTypeDict = new Dictionary<HudNumberType, HUDNumberConfig>();




    }
}