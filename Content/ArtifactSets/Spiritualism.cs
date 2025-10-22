using System;
using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Spiritualism : IArtifactSet
{
    public List<int> Items => [
        ItemType<UnendingDespair>(),
        ItemType<BoundlessSpirit>(),
        ItemType<PhantomPopper>()];

    public LocalizedText Name => Language.GetOrRegister($"Mods.TheBindingOfRarria.ArtifactSets.Spiritualism.Name");

    public Color NameColor => Color.Violet;

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<SpiritualismPlayer>(out var p))
            p.spiritualism = true;
    }
}

public class SpiritualismPlayer : ModPlayer
{
    public bool spiritualism = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new Spiritualism());
    }
}