using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Registries;
using TheBindingOfRarria.Content.Projectiles;
using TheBindingOfRarria.Content.Tiles;

namespace TheBindingOfRarria.Content.Items;

public class H20Volt : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 36;
        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(silver: 90);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.TryGetModPlayer<H2VPlayer>(out var modPlayer))
            modPlayer.Equipped = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Timer1Second)
            .AddIngredient(ModContent.ItemType<Fulgurbloom>(), 4)
            .AddIngredient(ItemID.CopperBar, 2)
            .AddTile(ModContent.TileType<TeslaStationTile>())
            .Register();
    }
}

public class H2VPlayer : ModPlayer
{
    public bool Equipped;

    public int counter = 0;

    public override void ResetEffects() => Equipped = false;

    public override void PostUpdateEquips()
    {
        counter++;
        if (!Equipped || counter < 0)
            return;


        counter = -60;
        int count = 4;

        int[] npcs = Helper.GetTargetIndices(Player.Center, count, 350f);

        if (npcs != null)
        {
            for (int i = 0; i < npcs.Length; i++)
            {
                NPC npc = Main.npc[npcs[i]];

                if (npc == null)
                    continue;

                Vector2 start = Player.Center;
                Vector2 end = npc.Center;

                Helper.NewProjectileBetter(Player.GetSource_FromAI(), start, Vector2.Normalize(end - start) * 10f, ModContent.ProjectileType<LightningBolt>(), 7, 0f, Player.whoAmI, self =>
                {
                    LightningBolt.SetPositions(start, end, self, npc.whoAmI);
                });
            }
        }
    }
}
