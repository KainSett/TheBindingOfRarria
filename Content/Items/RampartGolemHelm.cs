using System;
using Terraria.Localization;

namespace TheBindingOfRarria.Content.Items;

[AutoloadEquip(EquipType.Head)]
public class RampartGolemHelm : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 18;
        Item.defense = 12;
        Item.lifeRegen = 2;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(0, 4, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Generic) += 0.08f;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head.type == Item.type && body.type == ModContent.ItemType<RampartGolemChestplate>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.SetBonus");

        player.GetModPlayer<RampartPlayer>().Rampart = true;
    }

    public override void ArmorSetShadows(Player player)
    {
        player.armorEffectDrawOutlines = true;
    }
}

public class RampartPlayer : ModPlayer
{
    public bool Rampart = false;

    public int counter = 0;

    public (int source, int damage) hit = (0, 0);

    public override void ResetEffects()
    {
        if (!Rampart)
            hit = (0, 0);

        Rampart = false;
    }

    public override void PostUpdate()
    {
        counter = (counter + 1) % 60;

        if (counter == 0)
        { 
            hit.damage = Math.Max(0, hit.damage - 2);
            hit.source = Math.Max(0, hit.source - 2);
        }
    }

    public override void Load()
    {
        On_Player.Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float += StoreHitRampart;
    }

    private static double StoreHitRampart(On_Player.orig_Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float orig, Player self, PlayerDeathReason damageSource, int Damage, int hitDirection, out Player.HurtInfo info, bool pvp, bool quiet, int cooldownCounter, bool dodgeable, float armorPenetration, float scalingArmorPenetration, float knockback)
    {
        if (self.TryGetModPlayer<RampartPlayer>(out var p) && p.Rampart)
        {
            if (p.hit != (0, 0))
            {
                var D = Damage;
                Damage = p.hit.source;
                p.hit.source = D;
            }
            var dmg = orig(self, damageSource, Damage, hitDirection, out info, pvp, quiet, cooldownCounter, dodgeable, armorPenetration, scalingArmorPenetration, knockback);
            if (p.hit == (0, 0))
            {
                p.hit.source = Damage;
            }
            else
            {
                dmg = p.hit.damage;
                info.SourceDamage = p.hit.source;
                info.Damage = (int)dmg;
            }

            Player.HurtModifiers hurtModifiers = new()
            {
                DamageSource = damageSource,
                PvP = pvp,
                CooldownCounter = cooldownCounter,
                Dodgeable = dodgeable,
                HitDirection = hitDirection,
            };
            Player.HurtModifiers modifiers = hurtModifiers;
            PlayerLoader.ModifyHurt(self, ref modifiers);
            modifiers.ArmorPenetration += armorPenetration;
            modifiers.ScalingArmorPenetration += scalingArmorPenetration;
            self.ApplyVanillaHurtEffectModifiers(ref modifiers);
            var Info = modifiers.ToHurtInfo(Damage, self.statDefense, pvp ? 0.5f : self.DefenseEffectiveness.Value, knockback, self.noKnockback);

            p.hit.damage = (int)Info.Damage;

            return dmg;
        }
        else return orig(self, damageSource, Damage, hitDirection, out info, pvp, quiet, cooldownCounter, dodgeable, armorPenetration, scalingArmorPenetration, knockback);
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (Player.TryGetModPlayer<RampartPlayer>(out var p) && p.Rampart && p.hit == (0, 0))
        {
            modifiers.Cancel();
            Player.AddImmuneTime(modifiers.CooldownCounter, 60);
            Player.immune = true;
        }
    }
}