using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StardewSoils
{
    public static class CropGrowthRefrences
    {
            public static Dictionary<int, Vector3> CropLookup = new Dictionary<int, Vector3>
            {
                //Spring Crops
                { 597, new Vector3(0, -2, 0) }, // Blue Jazz
                { 190, new Vector3(0, 2, 0) }, // Cauliflower
                { 433, new Vector3(0, 0, 1) }, // Coffee Bean
                { 248, new Vector3(-2, 0, 0) }, // Garlic
                { 188, new Vector3(0, 1, 1) }, // Green Bean
                { 250, new Vector3(0, -1, 0) }, // Kale
                { 24, new Vector3(-2, 0, 0) }, // Parsnip
                { 192, new Vector3(1, 0, 0) }, // Potato
                { 252, new Vector3(2, 0, 0) }, // Ruhbarb
                { 400, new Vector3(0, 2, 0) }, // Strawberry
                { 591, new Vector3(0, -3, 0) }, // Tulip
                { 271, new Vector3(0, -2, -2) }, // Unmilled Rice

                //Summer Crops
                {258, new Vector3(0, 2, 1) }, // Blueberry
                {270, new Vector3(0, 1, 0) }, // Corn
                {304, new Vector3(0, 0, 2) }, // Hops
                {260, new Vector3(0, 1, 0) }, // Pepper
                {254, new Vector3(0, 2, 0) }, // Melon
                {376, new Vector3(0, -2, 0) }, // Poppy
                {264, new Vector3(-2, 0, 0) }, // Raddish
                {266, new Vector3(0, 1, 0) }, // Red Cabbage
                {268, new Vector3(0, 1, 3) }, // Starfruit
                {593, new Vector3(0, -2, 0) }, // Summer Spangle
                {421, new Vector3(0, -1, -2) }, // Sunflower
                {256, new Vector3(0, 1, 0) }, // Tomato
                {262, new Vector3(-1, -1, 0)}, // Wheat

                //Fall Crops
                {300, new Vector3(0, 1, 0)}, // Amaranth
                {274, new Vector3(0, 1, 1)}, // Artichoke
                {284, new Vector3(1, 0, 0)}, // Beat
                {278, new Vector3(-2, 0, 0)}, // Bok Choy
                {282, new Vector3(0, 2, 0)}, // Cranberries
                {272, new Vector3(0, 1, 0)}, // Egg Plant
                {595, new Vector3(0, -1, -1)}, // Fairy Rose
                {398, new Vector3(0, 2, 0)}, // Grapes
                {276, new Vector3(0, 2, 0)}, // Pumpkin
                {280, new Vector3(1, 0, 0)}, // Yam

                //Special
                {454, new Vector3(-1, -1, -1)}, // Ancient Fruit
                {90, new Vector3 (0, 0, 0)}, // Cactus Fruit
                {771, new Vector3 (0, 0, 0)}, // Fiber (0 Because it may not be balanced if the weeds are taken into account)
                {832, new Vector3(0, 1, 1)}, // Pineapple
                {830, new Vector3(-1, 0, -1)}, // Taro Root
                {417, new Vector3(0, 2, 2)}, // Sweet Gem Berry
                {815, new Vector3(0, -1, -1)} //Tea Bush
            };
        }
    }