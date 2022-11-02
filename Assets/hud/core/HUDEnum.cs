namespace GameHUD
{
    public enum HudComponentEnum
    {
        Blood,
        Name,
        Title,
        GuildName,
        GuildIcon,
        Total,
    }
    ///<summary>
    ///HUD样式类型,请当int来用,不要把字面意思当成实际意思.
    ///</summary>
    public enum HUDRelationEnum
    {
        Self,
        Team,
        Friend,
        Npc,
        Enemy,
        Monster0,
        Monster1,
        Monster2,
        Monster3,
        Monster4,

        All,
    }
    public enum AlignmentEnum
    {
        Left,
        Middle,
        Right,
    }
    public enum SliceTypeEnum
    {
        Horizontal,
        Vertical,
        HorizontalAndVertical,
    }
}