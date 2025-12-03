
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.GameInput;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader.Default;
using TheBindingOfRarria.Content.NPCs;
using static Terraria.Player;

namespace TheBindingOfRarria.Content.Items;

[AutoloadEquip(EquipType.Back, EquipType.Front)]
public class CovertCloak : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void Load()
    {
        EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
    }

    public override void SetDefaults()
    {
        Item.height = 64;
        Item.width = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Green;

        if (Main.netMode != NetmodeID.Server)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<CovertCloakPlayer>().Equipped = true;
        player.aggro -= 3000;
    }

    public override void UpdateVanity(Player player)
    {
        player.GetModPlayer<CovertCloakPlayer>().Equipped = true;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, 0, origin, scale * 1.4f, SpriteEffects.None, 0);

        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Main.GetItemDrawFrame(Type, out var texture, out var frame);
        var origin = frame.Size() / 2;
        var position = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y * 0.8f);

        spriteBatch.Draw(texture, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0);

        return false;
    }
}

public class CovertCloakPlayer : ModPlayer
{
    public int counter = 0;

    public bool Equipped = false;

    public bool Still = false;

    public Player? clone = null;

    public int cloneNPC = -1;

    public override void Load()
    {
        On_LegacyPlayerRenderer.DrawPlayer += On_LegacyPlayerRenderer_DrawPlayer;
    }

    private void On_LegacyPlayerRenderer_DrawPlayer(On_LegacyPlayerRenderer.orig_DrawPlayer orig, LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float scale)
    {
        orig (self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale);
        if (drawPlayer.whoAmI != 250 && shadow == 0 && drawPlayer.TryGetModPlayer<CovertCloakPlayer>(out var p) && p.clone != null && p.Still)
        {
            var info = new PlayerDrawSet
            {
                drawPlayer = p.clone,
                DrawDataCache = self._drawData,
                DustCache = self._dust,
                GoreCache = self._gore
            };
            self.DrawPlayer(camera, info.drawPlayer, info.drawPlayer.position, info.drawPlayer.fullRotation, info.drawPlayer.Size / 2);
            //orig(self, camera, p.clone, p.clone.position, rotation, rotationOrigin, shadow, scale);
        }
    }

    public override void OnEnterWorld()
    {
        clone = new Player
        {
            whoAmI = 250
        };
        Main.player[250] = clone;
    }

    public override void ResetEffects()
    {
        Equipped = false;
    }

    public override void PostUpdate()
    {
        if (counter > 0)
        {
            counter--;
            if (cloneNPC != -1)
                Main.npc[cloneNPC].StrikeInstantKill();
            cloneNPC = -1;

            clone?.position = Vector2.Zero;
            clone?.aggro = -10000;
        }
        else if (Still && clone != null && Player.whoAmI != 250)
        {
            if (clone.statLife == 0)
                counter = 36;

            if (cloneNPC == -1)
                cloneNPC = NPC.NewNPC(NPC.GetSource_None(), (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<CovertClone>(), ai0: Player.whoAmI);

            clone.aggro = 2000;
            var item = Player.HeldItem.Clone();
            clone.inventory[clone.selectedItem] = item;
            clone.whoAmI = 250;
            Main.player[250] = clone;
            clone.buffTime = Player.buffTime;
            clone.buffType = Player.buffType;
            clone.armor = Player.armor;
            for (int i = 54; i < 58; i++)
                clone.inventory[i] = Player.inventory[i].Clone();
            clone.ConsumedLifeCrystals = Player.ConsumedLifeCrystals;
            clone.ConsumedLifeFruit = Player.ConsumedLifeFruit;
            clone.ConsumedManaCrystals = Player.ConsumedManaCrystals;
            clone.GetModPlayer<ModAccessorySlotPlayer>().exAccessorySlot = Player.GetModPlayer<ModAccessorySlotPlayer>().exAccessorySlot;
            clone.active = true;
            clone.heldProj = Player.heldProj;
            clone.controlUseItem = true;
            clone.channel = true;
            clone.Update_NPCCollision();
            clone.UpdateControlHolds();
            PlayerLoader.PreUpdateBuffs(clone);
            clone.UpdateBuffs(clone.whoAmI);
            PlayerLoader.PostUpdateBuffs(clone);
            clone.UpdateEquips(clone.whoAmI);
            clone.UpdateArmorSets(clone.whoAmI);
            PlayerLoader.PostUpdateEquips(clone);
            clone.PlayerFrame();
            PlayerLoader.PostUpdate(clone);
            if (!item.IsAir)
            {
                var old = Main.myPlayer;
                Main.myPlayer = clone.whoAmI;
                if (CombinedHooks.CanShoot(clone, clone.HeldItem))
                {
                    //Player.PickAmmo(item, out var type, out var speed, out var dmg, out var kb, out var ammo, true);
                    //var vel = clone.Center.DirectionTo(Main.MouseScreen + Main.screenPosition) * speed;
                    //var pos = clone.Center;
                    //ItemLoader.ModifyShootStats(item, clone, ref pos, ref vel, ref type, ref dmg, ref kb);
                    //ItemLoader.Shoot(item, clone, new EntitySource_ItemUse_WithAmmo(clone, item, ammo), pos, vel, type, dmg, kb);

                    //IEntitySource projectileSource_Item_WithPotentialAmmo = clone.GetProjectileSource_Item_WithPotentialAmmo(clone.HeldItem, ammo);
                    //Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pos, vel, type, dmg, kb);

                    bool flag = true;
                    int type = clone.HeldItem.type;
                    if ((type == 65 || type == 676 || type == 723 || type == 724 || type == 757 || type == 674 || type == 675 || type == 989 || type == 1226 || type == 1227) && !clone.ItemAnimationJustStarted)
                    {
                        flag = false;
                    }
                    if (type == 3852 && clone.altFunctionUse == 2 && !clone.ItemAnimationJustStarted)
                    {
                        flag = false;
                    }
                    if (type == 5451 && clone.ownedProjectileCounts[1020] > 0)
                    {
                        flag = false;
                    }
                    if (clone.HeldItem.useLimitPerAnimation.HasValue && clone.ItemUsesThisAnimation >= clone.HeldItem.useLimitPerAnimation.Value)
                    {
                        flag = false;
                    }
                    clone.ItemCheck_TurretAltFeatureUse(clone.HeldItem, flag);
                    clone.ItemCheck_MinionAltFeatureUse(clone.HeldItem, flag);
                    bool flag2 = clone.itemAnimation >= 0 && clone.ItemTimeIsZero && flag;
                    if (clone.HeldItem.shootsEveryUse)
                    {
                        flag2 = clone.ItemAnimationJustStarted;
                    }
                    if (flag2)
                    {
                        clone.ItemCheck_Shoot(clone.whoAmI, clone.HeldItem, clone.HeldItem.damage);
                        clone.ApplyItemAnimation(clone.HeldItem);
                    }
                    //clone.ApplyItemTime(clone.HeldItem);
                    //
                }
                clone.ItemCheckWrapped(clone.whoAmI);
                clone.itemAnimation++;
                clone.itemTime++;
                Main.myPlayer = old;
            }
            clone.HorizontalMovement();
        }
        else if (cloneNPC != -1)
        {
            Main.npc[cloneNPC].StrikeInstantKill();
            cloneNPC = -1;
            counter = 60;
        }
    }

    public override void PreUpdateMovement()
    {
        if (Player.velocity.LengthSquared() > 1)
            Still = false;
        else Still = true;

        if (Still && clone != null && cloneNPC != -1)
        {
            clone.position = Main.npc[cloneNPC].position;
            clone.direction = Main.npc[cloneNPC].direction == 0 ? clone.direction : Main.npc[cloneNPC].direction;
            clone.velocity = Main.npc[cloneNPC].velocity;
        }
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.drawPlayer.GetModPlayer<CovertCloakPlayer>().Equipped || drawInfo.drawPlayer.front == EquipLoader.GetEquipSlot(Mod, "CovertCloak", EquipType.Front))
        {
            drawInfo.drawPlayer.head = EquipLoader.GetEquipSlot(Mod, "CovertCloak", EquipType.Head);
            drawInfo.colorHead = Color.Transparent;
            drawInfo.drawsBackHairWithoutHeadgear = false;
            drawInfo.drawPlayer.face = -1;
        }
    }

    public override void FrameEffects()
    {
        if (Equipped)
        {
            Player.head = EquipLoader.GetEquipSlot(Mod, "CovertCloak", EquipType.Head);
            Player.face = -1;
        }
    }

}

public class UseItemGlobalItem : GlobalItem
{
    public override bool? UseItem(Item item, Player player)
    {
        player.GetModPlayer<CovertCloakPlayer>().Still = false;
        return base.UseItem(item, player);
    }
}