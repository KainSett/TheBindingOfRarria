using System;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using TheBindingOfRarria.Content.Items;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.NPCs;

public class RekSai : ModNPC
{
    public override string Texture => ContentPath + "NPCs/" + Name;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 12;
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
        NPC.width = 60;
        NPC.height = 60;
        NPC.damage = 60;
        NPC.defense = 26;
        NPC.lifeMax = 3000;
        NPC.HitSound = SoundID.NPCHit21;
        NPC.DeathSound = SoundID.NPCDeath25;
        NPC.knockBackResist = 0.1f;
        NPC.value = Item.buyPrice(gold: 4);
        NPC.rarity = 5;
        NPC.npcSlots = 4f;

        NPC.aiStyle = -1;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (spawnInfo.Player.ZoneDesert)
            return 0.03f;

        return 0;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.TheBindingOfRarria.NPCs.RekSai.FlavorText")
            ]);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UnnaturalScales>(), 4));
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Corruption);
            }
        }
    }

    public enum State
    {
        Idle,
        Walk,
        Attack,
        Lunge
    }

    public State state
    {
        get { return (State)NPC.ai[0]; }
        set { NPC.ai[0] = (int)value; }
    }

    public override void AI()
    {
        NPC.velocity *= 0.99f;
        NPC.direction = Math.Sign(NPC.velocity.X);

        // This should almost always be the first code in AI() as it is responsible for finding the proper player target
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest();
        }

        if (!NPC.HasValidTarget)
        {
            state = State.Idle;
            NPC.EncourageDespawn(10);
            return;
        }

        else
        {
            Player player = Main.player[NPC.target];

            if (NPC.velocity.Y <= 0 && state == State.Idle)
                state = State.Walk;

            var dist = player.Center.DistanceSQ(NPC.Center);
            if (dist > 500 * 500 && state != State.Lunge)
            {
                NPC.frame.Y = 312;
                state = State.Lunge;
                NPC.ai[1] = 1;
            }

            else if (dist < 60 * 60 && state != State.Lunge)
                state = State.Attack;

            if ((NPC.velocity.Y > 0 && state != State.Lunge) || (NPC.frame.Y < 312 && state == State.Lunge))
                state = State.Idle; 

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

            case State.Lunge:
                Lunge();
                break;

            default:
                state = State.Idle;
                break;
        }
    }

    public void Idle()
    {
        NPC.velocity.X = NPC.Center.DirectionTo(Main.player[NPC.target].Center).X;
        NPC.velocity *= 0.93f;

        NPC.frameCounter++;
        if (NPC.frameCounter > 8)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y = NPC.frame.Y == 0 ? 62 : 0;
        }
    }

    public void Walk()
    {
        NPC.velocity.X = NPC.Center.DirectionTo(Main.player[NPC.target].Center).X;
        NPC.Collision_WalkDownSlopes();
        NPC.AI_107_ImprovedWalkers();


        NPC.frameCounter++;
        if (NPC.frameCounter > 8)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y = Math.Max(62, (NPC.frame.Y + 62) % 250);
        }
    }

    public void Attack()
    {
        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitX);
        NPC.position -= NPC.velocity;

        NPC.frameCounter++;
        if (NPC.frameCounter > 6)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y = Math.Max(558, NPC.frame.Y + 62) % 744;
        }

        if (NPC.frame.Y == 682)
        {
            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(NPC.direction * NPC.width / 2, 0), Vector2.Zero, ModContent.ProjectileType<RekSaiSwing>(), 60, 2);
        }
        else if (NPC.frame.Y == 0)
        {
            state = State.Idle;
        }
    }

    public void Lunge()
    {
        //NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitX);

        if (NPC.ai[1] != -1)
        {
            NPC.ai[1] = 1;
        }

        var old = NPC.frame.Y;

        if (NPC.frame.Y == 496 && NPC.ai[1] == 1)
        {
            if (NPC.velocity.Y > 0)
            {
                for (int x = 0; x < NPC.width / 16f + 1; x++)
                    for (int y = 2; y < NPC.height / 16f + 1; y++)
                        if (WorldGen.SolidOrSlopedTile(Main.tile[(NPC.position + NPC.velocity).ToTileCoordinates() + new Point(x, y)]))
                            NPC.ai[1] = -1;
            }
            else
                for (int x = -1; x < NPC.width / 16f + 1; x++)
                    for (int y = 0; y < NPC.height / 16f + 1; y++)
                        if (WorldGen.SolidOrSlopedTile(Main.tile[(NPC.position + NPC.velocity).ToTileCoordinates() + new Point(x, y)]))
                        {
                            var enemy = Main.player[NPC.target].Center;
                            NPC.Teleport(enemy + NPC.Center.DirectionTo(enemy) * 320, 1);
                            NPC.velocity = NPC.Center.DirectionTo(enemy) * 8;
                        }
        }

        NPC.frameCounter++;
        if (NPC.frameCounter > 6)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y = Math.Min(496, NPC.frame.Y + 62 * (int)NPC.ai[1]);
        }

        if (NPC.frame.Y > old && NPC.frame.Y == 496)
        {
            NPC.velocity = NPC.Center.DirectionTo(Main.player[NPC.target].Center) * 20;
            NPC.velocity = NPC.velocity.RotatedBy(-Math.Sign(NPC.velocity.X) * PiOver4);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var texture = TextureAssets.Npc[Type];

        var rect = NPC.frame;

        spriteBatch.Draw(texture.Value, NPC.Center - screenPos + new Vector2(0, 6), rect, drawColor, NPC.rotation, rect.Size() / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

        return false;
    }
}