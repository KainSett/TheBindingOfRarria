using Terraria.GameContent.Drawing;
using Terraria.Localization;
using Terraria.ObjectData;
using TheBindingOfRarria.Common.Registries;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.Tiles;

public class TeslaStationTile : ModTile
{
    public override string Texture => ContentPath + "Tiles/" + Name;

    public override void SetStaticDefaults()
    {
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileSpelunker[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.DrawYOffset = 2;


        LocalizedText name = CreateMapEntryName();
        AddMapEntry(Color.DodgerBlue, name);
        RegisterItemDrop(ModContent.ItemType<TeslaStation>());
        DustType = DustID.Electric;

        TileObjectData.addTile(Type);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        var tile = Main.tile[i, j];
        if (!TileDrawing.IsVisible(tile))
            return;

        if (tile.TileFrameY > 1 && tile.TileFrameX  > 1)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            var texture = Textures.ElectricLink;
            var frame = texture.Frame(1, 3, 0, (int)((Main.GlobalTimeWrappedHourly * 13 + i) % 3), 0, -2);
            var offset = new Vector2(-10, -16);
            spriteBatch.Draw(texture.Value, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero + offset, frame, Color.White, 0, frame.Size() / 2, 1f, SpriteEffects.None, 0);
        }
    }
}