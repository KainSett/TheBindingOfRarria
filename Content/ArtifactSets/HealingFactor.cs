using System;
using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class HealingFactor : IArtifactSet
{
    public string Name => "HealingFactor";

    public Color NameColor => Color.LightGreen;

    public List<int> Items =>
        [ItemID.BandofRegeneration,
        ItemType<HeartAmulet>(),
        ItemType<BloodBag>()];

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<HealingFactorPlayer>(out var p))
            p.HealingFactor = true;
    }
}

public class HealingFactorPlayer : ModPlayer
{
    public bool HealingFactor = false;

    public override void Load()
    {
        On_Player.UpdateLifeRegen += On_Player_UpdateLifeRegen;

        ArtifactSetSystem.ArtifactSets.Add(new HealingFactor());
    }

    private static void On_Player_UpdateLifeRegen(On_Player.orig_UpdateLifeRegen orig, Player self)
    {
        if (self.TryGetModPlayer<HealingFactorPlayer>(out var p) && p.HealingFactor && self.TryGetModPlayer<HeartAmuletPlayer>(out var plr))
        {
            self.lifeRegen = (int)(self.lifeRegen * plr.Power);
            p.HealingFactor = false;
        }

        orig(self);
    }
}