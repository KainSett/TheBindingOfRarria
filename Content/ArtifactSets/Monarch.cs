using System;
using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Monarch : IArtifactSet
{
    public string Name => nameof(Monarch);

    public Color NameColor => Color.DeepSkyBlue;

    public List<int> Items =>
        [ItemType<KingdomStone>(),
        ItemType<BlueSeal>(),
        ItemType<Sangreal>()];

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<MonarchPlayer>(out var p))
            p.monarch = true;
    }
}

public class MonarchPlayer : ModPlayer
{
    public bool monarch = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new Monarch());
    }
}