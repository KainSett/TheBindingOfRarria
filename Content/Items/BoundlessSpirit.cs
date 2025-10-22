using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using TheBindingOfRarria.Content.Buffs;
using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Content.ArtifactSets;

namespace TheBindingOfRarria.Content.Items;

public class BoundlessSpirit : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 34;
        Item.value = Item.sellPrice(gold: 7);
        Item.rare = ItemRarityID.Yellow;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.magicDamage.Multiplicative += 0.1f;
        player.GetModPlayer<BoundlessSpiritPlayer>().HasTier4 = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GalaxyPearl)
            .AddIngredient(ItemID.ArcaneCrystal)
            .AddIngredient(ItemID.ManaCrystal, 2)
            .AddIngredient(ItemID.Ectoplasm, 27)
            .AddCondition(Condition.NearShimmer)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        float value = Main.LocalPlayer.statManaMax2;

        string text = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.Tooltip"), $"{(int)(value / 10)}");

        int index = tooltips.FindIndex(line => line.Name == "Tooltip1");
        if (index != -1)
        {
            text = text[..text.LastIndexOf($"\n")];
            text = text[..text.LastIndexOf($"\n")];
            text = text[(text.LastIndexOf($"\n") + 1)..];
            tooltips[index].Text = text;
        }
    }
}

public class BoundlessSpiritPlayer : ModPlayer
{
    public bool HasTier4 = false;

    public bool Activated = false;

    public override void ResetEffects()
    {
        HasTier4 = false;
    }

    public override void PostUpdateMiscEffects()
    {
        if (HasTier4 && !Activated && Player.statMana < Player.statManaMax2 / 2)
        {
            Activated = true;

            Player.Heal(Player.statManaMax2 / 10);
            Player.AddBuff(ModContent.BuffType<SpiritRush>(), 240);

            if (Player.TryGetModPlayer<SpiritualismPlayer>(out var p) && p.spiritualism) 
            {
                p.spiritualism = false;
                Player.GetModPlayer<LifeSuckerPlayer>().counter = 300; 
            }
        }

        if (Player.statMana >= Player.statManaMax2) 
            Activated = false;
    }
}