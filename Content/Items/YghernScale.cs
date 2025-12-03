
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;
using TheBindingOfRarria.Content.Buffs;
using System;

namespace TheBindingOfRarria.Content.Items;

public class YghernScale : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;
    public override void SetDefaults()
    {
        Item.consumable = true;
        Item.maxStack = 23;
        Item.width = 26;
        Item.height = 24;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 1, 12);
        Item.buffType = ModContent.BuffType<YghernBuff>();
        Item.buffTime = 600;
        Item.useStyle = ItemUseStyleID.Guitar;
        Item.UseSound = SoundID.Item29;
    }

    public override bool CanUseItem(Player player)
    {
        if (player.GetModPlayer<YghernAccPlayer>().counter > 0 || player.GetModPlayer<YghernAccPlayer>().Block > 0 || player.HasBuff(ModContent.BuffType<YghernBuff_CD>()))
            return false;

        return base.CanUseItem(player);
    }

    public override void OnConsumeItem(Player player)
    {
        player.GetModPlayer<YghernAccPlayer>().Block = 2;
        player.GetModPlayer<YghernAccPlayer>().counter = 1800;
        player.AddBuff(ModContent.BuffType<YghernBuff_CD>(), 1800);
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        float value = Main.LocalPlayer.statLifeMax2 / 10;

        string text = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.Tooltip"), $"{value}");

        int index = tooltips.FindIndex(line => line.Name == "Tooltip0");
        if (index != -1)
        {
            text = text[..text.LastIndexOf($"\n")];
            tooltips[index].Text = text;
        }
    }
}

public class YghernAccPlayer : ModPlayer
{
    public int Block = 0;

    public int counter = 0;

    public override void ResetEffects()
    {
        if (!Player.HasBuff(ModContent.BuffType<YghernBuff>()))
            Block = 0;

        if (!Player.HasBuff(ModContent.BuffType<YghernBuff_CD>()) && counter > 0)
            Player.AddBuff(ModContent.BuffType<YghernBuff_CD>(), counter);

        if (Block <= 0)
            Player.ClearBuff(ModContent.BuffType<YghernBuff>());
    }

    public override void PostUpdate() => counter = Math.Max(0, counter - 1);

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (Block > 0)
        {
            modifiers.SetMaxDamage(Player.statLifeMax2 / 10);
            Block--;
        }
    }
}