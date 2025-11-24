using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using ModLiquidLib.Utils.Structs;
using System;
using Terraria.Audio;
using Terraria.GameContent.Liquid;
using Terraria.Graphics.Light;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Common;

public class BloodLiquid : ModLiquid
{
    public override string Texture => ContentPath + "Liquids/" + Name;
    public override string BlockTexture => ContentPath + "Liquids/" + Name + "_Block";
    public override string SlopeTexture => ContentPath + "Liquids/" + Name + "_Slope";


    //SetStaticDefaults are the defaults added when the game initially loads.
    //Here we set a few settings that this liquid will have.
    //SetStaticDefaults is only ever ran once just after all the content from mods are added to the game.
    public override void SetStaticDefaults()
    {
        //This is the viscosity of the liquid, only used visually.
        //Lava usually has this set to 200, while honey has this set to 240. All other liquids set this to 0 by default.
        LiquidRenderer.VISCOSITY_MASK[Type] = 180;

        //This is the length the liquid will visually have when flowing/falling downwards or if there is a slope underneath.
        LiquidRenderer.WATERFALL_LENGTH[Type] = 10;

        //This is the opacity of the liquid. How well you can see objects in the liquid.
        //The SlopeOpacity property is different, as slopes do not render the same as a normal liquid tile
        //DefaultOpacity is a vanilla array containing the definitions of each liquid type's default opacity for just white and color lighting modes
        LiquidRenderer.DEFAULT_OPACITY[Type] = 0.95f;
        SlopeOpacity = 1f;
        LiquidfallOpacityMultiplier = 0.5f; //Here we make the liquidfalls of this liquid draw at a 0.5x multiplier, making them seem much thicker
                                            //To change the old liquid rendering opacity, please see the RetroDrawEffects override.

        //For the Waves Quality setting, when set to Medium, waves are set to be the same distance no matter the liquid type.
        //To do this, the game applied a multiplier to make them all consistant between liquids. Here we set our own multiplier to make the waves the same distance.
        WaterRippleMultiplier = 0.3f;

        //This is used to specify what dust is used when splashing in this liquid.
        //Normally, when returning false in each OnSplash hook/method, this property is used in the mod liquid's default splash code
        //It returns -1 normally, which prevents the liquid from doing any splash dust
        //Here we set it, as we use the property in our OnSplash hooks to have one central variable that controls which dust ID is used in our custom splash
        SplashDustType = DustID.Blood;

        //This is used to specify what sound is played when an entity enters a liquid
        //Normally this property is used in the mod liquid's default splash code and returns null as no sound is played normally.
        //Similarly to SplashDustType, we use this to have 1 central place for the splash sound used accross each OnSplash hooks.
        SplashSound = SoundID.SplashWeak;

        FallDelay = 3; //The delay when liquids are falling. Liquids will wait this extra amount of frames before falling again.

        ChecksForDrowning = true; //If the player can drown in this liquid
        AllowEmitBreathBubbles = false; //Bubbles will come out of the player's mouth normally when drowning, here we can stop that by setting it to false.

        //For modders who don't want to reimplement the entire player movement for this liquid, this multiplier is used in the default mod liquid player movement.
        //Here we make our liquid slow the player down by half what honey would allow the player to move at.

        //Heres the defaults for each liquid:
        //Water/Lava/Regular modded liquid = 0.5f
        //Honey = 0.25f
        //Shimmer = 0.375f
        PlayerMovementMultiplier = 0.33f;
        StopWatchMPHMultiplier = PlayerMovementMultiplier; //We set stopwatch to the same multiplier as we don't want a different between whats felt and what the player can read their movement as.
        NPCMovementMultiplierDefault = PlayerMovementMultiplier; //NPCs have a similar modifier but as a field, here we set the default value as some other NPCs set this multiplier to 0. We set this to PlayerMovementMultiplier as we need them to all be the same.
        ProjectileMovementMultiplier = PlayerMovementMultiplier; //Simiarly to Players, Projectiles have this property for easy editing of a projectile velocity multiplier without needing to reimplement all of the projectile liquid movement code.

        FishingPoolSizeMultiplier = 2f; //The multiplier used for calculating the size of a fishing pool of this liquid. Here, each liquid tile counts as 2 for every tile in a fished pool.

        //For more dangerous liquids, we may want to have our liquid call On(Player/NPC/Projectile)Collision whenever an entity touches the liquid, rather than when an entity splashes in a liquid
        //For this we use similar collision calculations as lava using this boolean.
        //By default, this is disabled
        UsesLavaCollisionForWet = true;

        //Here we allow the extinguishing of the OnFire debuffs for both players and NPCs using this property
        ExtinguishesOnFireDebuffs = true;

        //This ID set controls what items classify as a sponge when trying to suck up this liquid
        //Here we remove the Ultra Absorbant sponge, Allow the Lava Absorbant sponge and the staff of regrowth to suck up this liquid
        LiquidID_TLmod.Sets.CanBeAbsorbedBy[Type].Add(ItemID.HoneyAbsorbantSponge);

        LiquidID_TLmod.Sets.CreateLiquidBucketItem[Type] = ModContent.ItemType<BloodBucket>();

        //UsesWaterFishingLootPool is used to prevent being able to get the default fishing loot from fishing in this liquid
        //By default this is false, but can be turned to true to allow for a modded liquids that want to replicate water
        LiquidID_TLmod.Sets.UsesWaterFishingLootPool[Type] = false;

        //We can add a map entry to our liquid, by doing so we can show where our liquid is on the map.
        //Unlike vanilla, we can also add a map entry name, which will display a name if the liquid is being selected on the map.
        AddMapEntry(new Color(89, 0, 22), CreateMapEntryName());
    }

    //Here with LiquidMerge, we are able to decide when the liquid generates with a different tile.
    //Using the otherLiquid param, we are able to select which liquid that collides with ours creates a specific tile.
    public override int LiquidMerge(int i, int j, int otherLiquid)
    {
        if (otherLiquid == LiquidID.Lava)
        {
            return TileID.Iron; 
        }
        else if (otherLiquid == LiquidID.Honey)
        {
            return TileID.Iron; 
        }
        else if (otherLiquid == LiquidID.Shimmer)
        {
            return TileID.ShimmerBlock; 
        }
        //The base return is what the liquid generates by default. This is useful for when this liquid collides with another modded liquids that this liquid has no support for.
        //usually by default, this method return TIleID.Stone, and generates a stone tile if it cannot recognise any predetermined tile type to generate with
        return 1;

        //NOTE: for custom collisions/tile creation, please see PreLiquidMerge to determine whether the liquid should do its normal tile merging,
        //or if you want to do other effects when this liquid merges with another liquid.
    }

    //LiquidMergeSound is played only on clients, and serves as editing the sound of when a liquid merges with another liquid.
    //Here we set a few custom sounds to play when this liquid merges with other liquids.
    public override void LiquidMergeSound(int i, int j, int otherLiquid, ref SoundStyle? collisionSound)
    {
        collisionSound = SoundID.SplashWeak with { Pitch = -0.5f };
    }

    //LiquidLightMaskMode is how the game decides what lightMaskMode to use when this liquid is over a tile
    //We set this to none, this is due to the liquid emitting light, needing no special lightMaskMode for its interaction with light.
    public override LightMaskMode LiquidLightMaskMode(int i, int j)
    {
        return LightMaskMode.Solid;
    }

    //ModifyLight allows the liquid to emit light similarly to any tile or wall.
    //You can use this to emit light similarly to lava or shimmer.
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0f;
        g = 0f;
        b = 0f;
    }

    //Using EvaporatesInHell, we are able to choose whether this liquid evaporates in hell, based on a condition.
    //For custom evaporation, use UpdateLiquid override.
    public override bool EvaporatesInHell(int i, int j)
    {
        //Here, our liquid in the bottom half of the underworld evaporates, while in the upper half does not evaporate
        if (j > Main.maxTilesY - 100)
        {
            return true;
        }
        return false;
    }

    //Using RetroDrawEffects, we can do stuff only during the rendering of liquids in the retro lighting style.
    //Here we set the opacity we want during retro lighting so that its consistant with the opacity of the liquid when not in the retro lighting style
    //NOTE: Despite being having RETRO in the name, this also applies to the "Trippy" Lighting style as well.
    public override void RetroDrawEffects(int i, int j, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
    {
        drawData.liquidAlphaMultiplier *= 1.8f;
        if (drawData.liquidAlphaMultiplier > 1f)
        {
            drawData.liquidAlphaMultiplier = 1f;
        }
    }

    //Here using ModifyNearbyTiles we do something similar to lava
    //by changing nearby tiles from one to another, not only transformaing grasses into their dirts, but also dirt into ash
    public override void ModifyNearbyTiles(int i, int j, int liquidX, int liquidY)
    {
        Tile tile = Main.tile[i, j];

        if (tile.TileType == TileID.Stone || tile.TileType == TileID.Mud || tile.TileType == TileID.Dirt || tile.TileType == TileID.Sand)
        {
            tile.TileType = TileID.Iron;
            WorldGen.SquareTileFrame(i, j);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, i, j, 3);
            }
        }
    }

    //Using the OnPump hook we are able to do extra effects and such when our liquid is being transported
    //Here we play an explosion sound by creating a custom packet and playing a sound inside of the executable method
    //pump logic (along with all other wiring logic is only executed on servers, which is why a custom packet is required
    public override bool OnPump(int inX, int inY, int outX, int outY)
    {
        return true;
    }

    //Here we use the OnNPCCollision and OnPlayerCollision hooks to apply effects to both entities
    //Firstly, we apply the dryad's ward debuff to NPCs
    public override void OnNPCCollision(NPC npc)
    {
        //Make sure that the NPC can take damage, and the game is not a player on a server
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {

        }
    }

    public override void OnPlayerCollision(Player player)
    {
        player.AddBuff(BuffID.Ironskin, 60 * 30, false, false);
    }

    //Here we animate our liquid seperately from other liquids in the game.
    //Instead of having our liquid animate normally, we animate it simiarly, except the liquid is animated almost half as slow
    public override void AnimateLiquid(GameTime gameTime, ref int frame, ref float frameState)
    {
        float frameSpeed = Main.windSpeedCurrent * 25f;

        frameSpeed = Main.windSpeedCurrent * 15f;
        frameSpeed = ((!(frameSpeed < 0f)) ? (frameSpeed + 5f) : (frameSpeed - 5f));
        frameSpeed = MathF.Abs(frameSpeed);
        frameState += frameSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (frameState < 0f)
            frameState += 16f;

        frameState %= 16f;

        frame = (int)frameState;
    }

    //These methods provide a way to edit the waves produced by a liquid
    //both modify the waves to be slightly larger and offset slightly less when moving through
    public override void NPCRippleModifier(NPC npc, ref float rippleStrength, ref float rippleOffset)
    {
        if (!npc.wet)
            rippleOffset = -1f;

        float factor = ((float)(int)npc.wetCount / 9f);

        factor = ((float)(int)npc.wetCount / 9f);
        rippleStrength += 0.25f * factor;

        rippleStrength *= 0.5f;
    }

    public override void PlayerRippleModifier(Player player, ref float rippleStrength, ref float rippleOffset)
    {
        if (!player.wet)
            rippleOffset = -1f;

        float factor = ((float)(int)player.wetCount / 9f);

        factor = ((float)(int)player.wetCount / 9f);
        rippleStrength += 0.5f * factor;

        rippleStrength *= 0.75f;
    }

    //The following region contains all logic related to modifying the movement of entities in this liquid, making Players, Items, NPCs and Projectiles move slower in this liquid
    #region Entity Movement Hooks/Methods
    //Here we replicate normal liquid movement behaviour using the PlayerLiquidMovement hook/method
    public override bool PlayerLiquidMovement(Player player, bool fallThrough, bool ignorePlats)
    {
        int num = ((!player.onTrack) ? player.height : (player.height - 20));
        Vector2 velocity = player.velocity;
        player.velocity = Collision.TileCollision(player.position, player.velocity, player.width, num, fallThrough, ignorePlats, (int)player.gravDir);
        Vector2 vector2 = player.velocity * PlayerMovementMultiplier; //We reuse the PlayerMovementMultiplier here for it to serve the same purpose
        if (player.velocity.X != velocity.X)
        {
            vector2.X = player.velocity.X;
        }
        if (player.velocity.Y != velocity.Y)
        {
            vector2.Y = player.velocity.Y;
        }
        player.position += vector2;
        player.TryFloatingInFluid();
        return false; //We return false as we do not want the normal liquid movement to execute after this hook/method
    }

    //Unfortunately, liquid movement is effected in 2 parts (for players)
    //Liquid velocity multipliers and gravity modifiers
    //Here we change the gravity of the player when in this liquid
    public override void PlayerGravityModifier(Player player, ref float gravity, ref float maxFallSpeed, ref int jumpHeight, ref float jumpSpeed)
    {
        //These values are half of what honey applies to the player
        gravity = 0.05f;
        maxFallSpeed = 1.5f;
        //In other liquids, the jump speed and height is increased to simulate "swimming"
        //We don't want that for our liquid so we don't modify the other hook/method parameters
    }

    //related above, we use this method/hook to make items move at half the speed that they would when in honey
    public override void ItemLiquidCollision(Item item, ref Vector2 wetVelocity, ref float gravity, ref float maxFallSpeed)
    {
        gravity = -0.02f;
        maxFallSpeed = 1f;
        wetVelocity = item.velocity * 0.125f;
    }

    //Like above, we set the refs to the values we want to control the gravity and maxfallspeed
    //This handles NPC movement in liquids, for example liquid specifically we make the gravity and maxFallSpeed half of what honey would be
    public override void NPCGravityModifier(NPC npc, ref float gravity, ref float maxFallSpeed)
    {
        gravity = 0.05f;
        maxFallSpeed = 1f;
        //You may notice that NPCLiquidMovement doesn't have a wetVelocity param, set the NPCMovementMultiplierDefault property in SetStaticDefaults to change the wet velocity multiplier for NPCs
        //The property is seperate because of NPCs indivdually setting the multiplier value based on type (DD2 npcs set it to 1f to ignore liquid movement)
    }

    //lastly, we reimplement the projectile movement in liquids using the ProjectileLiquidMovement
    //This hook is very similar and different to PlayerLiquidMovement, returning a bool and only having wetVelocity as a referenced parameter
    //Take a look at Projectile.HandleMovement to see how vanilla handles liquid movement for projectiles.
    public override bool ProjectileLiquidMovement(Projectile projectile, ref Vector2 wetVelocity, Vector2 collisionPosition, int Width, int Height, bool fallThrough)
    {
        Vector2 vector = projectile.velocity;
        projectile.velocity = Collision.TileCollision(collisionPosition, projectile.velocity, Width, Height, fallThrough, fallThrough);
        wetVelocity = projectile.velocity * ProjectileMovementMultiplier; //We reuse the ProjectileMovementMultiplier here for it to serve the same purpose
        if (projectile.velocity.X != vector.X)
        {
            wetVelocity.X = projectile.velocity.X;
        }
        if (projectile.velocity.Y != vector.Y)
        {
            wetVelocity.Y = projectile.velocity.Y;
        }
        return false; //We return false as we do not want the normal liquid movement to execute after this hook/method
    }
    #endregion

    //The following region contains all the logic for what this liquid does when being entered and exited by different entities.
    #region Splash Effects

    //Each hook/method is used to execute what would happen when something enters/exits a liquid.
    //There is a hook for the following
    // * Players
    // * NPCs
    // * Projectiles
    // * Items
    //Each hook has a "isEnter" param, which is true whenever entity is entering the liquid.
    //This is usually used to do different effects when entering a liquid rather than exiting one

    //The following hooks/methods have adapted code from vanilla's splash code for honey as the splash dusts themselves are based on the honey splash dust.
    public override bool OnPlayerSplash(Player player, bool isEnter)
    {

        return false;
    }

    public override bool OnNPCSplash(NPC npc, bool isEnter)
    {

        return false;
    }

    public override bool OnProjectileSplash(Projectile proj, bool isEnter)
    {

        return false;
    }

    public override bool OnItemSplash(Item item, bool isEnter)
    {

        return false;
    }
    #endregion
}

public class BloodBuffEnemy : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (npc.GetWet(LiquidLoader.LiquidType<BloodLiquid>()))
            npc.lifeRegen += 20;
    }
}