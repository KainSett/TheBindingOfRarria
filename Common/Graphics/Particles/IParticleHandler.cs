using System;
using System.Collections.Generic;

namespace TheBindingOfRarria.Common.Graphics.Particles;

public interface IParticleHandler
{
    List<Particle> Particles { get; set; }

    /// <summary>
    /// Can be used to restart the spritebatch or apply a shader to the respective batch of particles this handler should draw.
    /// <br>Only use this to prepare the spritebatch. By default, handles deferred additive rendering.</br>
    /// </summary>
    /// <param name="sb"></param>
    virtual void ApplyCustomEffect(SpriteBatch sb)
    {
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    /// <summary>
    /// Draws all particles associated with this handler.
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="particles"></param>
    void DrawParticles(SpriteBatch sb, Span<Particle> particles);

    /// <summary>
    /// Draw particles that should be pixelated here.
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="particles"></param>
    virtual void DrawPixelParticles(SpriteBatch sb, Span<Particle> particles) { }

    /// <summary>
    /// Updates all particles associated with this handler.
    /// </summary>
    /// <param name="particles"></param>
    void UpdateParticles(Span<Particle> particles);
}
