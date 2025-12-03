
using System;
using System.Linq;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader.Default;
using TheBindingOfRarria.Content.NPCs;

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
        if (Still && clone != null && Player.whoAmI != 250)
        {
            if (cloneNPC == -1)
                cloneNPC = NPC.NewNPC(NPC.GetSource_None(), (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<CovertClone>(), ai0: Player.whoAmI);

            clone.aggro = 2000;
            var item = clone.inventory.FirstOrDefault(i => i.type == clone.HeldItem.type);
            item = Player.HeldItem;
            clone.whoAmI = 250;
            Main.player[250] = clone;
            clone.active = true;
            clone.heldProj = Player.heldProj;
            clone.controlUseItem = true;
            clone.Update(clone.whoAmI);
            if (!item.IsAir)
            {
                var glob = item.GetGlobalItem<UseItemGlobalItem>();
                if (glob.CanUseItem(item, clone))
                {
                    glob.UseItem(item, clone);
                    ItemLoader.UseItem(item, clone);
                }
                if (glob.CanShoot(clone.HeldItem, clone))
                {
                    clone.PickAmmo(item, out var type, out var speed, out var dmg, out var kb, out var ammo);
                    var vel = clone.Center.DirectionTo(Main.MouseScreen + Main.screenPosition) * speed;
                    var pos = clone.Center;
                    glob.ModifyShootStats(item, clone, ref pos, ref vel, ref type, ref dmg, ref kb);
                    glob.Shoot(item, clone, new EntitySource_ItemUse_WithAmmo(clone, item, ammo), pos, vel, type, dmg, kb);
                }
            }
            clone.armor = Player.armor;
            clone.GetModPlayer<ModAccessorySlotPlayer>().exAccessorySlot = Player.GetModPlayer<ModAccessorySlotPlayer>().exAccessorySlot;
        }
        else if (cloneNPC != -1)
        {
            Main.npc[cloneNPC].StrikeInstantKill();
            cloneNPC = -1;
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
            clone.direction = Main.npc[cloneNPC].direction;
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

public class AggroOverrideCovertClone : GlobalNPC
{
    public override void Load()
    {
        On_NPC.AI += On_NPC_AI;
    }

    private void On_NPC_AI(On_NPC.orig_AI orig, NPC npc)
    {
        if (npc.target != -1 && Main.player[npc.target].TryGetModPlayer<CovertCloakPlayer>(out var p) && p.Still && p.clone != null)
        {
            npc.target = p.clone.whoAmI;

            p.clone.position = Main.npc[p.cloneNPC].position;

            orig(npc);

            npc.target = p.clone.whoAmI;
        }
        else if (npc.target == 250)
        {
            orig(npc);
        }
        else orig(npc);
    }

    public override bool PreAI(NPC npc)
    {
        if (npc.target != -1 && Main.player[npc.target].TryGetModPlayer<CovertCloakPlayer>(out var p) && p.Still && p.clone != null)
        {
            npc.target = p.clone.whoAmI;

            var boo = base.PreAI(npc);

            npc.target = p.clone.whoAmI;

            return boo;
        }
        else return base.PreAI(npc);
    }

    public override void AI(NPC npc)
    {
        if (npc.target != -1 && Main.player[npc.target].TryGetModPlayer<CovertCloakPlayer>(out var p) && p.Still && p.clone != null)
        {
            npc.target = p.clone.whoAmI;

            base.AI(npc);

            npc.target = p.clone.whoAmI;
        }
    }

    public override void PostAI(NPC npc)
    {
        if (npc.target != -1 && Main.player[npc.target].TryGetModPlayer<CovertCloakPlayer>(out var p) && p.Still && p.clone != null)
        {
            npc.target = p.clone.whoAmI;

            base.PostAI(npc);

            npc.target = p.clone.whoAmI;
        }
    }
}