using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Content.Buffs;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class AetherRose : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[ItemID.ObsidianRose] = Type;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ObsidianRose;
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 2);
        Item.rare = ItemRarityID.Orange;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.manaCost -= 0.06f;
        player.GetModPlayer<IcathiaPlayer>().IcathianRain = Item;
    }

    

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        float value = Main.LocalPlayer.statManaMax2;

        string text = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.Tooltip"), $"{(int)(value / 40)}");

        int index = tooltips.FindIndex(line => line.Name == "Tooltip0");
        if (index != -1)
        {
            text = text[..text.IndexOf($"\n")];
            tooltips[index].Text = text;
        }
    }
}

public class IcathiaPlayer : ModPlayer
{
    public Item IcathianRain = null;

    public bool Activated = false;

    public override void ResetEffects()
    {
        IcathianRain = null;
    }

    public override void PostUpdateMiscEffects()
    {
        if (IcathianRain != null && !Activated && Player.statMana < Player.statManaMax2 / 2)
        {
            Activated = true;

            for (int i = 0; i < Player.statManaMax2 / 40; i++)
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(IcathianRain, "AetherRose missiles"), Player.Center, new Vector2(0, -8).RotatedByRandom(PiOver4), ModContent.ProjectileType<HomingMissile>(), 10, 2, ai0: -1, ai1: 3);
            }
        }

        if (Player.statMana >= Player.statManaMax2)
            Activated = false;
    }
}