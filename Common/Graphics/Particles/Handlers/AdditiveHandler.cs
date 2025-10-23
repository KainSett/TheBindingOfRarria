using System;
using System.Collections.Generic;
using TheBindingOfRarria.Common.Registries;

namespace TheBindingOfRarria.Common.Graphics.Particles.Handlers;

//all these use ai[0] for velocity multiplication
//maybe behaviour could be a separate thing...

public class AdditiveParticleHandler : IParticleHandler
{
    public List<Particle> Particles { get; set; }

    public void ApplyCustomEffect(SpriteBatch sb)
    {
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public void DrawParticles(SpriteBatch sb, Span<Particle> particles)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            ref Particle particle = ref particles[i];

            Texture2D texture = Textures.Particles[(int)particle.texture].Value;

            sb.Draw(texture, particle.Position - Main.screenPosition, null,
                particle.DrawColor * particle.Opacity, particle.Rotation,
                texture.Size() / 2f, particle.Scale * particle.Squish, SpriteEffects.None, 0f
            );
        }
    }

    public void UpdateParticles(Span<Particle> particles)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            ref Particle particle = ref particles[i];

            float lifeRatio = particle.LifeTimer / particle.MaxTime;

            particle.LifeTimer += 1;

            particle.Position += particle.Velocity * particle.ai[0];
            particle.Rotation = particle.Velocity.ToRotation();

            particle.Scale = Lerp(particle.fadeData.scaleStart, particle.fadeData.scaleEnd, lifeRatio);
            particle.Opacity = Lerp(particle.fadeData.opacityStart, particle.fadeData.opacityEnd, lifeRatio);
        }
    }
}

public class PixelatedAdditiveParticleHandler : IParticleHandler
{
    public List<Particle> Particles { get; set; }

    public void ApplyCustomEffect(SpriteBatch sb)
    {
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public void DrawParticles(SpriteBatch sb, Span<Particle> particles) { }

    public void UpdateParticles(Span<Particle> particles)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            ref Particle particle = ref particles[i];

            float lifeRatio = particle.LifeTimer / particle.MaxTime;

            particle.LifeTimer += 1;

            particle.Position += particle.Velocity * particle.ai[0];
            particle.Rotation = particle.Velocity.ToRotation();

            particle.Scale = Lerp(particle.fadeData.scaleStart, particle.fadeData.scaleEnd, lifeRatio);
            particle.Opacity = Lerp(particle.fadeData.opacityStart, particle.fadeData.opacityEnd, lifeRatio);
        }
    }

    public void DrawPixelParticles(SpriteBatch sb, Span<Particle> particles)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            ref Particle particle = ref particles[i];

            Texture2D texture = Textures.Particles[(int)particle.texture].Value;

            sb.Draw(texture, particle.Position - Main.screenPosition, null,
                particle.DrawColor * particle.Opacity, particle.Rotation,
                texture.Size() / 2f, particle.Scale * particle.Squish, SpriteEffects.None, 0f
            );
        }
    }

}

public class SubtractivePixelatedParticleHandler : IParticleHandler
{
    public List<Particle> Particles { get; set; }

    public void ApplyCustomEffect(SpriteBatch sb)
    {
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public void DrawParticles(SpriteBatch sb, Span<Particle> particles) { }

    public void UpdateParticles(Span<Particle> particles)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            ref Particle particle = ref particles[i];

            float lifeRatio = particle.LifeTimer / particle.MaxTime;

            particle.LifeTimer += 1;

            particle.Position += particle.Velocity * particle.ai[0];
            particle.Rotation = particle.Velocity.ToRotation();

            particle.Scale = Lerp(particle.fadeData.scaleStart, particle.fadeData.scaleEnd, lifeRatio);
            particle.Opacity = Lerp(particle.fadeData.opacityStart, particle.fadeData.opacityEnd, lifeRatio);
        }
    }

    public void DrawPixelParticles(SpriteBatch sb, Span<Particle> particles)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            ref Particle particle = ref particles[i];

            Texture2D texture = Textures.Particles[(int)particle.texture].Value;

            sb.Draw(texture, particle.Position - Main.screenPosition, null,
                particle.DrawColor * particle.Opacity, particle.Rotation,
                texture.Size() / 2f, particle.Scale * particle.Squish, SpriteEffects.None, 0f
            );
        }
    }

}
