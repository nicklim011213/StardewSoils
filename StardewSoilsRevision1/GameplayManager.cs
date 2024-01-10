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

namespace StardewSoils
{
    public interface ISpaceCore
    {
        void RegisterSerializerType(Type type);
    }

    class LinkedTileSoilStats
    {
        public int Nitrogen = RandomGen.SoilRNG.Next(1,10);
        public int Phosphorus = RandomGen.SoilRNG.Next(1, 10);
        public int Potassium = RandomGen.SoilRNG.Next(1, 10);
        public Vector2 TilePos = new Vector2(-1,-1);
        public int CropType = -1; // No Crop is -1
        public bool aftergrowth = false;
        //public GameLocation Location;
        //public Crop crop;

        public LinkedTileSoilStats(Vector2 Tile)
        {
            TilePos = Tile;
            if (!TileList.AllRegisteredTiles.ContainsKey(TilePos))
            {
                TileList.AllRegisteredTiles.Add(TilePos, this);
            }
            GetCropOnTile(Tile);
        }

        public LinkedTileSoilStats()
        {
            //crop = null;
        }

        public void GrowthCheck()
        {
            GetCropOnTile(TilePos);
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
                if (CropNullCheck(TilePos, out TerrainFeature Crop) == false)
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
            if (CropNullCheck(TilePos, out TerrainFeature Crop) == false)
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

        public void GetCropOnTile(Vector2 TilePos)
        {
            if (CropNullCheck(TilePos, out TerrainFeature Crop) == false)
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

        public bool CropNullCheck(Vector2 TilePos, out TerrainFeature CurrentCrop)
        {
            Game1.getFarm().terrainFeatures.TryGetValue(TilePos, out TerrainFeature Crop);
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
        public static Dictionary<Vector2,LinkedTileSoilStats> AllRegisteredTiles = new Dictionary<Vector2, LinkedTileSoilStats>();
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
            helper.Events.GameLoop.DayEnding += (Obj, eventarg) => this.OnDayEnd(Obj, eventarg, helper);
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
                    Vector2 Pos = Game1.GlobalToLocal(new Vector2(Tile.Key.X * Game1.tileSize, Tile.Key.Y * Game1.tileSize));
                    e.SpriteBatch.Draw(OutLine, Pos, Color.White);
                    string TileMessage = "N: " + Tile.Value.Nitrogen + " \nP: " + Tile.Value.Phosphorus + " \nK: " + Tile.Value.Potassium;
                    e.SpriteBatch.DrawString(Game1.dialogueFont, TileMessage, Pos, Color.Black, 0.0f, new Vector2(0,0), 0.5f, SpriteEffects.None, 0);;
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

                if (TileTilled.Value is HoeDirt && !TileList.AllRegisteredTiles.ContainsKey(TileTilled.Key) && Check == "T")
                {
                    var NewTile = new LinkedTileSoilStats(TileTilled.Key);
                }
            }
            
        }

        private void OnDayStart(object sender, DayStartedEventArgs e, IModHelper helper)
        {
            foreach (var Tile in TileList.AllRegisteredTiles)
            {
                Tile.Value.GetCropOnTile(Tile.Key); // Finds if crop is on tile if crop is -1 no crop is not on tile
                if (Tile.Value.CropType != -1)
                {
                    Tile.Value.GrowthCheck();
                }
            }
        }

        private void OnDayEnd(object sender, DayEndingEventArgs e, IModHelper helper)
        {
            SaveHandler.WriteDataToFile(helper);
        }

        private void SaveLoader(object sender, SaveLoadedEventArgs e, IModHelper helper)
        {
            SaveHandler.LoadData(helper);
        }
    }
}
