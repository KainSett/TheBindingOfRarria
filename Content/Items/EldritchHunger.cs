using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

/*public class EldritchHunger : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 30;
        Item.height = 48;
        Item.rare = ItemRarityID.Expert;
        Item.value = Item.sellPrice(0, 7);
        Item.expert = true;
    }


    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        var p = player.GetModPlayer<BlackHolePlayer>();
        p.hunger = true;

        player.SpawnProjectileIfNotSpawned(ModContent.ProjectileType<BlackHoleAura>(), player.GetProjectileSource_Accessory(Item), 10);
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        int index = tooltips.FindIndex(t => t.Name == "Tooltip0");
        if (index != -1)
        {
            string text = string.Format(Language.GetTextValue("Mods.TheBindingOfRarria.Items.UnendingDespair.Tooltip"), $"{Main.LocalPlayer.statLifeMax2 / 10}");

            text = text[..text.LastIndexOf($"\n")];
            text = text[..text.LastIndexOf($"\n")];
            tooltips[index].Text = text;
        }
    }
}

public class BlackHolePlayer : ModPlayer
{
    public static string StatSource = "BlackHole";

    public bool hunger = false;

    public override void ResetEffects()
    {
        hunger = false;
    }

    public override void PostUpdateBuffs()
    {
        foreach (var a in Player.GetModPlayer<TemporaryLifePlayer>().bonuses)
        {
            if (a.Source == StatSource)
            {
                Player.statDefense += a.Amount;
            }
        }
    }
}
*/