using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.ArtifactSets;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class Multibinder : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 40;
        Item.height = 36;
        Item.rare = ItemRarityID.Master;
        Item.value = Item.sellPrice(0, 0, 7, 20);
        Item.master = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MultihealPlayer>().Multi = true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.InsertArtifactSetBonusTooltip(Type);
    }
}

public class MultibinderItemNPCShop : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.Merchant)
        {
            shop.Add(new Item(ModContent.ItemType<Multibinder>()), Condition.DownedSkeletron, Condition.InMasterMode);
        }
    }
}

public class MultihealPlayer : ModPlayer
{
    public bool Multi = false;

    public int counter = 0;

    public Item HealItem = null;

    public override void ResetEffects()
    {
        Multi = false;
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        if (Multi)
        {
            if (Player.TryGetModPlayer<DoctrinePlayer>(out var p) && p.doctrine && counter < 900 && HealItem is not null)
                healValue = (int)(healValue * 1.5f);
            else healValue = (int)(healValue * 0.85f);
        }
    }

    public override void Load()
    {
        On_Player.ApplyPotionDelay += On_Player_ApplyPotionDelay;
    }

    private static void On_Player_ApplyPotionDelay(On_Player.orig_ApplyPotionDelay orig, Player self, Item sItem)
    {
        var p = self.GetModPlayer<MultihealPlayer>();
        if (p.Multi && p.counter == 0)
        {
            p.counter = 900;
            p.HealItem = sItem;
        }

        orig(self, sItem);
    }

    public override void PreUpdate()
    {
        counter = (int)MathHelper.Max(0, counter - 1);

        if (counter <= 0 && HealItem != null)
        {
            // stolen from QuickHeal()
            SoundEngine.PlaySound(HealItem.UseSound, Player.position);
            ItemLoader.UseItem(HealItem, Player);
            Player.ApplyLifeAndOrMana(HealItem);
            if (HealItem.type == ItemID.Mushroom)
            {
                Player.TryToResetHungerToNeutral();
            }
            if (HealItem.buffType > 0)
            {
                int num = HealItem.buffTime;
                if (num == 0)
                {
                    num = 3600;
                }
                Player.AddBuff(HealItem.buffType, num);
            }
            //

            HealItem = null;
            counter = 0;
        }
    }
}