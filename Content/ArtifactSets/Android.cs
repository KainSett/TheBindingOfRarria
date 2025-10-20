using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Android : ModPlayer
{
    public bool android = false;

    public override void Load()
    {
        ArtifactSetSystem.ArtifactSets.Add(new ArtifactSet(Language.GetOrRegister($"Mods.{Mod.Name}.ArtifactSets.{Name}.Name"),
            Color.LightSlateGray,
            (plr) =>
            {
                if (Main.player[plr].TryGetModPlayer<Android>(out var p))
                    p.android = true;
            },
        i => i == ItemID.MechanicalGlove || Main.recipe.Any(r => r.HasIngredient(ItemID.MechanicalGlove) && r.HasResult(i)),
        i => i == ItemType<H20Volt>() || Main.recipe.Any(r => r.HasIngredient(ItemType<H20Volt>()) && r.HasResult(i)),
        i => i == ItemType<RobotCarcass>() || Main.recipe.Any(r => r.HasIngredient(ItemType<RobotCarcass>()) && r.HasResult(i))
        ));
    }
}