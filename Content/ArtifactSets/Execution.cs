using System;
using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Execution : IArtifactSet
{
    public string Name => nameof(Execution);

    public Color NameColor => Color.IndianRed;

    public List<int> Items =>
        [ItemType<TheLastOffering>(),
        ItemType<Guillotine>(),
        ItemType<DeathsBell>()];

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<ExecutionPlayer>(out var p))
            p.execution = true;
    }
}

public class ExecutionPlayer : ModPlayer
{
    public bool execution = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new Execution());
    }

    public override void ResetEffects()
    {
        execution = false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (execution && !target.boss && target.life == target.lifeMax && Main.rand.NextFloat() < 0.08f)
        {
            modifiers.SetInstantKill();
        }
    }
}