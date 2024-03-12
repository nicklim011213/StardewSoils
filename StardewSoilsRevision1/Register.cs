using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.IO;
using StardewValley.Menus;

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

        // Register Textures
        private void RegisterTextures(IModHelper helper)
        {
            Texture2D OutLine = helper.ModContent.Load<Texture2D>(Path.Combine(helper.DirectoryPath, "Assets/Tile_Outline.png"));
            helper.Events.Display.Rendered += (Obj, eventarg) => GameMethods.RenderOverlay(Obj, eventarg, helper, OutLine);
        }

        // Register Event Handlers
        private void RegisterEvents(IModHelper helper, IMonitor monitor)
        {
            helper.Events.World.TerrainFeatureListChanged += (Obj, eventarg) => GameMethods.OnTerrainFeatureListChanged(Obj, eventarg, helper, monitor, ModManifest.UniqueID.ToString());
            helper.Events.GameLoop.DayStarted += (Obj, eventarg) => GameMethods.OnDayStart(Obj, eventarg, helper);
            helper.Events.GameLoop.GameLaunched += (Obj, eventarg) => this.RegisterItems(Obj, eventarg, helper);
            helper.Events.Display.MenuChanged += this.RegisterShop;
            helper.Events.GameLoop.SaveLoaded += (Obj, eventarg) => GameMethods.SaveLoader(Obj, eventarg, helper);
            helper.Events.Content.AssetRequested += (Obj, eventarg) => SoilQualityTextHandler.SeedDescriptionFixer(Obj, eventarg, helper , Monitor);
            helper.Events.Multiplayer.ModMessageReceived += (Obj, eventarg) => GameMethods.TileSync(Obj, eventarg, helper, ModManifest.UniqueID.ToString());
            helper.Events.Multiplayer.PeerConnected += (Obj, eventarg) => GameMethods.SendHostTiles(Obj, eventarg, helper, ModManifest.UniqueID.ToString());
        }

        // Registers custom items to shops
        private void RegisterShop(object sender, MenuChangedEventArgs e)
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

        // Registers custom items to show up in game
        private void RegisterItems(object sender, GameLaunchedEventArgs e, IModHelper helper)
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
    }
}
