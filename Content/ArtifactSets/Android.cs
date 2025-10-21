using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Android : IArtifactSet
{
    public LocalizedText Name => Language.GetOrRegister($"Mods.TheBindingOfRarria.ArtifactSets.Android.Name");

    public Color NameColor => Color.LightSlateGray;

    public List<int> Items => 
        [ItemID.MechanicalGlove,
        ItemType<H20Volt>(),
        ItemType<RobotCarcass>()];

    public HashSet<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }
    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<AndroidPlayer>(out var p))
            p.android = true;
    }
}

public class AndroidPlayer : ModPlayer
{
    public bool android = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new Android());
    }
}