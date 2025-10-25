using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Doctrine : IArtifactSet
{
    public string Name => nameof(Doctrine);

    public Color NameColor => Color.Violet;

    public List<int> Items =>
        [ItemID.PhilosophersStone,
        ItemType<Multibinder>(),
        ItemType<InjectorBand>()];

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<DoctrinePlayer>(out var p))
            p.doctrine = true;
    }
}

public class DoctrinePlayer : ModPlayer
{
    public bool doctrine = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new Doctrine());
    }

    public override void ResetEffects()
    {
        doctrine = false;
    }
}