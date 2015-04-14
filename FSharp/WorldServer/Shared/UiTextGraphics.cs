using System;

namespace Sean.World
{
    public static class UiTextGraphics
    {
        /// <summary>Block types starting with 'Placeholder' are not included in the action picker buttons grid and will appear white because they dont have associated textures.</summary>
        public static char[] BlockTypeGraphic = new char[56]
        {
            //Naturally occurring
            ' ', //Air = 0,
            '-', //Water = 1,
            '.', //Dirt = 2,
            ',', //Grass = 3,
            '"', //Snow = 4,
            ':', //Sand = 5,
            ';', //SandDark = 6,
            '=', //Ice = 7,
            '+', //Gravel = 8,
            '#', //Rock = 9,
            '%', //Coal = 10,
            '%', //Copper = 11,
            '%', //Iron = 12,
            '%', //Gold = 13,
            '%', //Oil = 14,
            'T', //Tree = 15,
            'T', //ElmTree = 16,
            'L', //Leaves = 17,
            'L', //SnowLeaves = 18,
            '*', //Lava = 19,
            '*', //LavaRock = 20,
            '?','?','?','?','?','?','?','?','?','?',
            '?','?','?','?','?','?','?','?','?','?',
            '?','?','?','?','?','?','?','?','?',
            //Crafted Material
            'W', //WoodTile1 = 50,
            'W', //WoodTile2,
            'B', //Bricks,
            '?',
            'C', //Cobble = 54,            
            '?', //Crafted = 100..., other = 220...
        };

    }
}

