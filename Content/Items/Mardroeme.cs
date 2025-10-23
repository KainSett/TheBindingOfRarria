using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using TheBindingOfRarria.Content.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TheBindingOfRarria.Content.Items;

public class Mardroeme : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 30;
        Item.maxStack = 99;
        Item.consumable = true;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.EatFood;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item2;
        Item.potionDelay = 3600;
        Item.healLife = 230;
        Item.potion = true;
    }

    public override bool CanUseItem(Player player)
    {
        if (player.potionDelay > 0 || player.statLife < 50)
            return false;
        return true;
    }

    public override bool? UseItem(Player player)
    {
        player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral($"{player.name} sacrificed their vitality.")), 50, 0, false, false, 0, false, 999);
        player.GetModPlayer<MardroemePlayer>().counter = 300;
        player.ApplyPotionDelay(Item);
        //player.GetHealLife(Item);

        return true;
    }

    public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
    {
        if (healValue == 0)
            healValue = player.GetModPlayer<MardroemePlayer>().heal;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        Main.LocalPlayer.GetHealLife(Item);
        Item.healLife = Main.LocalPlayer.GetModPlayer<MardroemePlayer>().heal;

        var index = tooltips.FindIndex(t => t.Name == "HealLife");

        if (index != -1)
        {
            var t = "";

            foreach (var ch in tooltips[index].Text)
            {
                if (char.IsNumber(ch))
                    t += ch;
            }

            if (t == "")
                t = "0";

            tooltips[index].Text = tooltips[index].Text.Replace(t, Item.healLife.ToString());
        }
    }
}

public class MardroemePlayer : ModPlayer
{
    public int counter = 0;

    public int heal = 230;

    public override void PostUpdate()
    {
        if (counter > 0)
        {
            counter--;
            if (counter == 0)
            {
                Player.Heal(heal);
            }
        }
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        if (item.type == ModContent.ItemType<Mardroeme>())
        {
            heal = healValue;
            healValue = 0;
        }
    }
}