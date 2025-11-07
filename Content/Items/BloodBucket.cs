using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using TheBindingOfRarria.Common;

namespace TheBindingOfRarria.Content.Items;

//This class showcases making a base item
//Since this mod contains a lot of bucket items, we prevent repeated copy pasting of code here, but creating an abstract ModItem class.
//(abstract prevents the item being loaded, so this class can be used by other classes).
//Please see ExampleBucket for comments on how buckets work using ModLiquidLib
public abstract class ExampleBucketBase : ModItem
{
    public int BucketLiquidType = ItemID.EmptyBucket;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Type] = true;
        ItemID.Sets.AlsoABuildingItem[Type] = true;
        ItemID.Sets.DuplicationMenuToolsFilter[Type] = true;

        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 24;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
    }

    public override void HoldItem(Player player)
    {
        if (!player.JustDroppedAnItem)
        {
            if (player.whoAmI != Main.myPlayer)
            {
                return;
            }
            if (player.noBuilding ||
                !(player.position.X / 16f - (float)Player.tileRangeX - (float)Item.tileBoost <= (float)Player.tileTargetX) ||
                !((player.position.X + (float)player.width) / 16f + (float)Player.tileRangeX + (float)Item.tileBoost - 1f >= (float)Player.tileTargetX) ||
                !(player.position.Y / 16f - (float)Player.tileRangeY - (float)Item.tileBoost <= (float)Player.tileTargetY) ||
                !((player.position.Y + (float)player.height) / 16f + (float)Player.tileRangeY + (float)Item.tileBoost - 2f >= (float)Player.tileTargetY))
            {
                return;
            }
            if (!Main.GamepadDisableCursorItemIcon)
            {
                player.cursorItemIconEnabled = true;
                Main.ItemIconCacheUpdate(Item.type);
            }
            if (!player.ItemTimeIsZero || player.itemAnimation <= 0 || !player.controlUseItem)
            {
                return;
            }
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if (tile.LiquidAmount >= 200)
            {
                return;
            }
            if (tile.HasUnactuatedTile)
            {
                bool[] tileSolid = Main.tileSolid;
                if (tileSolid[tile.TileType])
                {
                    bool[] tileSolidTop = Main.tileSolidTop;
                    if (!tileSolidTop[tile.TileType])
                    {
                        if (tile.TileType != TileID.Grate)
                        {
                            return;
                        }
                    }
                }
            }
            if (tile.LiquidAmount != 0)
            {
                if (tile.LiquidType != BucketLiquidType)
                {
                    return;
                }
            }
            SoundEngine.PlaySound(SoundID.SplashWeak, player.position);
            tile.LiquidType = BucketLiquidType;
            tile.LiquidAmount = byte.MaxValue;
            WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);
            Item.stack--;
            player.PutItemInInventoryFromItemUsage(ItemID.EmptyBucket, player.selectedItem);
            player.ApplyItemTime(Item);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
            }
        }
    }
}

public class BloodBucket : ExampleBucketBase
{
    public override string Texture => ContentPath + "Items/" + Name;

    public BloodBucket()
    {
        BucketLiquidType = LiquidLoader.LiquidType<BloodLiquid>();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults(); //base is called so we have the original contents (shimmer, ID sets, etc) added as well
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.WaterBucket;
        LiquidID_TLmod.Sets.CreateLiquidBucketItem[LiquidLoader.LiquidType<BloodLiquid>()] = Type;
    }
}