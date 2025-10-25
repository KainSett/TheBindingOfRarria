using Steamworks;
using System;
using System.Collections.Generic;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Kokusen : IArtifactSet
{
    public string Name => nameof(Kokusen);

    public Color NameColor => Color.DarkRed;

    public List<int> Items =>
    [   
        ItemType<LoupesForWeakness>(),
        ItemType<DivergentsFist>(),
        ItemType<TheLastFinger>()
    ];

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<KokusenPlayer>(out var p))
            p.SetActive = true;
    }
}

public class KokusenPlayer : ModPlayer
{
    public bool SetActive = false;

    public static KokusenPlayer Get(Player p) => p.GetModPlayer<KokusenPlayer>();

    public override void Load()
    {
        //ArtifactSetSystem.ArtifactSets.Add(new Kokusen());
    }
}
