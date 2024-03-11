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

namespace StardewSoils
{
    public interface ISpaceCore
    {
        void RegisterSerializerType(Type type);
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
            helper.Events.Multiplayer.ModMessageReceived += (Obj, eventarg) => this.TileSync(Obj, eventarg, helper);
            helper.Events.Multiplayer.PeerConnected += (Obj, eventarg) => this.SendHostTiles(Obj, eventarg, helper);
        }

        private void SendHostTiles(object obj, PeerConnectedEventArgs eventarg, IModHelper helper)
        {
            if (Game1.IsMasterGame)
            {
                foreach(var Tile in TileList.AllRegisteredTiles)
                {
                    helper.Multiplayer.SendMessage<LinkedTileSoilStats>(Tile.Value, "TileSync", modIDs: new[] { this.ModManifest.UniqueID });
                }
            }
        }

        private void TileSync(object sender, ModMessageReceivedEventArgs e, IModHelper helper)
        {
            if (e.FromModID == this.ModManifest.UniqueID)
            {
                var Tile = e.ReadAs<LinkedTileSoilStats>();
                TileList.AllRegisteredTiles.Add(new TilePosAndLoc(Tile.TilePos, Tile.Location.ToString()), Tile);
                if (Game1.IsMasterGame)
                {
                    var Location = Tile.Location;
                    var Key = SoilDataBuilder.PositionToKeyClass(Tile.TilePos);
                    Location.modData[Key] = SoilDataBuilder.ClassToValueString(Tile);
                }
            }
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
                    var Newtile = SoilDataBuilder.StringToClass(TileData, TileKey, TileLocation);
                    helper.Multiplayer.SendMessage<LinkedTileSoilStats>(Newtile, "TileSync", modIDs: new[] {this.ModManifest.UniqueID});
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
