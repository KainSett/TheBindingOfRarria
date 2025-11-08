using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Graphics.Particles;
using TheBindingOfRarria.Common.Graphics.Particles.Handlers;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Registries;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.ArtifactSets;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class DivergentsFist : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 34;
        Item.height = 40;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 3);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<YujiItemPlayer>().counter--;
        player.GetModPlayer<YujiItemPlayer>().Fist = Item;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<CursedBlood>())
            .AddIngredient(ItemID.PowerGlove)
            .AddIngredient(ItemID.SoulofFright, 4)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        int index = tooltips.FindIndex(t => t.Name == "Tooltip0");
        if (index != -1)
        {
            string text = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.Tooltip"), $"{Main.LocalPlayer.statDefense / 10}", $"{(int)MathHelper.Max(1, Main.LocalPlayer.statDefense / 2)}");

            text = text[..text.LastIndexOf($"\n")];
            tooltips[index].Text = text;
        }
    }
}

public class YujiItemPlayer : ModPlayer
{
    public Item Fist = null;
    
    public int counter = 0;

    public int reducedDefense = 0;

    public override void ResetEffects() => Fist = null;

    public override void PostUpdateEquips()
    {
        if (reducedDefense > 0 && counter > 0)
        {
            Player.statDefense -= reducedDefense;

            if (counter % 9 == 0)
                reducedDefense--;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Fist != null && counter <= 0 && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed))
        {
            bool setActive = KokusenPlayer.Get(Player).SetActive;

            if (setActive && Main.rand.Next(100) < 96)
            {
                counter = 2;
                reducedDefense = Player.statDefense / 10;

                if (Main.myPlayer == Player.whoAmI)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var proj = Projectile.NewProjectileDirect(Player.GetSource_OnHit(target), target.Center, Vector2.Zero, ProjectileType<KokusenImpactVFX>(), 0, 0, Main.myPlayer);
                        var bolt = proj.ModProjectile as KokusenImpactVFX;

                        bolt.AnchorPoint = target.Center;
                        bolt.EndOffset = target.Center.DirectionTo(Player.Center).RotatedBy(Main.rand.NextFloat() * PiOver4 - PiOver4 / 2);
                        bolt.StartOffset = bolt.EndOffset + Vector2.Normalize(bolt.EndOffset) * Main.rand.NextFloat(10, 100);
                        //proj.ai[1] = Main.rand.Next(0, 2); //used to randomize drawing the red bit or not
                    }
                }
            }

            else
            {
                counter = 180;
                reducedDefense = Player.statDefense / 10;
                Vector2 offset = target.Center.DirectionTo(Player.Center) * target.Hitbox.Size() * 0.5f;

                if (Main.myPlayer != Player.whoAmI)
                    return;

                Projectile.NewProjectile(Player.GetSource_Accessory(Fist, "Yuji fist attack"), target.Center + offset, -offset, ProjectileType<CEFist>(), (int)MathHelper.Max(1, Player.statDefense / 2), 5, Player.whoAmI, target.whoAmI, offset.X, offset.Y);
            }
        }
    }
}

public class KokusenImpactVFX : ModProjectile
{
    public Vector2 AnchorPoint = default;
    public Vector2 StartOffset = default, EndOffset = default;
    public bool CreatedPositions = false;
    public List<Vector2> positions = [];

    public ref float Time => ref Projectile.ai[0];

    public override string Texture => Helper.GetVanillaExtraTexture(179);

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 20;

        Projectile.aiStyle = -1;
        Projectile.timeLeft = 60;
        Projectile.extraUpdates = 1;
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
        Projectile.Center = AnchorPoint + EndOffset;

        if (!CreatedPositions)
        {
            positions = CreatePoints(StartOffset + AnchorPoint, EndOffset + AnchorPoint, 20f, 1.8f);
            CreatedPositions = true;
            Projectile.netUpdate = true;
        }

        else
        {
            Time++;

            if (Time < 4)
            {
                //scale in
                float t = Time / 4f;

                Projectile.Opacity = Lerp(0f, 1f, t);
                Projectile.scale = Projectile.Opacity;
            }

            if (Time >= 6 && Time < 10)
            {
                //scale out after a slight delay
                float t = (Time - 6f) / 4f;

                Projectile.Opacity = Lerp(1f, 0f, t);
                //Projectile.scale = Projectile.Opacity;
            }

            if (Time == 10)
                CreatedPositions = false;

            if (Time > 10 && Time < 15)
            {
                //scale in again
                float t = (Time - 10f) / 5f;

                Projectile.Opacity = Lerp(0f, 1f, t);
                //Projectile.scale = Projectile.Opacity;
            }

            if (Time >= 15 && Time < 30)
            {
                //scale out slower
                float t = (Time - 15f) / 15f;

                Projectile.Opacity = Lerp(1f, 0f, t);
                //Projectile.scale = Projectile.Opacity;
            }

            if (Time >= 30)
                Projectile.Kill();
        }
    }

    public override bool ShouldUpdatePosition() => false;

    public override bool PreDraw(ref Color lightColor)
    {
        PixellationSystem.QueuePixellationAction(() => 
        {
            Texture2D texture = Textures.Particles[0].Value;

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
                        null, Color.Black * Projectile.Opacity * Projectile.scale * (0.85f + (Projectile.ai[1] / 2f)), 0f,
                        texture.Size() / 2, Projectile.scale * 0.02f,
                        SpriteEffects.None
                    );

                    if (Projectile.ai[1] == 0)
                    {
                        Main.EntitySpriteDraw(texture, (drawPos - Main.screenPosition),
                            null, Color.Lerp(Color.Black, Color.Black, 0.37f) with { A = 0 } * Projectile.Opacity * Projectile.scale * 0.5f, 0f,
                            texture.Size() / 2, Projectile.scale * 0.032f,
                            SpriteEffects.None
                        );

                        Main.EntitySpriteDraw(texture, (drawPos - Main.screenPosition),
                          null, Color.Lerp(Color.Black, Color.Black, 0.87f) with { A = 0 } * Projectile.Opacity * Projectile.scale * 1.5f, 0f,
                          texture.Size() / 2, Projectile.scale * 0.021f,
                          SpriteEffects.None
                        );
                    }
                }
            }
        }, PixellationSystem.RenderType.AlphaBlend, PixellationSystem.RenderLayer.Projectiles);

        PixellationSystem.QueuePixellationAction(() =>
        {
            Texture2D texture = Textures.Particles[0].Value;

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
                        null, Color.Red * Projectile.Opacity * Projectile.scale, 0f,
                        texture.Size() / 2, Projectile.scale * 0.10f,
                        SpriteEffects.None
                    );

                    if (Projectile.ai[1] == 1)
                    {
                        Main.EntitySpriteDraw(texture, (drawPos - Main.screenPosition),
                            null, Color.Lerp(Color.Red, Color.DarkRed, 0.37f) * Projectile.Opacity * Projectile.scale * 0.5f, 0f,
                            texture.Size() / 2, Projectile.scale * 0.032f,
                            SpriteEffects.None
                        );

                        Main.EntitySpriteDraw(texture, (drawPos - Main.screenPosition),
                          null, Color.Lerp(Color.DarkBlue, Color.DarkRed, 0.87f) * Projectile.Opacity * Projectile.scale * 1.5f, 0f,
                          texture.Size() / 2, Projectile.scale * 0.021f,
                          SpriteEffects.None
                        );
                    }
                }
            }

        }, PixellationSystem.RenderType.Additive, PixellationSystem.RenderLayer.Projectiles);


        return false;
    }
}