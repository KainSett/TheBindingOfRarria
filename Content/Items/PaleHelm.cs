using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.Localization;
using TheBindingOfRarria.Common.Config;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.Projectiles;
using TheBindingOfRarria.Content.Tiles;

namespace TheBindingOfRarria.Content.Items;

[AutoloadEquip(EquipType.Head)]
public class PaleHelm : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 20;
        Item.defense = 13;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 0, 50, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.aggro += 100;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head.type == Item.type && body.type == ModContent.ItemType<PalePlatemail>() && legs.type == ModContent.ItemType<PaleGreaves>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.SetBonus");

        player.GetModPlayer<PalePlayer>().Set = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<PaleOre>(), 8)
            .AddTile(ModContent.TileType<TeslaStationTile>())
            .Register();
    }
}

public class PalePlayer : ModPlayer
{
    public bool Set = false;

    public int counter = 0;

    public override void ResetEffects()
    {
        Set = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Set)
        {
            int frames = counter * 60 / 1800;

            Player.AddImmuneTime(info.CooldownCounter, frames);
            Player.SpawnProjectileIfNotSpawned(ModContent.ProjectileType<StalwartAura>(), Player.GetProjectileSource_Item(Player.armor[0]));

            counter = 0;
        }
    }

    public override void PostUpdate()
    {
        if (Set)
        {
            counter = Math.Clamp(counter + 1, 0, 1800);
        }
    }
}