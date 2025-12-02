using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class WintersBane : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 48;
        Item.height = 50;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 1, 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<WinterPlayer>().Winter = true;
        player.statManaMax2 += 50;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BandofStarpower)
            .AddIngredient(ModContent.ItemType<BlueSeal>())
            .AddIngredient(ItemID.ManaCrystal)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        float value = Main.LocalPlayer.statManaMax2;

        string text = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.Tooltip"), $"{(int)(value / 5)}");

        int index = tooltips.FindIndex(line => line.Name == "Tooltip1");
        if (index != -1)
        {
            text = text[..text.LastIndexOf($"\n")];
            text = text[(text.LastIndexOf($"\n") + 1)..];
            tooltips[index].Text = text;
        }
    }
}

public class WinterPlayer : ModPlayer
{
    public bool Winter = false;

    public override void ResetEffects()
    {
        Winter = false;
    }

    public override void PostUpdateEquips()
    {
        if (Winter)
        {
            Player.statLifeMax2 += Player.statManaMax2 / 5;
        }
    }
}