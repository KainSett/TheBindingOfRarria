using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.ArtifactSets;
using TheBindingOfRarria.Content.Tiles;

namespace TheBindingOfRarria.Content.Items;

public class RobotInternals : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.height = 32;
        Item.width = 32;
        Item.accessory = true;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 1, 60, 0);
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<RobotConversionPlayer>().Robot = true;
        player.statDefense += player.GetModPlayer<RobotConversionPlayer>().counter * 3;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.InsertArtifactSetBonusTooltip(Type);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Timer1Second)
            .AddIngredient(ItemID.SoulofMight, 10)
            .AddIngredient(ModContent.ItemType<Fulgurbloom>(), 6)
            .AddIngredient(ItemID.IronBar, 10)
            .AddIngredient(ModContent.ItemType<CopperWire>(), 10)
            .AddIngredient(ItemID.WirePipe, 2)
            .AddTile(ModContent.TileType<TeslaStationTile>())
            .Register();
    }
}

public class RobotConversionPlayer : ModPlayer
{
    public bool Robot = false;

    public int counter = 0;

    public int timer = 0;

    public override void ResetEffects()
    {
        Robot = false;
    }

    public override void Load()
    {
        On_Player.UpdateLifeRegen += On_Player_UpdateLifeRegen;
    }

    public override void Unload()
    {
        On_Player.UpdateLifeRegen -= On_Player_UpdateLifeRegen;
    }

    private static void On_Player_UpdateLifeRegen(On_Player.orig_UpdateLifeRegen orig, Player self)
    {
        orig(self);

        var p = self.GetModPlayer<RobotConversionPlayer>();
        if (p.Robot)
        {
            if (p.timer != 0 && p.counter != self.lifeRegen / 2)
            {
                p.timer--;
                return;
            }
            p.counter = self.lifeRegen / 2;
            p.timer = 60;
        }
    }

    public override void UpdateBadLifeRegen()
    {
        if (Robot)
            Player.lifeRegen -= counter / (Player.GetModPlayer<AndroidPlayer>().android ? 2 : 1);

        Player.GetModPlayer<AndroidPlayer>().android = false;
    }
}