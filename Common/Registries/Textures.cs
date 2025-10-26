using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace TheBindingOfRarria.Common.Registries;

    // [Autoload(Side = ModSide.Client)]
public static class Textures
{
    private const string prefix = "TheBindingOfRarria/Assets/Textures/";

    public static readonly Asset<Texture2D>[] BloodOrbs = LoadTexture2Ds("BloodOrb", 2);

        // I'm gonna pretend I know what this is.
    public static readonly Asset<Texture2D>[] CD = LoadTexture2Ds("CD", 2);

    public static readonly Asset<Texture2D> Fish = LoadTexture2D("Fish");

    public static readonly Asset<Texture2D> Crypt = LoadTexture2D("Crypt");

    public static readonly Asset<Texture2D> TrailTexture = LoadTexture2D("TrailTexture");

    public static readonly Asset<Texture2D> TrailTexture2 = LoadTexture2D("TrailTexture2");

    public static readonly Asset<Texture2D> ElectricLink = LoadTexture2D("ElectricLink");


    private static readonly Lazy<Asset<Texture2D>[]> _particle = new(() => LoadTexture2Ds("Particles/Particle", 2));
    public static Asset<Texture2D>[] Particles => _particle.Value;

    private static Asset<Texture2D> LoadTexture2D(string TexturePath)
    {
        if (Main.dedServ)
            return null;

        return ModContent.Request<Texture2D>(prefix + TexturePath);
    }

    private static Asset<Texture2D>[] LoadTexture2Ds(string TexturePath, int count)
    {
        if (Main.dedServ)
            return null;

        Asset<Texture2D>[] textures = new Asset<Texture2D>[count];

        for (int i = 0; i < count; i++)
            textures[i] = ModContent.Request<Texture2D>(prefix + TexturePath + i);

        return textures;
    }
}
