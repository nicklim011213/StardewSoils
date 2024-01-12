using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.IO;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using xTile.Tiles;
using xTile.Layers;
using xTile.Dimensions;
using StardewValley.Locations;

namespace StardewSoils
{
    public interface ISpaceCore
    {
        void RegisterSerializerType(Type type);
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
            Int32.TryParse(EncodedClass.Substring(PotassiumIndex, CropTypeIndex - 2 - PotassiumIndex), out int Potassium);
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
        public int Nitrogen = RandomGen.SoilRNG.Next(1,10);
        public int Phosphorus = RandomGen.SoilRNG.Next(1, 10);
        public int Potassium = RandomGen.SoilRNG.Next(1, 10);
        public Vector2 TilePos = new Vector2(-1,-1);
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
            if ((Crop as HoeDirt).crop.currentPhase.Value == (Crop as HoeDirt).crop.phaseDays.Count() - 1 && aftergrowth == false)
            {
                return true;
            }
            else if ((Crop as HoeDirt).crop.currentPhase.Value != (Crop as HoeDirt).crop.phaseDays.Count() - 1 && aftergrowth == true)
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
        public static Dictionary<TilePosAndLoc,LinkedTileSoilStats> AllRegisteredTiles = new Dictionary<TilePosAndLoc, LinkedTileSoilStats>();
    }

    static class RandomGen
    {
        public static Random SoilRNG = new Random((int)DateTime.Now.Ticks);
    }

    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            RegisterTextures(helper);
            RegisterEvents(helper, this.Monitor);
        }

        private void RegisterTextures(IModHelper helper)
        {
            Texture2D OutLine = helper.ModContent.Load<Texture2D>(Path.Combine(helper.DirectoryPath, "Assets/Tile_Outline.png"));
            helper.Events.Display.Rendered += (Obj, eventarg) => this.RenderOverlay(Obj, eventarg, helper, OutLine);
        }

        private void RegisterEvents(IModHelper helper, IMonitor monitor)
        {
            helper.Events.World.TerrainFeatureListChanged += (Obj, eventarg) => this.OnTerrainFeatureListChanged(Obj, eventarg, helper, monitor);
            helper.Events.GameLoop.DayStarted += (Obj, eventarg) => this.OnDayStart(Obj, eventarg, helper);
            helper.Events.GameLoop.GameLaunched += (Obj, eventarg) => this.OnGameStart(Obj, eventarg, helper);
            helper.Events.Display.MenuChanged += this.AddItemsToShop;
            helper.Events.GameLoop.SaveLoaded += (Obj, eventarg) => this.SaveLoader(Obj, eventarg, helper);
            helper.Events.Content.AssetRequested += (Obj, eventarg) => SoilQualityTextHandler.SeedDescriptionFixer(Obj, eventarg, helper , Monitor);
        }

        private void AddItemsToShop(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shop && shop.storeContext == "SeedShop")
            {
                var SoilReaderItem = new SoilReader();
                shop.itemPriceAndStock.Add(SoilReaderItem, new[] { 50, int.MaxValue });
                shop.forSale.Add(SoilReaderItem);
                var NitrogenFert = new FertilizerNitrogen1();
                shop.itemPriceAndStock.Add(NitrogenFert, new[] { 100, int.MaxValue });
                shop.forSale.Add(NitrogenFert);
                var PhosphorusFert = new FertilizerPhosphorus1();
                shop.itemPriceAndStock.Add(PhosphorusFert, new[] { 100, int.MaxValue });
                shop.forSale.Add(PhosphorusFert);
                var PotassiumFert = new FertilizerPotassium1();
                shop.itemPriceAndStock.Add(PotassiumFert, new[] { 100, int.MaxValue });
                shop.forSale.Add(PotassiumFert);
            }
        }

        private void RenderOverlay(object sender, RenderedEventArgs e, IModHelper helper, Texture2D OutLine)
        {
            if (Game1.player.CurrentItem != null && Game1.player.CurrentItem.DisplayName == "SoilReader" && !Game1.player.hasMenuOpen.Value && (Game1.player.currentLocation.IsFarm || Game1.player.currentLocation.IsGreenhouse))
            {   
                foreach (var Tile in TileList.AllRegisteredTiles)
                {
                    if (Tile.Value.Location == Game1.player.currentLocation)
                    {
                        Vector2 Pos = Game1.GlobalToLocal(new Vector2(Tile.Value.TilePos.X * Game1.tileSize, Tile.Value.TilePos.Y * Game1.tileSize));
                        e.SpriteBatch.Draw(OutLine, Pos, Color.White);
                        string TileMessage = "N: " + Tile.Value.Nitrogen + " \nP: " + Tile.Value.Phosphorus + " \nK: " + Tile.Value.Potassium;
                        e.SpriteBatch.DrawString(Game1.dialogueFont, TileMessage, Pos, Color.Black, 0.0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0); ;
                    }
                }
                
            }
        }

        private void OnGameStart(object sender, GameLaunchedEventArgs e, IModHelper helper)
        {
            ISpaceCore SpaceCoreAPI = helper.ModRegistry.GetApi<ISpaceCore>("spacechase0.SpaceCore");
            SpaceCoreAPI.RegisterSerializerType(typeof(SoilReader));
            SpaceCoreAPI.RegisterSerializerType(typeof(FertilizerNitrogen1));
            SpaceCoreAPI.RegisterSerializerType(typeof(FertilizerPhosphorus1));
            SpaceCoreAPI.RegisterSerializerType(typeof(FertilizerPotassium1));

            SoilReader.helper = helper;
            FertilizerNitrogen1.helper = helper;
            FertilizerPotassium1.helper = helper;
            FertilizerPhosphorus1.helper = helper;
        }

        private void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e, IModHelper helper, IMonitor monitor)
        {
            foreach (var TileTilled in e.Added)
            {

                GameLocation TileLocation = TileTilled.Value.currentLocation;   
                var Check = TileLocation.doesTileHaveProperty((int)TileTilled.Key.X, (int)TileTilled.Key.Y, "Diggable", "Back");

                if (TileTilled.Value is HoeDirt && Check == "T")
                {
                    string TileData = SoilDataBuilder.NewValueString();
                    string TileKey = SoilDataBuilder.PositionToKeyClass(TileTilled.Key);
                    Game1.player.currentLocation.modData[TileKey] = TileData;
                    SoilDataBuilder.StringToClass(TileData, TileKey, TileLocation);
                }
            }
            
        }

        private void OnDayStart(object sender, DayStartedEventArgs e, IModHelper helper)
        {
            foreach (var Tile in TileList.AllRegisteredTiles)
            {
                Tile.Value.GrowthCheck();
                Tile.Value.Location.modData[SoilDataBuilder.PositionToKeyClass(Tile.Value.TilePos)] = SoilDataBuilder.ClassToValueString(Tile.Value);
            }
        }

        private void SaveLoader(object sender, SaveLoadedEventArgs e, IModHelper helper)
        {
            PlantableLocations.InitlizeLocations();
            foreach (GameLocation loc in PlantableLocations.Areas)
            {
                foreach (var Tile in loc.modData.Keys) 
                {
                    var Value = loc.modData[Tile];
                    SoilDataBuilder.StringToClass(Value, Tile, loc);
                }
            }
        }
    }
}
