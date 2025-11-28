
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.WorldBuilding;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Registries;

namespace TheBindingOfRarria.Content.Items;

public class UnnaturalScales : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void Load()
    {
        // All code below runs only if we're not loading on a server
        if (Main.netMode != NetmodeID.Server)
        {
            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.vanity = true;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;

        if (Main.netMode != NetmodeID.Server)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<RekSaiArmorPlayer>().Equipped = true;
        for (int i = 0; i < 3; i++)
        {
            Item.NewItem(player.armor[i].GetSource_DropAsItem(), player.Center, player.armor[i]);
            player.armor[i].TurnToAir();
        }

        if (!hideVisual)
            player.GetModPlayer<RekSaiArmorPlayer>().Echo = true;
    }

    public override void UpdateVanity(Player player)
    {
        player.GetModPlayer<RekSaiArmorPlayer>().Equipped = true;
    }
}

public class RekSaiArmorPlayer : ModPlayer
{
    public bool Equipped = false;

    public bool Echo = false;

    public override void PreUpdate()
    {
        Equipped = false;
        Echo = false;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.drawPlayer.GetModPlayer<RekSaiArmorPlayer>().Equipped)
        {
            drawInfo.drawPlayer.legs = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Legs);
            drawInfo.drawPlayer.body = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Body);
            drawInfo.drawPlayer.head = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Head);
            drawInfo.colorLegs = Color.Transparent;
            drawInfo.colorPants = Color.Transparent;
            drawInfo.colorBodySkin = Color.Transparent;
            drawInfo.hidesBottomSkin = true;
            drawInfo.hidesTopSkin = true;
            drawInfo.armorHidesArms = true;
            drawInfo.colorHead = Color.Transparent;
            drawInfo.drawsBackHairWithoutHeadgear = false;
            drawInfo.drawPlayer.face = -1;
            drawInfo.legsOffset += new Vector2(-6, 0) * drawInfo.drawPlayer.direction;
            EcholocationSystem.Instance.ApplySobelDarkening(0.2f, 10, 10, 0.035f, 1.2f);
        } 
    }

    public override void FrameEffects()
    {
        if (Equipped)
        {
            Player.legs = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Legs);
            Player.body = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Body);
            Player.head = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Head);
            Player.face = -1;
        }
    }
}
public class ScreenShaderSettings
{
    public bool UseEffect;

    public float Power;
    public float PowerTarget;

    public float Timer;

    public float[] Data = new float[8];
}

public class EcholocationSystem : ModSystem
{
    public static EcholocationSystem Instance => GetInstance<EcholocationSystem>();

    public static string SobelKey = "VanillaPlus:SobelDarken";

    public ScreenShaderSettings SobelSettings = new();

    private static FilterManager FilterManager => Filters.Scene;

    public static void LoadFilters()
    {
        Filters.Scene[SobelKey] = new Filter(new ScreenShaderData(Effects.SobelDarken, "ScreenPass"), EffectPriority.Medium);
    }

    #region sobel

    public void ApplySobelDarkening(float power, float exponent = 2f, float duration = -1, float applicationPower = 0.05f, float sumOffset = 0, float softness = 0.3f, float radiusPercent = 0.7f)
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        SobelSettings.UseEffect = true;
        SobelSettings.PowerTarget = power;
        SobelSettings.Data[0] = exponent;
        SobelSettings.Data[1] = applicationPower;
        SobelSettings.Data[2] = sumOffset;
        SobelSettings.Data[3] = softness;
        SobelSettings.Data[4] = radiusPercent;

        if (duration != -1)
            SobelSettings.Timer = duration;
    }

    public void ClearSobelDarkening()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        SobelSettings.UseEffect = false;
    }

    private void UpdateSobel()
    {
        if (FilterManager[SobelKey] == null)
            return;

        if (SobelSettings.Timer > 0)
        {
            if (--SobelSettings.Timer <= 0)
                SobelSettings.UseEffect = false;
        }

        SobelSettings.Power = Clamp(SobelSettings.Power, 0, SobelSettings.PowerTarget);

        if (SobelSettings.UseEffect)
        {
            if (!FilterManager[SobelKey].IsActive())
                FilterManager.Activate(SobelKey);

            SobelSettings.Power = Lerp(SobelSettings.Power, SobelSettings.PowerTarget, SobelSettings.Data[1]);

            //you could fade the vignette in/out using the above method with another Data[] index.

            var shaderData = FilterManager[SobelKey].GetShader();
            shaderData.Shader.Parameters["progress"].SetValue(SobelSettings.Power);
            shaderData.Shader.Parameters["power"].SetValue(SobelSettings.Data[0]);
            shaderData.Shader.Parameters["offset"].SetValue(SobelSettings.Data[2]);
            shaderData.Shader.Parameters["vignetteSoftness"].SetValue(SobelSettings.Data[3]);
            shaderData.Shader.Parameters["vignetteRadius"].SetValue(SobelSettings.Data[4]);
        }

        else
        {
            SobelSettings.Power = Lerp(SobelSettings.Power, 0f, SobelSettings.Data[1]);

            if (SobelSettings.Power <= 0f)
            {
                if (FilterManager[SobelKey].IsActive())
                    FilterManager.Deactivate(SobelKey);
            }

            else
            {
                var shaderData = FilterManager[SobelKey].GetShader();
                shaderData.Shader.Parameters["progress"].SetValue(SobelSettings.Power);
                shaderData.Shader.Parameters["power"].SetValue(SobelSettings.Data[0]);
                shaderData.Shader.Parameters["offset"].SetValue(SobelSettings.Data[2]);
                shaderData.Shader.Parameters["vignetteSoftness"].SetValue(SobelSettings.Data[3]);
                shaderData.Shader.Parameters["vignetteRadius"].SetValue(SobelSettings.Data[4]);
            }
        }
    }

    #endregion

    public override void PostUpdateEverything()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        UpdateSobel();
    }
    
    public override void Load()
    {
        LoadFilters();
        On_Main.DoDraw_WallsTilesNPCs += On_Main_DoDraw_WallsTilesNPCs;
        On_Main.DrawProjectiles += On_Main_DrawProjectiles;
        On_Main.DrawBackground += On_Main_DrawBackground;
        if (!Main.dedServ)
        {
            Main.OnResolutionChanged += InitializeRT;
            Main.RunOnMainThread(() =>
            {
                Target = new(Main.instance.GraphicsDevice,
                    Main.screenWidth, Main.screenHeight,
                    false, SurfaceFormat.Color, DepthFormat.None, 0,
                    RenderTargetUsage.PreserveContents
                );
            });
        }
    }

    private static void InitializeRT(Vector2 obj)
    {
        if (Main.dedServ)
            return;

        Target?.Dispose();

        GraphicsDevice gd = Main.instance.GraphicsDevice;
        int width = Main.screenWidth;
        int height = Main.screenHeight;

        Target = new(gd, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }

    private static RenderTarget2D Target { get; set; }

    private void On_Main_DrawBackground(On_Main.orig_DrawBackground orig, Main self)
    {
        if (!Main.LocalPlayer.GetModPlayer<RekSaiArmorPlayer>().Echo)
        {
            orig(self);
        }
    }

    private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
    {
        if (Main.LocalPlayer.GetModPlayer<RekSaiArmorPlayer>().Echo)
        {
            var effect = Effects.SobelDarken;

            if (effect == null || effect.Value == null)
                return;

            var gd = Main.graphics.GraphicsDevice;

            if (gd.PresentationParameters.RenderTargetUsage != RenderTargetUsage.PreserveContents)
            {
                gd.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            }

            if (SobelSettings.Power + 0.0005f >= SobelSettings.PowerTarget)
                Lighting.AddLight(Main.LocalPlayer.Center, new Vector3(1) * 30);

            gd.SetRenderTarget(Target);
            gd.Clear(Color.Transparent);

            orig(self);

            Helper.SpritebatchParameters parameters = new();
            var beginned = Main.spriteBatch.beginCalled;

            if (beginned)
                Main.spriteBatch.End(out parameters);

            EcholocationSystem.Instance.ApplySobelDarkening(0.2f, 10, 10, 0.035f, 1.2f);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, effect.Value);

            Main.spriteBatch.Draw(Target, new Vector2(0, 0), Color.White);


            Main.spriteBatch.End();
            if (beginned)
                Main.spriteBatch.Begin(parameters);
        }
        else orig(self);
    }

    private void On_Main_DoDraw_WallsTilesNPCs(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
    {
        if (!Main.LocalPlayer.GetModPlayer<RekSaiArmorPlayer>().Echo)
        {
            var effect = Effects.SobelDarken;

            if (effect == null || effect.Value == null)
                return;

            var gd = Main.graphics.GraphicsDevice;

            if (gd.PresentationParameters.RenderTargetUsage != RenderTargetUsage.PreserveContents)
            {
                gd.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            }

            if (SobelSettings.Power == SobelSettings.PowerTarget)
                Lighting.AddLight(Main.LocalPlayer.Center, new Vector3(1) * 30);

            gd.SetRenderTarget(Target);
            gd.Clear(Color.Transparent);

            orig(self);

            Helper.SpritebatchParameters parameters = new();
            var beginned = Main.spriteBatch.beginCalled;

            if (beginned)
                Main.spriteBatch.End(out parameters);

            EcholocationSystem.Instance.ApplySobelDarkening(0.2f, 10, 10, 0.035f, 1.2f);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, effect.Value);

            Main.spriteBatch.Draw(Target, new Vector2(0, 0), Color.White);


            Main.spriteBatch.End();
            if (beginned)
                Main.spriteBatch.Begin(parameters);
        }
        else orig(self);
    }
}