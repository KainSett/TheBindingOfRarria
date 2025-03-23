using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheBindingOfRarria.Content.Projectiles
{
    public class RighteousCrack : ModProjectile
    {
        private static int cooldownTime = 300;

        private const int Frames = 4;
        private const int FrameSpeed = 16;

        public override void SetStaticDefaults() => Main.projFrames[Type] = Frames;

        public override void SetDefaults()
        {
            Projectile.width = 112;
            Projectile.height = 102;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 60;
            Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.damage = 10;
            Projectile.scale = 1.0F;
            Projectile.frame = 0;

            Projectile.velocity = Vector2.Zero;
        }

        public override void AI()
        {
            
            if (Projectile.timeLeft % FrameSpeed == 0)
            {
                Projectile.frame++;
                if (Projectile.frame >= Frames)
                {
                    Projectile.frame = 0;
                }
            }

            
            Player player = Main.player[Projectile.owner];

            
            if (player.active && !player.HasBuff(ModContent.BuffType<Buffs.CrackedOrb>()))
            {
                player.AddBuff(ModContent.BuffType<Buffs.CrackedOrb>(), 2); 
            }
        }
    }
}