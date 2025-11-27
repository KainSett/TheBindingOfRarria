using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;

namespace TheBindingOfRarria.Content.Items;

public class BleedingPendant : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 32;
        Item.accessory = true;
        Item.lifeRegen = 2;
        Item.value = Item.sellPrice(0, 2);
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<BleedNerfPlayer>().Bleeds = true;
    }
}

public class BleedNerfPlayer : ModPlayer
{
    public bool Bleeds = false;

    public override void Load()
    {
        On_Player.UpdateLifeRegen += On_Player_UpdateLifeRegen;
    }

    public override void Unload()
    {
        On_Player.UpdateLifeRegen -= On_Player_UpdateLifeRegen;
    }

    private static void On_Player_UpdateLifeRegen(On_Player.orig_UpdateLifeRegen orig, Player self)
    {
        var player = self.GetModPlayer<BleedNerfPlayer>();

        if (player.Bleeds && self.lifeRegen < 0)
        {
            self.lifeRegen = (int)(self.lifeRegen * 0.67f);
        }

        player.Bleeds = false;

        orig(self);
    }
}