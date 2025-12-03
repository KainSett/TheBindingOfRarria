using System;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using TheBindingOfRarria.Content.Items;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.NPCs;

public class CovertClone : ModNPC
{
    public override string Texture => ContentPath + "Projectiles/" + "TripleFlail";

    public override void SetStaticDefaults()
    {
        NPCID.Sets.ImmuneToAllBuffs[Type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = 24;
        NPC.height = 42;
        NPC.defense = 0;
        NPC.lifeMax = 1000;
        NPC.knockBackResist = 0f;
        NPC.rarity = 0;
        NPC.npcSlots = 0f;

        NPC.aiStyle = -1;
    }

    public override void OnKill()
    {

    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
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
        Walk
    }

    public State state
    {
        get { return (State)NPC.ai[1]; }
        set { NPC.ai[1] = (int)value; }
    }

    public override void AI()
    {
        NPC.direction = Math.Sign(NPC.velocity.X);

        var dist = NPC.Center.DistanceSQ(Main.player[(int)NPC.ai[0]].Center);
        if (dist >= 600 * 600)
        {
            target = Main.player[(int)NPC.ai[0]].Center;
            state = State.Walk;
        }
        else if (dist < 300 * 300 && state == State.Idle)
        {
            target = Main.player[(int)NPC.ai[0]].Center + new Vector2(300, 0) * Main.rand.NextFromList(-1, 1);
            state = State.Walk;
        }
        else if (dist > 300 * 300) state = State.Idle;

        if (NPC.velocity.Y > 0 && state != State.Walk)
            state = State.Idle;

        switch (state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Walk:
                Walk();
                break;

            default:
                state = State.Idle;
                break;
        }
    }

    public void Idle()
    {
        NPC.velocity *= 0.93f;
    }

    public Vector2 target = Vector2.Zero;

    public void Walk()
    {
        NPC.velocity.X = NPC.Center.DirectionTo(target).X;
        NPC.Collision_WalkDownSlopes();
        NPC.AI_107_ImprovedWalkers();
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return false;
    }
}