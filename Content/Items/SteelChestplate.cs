using System;
using Terraria.Localization;
using TheBindingOfRarria.Common.Config;
using TheBindingOfRarria.Content.Buffs;

namespace TheBindingOfRarria.Content.Items;
/*
[AutoloadEquip(EquipType.Body)]
public class SteelChestplate : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 28;
        Item.value = Item.sellPrice(0, 4);
        Item.defense = 5;
        Item.rare = ItemRarityID.Orange;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head is not null && legs is not null && !head.IsAir && !legs.IsAir && body.type == Type;
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.SetBonus");

        player.GetModPlayer<SteelPlayer>().Iron = true;
    }
}
public class SteelPlayer : ModPlayer
{
    public const int duration = 1800;

    public bool Iron = false;

    public int Stacks = 0;

    public override void ResetEffects()
    {
        if (!Player.HasBuff(ModContent.BuffType<SteelEndurance>()))
            Stacks = 0;
        Iron = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Iron)
        {
            if (!Player.HasBuff(ModContent.BuffType<SteelEndurance>()) || (Player.HasBuff(ModContent.BuffType<SteelEndurance>()) && duration / 2 > Player.buffTime[Array.FindIndex(Player.buffType, e => e == ModContent.BuffType<SteelEndurance>())]))
            {
                Stacks++;
                if (Stacks > 5)
                    Stacks = 5;
                Player.AddBuff(ModContent.BuffType<SteelEndurance>(), duration);
            }
            else Player.AddBuff(ModContent.BuffType<SteelEndurance>(), duration);
        }
        else if (Player.HasBuff(ModContent.BuffType<SteelEndurance>()))
            Player.ClearBuff(ModContent.BuffType<SteelEndurance>());
    }
}*/