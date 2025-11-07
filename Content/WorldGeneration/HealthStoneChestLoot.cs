using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.WorldGeneration;

public class HealthStoneChestLoot : ModSystem
{
    public static int HAmount = 0;
    public override void PostWorldGen()
    {
        if (!Main.masterMode)
            return;

        for (int chestIndex = Main.maxChests; chestIndex > 2; chestIndex -= 2)
        {
            Chest chest = Main.chest[chestIndex];

            if (chest == null)
                continue;

            if (HAmount < 6 && (HAmount < 3 || WorldGen.genRand.NextFloat() > 0.8f) && chest.item.Any(item => item.type == ItemID.BandofRegeneration || item.type == ItemID.MagicMirror || item.type == ItemID.CloudinaBottle || item.type == ItemID.HermesBoots))
            {
                for (int inventoryIndex = Chest.maxItems - 1; inventoryIndex > 0; inventoryIndex--)
                {
                    chest.item[inventoryIndex].SetDefaults(chest.item[inventoryIndex - 1].type);
                    if (inventoryIndex == 1)
                    {
                        HAmount++;
                        chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<HealthStone>());
                        break;
                    }
                }

            }
        }
    }
}