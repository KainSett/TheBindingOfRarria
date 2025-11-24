using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using TheBindingOfRarria.Common;
using TheBindingOfRarria.Content.Items;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.NPCs;

public class RedLeech : ModNPC
{
    public override string Texture => ContentPath + "NPCs/" + Name;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 3;

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
        NPC.width = 10;
        NPC.height = 10;
        NPC.damage = 20;
        NPC.defense = 5;
        NPC.lifeMax = 100;
        NPC.HitSound = SoundID.NPCHit9;
        NPC.DeathSound = SoundID.NPCDeath11;
        NPC.knockBackResist = 0.6f;
        NPC.value = Item.buyPrice(silver: 5);
        NPC.rarity = 2;
        NPC.npcSlots = 0.5f;

        NPC.aiStyle = -1;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        var tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1];
        if (tile.LiquidType == LiquidLoader.LiquidType<BloodLiquid>() && tile.LiquidAmount > 0)
            return 0.5f;

        else if (Main.bloodMoon && tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water)
            return 0.0f;

        return 0;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
                new FlavorTextBestiaryInfoElement("Mods.TheBindingOfRarria.NPCs.RedLeech.FlavorText")
            ]);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<YorghsRing>(), 4));
    }

    public enum State
    {
        Idle,
        Attack,
        Leech,
        Pathing
    }

    public State state
    {
        get { return (State)NPC.ai[0]; }
        set { NPC.ai[0] = (int)value; }
    }

    public override void AI()
    {
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

            if (state != State.Leech)
            {
                var old = NPC.target;
                var oldAI = NPC.ai[3];
                NPC.ai[3] = 120;
                NPC.target = -1;
                var coord = player.Center.ToTileCoordinates();
                for (int y = 0; y < 4; y++)
                {
                    var tile = Main.tile[coord + new Point(0, y)];
                    if (tile.LiquidAmount > 0 && (tile.LiquidType == LiquidLoader.LiquidType<BloodLiquid>() || tile.LiquidType == LiquidID.Water))
                    {
                        NPC.ai[3] = oldAI;
                        NPC.target = old;
                    }
                }
            }
            if (NPC.target != -1)
            {
                NPC.ai[3] = Math.Max(0, NPC.ai[3] - 1);

                NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.15f);

                if (state == State.Attack)
                    NPC.velocity = NPC.Center.DirectionTo(player.Center) * 10f;

                else if (state == State.Leech)
                {
                    NPC.rotation = 0;
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = player.Center + new Vector2(NPC.ai[1], NPC.ai[2]);
                }

                else if (NPC.ai[3] == 0) state = State.Attack;
            }
            else if (state != State.Pathing) state = State.Idle;
        }

        if (state == State.Idle || (state != State.Leech && !NPC.GetWet(LiquidLoader.LiquidType<BloodLiquid>()) && !NPC.wet))
        {
            if (NPC.ai[3] == 0)
                NPC.ai[3] = 120;
            if (state != State.Pathing)
                state = State.Idle;
            NPC.velocity *= 0.93f;
            //NPC.velocity.Y += 0.3f;

            if (WorldGen.SolidOrSlopedTile(Main.tile[(NPC.Center + new Vector2(0, NPC.height + 2)).ToTileCoordinates()]) && !NPC.GetWet(LiquidLoader.LiquidType<BloodLiquid>()) && !NPC.wet)
            {
                if (state != State.Pathing)
                {
                    for (int y = -2; y < 10; y++)
                    {
                        for (int x = -20; x < 20; x++)
                        {
                            var tile = Main.tile[(NPC.Center + new Vector2(0, NPC.height + 2)).ToTileCoordinates() + new Point(x, y)];
                            if (tile.LiquidAmount > 0 && (tile.LiquidType == LiquidLoader.LiquidType<BloodLiquid>() || tile.LiquidType == LiquidID.Water))
                            {
                                var pos = (NPC.Center.ToTileCoordinates() + new Point(x, y)).ToWorldCoordinates();
                                NPC.ai[1] = pos.X;
                                NPC.ai[2] = pos.Y;
                                state = State.Pathing;
                                break;
                            }
                        }
                        if (state == State.Pathing)
                            break;
                    }
                }
                else
                {
                    NPC.velocity = NPC.Center.DirectionTo(new Vector2(NPC.ai[1], NPC.ai[2])) * 2;
                    if (WorldGen.SolidOrSlopedTile(Main.tile[(NPC.Center + new Vector2(Math.Sign(NPC.velocity.X) * NPC.width, 0)).ToTileCoordinates()]))
                        NPC.velocity += new Vector2(0, -7);
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.15f);
                }
            }
            else state = State.Idle;


        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        state = State.Leech;
        var vect = (NPC.Center - target.Center).SafeNormalize(-Vector2.UnitY) * 0.8f * Math.Min(target.height, target.width);
        NPC.ai[1] = vect.X;
        NPC.ai[2] = vect.Y;
    }

    public override void UpdateLifeRegen(ref int damage)
    {
        if (state == State.Leech)
        {
            NPC.lifeRegen += 40;
        }
        else if (!NPC.GetWet(LiquidLoader.LiquidType<BloodLiquid>()) && !NPC.wet)
            NPC.lifeRegen -= 20;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter = (NPC.frameCounter + 1) % 6;
        if (NPC.frameCounter == 0)
            NPC.frame.Y = (NPC.frame.Y + frameHeight) % (Main.npcFrameCount[Type] * frameHeight);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var texture = TextureAssets.Npc[Type];

        var rect = NPC.frame;

        spriteBatch.Draw(texture.Value, NPC.Center - screenPos, rect, drawColor, NPC.rotation - PI * (NPC.direction - 1) / 2, rect.Size() / 2, NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        return false;
    }
}