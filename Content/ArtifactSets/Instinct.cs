using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Instinct : IArtifactSet
{
    public string Name => "Instinct";

    public Color NameColor => Color.PaleVioletRed;

    public List<int> Items =>
        [ItemID.FeralClaws,
        ItemType<BeastCrest>(),
        ItemType<Longclaw>()];

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<InstinctPlayer>(out var p))
            p.instinct = true;
    }
}

public class InstinctPlayer : ModPlayer
{
    public bool instinct = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new Instinct());
    }

    public override void ResetEffects()
    {
        instinct = false;
    }
}