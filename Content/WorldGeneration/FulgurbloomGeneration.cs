using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using TheBindingOfRarria.Content.Tiles;

namespace TheBindingOfRarria.Content.WorldGeneration;

public class ElectroEarth : GlobalTile
{
    public static int counter = 0;

    public override void RandomUpdate(int x, int y, int type)
    {
        if (!Main.rand.NextBool(7000))
            return;

        if (Main.maxTilesY / 4 > y && Main.tile[x, y].TileType == TileID.Grass && WorldGen.TileEmpty(x, y - 1))
        {
            WorldGen.PlaceTile(x, y - 1, ModContent.TileType<FulgurbloomTile>());
            counter++;
        }
    }
}