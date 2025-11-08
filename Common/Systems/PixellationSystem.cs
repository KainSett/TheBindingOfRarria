using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Helpers;

namespace TheBindingOfRarria.Common.Systems;
public class PixellationSystem : ModSystem
{
    // credits for the base for this system to naka, also thanks to zen, stormytuna and some others I forgor about for helping me change this thing to fit my needs
    // rewritten multiple times

    public enum RenderType
    {
        Additive,
        AlphaBlend
    }

    public enum RenderLayer
    {
        OverEverything,
        Projectiles
    }

    private static RenderTarget2D AlphaBlendTarget { get; set; }
    private static RenderTarget2D AdditiveTarget { get; set; }

    private static Dictionary<RenderLayer, Queue<(Action, RenderType)>> Actions { get; set; } = [];

    public override void Load()
    {
        if (!Main.dedServ)
        {
            Main.OnResolutionChanged += InitializeRT;
            Main.RunOnMainThread(() =>
            {
                AlphaBlendTarget = new(Main.instance.GraphicsDevice,
                    Main.screenWidth / 2, Main.screenHeight / 2,
                    false, SurfaceFormat.Color, DepthFormat.None, 0,
                    RenderTargetUsage.PreserveContents
                );
                AdditiveTarget = new(Main.instance.GraphicsDevice,
                    Main.screenWidth / 2, Main.screenHeight / 2,
                    false, SurfaceFormat.Color, DepthFormat.None, 0,
                    RenderTargetUsage.PreserveContents
                );
            });
        }

        On_Main.DrawInfernoRings += DrawPixellatedOverEverything;
        On_Main.DrawProjectiles += DrawPixellatedProjectiles;
    }

    private static void DrawPixellatedProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
    {
        orig(self);
        DrawPixellated(RenderLayer.Projectiles);
    }

    private static void DrawPixellatedOverEverything(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        DrawPixellated(RenderLayer.OverEverything);
    }

    private static void InitializeRT(Vector2 obj)
    {
        if (Main.dedServ)
            return;

        AlphaBlendTarget?.Dispose();
        AdditiveTarget?.Dispose();

        GraphicsDevice gd = Main.instance.GraphicsDevice;
        int width = Main.screenWidth / 2;
        int height = Main.screenHeight / 2;

        AlphaBlendTarget = new(gd, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        AdditiveTarget = new(gd, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }

    public static void QueuePixellationAction(Action action, RenderType type, RenderLayer layer)
    {
        if (!Actions.TryGetValue(layer, out Queue<(Action, RenderType)> value))
        {
            var nullQueue = new Queue<(Action, RenderType)>();
            value = nullQueue;
            Actions.Add(layer, value);
        }

        value.Enqueue((action, type));
    }

    /// <summary>
    /// Invokes the passed draw action on the rt and draws the rt with 2x scale
    /// </summary>
    private static void DrawPixellated(RenderLayer layer)
    {
        if (Actions is null || Actions.Count <= 0 || !Actions.TryGetValue(layer, out Queue<(Action action, RenderType type)> value) || value is null || value.Count <= 0)
            return;

        var gd = Main.graphics.GraphicsDevice;

        if (gd.PresentationParameters.RenderTargetUsage != RenderTargetUsage.PreserveContents)
        {
            gd.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        var oldTargets = gd.GetRenderTargets();

        foreach (var target in oldTargets)
        {
            if (target.RenderTarget is RenderTarget2D rt)
                rt.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        Helper.SpritebatchParameters parameters = new();
        var beginned = Main.spriteBatch.beginCalled;

        if (beginned)
            Main.spriteBatch.End(out parameters);

        gd.SetRenderTarget(AlphaBlendTarget);
        gd.Clear(Color.Transparent);

        for (int i = 0; i < Actions[layer].Count; i++)
        {
            var (action, type) = Actions[layer].Dequeue();
            if (type != RenderType.AlphaBlend)
                continue;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Matrix.CreateScale(0.5f, 0.5f, 1f));
            action.Invoke();
            Main.spriteBatch.End();
        }

        gd.SetRenderTarget(AdditiveTarget);
        gd.Clear(Color.Transparent);

        for (int i = 0; i < Actions[layer].Count; i++)
        {
            var (action, type) = Actions[layer].Dequeue();
            if (type != RenderType.Additive)
                continue;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Matrix.CreateScale(0.5f, 0.5f, 1f));
            action.Invoke();
            Main.spriteBatch.End();
        }

        Main.graphics.GraphicsDevice.SetRenderTargets(oldTargets); 

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.Draw(AdditiveTarget, new Vector2(0), null, Color.White, 0, new Vector2(0), 2, SpriteEffects.None, 0);
        Main.spriteBatch.End();

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.Draw(AlphaBlendTarget, new Vector2(0), null, Color.White, 0, new Vector2(0), 2, SpriteEffects.None, 0);
        Main.spriteBatch.End();

        if (beginned)
            Main.spriteBatch.Begin(parameters);
    }
}
