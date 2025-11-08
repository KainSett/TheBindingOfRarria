using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TheBindingOfRarria.Common.Systems;

namespace TheBindingOfRarria.Common.Graphics.Particles;

public class ParticleManager : ModSystem //goated ass handler that can easily draw and update 20k particles at once
{                                        //for particles that are drawn using primitives, use a DynamicVertexBuffer and PrimitiveType.TriangleList to draw them all at once
    public static ParticleManager Instance = null;

    internal List<IParticleHandler> Handlers = [];
    internal List<IParticleHandler> Handlers_Pixelation = [];
    internal List<IParticleHandler> Handlers_SubtractivePixelation = [];

    public Dictionary<Type, IParticleHandler> HandlerMap { get; } = [];

    public static void RegisterHandler(IParticleHandler handler) => Instance.HandlerMap[handler.GetType()] = handler;

    public static void SpawnParticle<T>(Vector2 position, Vector2 velocity, int time, ParticleFadeData fadeData, Color drawColor, Vector2 squish, ParticleTextureType texture, params float[] ai) where T : IParticleHandler
    {
        if (!Instance.HandlerMap.TryGetValue(typeof(T), out var handler))
        {
            Main.NewText($"handler of type {typeof(T)} could not be found");
            return;
        }

        Particle spawn = new()
        {
            Position = position,
            Velocity = velocity,

            MaxTime = time,
            LifeTimer = 0,

            DrawColor = drawColor,

            Squish = squish,
            Scale = fadeData.scaleStart,
            Opacity = fadeData.opacityStart,

            fadeData = fadeData,
            texture = texture,

            ai = ai,
        };

        handler.Particles.Add(spawn);
    }

    private static bool CheckOverride(Type type, Type iface, string name)
    {
        var virtMethod = iface.GetMethod(name);
        var overrided = type.GetMethod(name);

        return overrided != null && overrided.DeclaringType != virtMethod.DeclaringType;
    }

    public override void Load()
    {
        Instance = this;

        if (!Main.dedServ)
            On_Main.DrawDust += DrawParticles;

        Instance.Handlers?.Clear();
        Instance.Handlers_Pixelation?.Clear();
        Instance.Handlers_SubtractivePixelation?.Clear();

        Instance.HandlerMap?.Clear();

        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes().Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IParticleHandler))))
        {
            if (Activator.CreateInstance(type) is IParticleHandler handler)
            {
                handler.Particles = [];
                RegisterHandler(handler);

                Instance.Handlers.Add(handler);

                if (CheckOverride(type, typeof(IParticleHandler), nameof(IParticleHandler.DrawPixelParticles)))
                {
                    if (type.Name.StartsWith("Subtractive")) //this is horrid and i should just make another method for subtractive drawing :3
                        Instance.Handlers_SubtractivePixelation.Add(handler);
                    else Instance.Handlers_Pixelation.Add(handler);
                }
            }
        }
    }

    public override void Unload()
    {
        if (!Main.dedServ)
            On_Main.DrawDust -= DrawParticles;

        Instance.Handlers.Clear();
        Instance.Handlers_Pixelation.Clear();
        Instance.Handlers_SubtractivePixelation.Clear();

        Instance.HandlerMap.Clear();

        Instance = null;
    }

    public override void PostUpdateDusts()
    {
        if (Main.dedServ)
            return;

        int count = Instance.Handlers.Count;

        if (count != 0)
        {
            for (int i = 0; i < count; i++)
            {
                var handler = Instance.Handlers[i];

                if (handler.Particles.Count == 0)
                    continue;

                handler.Particles.RemoveAll(p => p.LifeTimer > p.MaxTime);
                handler.UpdateParticles(CollectionsMarshal.AsSpan(handler.Particles));
            }
        }
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);

        // greatest variable names of all time

        int count = Instance.Handlers.Count;
        int count2 = Instance.Handlers_Pixelation.Count;
        int count3 = Instance.Handlers_SubtractivePixelation.Count;

        if (count != 0)
        {
            for (int i = 0; i < count; i++)
            {
                var handler = Instance.Handlers[i];

                if (handler.Particles.Count == 0)
                    continue;

                handler.ApplyCustomEffect(Main.spriteBatch);
                handler.DrawParticles(Main.spriteBatch, CollectionsMarshal.AsSpan(handler.Particles));

                Main.spriteBatch.End();
            }
        }

        if (count2 != 0)
        {
            PixellationSystem.QueuePixellationAction(() => // queue it all at once to limit spritebatch restarts
            {
                for (int i = 0; i < count2; i++)
                {
                    var handler = Instance.Handlers_Pixelation[i];

                    if (handler.Particles.Count == 0)
                        continue;

                    handler.DrawPixelParticles(Main.spriteBatch, CollectionsMarshal.AsSpan(handler.Particles));
                }

            }, PixellationSystem.RenderType.Additive, PixellationSystem.RenderLayer.Projectiles);
        }

        if (count3 != 0)
        {
            PixellationSystem.QueuePixellationAction(() => // same here
            {
                for (int i = 0; i < count3; i++)
                {
                    var handler = Instance.Handlers_SubtractivePixelation[i];

                    if (handler.Particles.Count == 0)
                        continue;

                    handler.DrawPixelParticles(Main.spriteBatch, CollectionsMarshal.AsSpan(handler.Particles));
                }

            }, PixellationSystem.RenderType.AlphaBlend, PixellationSystem.RenderLayer.Projectiles);
        }
    }

    public override void ClearWorld()
    {
        for (int i = 0; i < Handlers.Count; i++)
            Handlers[i]?.Particles?.Clear();
    }
}

