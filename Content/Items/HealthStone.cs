using System;
using System.Collections.Generic;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Buffs;

namespace TheBindingOfRarria.Content.Items;

public class HealthStone : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.height = 44;
        Item.width = 42;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Master;
        Item.master = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<HealthHeartPlayer>().Health = true;
        player.GetModPlayer<HealthHeartPlayer>().Power++;
    }
}

public class HealthHeartPlayer : ModPlayer
{
    public int Power = 0;

    public bool Health = false;

    public Dictionary<int, int> Buffs = [];

    public override void ResetEffects() => Health = false;
    public override void Load()
    {
        base.Load();
        On_Player.AddBuff_ActuallyTryToAddTheBuff += On_Player_AddBuff_ActuallyTryToAddTheBuff;
        On_Player.AddBuff_DetermineBuffTimeToAdd += HealthBuffTime;
    }

    private static int HealthBuffTime(On_Player.orig_AddBuff_DetermineBuffTimeToAdd orig, Player self, int type, int time1)
    {
        int buffTime = orig(self, type, time1);

        if (!self.GetModPlayer<HealthHeartPlayer>().Health)
            return buffTime;

        else if (!Main.debuff[type])
            return (int)(1.2f * buffTime);

        else
            return buffTime;
    }

    private static bool On_Player_AddBuff_ActuallyTryToAddTheBuff(On_Player.orig_AddBuff_ActuallyTryToAddTheBuff orig, Player self, int type, int time)
    {
        if (time > 3 && !self.HasBuff(type) && !Main.debuff[type] && self.TryGetModPlayer<HealthHeartPlayer>(out var p) & p.Health && !p.Buffs.ContainsKey(type))
        {
            if (p.Power > 1)
                self.GetModPlayer<TemporaryLifePlayer>().bonuses.Add(new LifeBonus(p.Power * 10, time, cond => self.dead, "Health"));

            for (int i = 0; i < p.Power; i++)
            {
                Main.item[Item.NewItem(Item.GetSource_None(), self.Center, ItemID.Heart, noGrabDelay: false)].noGrabDelay = 100;
            }

            p.Buffs.Add(type, time);
        }

        return orig(self, type, time);
    }

    public override void PreUpdate()
    {
        foreach(var b in Buffs)
        {
            Buffs[b.Key] = Math.Max(0, b.Value - 1);
            if (Buffs[b.Key] == 0)
                Buffs.Remove(b.Key);
        }

        Power = 0;
    }
}