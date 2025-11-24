using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Systems;

namespace TheBindingOfRarria.Content.Projectiles;

public class LightningBolt : ModProjectile
{
    public Vector2 Start = default;
    public Vector2 End = default;
    public int Target = -1;
    internal bool CreatedPositions = false;
    internal List<Vector2> positions = [];
    internal List<Vector2> positionsNormalLine = [];

    internal static void SetPositions(Vector2 start, Vector2 end, Projectile instance, int target = -1)
    {
        LightningBolt bolt = instance.As<LightningBolt>();

        bolt.Start = start;
        bolt.End = end;
        bolt.Target = target;
        bolt.CreatedPositions = false;
        instance.netUpdate = true;
    }

    public override string Texture => Helper.GetVanillaExtraTexture(179);

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
    }

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 20;

        Projectile.aiStyle = -1;
        Projectile.timeLeft = 60;
        Projectile.extraUpdates = 3;
        Projectile.DamageType = DamageClass.Default;

        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.OriginalArmorPenetration = 20;

        Projectile.ignoreWater = true;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;

        Projectile.scale = 0f;
        Projectile.Opacity = 0f;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(Start);
        writer.WriteVector2(End);
        writer.Write(Projectile.hostile);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Start = reader.ReadVector2();
        End = reader.ReadVector2();
        Projectile.hostile = reader.ReadBoolean();
    }

    public static List<Vector2> CreatePoints(Vector2 source, Vector2 dest, float sway = 80f, float constrain = 1f)
    {
        List<Vector2> results = [];

        Vector2 tangent = dest - source;
        Vector2 normal = Vector2.Normalize(new(tangent.Y, -tangent.X));

        List<float> positions = [0];
        float magnitude = tangent.Length();

        for (int i = 0; i < magnitude / 16f; i++)
            positions.Add(Main.rand.NextFloat());

        positions.Sort();

        float jaggedness = constrain / (float)sway;

        Vector2 previousPosition = source;
        float previousDisplacement = 0f;

        for (int i = 1; i < positions.Count; i++)
        {
            float pos = positions[i];
            float scale = magnitude * jaggedness * (pos - positions[i - 1]);
            float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;
            float displacement = Main.rand.NextFloat(-sway, sway);

            displacement -= (displacement - previousDisplacement) * (1 - scale);
            displacement *= envelope;

            Vector2 point = source + pos * tangent + displacement * normal;
            results.Add(point);

            previousPosition = point;
            previousDisplacement = displacement;
        }

        results.Add(previousPosition);
        results.Add(dest);
        results.Insert(0, source);

        return results;
    }

    public override void AI()
    {
        if (End == default && Start == default) 
            return;

        if (Target != -1)
        {
            NPC npc = Main.npc[Target];

            if (npc.active)
            {
                if (CreatedPositions)
                {
                    if (positions[^1].Distance(End) > 15f)
                    {
                        positions = CreatePoints(Start, End, 100f, 0.8f); //more aggressive when the target moves

                        positionsNormalLine.Add(Start);

                        for (int i = 1; i < positions.Count - 1; i++)
                            positionsNormalLine.Add(Vector2.Lerp(Start, End, i / (float)positions.Count));

                        positionsNormalLine.Add(End);
                    }
                }

                End = npc.Center;
            }
        }
        else if (Projectile.Center != End)
        {
            SoundEngine.PlaySound(SoundID.Thunder);

            Helper.Screenshake(Start, 10f, 5f, 1000f, 10);
        }

        Projectile.Center = End;

        if (!CreatedPositions)
        {
            positions = CreatePoints(Start, End, 100f, 0.8f);

            positionsNormalLine.Add(Start);

            for (int i = 1; i < positions.Count - 1; i++)
                positionsNormalLine.Add(Vector2.Lerp(Start, End, i / (float)positions.Count));

            positionsNormalLine.Add(End);

            for (int i = 0; i < positions.Count; i++)
                Dust.NewDustDirect(positions[i], 0, 0, DustID.GemTopaz, 0, 0, Scale: Main.rand.NextFloat(0.3f, 0.66f)).noGravity = true;

            CreatedPositions = true;
            Projectile.netUpdate = true;
        }

        else
        {
            if (Projectile.timeLeft > 50)
            {
                Projectile.scale = Lerp(Projectile.scale, 20f, 0.25f);
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.15f);
            }

            if (Projectile.timeLeft < 50)
            {
                Projectile.scale *= 0.998f;
                Projectile.Opacity *= 0.923f;

                if (positions.Count == positionsNormalLine.Count)
                {
                    for (int i = 1; i < positionsNormalLine.Count - 1; i++)
                        positions[i] = Vector2.Lerp(positions[i], positionsNormalLine[i] + Main.rand.NextVector2Circular(42f, 42f), Main.rand.NextFloat(0.01f, 0.133f));
                }
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!modifiers.SuperArmor)
            modifiers.DefenseEffectiveness *= 0f;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Start, End, 22, ref _);
    }

    public override bool ShouldUpdatePosition() => false;

    public override bool PreDraw(ref Color lightColor)
    {
        PixellationSystem.QueuePixellationAction(() =>
        {
            Texture2D texture = TextureAssets.Extra[98].Value;

            for (int i = 1; i < positions.Count; i++)
            {
                Vector2 start = positions[i - 1];
                Vector2 end = positions[i];

                float count = (end - start).Length() * 2f;

                for (int j = 0; j < count; j++)
                {
                    float lerp = j / (float)count;
                    Vector2 drawPos = Vector2.Lerp(start, end, lerp);

                    Main.EntitySpriteDraw(texture, (drawPos - Main.screenPosition),
                        null, Color.Lerp(Color.Yellow, Color.LightYellow, 0.37f) * Projectile.Opacity * Projectile.scale * 0.5f, start.DirectionTo(end).ToRotation(),
                        texture.Size() / 2, Projectile.scale * 0.2f * new Vector2(0.4f, 0.01f),
                        SpriteEffects.None
                    );

                    Main.EntitySpriteDraw(texture, (drawPos - Main.screenPosition),
                      null, Color.Lerp(Color.Yellow, Color.White, 0.87f) * Projectile.Opacity * Projectile.scale * 1.5f, start.DirectionTo(end).ToRotation(),
                      texture.Size() / 2, Projectile.scale * 0.15f * new Vector2(0.42f, 0.007f),
                      SpriteEffects.None
                    );

                    Lighting.AddLight(drawPos, Color.LightGoldenrodYellow.ToVector3());
                }
            }
        }, PixellationSystem.RenderType.Additive, PixellationSystem.RenderLayer.Projectiles);
        
        return false;
    }
}