using System;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace TheBindingOfRarria.Content.Items;

public class SpiritKnuckle : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.height = 28;
        Item.width = 30;
        Item.defense = 6;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 9, 60);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<SoulPlayer>().IsKnuckle = true;
        player.GetModPlayer<SoulPlayer>().IsSoul = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FleshKnuckles)
            .AddIngredient(ModContent.ItemType<SoulCatcher>())
            .AddIngredient(ItemID.SoulofNight, 6)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}

public partial class SoulPlayer : ModPlayer
{
    public int defense = 0;
    public int counter = 0;

    public override void PostUpdateEquips()
    {
        if (defense != 0)
            return;

        Player.statDefense += defense;

        counter = Math.Max(0, counter - 1);
        if (counter == 0)
            defense = 0;
    }

    public void OnHitWithKnuckle(ref NPC target)
    {
        var info = target.CalculateHitInfo(40, 1, damageType: DamageClass.Magic);
        defense = Math.Max(defense, info.Damage / 10);
        counter = 120;
        target.StrikeNPC(info);
    }
}