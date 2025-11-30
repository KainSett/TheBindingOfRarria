using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.Items;
using TheBindingOfRarria.Content.Projectiles;
using static System.Net.Mime.MediaTypeNames;

namespace TheBindingOfRarria.Content.Tiles;

public enum PlantStage : byte
{
    Planted,
    Growing,
    Grown
}

public class FulgurbloomTile : ModTile
{
    public override string Texture => ContentPath + "Tiles/" + Name;

    private const int FrameWidth = 18; // A constant for readability and to kick out those magic numbers

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileCut[Type] = true;
        Main.tileNoFail[Type] = true;
        TileID.Sets.ReplaceTileBreakUp[Type] = true;
        TileID.Sets.IgnoredInHouseScore[Type] = true;
        TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

        Main.tileShine2[Type] = true;
        Main.tileShine[Type] = 600;
        // We do not use this because our tile should only be spelunkable when it's fully grown. That's why we use the IsTileSpelunkable hook instead
        //Main.tileSpelunker[Type] = true;

        // Do NOT use this, it causes many unintended side effects
        //Main.tileAlch[Type] = true;

        LocalizedText name = CreateMapEntryName();
        AddMapEntry(Color.LightGoldenrodYellow, name);

        TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
        TileObjectData.newTile.AnchorValidTiles = [
            TileID.Grass,
                TileID.HallowedGrass
        ];

        TileObjectData.newTile.AnchorAlternateTiles = [
            TileID.ClayPot,
                TileID.PlanterBox
        ];
        TileObjectData.addTile(Type);

        HitSound = SoundID.Grass;
        DustType = DustID.YellowTorch;
        
    }

    public override bool CanPlace(int i, int j)
    {
        Tile tile = Framing.GetTileSafely(i, j); // Safe way of getting a tile instance

        if (tile.HasTile)
        {
            int tileType = tile.TileType;
            if (tileType == Type)
            {
                PlantStage stage = GetStage(i, j); // The current stage of the herb

                // Can only place on the same herb again if it's grown already
                return stage == PlantStage.Grown;
            }
            else
            {
                // Support for vanilla herbs/grasses:
                if (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip)
                {
                    bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
                    bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
                    bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);

                    if (foliageGrass || moddedFoliage || harvestableVanillaHerb)
                    {
                        WorldGen.KillTile(i, j);
                        if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.TileManipulation, number: 0, number2: i, number3: j);
                        }

                        return true;
                    }
                }

                return false;
            }
        }

        return true;
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
    {
        if (i % 2 == 0)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        offsetY = -2; // This is -1 for tiles using StyleAlch, but vanilla sets to -2 for herbs, which causes a slight visual offset between the placement preview and the placed tile. 
    }

    public override bool CanDrop(int i, int j)
    {
        PlantStage stage = GetStage(i, j);

        if (stage == PlantStage.Planted)
        {
            // Do not drop anything when just planted
            return false;
        }

        return true;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        PlantStage stage = GetStage(i, j);

        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
        Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

        int herbItemType = ModContent.ItemType<Fulgurbloom>();
        int herbItemStack = 1;

        if (nearestPlayer.active && (nearestPlayer.HeldItem.type == ItemID.StaffofRegrowth || nearestPlayer.HeldItem.type == ItemID.AcornAxe))
        {
            // Increased yields with Staff of Regrowth, even when not fully grown
            herbItemStack = Main.rand.Next(1, 3);
        }

        else if (stage == PlantStage.Grown)
        {
            // Default yields, only when fully grown
            herbItemStack = Main.rand.Next(1, 3);
        }

        if (herbItemType > 0 && herbItemStack > 0)
        {
            yield return new Item(herbItemType, herbItemStack);
        }
    }

    public override bool IsTileSpelunkable(int i, int j)
    {
        PlantStage stage = GetStage(i, j);

        // Only glow if the herb is grown
        return stage == PlantStage.Grown;
    }

    public override void NearbyEffects(int i, int j, bool closer)
    {
        if (closer || Main.gamePaused || GetStage(i, j) != PlantStage.Grown)
            return;

        Vector2 flowerPos = new(i, j);



        if (!Main.rand.NextBool(100))
            return;

        Tile current = Main.tile[i, j];

        if (current.TileType == ModContent.TileType<FulgurbloomTile>())
        {
            var pos = flowerPos.ToPoint() + new Vector2(Main.rand.NextFloat(-500f, 500f), 0).ToTileCoordinates();
            for (int a = -40; a < 40; a++)
            {
                if (WorldGen.SolidOrSlopedTile(pos.X, pos.Y))
                    break;
                pos.Y++;
            }


            Vector2 end = pos.ToWorldCoordinates() + new Vector2(Main.rand.NextFloat(-100f, 100f), -1100f);

            var start = pos.ToWorldCoordinates();
            var proj = Projectile.NewProjectileDirect(
                new EntitySource_WorldEvent(), start, Vector2.Zero,
                ModContent.ProjectileType<LightningBolt>(), 33,
                2, Main.myPlayer
            );

            proj.hostile = true;
            LightningBolt.SetPositions(start, end, proj, -1);


            return;
        }
    }

    public override void RandomUpdate(int i, int j)
    {
        Tile tile = Framing.GetTileSafely(i, j);
        PlantStage stage = GetStage(i, j);

        // Only grow to the next stage if there is a next stage. We don't want our tile turning pink!
        if (stage != PlantStage.Grown)
        {
            // Increase the x frame to change the stage
            tile.TileFrameX += FrameWidth;

            // If in multiplayer, sync the frame change
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }
    }

    // A helper method to quickly get the current stage of the herb (assuming the tile at the coordinates is our herb)
    internal static PlantStage GetStage(int i, int j)
    {
        Tile tile = Framing.GetTileSafely(i, j);
        return (PlantStage)(tile.TileFrameX / FrameWidth);
    }
}