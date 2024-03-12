using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using xTile.Tiles;

namespace StardewSoils
{
    internal static class GameMethods
    {
        // Send TileList from host to all peers
        public static void SendHostTiles(object obj, PeerConnectedEventArgs eventarg, IModHelper helper, string ID)
        {
            //Only the host needs to send the tilelist
            if (Game1.IsMasterGame)
            {
                foreach (var Tile in TileList.AllRegisteredTiles)
                {
                    string TileData = TileSoilBuilder.ClassToSoilData(Tile.Value);
                    string TileKey = TileSoilBuilder.PositionToKeyData(Tile.Key.pos);
                    // May not send game location
                    helper.Multiplayer.SendMessage(TileData, "TileDataSync", modIDs: new[] { ID });
                    helper.Multiplayer.SendMessage(TileKey, "TileKeySync", modIDs: new[] { ID });
                    helper.Multiplayer.SendMessage(Tile.Value.Location.ToString(), "TileKeyLocationSync", modIDs: new[] { ID });
                }
            }
        }

        // Recive Tiles and add them to your list
        public static void TileSync(object sender, ModMessageReceivedEventArgs e, IModHelper helper, string ID)
        {
            if (e.FromModID == ID)
            {
                // Build out Data and Key Handler (also work out Gamelocation)
                var Tile = e.ReadAs<TileSoil>();
                TileList.AllRegisteredTiles.Add(new TilePosAndLocation(Tile.TilePos, Tile.Location.ToString()), Tile);
                // Only the host needs to save the data
                if (Game1.IsMasterGame)
                {
                    var Location = Tile.Location;
                    var Key = TileSoilBuilder.PositionToKeyData(Tile.TilePos);
                    Location.modData[Key] = TileSoilBuilder.ClassToSoilData(Tile);
                }
            }
        }

        // Handles drawing the info overlay
        public static void RenderOverlay(object sender, RenderedEventArgs e, IModHelper helper, Texture2D OutLine)
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

        // Handles adding and sync tiles between players
        public static void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e, IModHelper helper, IMonitor monitor, string ID)
        {
            foreach (var TileTilled in e.Added)
            {

                GameLocation TileLocation = TileTilled.Value.currentLocation;
                var Check = TileLocation.doesTileHaveProperty((int)TileTilled.Key.X, (int)TileTilled.Key.Y, "Diggable", "Back");

                if (TileTilled.Value is HoeDirt && Check == "T")
                {
                    string TileData = TileSoilBuilder.GenerateSoilData();
                    string TileKey = TileSoilBuilder.PositionToKeyData(TileTilled.Key);
                    Game1.player.currentLocation.modData[TileKey] = TileData;
                    var Newtile = TileSoilBuilder.DataToClass(TileData, TileKey, TileLocation);
                    helper.Multiplayer.SendMessage(TileData, "TileDataSync", modIDs: new[] { ID });
                    helper.Multiplayer.SendMessage(TileKey, "TileKeySync", modIDs: new[] { ID });
                    helper.Multiplayer.SendMessage(TileLocation.ToString(), "TileKeyLocationSync", modIDs: new[] { ID });
                }
            }
        }

        // Handles growth of crops and saves
        public static void OnDayStart(object sender, DayStartedEventArgs e, IModHelper helper)
        {
            // Only the host needs to handle crop growth
            if (Game1.IsMasterGame)
            {
                foreach (var Tile in TileList.AllRegisteredTiles)
                {
                    Tile.Value.GrowthCheck();
                    Tile.Value.Location.modData[TileSoilBuilder.PositionToKeyData(Tile.Value.TilePos)] = TileSoilBuilder.ClassToSoilData(Tile.Value);
                }
            }
        }

        // Handles registering plantable locations
        public static void SaveLoader(object sender, SaveLoadedEventArgs e, IModHelper helper)
        {
            PlantableLocations.InitlizeLocations();
            foreach (GameLocation loc in PlantableLocations.Areas)
            {
                foreach (var Tile in loc.modData.Keys)
                {
                    var Value = loc.modData[Tile];
                    TileSoilBuilder.DataToClass(Value, Tile, loc);
                }
            }
        }

    }
}
