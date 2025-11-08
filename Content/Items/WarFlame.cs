using System;
using System.Collections.Generic;
using System.Linq;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class WarFlame : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 34;
        Item.height = 44;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(0, 0, 1, 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<WarPlayer>().Flame = true;
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.ExplosivePowder, 10)
            .AddIngredient(ItemID.LivingFireBlock, 10)
            .AddIngredient(ItemID.LivingCursedFireBlock, 10)
            .AddIngredient(ItemID.LivingFrostFireBlock, 10)
            .AddIngredient(ItemID.LivingDemonFireBlock, 10)
            .AddTile(TileID.Hellforge)
            .Register();
    }
}

public class WarPlayer : ModPlayer
{
    public static Dictionary<int, int> flames = [];

    public bool Flame = false;

    public override void ResetEffects() => Flame = false;

    public override void Load()
    {
        flames.Clear();

        flames.Add(BuffID.OnFire, 4);
        flames.Add(BuffID.OnFire3, 15);
        flames.Add(BuffID.Frostburn, 8);
        flames.Add(BuffID.Frostburn2, 25);
        flames.Add(BuffID.ShadowFlame, 15);
        flames.Add(BuffID.CursedInferno, 24);

        On_NPC.AddBuff += ExplodeDebuffs;
    }

    private static void ExplodeDebuffs(On_NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet)
    {

        if (self.CountsAsACritter || self.buffImmune[type] || !flames.TryGetValue(type, out int value) || !Main.player.Any(p => p.active && p.GetModPlayer<WarPlayer>().Flame && p.Center.DistanceSQ(self.Center) < 700 * 700))
        {
            orig(self, type, time, quiet);
            return;
        }

        var Time = self.HasBuff(type) ? self.buffTime[self.FindBuffIndex(type)] : 0;

        if (self.HasBuff(BuffID.Oiled))
            value += 25;

        var dmg = value * (1 / 120f * (time - Time));
        self.SimpleStrikeNPC((int)dmg, 0);


        time -= (time - Time) / 2;

        orig(self, type, time, quiet);
    }
}