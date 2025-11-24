using System;
using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Buffs;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class NaturesBlessing : IArtifactSet
{
    public string Name => nameof(NaturesBlessing);

    public Color NameColor => Color.Goldenrod;

    public List<int> Items =>
        [ItemID.ShinyStone,
        ItemType<BlessedDewTalisman>(),
        ItemType<DoransShield>()];

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<NaturesBlessingPlayer>(out var p))
            p.NaturesBlessing = true;
    }
}

public class NaturesBlessingPlayer : ModPlayer
{
    public bool NaturesBlessing = false;

    public override void ResetEffects()
    {
        NaturesBlessing = false;
    }

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new NaturesBlessing());
    }
}