using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Instinct : ModPlayer
{
    public bool instinct = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new ArtifactSet(Language.GetOrRegister($"Mods.{Mod.Name}.ArtifactSets.{Name}.Name"),
            Color.PaleVioletRed,
            (plr) =>
            {
                if (Main.player[plr].TryGetModPlayer<Instinct>(out var p))
                    p.instinct = true;
            },
        i => i == ItemID.FeralClaws || Main.recipe.Any(r => r.HasIngredient(ItemID.FeralClaws) && r.HasResult(i)),
        i => i == ItemType<BeastCrest>() || Main.recipe.Any(r => r.HasIngredient(ItemType<BeastCrest>()) && r.HasResult(i)),
        i => i == ItemType<Longclaw>() || Main.recipe.Any(r => r.HasIngredient(ItemType<Longclaw>()) && r.HasResult(i))
        ));
    }

    public override void ResetEffects()
    {
        instinct = false;
    }
}