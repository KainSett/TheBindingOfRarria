using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.Graphics.CameraModifiers;
using Terraria.Localization;
using TheBindingOfRarria.Content.Items;
using TheBindingOfRarria.Content.Projectiles;


namespace TheBindingOfRarria.Content.NPCs;

public class Rebel : ModNPC
{
    public override string Texture => ContentPath + "NPCs/" + Name;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 2;
        NPCID.Sets.TrailingMode[Type] = 0;

        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

        // Influences how the NPC looks in the Bestiary
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            PortraitScale = 1f, // Portrait refers to the full picture when clicking on the icon in the bestiary
            PortraitPositionYOverride = 0f,
        };
        
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void SetDefaults()
    {
        NPC.width = 50;
        NPC.height = 100;
        NPC.damage = 12;
        NPC.defense = 8;
        NPC.lifeMax = 1600;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath6;
        NPC.knockBackResist = 0f;
        NPC.value = Item.buyPrice(gold: 2);
        NPC.rarity = 3; 
        NPC.npcSlots = 5f;

        NPC.aiStyle = -1;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (spawnInfo.SpawnTileType is TileID.BlueDungeonBrick or TileID.GreenDungeonBrick or TileID.PinkDungeonBrick && Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1].WallType == 0)
            return 0.4f;

        return 0;
    }
    
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
				new FlavorTextBestiaryInfoElement("Mods.TheBindingOfRarria.NPCs.Rebel.FlavorText")
            ]);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<YorghsRing>(), 4));
    }

    public override void OnKill()
    {

    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
        return true;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith);
            }
        }
    }

    public enum State
    {
        Idle,
        Walk,
        Attack,
        Teleport
    }

    public State state
    {
        get { return (State)NPC.ai[0]; }
        set { NPC.ai[0] = (int)value; }
    }

    public override void AI()
    {
        NPC.gravity += 1;
        NPC.direction = Math.Sign(NPC.velocity.X);

        // This should almost always be the first code in AI() as it is responsible for finding the proper player target
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest();
        }

        if (!NPC.HasValidTarget)
        {
            state = State.Teleport;
            NPC.EncourageDespawn(10);
            return;
        }

        else
        {
            Player player = Main.player[NPC.target];
            var frame = (int)state;

            if (NPC.velocity.Y <= 0 && state == State.Idle) 
                state = State.Walk;

            var dist = player.Center.DistanceSQ(NPC.Center);
            if (dist > 700 * 700)
                state = State.Teleport;

            else if (dist < 90 * 90 && state != State.Teleport)
                state = State.Attack;

            if (NPC.velocity.Y > 0 && state != State.Walk)
                state = State.Idle;

            if (frame != (int)state)
                frameX = 0;
        }

        switch (state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Walk:
                Walk();
                break;

            case State.Attack:
                Attack();
                break;

            case State.Teleport:
                Teleport();
                break;

            default:
                state = State.Idle;
                break;
        }

        for (int x = 0; x < NPC.width / 16f + 0.9f; x++)
            for (int y = 0; y < NPC.height / 16f - 1; y++)
                if (WorldGen.SolidOrSlopedTile(Main.tile[(NPC.position + NPC.velocity).ToTileCoordinates() + new Point(x, y)]))
                    state = State.Teleport;
    }

    public void Idle()
    {
        NPC.velocity.X = NPC.Center.DirectionTo(Main.player[NPC.target].Center).X;
        NPC.velocity *= 0.93f;

        NPC.frameCounter++;
        if (NPC.frameCounter > 8)
        {
            NPC.frameCounter = 0;
            frameX = (frameX + 1) % 2;
        }
        frameY = 0;
    }

    public void Walk()
    {
        NPC.velocity.X = NPC.Center.DirectionTo(Main.player[NPC.target].Center).X;
        NPC.Collision_WalkDownSlopes();
        NPC.AI_107_ImprovedWalkers(); ;


        NPC.frameCounter++;
        if (NPC.frameCounter > 8)
        {
            NPC.frameCounter = 0;
            frameX = Math.Max((frameX + 1) % 9, 2);
        }
        frameY = 0;
    }

    public void Attack()
    {
        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitX);
        NPC.position -= NPC.velocity;

        NPC.frameCounter++;
        if (NPC.frameCounter > 6)
        {
            NPC.frameCounter = 0;
            frameX = Math.Max(frameX + 1, 0);

            if (NPC.ai[1] < 2 && frameX == 7)
            {
                NPC.frameCounter -= 7f;
                NPC.ai[1]++;
                frameX -= 2;
            }
        }
        frameY = 1;

        if (frameX == 11)
        {
            NPC.ai[1] = 0;
            state = State.Idle;
            NPC.velocity.X = NPC.Center.DirectionTo(Main.player[NPC.target].Center).X;
        }
        else if (frameX is 4 or 7)
        {
            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(NPC.direction * NPC.width, 0), Vector2.Zero, ModContent.ProjectileType<RebelSwing>(), NPC.damage, 2);
        }
    }

    public void Teleport()
    {
        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitX);
        NPC.position -= NPC.velocity;
        NPC.teleporting = true;

        frameY = 0;
        frameX = 0;


        NPC.ai[1]++;
        if (NPC.ai[1] > 60)
        {
            if (NPC.ai[1] > 100)
            {
                var targetPosition = Main.player[NPC.target].Center;
                if (targetPosition != Vector2.Zero)
                {
                    NPC.Teleport(targetPosition - NPC.Size * new Vector2(0.5f, 1f) + new Vector2(Math.Sign(NPC.Center.DirectionTo(targetPosition).X) * NPC.width * 1.5f, 0), 1);
                    state = State.Idle;
                    NPC.ai[1] = 0;

                    NPC.netUpdate = true;
                }
            }
        }
    }

    public int frameX = 0;

    public int frameY = 0;

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var texture = TextureAssets.Npc[Type];

        var rect = texture.Frame(11, 2, frameX, frameY, -2, -2);

        spriteBatch.Draw(texture.Value, NPC.Center - screenPos + new Vector2(0, 8), rect, drawColor, NPC.rotation, rect.Size() / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        return false;
    }
}