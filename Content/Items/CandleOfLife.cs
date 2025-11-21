using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class CandleOfLife : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 34;
        Item.height = 48;
        Item.rare = ItemRarityID.Yellow;
        Item.lifeRegen = 4;
        Item.value = Item.sellPrice(0, 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<CandlePlayer>().candle = true;

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.WaterCandle)
            .AddIngredient(ModContent.ItemType<BloodBag>())
            .AddIngredient(ModContent.ItemType<BlessedDewTalisman>())
            .AddIngredient(ModContent.ItemType<LifeJuice>(), 3)
            .AddTile(TileID.LivingLoom)
            .Register();
    }
}

public class CandlePlayer : ModPlayer
{
    public bool candle = false;

    public override void PreUpdateBuffs()
    {
        if (candle)
        {
            Player.statLifeMax2 += (int)MathHelper.Max(0, Player.statLifeMax2 - Player.statLife);
        }
        candle = false;
    }

    public override void NaturalLifeRegen(ref float regen)
    {
        Player.lifeRegenTime += 4f;
    }
}