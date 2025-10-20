using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Doctrine : ModPlayer
{
    public bool doctrine = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new ArtifactSet(Language.GetOrRegister($"Mods.{Mod.Name}.ArtifactSets.{Name}.Name"),
            Color.Violet,
            (plr) =>
            {
                if (Main.player[plr].TryGetModPlayer<Doctrine>(out var p))
                    p.doctrine = true;
            },
        i => i == ItemID.PhilosophersStone || Main.recipe.Any(r => r.HasIngredient(ItemID.PhilosophersStone) && r.HasResult(i)),
        i => i == ItemType<Multibinder>() || Main.recipe.Any(r => r.HasIngredient(ItemType<Multibinder>()) && r.HasResult(i)),
        i => i == ItemType<InjectorBand>() || Main.recipe.Any(r => r.HasIngredient(ItemType<InjectorBand>()) && r.HasResult(i))
        ));
    }

    public override void ResetEffects()
    {
        doctrine = false;
    }
}