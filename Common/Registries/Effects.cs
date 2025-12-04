using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace TheBindingOfRarria.Common.Registries;

// [Autoload(Side = ModSide.Client)]
public static class Effects
{
    private const string prefix = "TheBindingOfRarria/Assets/Effects/";

    public static readonly Asset<Effect> ShpereShader = LoadEffect("Sphere");

    public static readonly Asset<Effect> SpinAura = LoadEffect("SpinAura");

    public static readonly Asset<Effect> Trail = LoadEffect("Trail");

    public static readonly Asset<Effect> Quantizer = LoadEffect("Quantizer");

    public static readonly Asset<Effect> CullAndPixel = LoadEffect("CullAndPixel");

    public static readonly Asset<Effect> Outline = LoadEffect("Outline");

    public static readonly Asset<Effect> SobelDarken = LoadEffect("SobelDarken");

    public static readonly Asset<Effect> Tint = LoadEffect("Tint");

    public static readonly Asset<Effect> Glitch = LoadEffect("Glitch");

    private static Asset<Effect> LoadEffect(string EffectPath)
    {
        if (Main.dedServ)
            return null;

        return ModContent.Request<Effect>(prefix + EffectPath);
    }

    private static Asset<Effect>[] LoadEffects(string EffectPath, int count)
    {
        if (Main.dedServ)
            return null;

        Asset<Effect>[] effects = new Asset<Effect>[count];

        for (int i = 0; i < count; i++)
            effects[i] = ModContent.Request<Effect>(prefix + EffectPath + i);

        return effects;
    }
}
