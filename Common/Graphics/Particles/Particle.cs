using System.Collections.Generic;

namespace TheBindingOfRarria.Common.Graphics.Particles;

public struct Particle
{
    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 Squish;
    public float LifeTimer;
    public float MaxTime;
    public float Scale;
    public float Rotation;
    public float Opacity;
    public Color DrawColor;
    public float[] ai;

    public ParticleFadeData fadeData;
    public ParticleTextureType texture;

    public readonly bool Active => LifeTimer < MaxTime;
}
