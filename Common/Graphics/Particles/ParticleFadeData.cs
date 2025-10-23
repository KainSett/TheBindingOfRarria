namespace TheBindingOfRarria.Common.Graphics.Particles;

public struct ParticleFadeData(float scaleStart, float scaleEnd, float opacityStart, float opacityEnd)
{
    public float scaleStart = scaleStart;
    public float scaleEnd = scaleEnd;
    public float opacityStart = opacityStart;
    public float opacityEnd = opacityEnd;
}
