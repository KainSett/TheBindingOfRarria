using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Doctrine : IArtifactSet
{
    public LocalizedText Name => Language.GetOrRegister($"Mods.TheBindingOfRarria.ArtifactSets.Doctrine.Name");

    public Color NameColor => Color.Violet;

    public List<int> Artifacts =>
        [ItemID.PhilosophersStone,
        ItemType<Multibinder>(),
        ItemType<InjectorBand>()];

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