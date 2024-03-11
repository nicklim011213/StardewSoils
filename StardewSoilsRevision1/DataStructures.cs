using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

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
                //{90, new Vector3 (0, 0, 0)}, // Cactus Fruit (Can only be grown indoors UH OH SPAGETI-O)
                {771, new Vector3 (0, 0, 0)}, // Fiber (0 Because it may not be balanced if the weeds are taken into account)
                {832, new Vector3(0, 1, 1)}, // Pineapple
                {830, new Vector3(-1, 0, -1)}, // Taro Root
                {417, new Vector3(0, 2, 2)}, // Sweet Gem Berry
                {815, new Vector3(0, -1, -1)} //Tea Bush
            };

            public static Dictionary<int, int> SeedToCrop = new Dictionary<int, int>
            {
                {597, 429}, // Jazz Seeds
                {190, 474}, // Cauliflower seeds
                {433, 433}, // Coffee Beans
                {248, 476}, // Garlic Seeds
                {188, 473}, // Bean Starter
                {250, 477}, // Kale Seeds
                {24,  472}, // Parsnip Seeds
                {192, 475}, // Potato Seeds
                {252, 478}, // Ruhbarb Seeds
                {400, 745}, // Strawbery Seeds
                {591, 427}, // Tulip Bulb
                {271, 273}, // Rice Shoot

                {258, 481}, // Blueberry Seeds
                {270, 487}, // Corn Seeds
                {304, 302}, // Hops Starter
                {260, 482}, // Pepper Seeds
                {254, 479}, // Melon Seeds
                {376, 453}, // Poppy Seeds
                {264, 484}, // Raddish Seeds
                {266, 485}, // Red Cabbage Seeds
                {286, 486}, // Starfruit Seeds
                {593, 455}, // Spangle Seeds
                {431, 421}, // Sunflower Seeds
                {256, 480}, // Tomato Seeds
                {262, 483}, // Wheat Seeds

                {300, 299}, // Amarath Seeds
                {274, 489}, // Artichoke Seeds
                {284, 494}, // Beet Seeds
                {278, 491}, // Bok Choy Seeds
                {282, 493}, // Cranberry Seeds
                {272, 488}, // Eggplant Seeds
                {595, 425}, // Fairy Seeds
                {398, 301}, // Grape Starter
                {276, 490}, // Pumpkin Seeds
                {280, 492}, // Yam Seeds
                {454, 499}, // Ancient Seeds
                {771, 885}, // Fiber Seeds
                {832, 833}, // Pineapple Seeds
                {830, 831}, // Tarop Root Seeds
                {417, 347}, // Rare Seed (Sweet Gem Berry Seed)
                {815, 251}  // Tea Sapling
            };
    }

    public static class PlantableLocations
    {
        public static List<GameLocation> Areas = new List<GameLocation>();


        public static void InitlizeLocations()
        {
            foreach (var Location in Game1.locations)
            {
                if (Location.IsFarm == true || Location.IsGreenhouse == true)
                {
                    Areas.Add(Location);
                }
            }
        }
    }

    public static class SoilDataBuilder
    {
        public static string NewValueString()
        {
            int Nitrogen = RandomGen.SoilRNG.Next(1, 10);
            int Phosphorus = RandomGen.SoilRNG.Next(1, 10);
            int Potassium = RandomGen.SoilRNG.Next(1, 10);
            int CropType = -1;
            bool afterGrowth = false;
            string afterGrowthString = "F";
            if (afterGrowth)
                afterGrowthString = "T";
            else
                afterGrowthString = "F";


            return "N:" + Nitrogen + " P:" + Phosphorus + " K:" + Potassium + " CT:" + CropType + " AG:" + afterGrowthString;
        }

        public static string ClassToValueString(LinkedTileSoilStats Tile)
        {
            string afterGrowthString = "F";
            if (Tile.aftergrowth)
                afterGrowthString = "T";
            else
                afterGrowthString = "F";
            return "N:" + Tile.Nitrogen + "P:" + Tile.Phosphorus + "K:" + Tile.Potassium + "CT:" + Tile.CropType + "AG:" + afterGrowthString;
        }

        public static LinkedTileSoilStats StringToClass(string EncodedClass, string EncodedKey, GameLocation location)
        {
            int NitrogenIndex = EncodedClass.IndexOf("N:") + 2;
            int PhosphorusIndex = EncodedClass.IndexOf("P:") + 2;
            int PotassiumIndex = EncodedClass.IndexOf("K:") + 2;
            int CropTypeIndex = EncodedClass.IndexOf("CT:") + 3;
            int AfterGrowthIndex = EncodedClass.IndexOf("AG:") + 3;

            Int32.TryParse(EncodedClass.Substring(NitrogenIndex, PhosphorusIndex - 2 - NitrogenIndex), out int Nitrogen);
            Int32.TryParse(EncodedClass.Substring(PhosphorusIndex, PotassiumIndex - 2 - PhosphorusIndex), out int Phosphorus);
            Int32.TryParse(EncodedClass.Substring(PotassiumIndex, CropTypeIndex - 3 - PotassiumIndex), out int Potassium);
            Int32.TryParse(EncodedClass.Substring(CropTypeIndex, AfterGrowthIndex - 3 - CropTypeIndex), out int CropType);
            string Val = EncodedClass.Substring(AfterGrowthIndex);
            bool afterGrowth = false;
            if (Val.Contains("T"))
                afterGrowth = true;
            else
                afterGrowth = false;

            int XStart = EncodedKey.IndexOf("SaltWaterHippo.StardewSoil_") + "SaltWaterHippo.StardewSoil_".Length;
            int XEnd = EncodedKey.IndexOf("-");
            Int32.TryParse(EncodedKey.Substring(XStart, XEnd - XStart), out int KeyX);
            Int32.TryParse(EncodedKey.Substring(XEnd + 1), out int KeyY);
            Vector2 Pos = new Vector2(KeyX, KeyY);


            return new LinkedTileSoilStats(Nitrogen, Phosphorus, Potassium, CropType, afterGrowth, Pos, location, EncodedKey);
        }

        public static string PositionToKeyClass(Vector2 pos)
        {
            return "SaltWaterHippo.StardewSoil_" + pos.X.ToString() + "-" + pos.Y.ToString();
        }
    }

    public class LinkedTileSoilStats
    {
        public int Nitrogen = RandomGen.SoilRNG.Next(1, 10);
        public int Phosphorus = RandomGen.SoilRNG.Next(1, 10);
        public int Potassium = RandomGen.SoilRNG.Next(1, 10);
        public Vector2 TilePos = new Vector2(-1, -1);
        public int CropType = -1; // No Crop is -1
        public bool aftergrowth = false;
        public GameLocation Location = null;

        public LinkedTileSoilStats(int N, int P, int K, int C, bool A, Vector2 pos, GameLocation loc, string Key)
        {
            Nitrogen = N;
            Phosphorus = P;
            Potassium = K;
            CropType = C;
            aftergrowth = A;
            this.TilePos = pos;
            Location = loc;
            GetCropOnTile(this.TilePos, loc);
            TileList.AllRegisteredTiles[new TilePosAndLoc(this.TilePos, this.Location.ToString())] = this;
        }

        public void GrowthCheck()
        {
            if (CropGrowthRefrences.CropLookup.TryGetValue(CropType, out var reqs) == true)
            {
                if (Nitrogen < reqs.X || Phosphorus < reqs.X || Potassium < reqs.X)
                    GrowthPause();
                else
                    CropFinish();
            }
            return;
        }

        void GrowthPause()
        {
            if (CropNullCheck(TilePos, out TerrainFeature Crop, Location) == false)
            {
                return;
            }

            int day = (Crop as HoeDirt).crop.dayOfCurrentPhase.Value;
            int phase = (Crop as HoeDirt).crop.currentPhase.Value;

            if (day == 0 && phase == 0)
            {
                return;
            }
            else if (day != 0)
            {
                (Crop as HoeDirt).crop.dayOfCurrentPhase.Set(day - 1);
            }
            else if (phase != 0)
            {
                (Crop as HoeDirt).crop.currentPhase.Set(phase - 1);
            }
            else
            {
                return;
            }
        }

        void CropFinish()
        {
            if (CropNullCheck(TilePos, out TerrainFeature Crop, Location) == false)
            {
                return;
            }

            if (IsDoneGrowing(Crop))
            {
                Nitrogen -= (int)CropGrowthRefrences.CropLookup[CropType].X;
                Potassium -= (int)CropGrowthRefrences.CropLookup[CropType].Y;
                Phosphorus -= (int)CropGrowthRefrences.CropLookup[CropType].Z;
                aftergrowth = true;
            }
            else if (!IsDoneGrowing(Crop))
            {
                aftergrowth = false;
            }
        }

        public void GetCropOnTile(Vector2 TilePos, GameLocation location)
        {
            if (CropNullCheck(TilePos, out TerrainFeature Crop, Location) == false)
            {
                return;
            }
            else
            {
                CropType = (Crop as HoeDirt).crop.indexOfHarvest.Value;
                //(Crop as HoeDirt).crop = (Crop as HoeDirt).crop;
            }
            return;
        }

        public bool IsDoneGrowing(TerrainFeature Crop)
        {
            if ((Crop as HoeDirt).crop.currentPhase.Value == (Crop as HoeDirt).crop.phaseDays.Count - 1 && aftergrowth == false)
            {
                return true;
            }
            else if ((Crop as HoeDirt).crop.currentPhase.Value != (Crop as HoeDirt).crop.phaseDays.Count - 1 && aftergrowth == true)
            {
                return false;
            }
            return false;
        }

        public bool CropNullCheck(Vector2 TilePos, out TerrainFeature CurrentCrop, GameLocation location)
        {
            location.terrainFeatures.TryGetValue(TilePos, out TerrainFeature Crop);
            if (Crop == null || (Crop as HoeDirt) == null || (Crop as HoeDirt).crop == null)
            {
                CurrentCrop = null;
                return false;
            }
            CurrentCrop = Crop;
            return true;
        }
    }

    static class TileList
    {
        public static Dictionary<TilePosAndLoc, LinkedTileSoilStats> AllRegisteredTiles = new Dictionary<TilePosAndLoc, LinkedTileSoilStats>();
    }

    static class RandomGen
    {
        public static Random SoilRNG = new Random((int)DateTime.Now.Ticks);
    }

    public struct TilePosAndLoc
    {
        public Vector2 pos;
        public string location;

        public TilePosAndLoc(Vector2 pos, string location)
        {
            this.pos = pos;
            this.location = location;
        }

    }
}