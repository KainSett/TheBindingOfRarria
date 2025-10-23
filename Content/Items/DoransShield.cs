using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.Buffs;

namespace TheBindingOfRarria.Content.Items;

public class DoransShield : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 26;
        Item.height = 30;
        Item.rare = ItemRarityID.Master;
        Item.master = true;
        Item.value = Item.buyPrice(0, 4, 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<TankStartPlayer>().AgainstAnADC = true;

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.InsertArtifactSetBonusTooltip(Type);
    }
}

public class TankStartPlayer : ModPlayer
{
    public bool AgainstAnADC = false;

    public override void ResetEffects() => AgainstAnADC = false;

    public override void NaturalLifeRegen(ref float regen)
    {
        if (Player.HasBuff(ModContent.BuffType<SecondBreath>()))
        {
            Player.lifeRegenTime += 3;
        }
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (AgainstAnADC)
        {
            Player.AddBuff(ModContent.BuffType<SecondBreath>(), 600);
        }
    }
}

public partial class DoranItemsNPCShop : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.Merchant)
        {
            PatienceShield(shop);

            PetriCuffs(shop);
        }
    }

    public void PatienceShield(NPCShop shop)
    {
        if (shop.TryGetEntry(ItemID.IronAnvil, out var entry))
            shop.InsertAfter(entry, new Item(ModContent.ItemType<DoransShield>()), Condition.InMasterMode);

        else if (shop.TryGetEntry(ItemID.LeadAnvil, out entry))
            shop.InsertAfter(entry, new Item(ModContent.ItemType<DoransShield>()), Condition.InMasterMode);
    }
}