using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using static TheBindingOfRarria.Content.Items.AegisPlayer;

namespace TheBindingOfRarria.Content.Items;

public class BeastCrest : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 34;
        Item.height = 42;
        Item.rare = ItemRarityID.Expert;
        Item.value = Item.sellPrice(0, 8, 80);
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.lifeSteal < 80)
        player.lifeSteal += 0.1f;

        player.GetModPlayer<BeastPlayer>().Beast = true;
        player.GetModPlayer<BeastPlayer>().counter--;
    }

}

public class BeastCrestItemNPCShop : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.BestiaryGirl)
        {
            shop.Add(new Item(ModContent.ItemType<BeastCrest>()), Condition.InExpertMode, Condition.DownedSkeletron);
        }
    }
}

public class BeastPlayer : ModPlayer
{
    public bool Beast = false;

    public Item healItem = null;

    public int counter = 0;

    public override void ResetEffects()
    {

        if (!Beast || counter <= 0)
            healItem = null;

        Beast = false;
    }

    public override void Load()
    {
        On_Player.ApplyLifeAndOrMana += On_Player_ApplyLifeAndOrMana;
    }

    private static void On_Player_ApplyLifeAndOrMana(On_Player.orig_ApplyLifeAndOrMana orig, Player self, Item item)
    {
        if (self.TryGetModPlayer<BeastPlayer>(out var p) && p.Beast && item.healLife > 0)
        {
            p.healItem = item;
            p.counter = 480;
            self.Heal(0);
        }

        else orig(self, item);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Beast || healItem == null || Player.lifeSteal <= 0 || !target.canGhostHeal || target.immortal || target.lifeMax <= 5)
            return;

        var amount = (int)(damageDone * (healItem.healLife / 150f) / 8f);

        Player.Heal(amount);
        Player.lifeSteal -= amount;
    }
}

public class BeastPotion : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        var index = tooltips.FindIndex(t => t.Name == "HealLife");

        if (index == -1 || !item.potion || !Main.LocalPlayer.GetModPlayer<BeastPlayer>().Beast)
            return;

        tooltips[index].Text = string.Format(Language.GetTextValue("Mods.TheBindingOfRarria.Items.BeastCrest.HealPot"), $"{(1f * (item.healLife / 150f) / 8f) * 100}");
    }
}