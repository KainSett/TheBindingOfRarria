using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using TheBindingOfRarria.Content.Buffs;

namespace TheBindingOfRarria.Content.Items;

public class MortalWard : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 44;
        Item.height = 50;
        Item.rare = ItemRarityID.Master;
        Item.value = Item.sellPrice(0, 0, 7, 20);
        Item.master = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        var p = player.GetModPlayer<DeathsDoorPlayer>();
        if (player.statLife <= player.statLifeMax2 / 3 && !p.CanDie)
        {
            player.AddBuff(ModContent.BuffType<DeathsDoor>(), 300);
            p.CanDie = true;
            p.life = player.statLife;
        }
        else if (player.statLife >= player.statLifeMax2)
        {
            p.CanDie = false;
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        float value = Main.LocalPlayer.statLifeMax2;

        string text = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.Tooltip"), $"{(int)(value / 3)}");

        int index = tooltips.FindIndex(line => line.Name == "Tooltip0");
        if (index != -1)
        {
            text = text[..text.IndexOf($"\n")];
            tooltips[index].Text = text;
        }
    }
}

public class SkeleBagLoot : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        if (item.type == ItemID.SkeletronBossBag)
        {
            IItemDropRule rule = new ItemDropWithConditionRule(ModContent.ItemType<MortalWard>(), 3, 1, 1, new Conditions.IsMasterMode());
            itemLoot.Add(rule);
        }
    }
}

public class DeathsDoorPlayer : ModPlayer
{
    public bool CanDie = false;

    public int life = 100;

    public override void Load()
    {
        On_Player.Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float += DontDie;
        On_Player.KillMeForGood += DontDie2;
        On_Player.Heal += DontHeal;
        On_Player.HealEffect += DontHeal2;
    }

    private void DontHeal2(On_Player.orig_HealEffect orig, Player self, int healAmount, bool broadcast)
    {
        orig(self, healAmount, broadcast);

        if (self.HasBuff(ModContent.BuffType<DeathsDoor>()))
            self.statLife = self.GetModPlayer<DeathsDoorPlayer>().life;
    }

    private void DontHeal(On_Player.orig_Heal orig, Player self, int amount)
    {
        orig(self, amount);

        if (self.HasBuff(ModContent.BuffType<DeathsDoor>()))
            self.statLife = self.GetModPlayer<DeathsDoorPlayer>().life;
    }

    private void DontDie2(On_Player.orig_KillMeForGood orig, Player self)
    {
        if (self.HasBuff(ModContent.BuffType<DeathsDoor>()))
            self.statLife = self.GetModPlayer<DeathsDoorPlayer>().life;

        orig(self);

        if (self.HasBuff(ModContent.BuffType<DeathsDoor>()))
            self.statLife = self.GetModPlayer<DeathsDoorPlayer>().life;
    }

    private double DontDie(On_Player.orig_Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float orig, Player self, PlayerDeathReason damageSource, int Damage, int hitDirection, out Player.HurtInfo info, bool pvp, bool quiet, int cooldownCounter, bool dodgeable, float armorPenetration, float scalingArmorPenetration, float knockback)
    {
        var dmg = orig(self, damageSource, Damage, hitDirection, out info, pvp, quiet, cooldownCounter, dodgeable, armorPenetration, scalingArmorPenetration, knockback);
        if (self.HasBuff(ModContent.BuffType<DeathsDoor>()))
            self.statLife = self.GetModPlayer<DeathsDoorPlayer>().life;

        return dmg; 
    }
}