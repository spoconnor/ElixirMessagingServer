using System;

namespace Sean.Textures
{
    #region Texture Enums
    /// <summary>Block texture index. The order can be changed without affecting anything, so keep this alphabetical with Air always at 0 and water last.</summary>
    public enum BlockTextureType
    {
        Air, //not actually a texture, nothing will be rendered
        Bricks,
        Coal,
        Cobble,
        Copper,
        Crate,
        CrateSide,
        Dirt,
        ElmTree,
        FancyBlack,
        FancyGreen,
        FancyRed,
        FancyWhite,
        Gold,
        Grass,
        GrassSide,
        Gravel,
        Ice,
        Iron,
        Lava,
        LavaRock,
        Leaves,
        Oil,
        PrisonBars,
        Sand,
        SandDark,
        Shelf1,
        Snow,
        SnowLeaves,
        SnowSide,
        Speaker,
        SteelDoorBottom,
        SteelDoorTop,
        SteelPlate,
        SteelPlate2,
        Rock,
        Tree,
        TreeTrunk,
        WoodTile1,
        WoodTile2,
        /// <summary>First texture in the water animation. All water VBOs are always assigned to this texture id.</summary>
        Water,
        /// <summary>Used for water animation only. No VBO will be assigned to this texture id.</summary>
        Water2,
        /// <summary>Used for water animation only. No VBO will be assigned to this texture id.</summary>
        Water3,
        /// <summary>Used for water animation only. No VBO will be assigned to this texture id.</summary>
        Water4
    }
    
    /// <summary>Clutter texture index. The order can be changed without affecting anything.</summary>
    public enum ClutterTextureType
    {
        Bush,
        Grass1,
        Grass2,
        Grass3,
        Grass4,
        Grass5,
        Grass6,
        Grass7,
        Grass8
    }
    
    public enum EnvironmentTextureType
    {
        Sun,
        Moon
    }
    
    /// <summary>All items regardless of type (light sources, etc.) go in this enum so they can share the same resource file. The order can be changed without affecting anything.</summary>
    public enum ItemTextureType
    {
        Lantern
    }
    
    public enum UiTextureType
    {
        CompassArrow,
        BlockCursor,
        ToolDefault,
        ToolCuboid,
        ToolFastBuild,
        ToolFastDestroy,
        ToolTree,
        CrossHairs,
        Axe,
        Shovel,
        PickAxe,
        Tower,
        SmallKeep,
        LargeKeep,
        BaseCharacter
    }
    
    /// <summary>All units regardless of type (players, mobs etc.) go in this enum so they can share the same resource file. The order can be changed without affecting anything.</summary>
    public enum UnitTextureType
    {
        
    }
#endregion
}
