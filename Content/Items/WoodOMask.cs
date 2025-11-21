using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Common;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

/*public class WoodOMask : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(0, 0, 7, 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<PoisonMinionsPlayer>().mask = Item;
        player.maxMinions += 1;
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.PygmyNecklace)
            .AddIngredient(ItemID.RichMahogany, 13)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}

public class PoisonMinionsPlayer : ModPlayer
{
    public Item mask = null;

    public override void ResetEffects() => mask = null;

    public override void Load()
    {
        On_NPC.AddBuff += SendMissilesAtVulnerableKiddies;
    }

    private static void SendMissilesAtVulnerableKiddies(On_NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet)
    {
        if (self.CountsAsACritter || self.HasBuff(type) || !Main.debuff[type] || self.buffImmune[type])
        {
            orig(self, type, time, quiet);
            return;
        }


        var shaman = Array.Find(Main.player, p => p.active && p.GetModPlayer<PoisonMinionsPlayer>().mask != null && p.Center.DistanceSQ(self.Center) < 700 * 700);
        if (shaman != null)
        {
            var minions = Array.FindAll(Main.projectile, p => p.active && p.owner == shaman.whoAmI && p.minion);
            for (int i = 0; i < minions.Length; i++) 
            {
                Projectile.NewProjectile(shaman.GetSource_Accessory(shaman.GetModPlayer<PoisonMinionsPlayer>().mask, "WoodOMask missiles"), minions[i].Center, new Vector2(0, -8), ModContent.ProjectileType<HomingMissile>(), 20, 2, ai0: -1, ai1: 1);
            } 
        }

        orig(self, type, time, quiet);
    }
}*/