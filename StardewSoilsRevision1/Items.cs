using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.IO;
using System.Xml.Serialization;

namespace StardewSoils
{
    [XmlType("Mods_StardewSoilsSoilReader")]
    public class SoilReader : Item, ISalable
    {
        public static IModHelper helper;
        public static Texture2D Sprite;
        public SoilReader()
        {
            this.Category = Object.toolCategory;
            Sprite = helper.ModContent.Load<Texture2D>(Path.Combine(helper.DirectoryPath, "Assets/SoilReader.png"));
        }

        public override Item getOne()
        {
            return new SoilReader();
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(Sprite, location, new Rectangle(0, 0, 16, 16), Color.White * transparency, 0.0f, new Vector2(-2, -2), Game1.pixelZoom * scaleSize, SpriteEffects.None, 0);
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public override string DisplayName
        {
            get => "SoilReader";
            set
            {
                return;
            }
        }

        public override int Stack
        {
            get => 1;
            set
            {
                return;
            }
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override string getDescription()
        {
            return "Reads Soil Nutrients";
        }

        public override int addToStack(Item stack)
        {
            return 1;
        }

        public override int salePrice()
        {
            return 50;
        }
    }

    [XmlType("Mods_StardewSoilsFertilizerNitrogen1")]
    public class FertilizerNitrogen1 : StardewValley.Tool
    {
        public static IModHelper helper;
        public static Texture2D Sprite;

        public FertilizerNitrogen1()
        {
            this.Category = StardewValley.Object.toolCategory;
            Sprite = helper.ModContent.Load<Texture2D>(Path.Combine(helper.DirectoryPath, "Assets/Nitrogen.png"));
            this.Name = "Basic Nitrogen Fertilizer";
            this.BaseName = "Basic Nitrogen Fertilizer";
            this.InstantUse = true;
        }
        
        public override void leftClick(Farmer who)
        {
            // 0 is up 1 is right 2 is down 3 is left
            var Tile = who.getTileLocation();
            switch (who.FacingDirection)
            {
                case 0:
                    Tile.Y++;
                    break;
                case 1:
                    Tile.X++;
                    break;
                case 2:
                    Tile.Y--;
                    break;
                case 3:
                    Tile.X--;
                    break;
            }

            if (TileList.AllRegisteredTiles.ContainsKey(new TilePosAndLocation(Tile, who.currentLocation.ToString())))
            {
                TileList.AllRegisteredTiles[new TilePosAndLocation(Tile, who.currentLocation.ToString())].Nitrogen += 2;
                who.removeItemFromInventory(this);
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("Cant add fertilizer here. Make sure the tile has been tilled before"));
            }
            base.endUsing(who.currentLocation, who);
        }

        public override Item getOne()
        {
            return new FertilizerNitrogen1();
        }

        protected override string loadDisplayName()
        {
            return "Basic Nitrogen Fertilizer";
        }

        protected override string loadDescription()
        {
            return "Adds 2 Nitrogen to a tile";
        }

        public override int maximumStackSize()
        {
            return 50;
        }

        public override int addToStack(Item stack)
        {
            return 1;
        }

        public override int salePrice()
        {
            return 100;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(Sprite, location, new Rectangle(0, 0, 16, 16), Color.White * transparency, 0.0f, new Vector2(0, 0), Game1.pixelZoom * scaleSize, SpriteEffects.None, 0);
        }
    }

    [XmlType("Mods_StardewSoilsFertilizerPhosphorus1")]
    public class FertilizerPhosphorus1 : Tool
    {
        public static IModHelper helper;
        public static Texture2D Sprite;

        public FertilizerPhosphorus1()
        {
            this.Category = Object.toolCategory;
            Sprite = helper.ModContent.Load<Texture2D>(Path.Combine(helper.DirectoryPath, "Assets/Phosphorus.png"));
            this.Name = "Basic Phosphorus Fertilizer";
            this.BaseName = "Basic Phosphorus Fertilizer";
        }

        public override void leftClick(Farmer who)
        {
            // 0 is up 1 is right 2 is down 3 is left
            var Tile = who.getTileLocation();
            switch (who.FacingDirection)
            {
                case 0:
                    Tile.Y++;
                    break;
                case 1:
                    Tile.X++;
                    break;
                case 2:
                    Tile.Y--;
                    break;
                case 3:
                    Tile.X--;
                    break;
            }

            if (TileList.AllRegisteredTiles.ContainsKey(new TilePosAndLocation(Tile, who.currentLocation.ToString())))
            {
                TileList.AllRegisteredTiles[new TilePosAndLocation(Tile, who.currentLocation.ToString())].Phosphorus += 2;
                who.removeItemFromInventory(this);
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("Cant add fertilizer here. Make sure the tile has been tilled before"));
            }
            base.endUsing(who.currentLocation, who);
        }

        public override void draw(SpriteBatch b)
        {
            //base.draw(b);
        }

        public override Item getOne()
        {
            return new FertilizerPhosphorus1();
        }

        protected override string loadDisplayName()
        {
            return "Basic Phosphorus Fertilizer";
        }

        protected override string loadDescription()
        {
            return "Adds 2 Phosphorus to a tile";
        }

        public override int maximumStackSize()
        {
            return 50;
        }

        public override int addToStack(Item stack)
        {
            return 1;
        }

        public override int salePrice()
        {
            return 100;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(Sprite, location, new Rectangle(0, 0, 16, 16), Color.White * transparency, 0.0f, new Vector2(0, 0), Game1.pixelZoom * scaleSize, SpriteEffects.None, 0);
        }
    }

    [XmlType("Mods_StardewSoilsFertilizerPotassium1")]
    public class FertilizerPotassium1 : Tool
    {
        public static IModHelper helper;
        public static Texture2D Sprite;

        public FertilizerPotassium1()
        {
            this.Category = Object.toolCategory;
            Sprite = helper.ModContent.Load<Texture2D>(Path.Combine(helper.DirectoryPath, "Assets/Potassium.png"));
            this.Name = "Basic Potassium Fertilizer";
            this.BaseName = "Basic Potassium Fertilizer";
        }

        public override void leftClick(Farmer who)
        {
            // 0 is up 1 is right 2 is down 3 is left
            var Tile = who.getTileLocation();
            switch (who.FacingDirection)
            {
                case 0:
                    Tile.Y++;
                    break;
                case 1:
                    Tile.X++;
                    break;
                case 2:
                    Tile.Y--;
                    break;
                case 3:
                    Tile.X--;
                    break;
            }

            if (TileList.AllRegisteredTiles.ContainsKey(new TilePosAndLocation(Tile, who.currentLocation.ToString())))
            {
                TileList.AllRegisteredTiles[new TilePosAndLocation(Tile, who.currentLocation.ToString())].Potassium += 2;
                who.removeItemFromInventory(this);
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("Cant add fertilizer here. Make sure the tile has been tilled before"));
            }
            base.endUsing(who.currentLocation, who);
        }

        public override void draw(SpriteBatch b)
        {
            //base.draw(b);
        }

        public override Item getOne()
        {
            return new FertilizerPotassium1();
        }

        protected override string loadDisplayName()
        {
            return "Basic Potassium Fertilizer";
        }

        protected override string loadDescription()
        {
            return "Adds 2 Potassium to a tile";
        }

        public override int maximumStackSize()
        {
            return 50;
        }

        public override int addToStack(Item stack)
        {
            return 1;
        }

        public override int salePrice()
        {
            return 100;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(Sprite, location, new Rectangle(0, 0, 16, 16), Color.White * transparency, 0.0f, new Vector2(0, 0), Game1.pixelZoom * scaleSize, SpriteEffects.None, 0);
        }
    }
}
