using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Systems;

namespace TheBindingOfRarria.Content.Items;

public class HeartAmulet : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.height = 32;
        Item.width = 38;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.Master;
        Item.master = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<HeartAmuletPlayer>().HealingBooster = true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.InsertArtifactSetBonusTooltip(Type);
    }
}

public class HeartItemNPCShop : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.DD2Bartender)
        {
            shop.Add(new Item(ModContent.ItemType<HeartAmulet>()) 
            { 
                shopCustomPrice = 8, 
                shopSpecialCurrency = CustomCurrencyID.DefenderMedals 
            }, 
            Condition.InMasterMode, Condition.Hardmode);
        }
    }
}

public class HeartAmuletPlayer : ModPlayer
{
    public float Power = 1.2f;

    public bool HealingBooster = false;

    public bool Healed = false;

    public override void ResetEffects() => HealingBooster = false;
    public override void Load()
    {
        base.Load();
        On_Player.Heal += On_Player_Heal;
        On_Player.HealEffect += On_Player_HealEffect;
    }

    private static void On_Player_HealEffect(On_Player.orig_HealEffect orig, Player self, int healAmount, bool broadcast)
    {
        if (!self.GetModPlayer<HeartAmuletPlayer>().Healed && self.GetModPlayer<HeartAmuletPlayer>().HealingBooster)
        {
            int bonus = (int)(healAmount * (self.GetModPlayer<HeartAmuletPlayer>().Power - 1));
            if (bonus > 0)
            {
                self.statLife += bonus;
                healAmount += bonus;
            }
        }

        self.GetModPlayer<HeartAmuletPlayer>().Healed = false;
        orig(self, healAmount, broadcast);
    }

    private static void On_Player_Heal(On_Player.orig_Heal orig, Player self, int amount)
    {
        if (self.GetModPlayer<HeartAmuletPlayer>().HealingBooster)
        {
            amount = (int)(amount * self.GetModPlayer<HeartAmuletPlayer>().Power);
        }

        self.GetModPlayer<HeartAmuletPlayer>().Healed = true;
        orig(self, amount);
    }
}