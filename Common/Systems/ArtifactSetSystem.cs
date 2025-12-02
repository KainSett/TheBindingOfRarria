using Daybreak.Common.Features.ItemSlots;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Terraria.Localization;
using Terraria.ModLoader.Default;
using Terraria.UI;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Common.Registries;
using TheBindingOfRarria.Content.ArtifactSets;
using TheBindingOfRarria.Content.Items;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Common.Systems;

public class ArtifactSetSystem : ModSystem
{
    public static List<IArtifactSet> ArtifactSets = [];

    public override void Unload()
    {
        ArtifactSets.Clear();
    }

    public override void Load()
    {
        On_ItemSlot.DrawItemIcon += On_ItemSlot_DrawItemIcon;
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

    private float On_ItemSlot_DrawItemIcon(On_ItemSlot.orig_DrawItemIcon orig, Item item, int context, SpriteBatch spriteBatch, Vector2 screenPositionForItemCenter, float scale, float sizeLimit, Color environmentColor)
    {
        if (Main.LocalPlayer.TryGetModPlayer<ArtifactSetPlayer>(out var p) && p.Sets is not null && p.Sets.Count > 0 && p.Sets.All(s => s.Artifacts is not null && s.Artifacts.Count > 0) && p.Sets.Any(s => s.Contains(item.type) && s.Count >= s.Artifacts.Count) && p.Equipped.Contains(item.type))
        {/*
            var texture = Textures._light[2];
            var color = environmentColor.MultiplyRGB(p.Sets.FirstOrDefault(s => s.Contains(item.type) && s.Count >= s.Artifacts.Count).NameColor) * 1.0f;
            color.A = 0;
            var rotation = Main.GlobalTimeWrappedHourly * 2 + item.type;
            var Scale = 0.07f + 0.01f * float.Sin(Main.GlobalTimeWrappedHourly);
            color *= 135 / 255f;

            for (int i = 0; i < 1; i ++)
                spriteBatch.Draw(texture.Value, screenPositionForItemCenter, null, color, rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
            
            texture = Textures._light[1];
            rotation = -Main.GlobalTimeWrappedHourly * 2 + item.type;
            Scale = 0.07f;

            spriteBatch.Draw(texture.Value, screenPositionForItemCenter, null, color, rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
        return orig(item, context, spriteBatch, screenPositionForItemCenter, scale, sizeLimit, environmentColor);*/
            var effect = Effects.Outline;
            if (effect != null && effect.Value != null)
            {
                var gd = Main.graphics.GraphicsDevice;

                if (gd.PresentationParameters.RenderTargetUsage != RenderTargetUsage.PreserveContents)
                {
                    gd.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                }

                var oldTargets = gd.GetRenderTargets();

                foreach (var target in oldTargets)
                {
                    if (target.RenderTarget is RenderTarget2D rt)
                        rt.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                }

                //gd.SetRenderTarget(Target);
                //gd.Clear(Color.Transparent);


                var started = spriteBatch.beginCalled;
                var parameters = new Helper.SpritebatchParameters();
                if (started)
                    spriteBatch.End(out parameters);

                var buffer = 2f;
                effect.Value.Parameters["Scale"].SetValue(scale);
                effect.Value.Parameters["Pixels"].SetValue(3f);
                effect.Value.Parameters["ScaleBuffer"].SetValue(buffer);
                effect.Value.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Value.Parameters["uImageSize0"].SetValue(!Main.itemAnimationsRegistered.Contains(item.type) ? TextureAssets.Item[item.type].Value.Size() : Main.itemAnimations[item.type].GetFrame(TextureAssets.Item[item.type].Value).Size());
                effect.Value.Parameters["Color"].SetValue(environmentColor.MultiplyRGB(p.Sets.FirstOrDefault(s => s.Contains(item.type) && s.Count >= s.Artifacts.Count).NameColor).ToVector3()); 

                //effect.Value.Parameters["color"].SetValue(environmentColor.MultiplyRGB(p.Sets.FirstOrDefault(s => s.Contains(item.type) && s.Count >= s.Artifacts.Count).NameColor).ToVector3()); 
                //effect.Value.Parameters["opacity"].SetValue(1);
                effect.Value.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Begin(parameters with { effect = effect.Value });
                //var oldEffect = spriteBatch.customEffect;
                //spriteBatch.customEffect = effect.Value;

                var result = orig(item, context, spriteBatch, screenPositionForItemCenter, scale * buffer, sizeLimit, environmentColor);
                //spriteBatch.Draw(Target, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);

                spriteBatch.End();

                if (started)
                    spriteBatch.Begin(parameters);

                //spriteBatch.customEffect = oldEffect;
                //gd.SetRenderTargets(oldTargets);

                return result;
            }
            else return orig(item, context, spriteBatch, screenPositionForItemCenter, scale, sizeLimit, environmentColor);
        }
        else return orig(item, context, spriteBatch, screenPositionForItemCenter, scale, sizeLimit, environmentColor);
    }
}


public interface IArtifactSet
{
    public List<int> Items { get; }

    public List<Predicate<int>> Artifacts { get; set; }

    public string Name { get; }

    public Color NameColor { get; }

    public void Update(int who)
    {
        if (Artifacts is null || Artifacts.Count <= 0)
            SetConditions();

        if (!Main.player[who].TryGetModPlayer<ArtifactSetPlayer>(out var p) || !Main.player[who].TryGetModPlayer<ModAccessorySlotPlayer>(out var plr))
            return;

        foreach (var item in Artifacts)
        {
            if (!CheckEquipped(who, item, out var Item))
                continue;
            else
            {
                AccessoryNames[Artifacts.IndexOf(item)] = Item.Name;

                Count += 1;
            }
        }
        if (Count < Artifacts.Count)
            return;

        Effect(who);
    }

    public void Effect(int who)
    {

    }

    public void SetConditions()
    {
        Artifacts = [];
        AccessoryNames = [];
        foreach (var item in Items)
        {
            var tree = Array.FindAll(Main.recipe, (r => r.HasIngredient(item)));

            List<int> i = [];
            foreach (var it in tree)
                i.Add(it.createItem.type);

            if (tree is null || i is null || i.Count <= 0)
            {
                Artifacts.Add(c => c == item);
            }

            else
            {
                Artifacts.Add(c => i.Contains(c) || c == item);
            }

            AccessoryNames.Add(new Item(item).Name);
        }
    }

    public bool Contains(int type)
    {
        return Artifacts.Any(art => art(type));
    }

    public bool CheckEquipped(int who, Predicate<int> predicate, out Item item)
    {
        var p = Main.player[who].GetModPlayer<ArtifactSetPlayer>();

        var modded = Main.player[who].GetModPlayer<ModAccessorySlotPlayer>().exAccessorySlot.FirstOrDefault(i => predicate(i.type));
        var vanilla = Main.player[who].armor.FirstOrDefault(i => predicate(i.type));
        item = vanilla is null ? modded : vanilla;

        return p.Equipped.Any(i => predicate(i));
    }

    public List<string> AccessoryNames { get; set; }

    public int Count { get; set; }
}

public class ArtifactSetPlayer : ModPlayer
{
    public HashSet<IArtifactSet> Sets = [];

    public List<int> Equipped = [];

    public override void PostUpdateEquips()
    {
        foreach (var set in Sets)
            set.Update(Player.whoAmI);
    }

    public override void PreUpdate()
    {
        foreach (var set in Sets)
           set.Count = 0;

        Equipped.Clear();
    }

    public override void Initialize()
    {
        Sets.Clear();
        foreach (var set in ArtifactSetSystem.ArtifactSets)
        {
            Sets.Add(set);
        }
    }
}

public class Artifact : GlobalItem
{
    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        player.GetModPlayer<ArtifactSetPlayer>().Equipped.Add(item.type);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!tooltips.Any(t => t.Name.Contains("Social")) && Main.LocalPlayer.TryGetModPlayer<ArtifactSetPlayer>(out var p))
        {
            foreach (var set in p.Sets)
                if (set.Contains(item.type))
                {
                    var shift = Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.Info.Closed");
                    var name = $"[c/{set.NameColor.Hex3()}:" + Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{set.Name}.Name");
                    if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
                    {
                        shift = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.Info.Equipped"), set.Count, set.Artifacts.Count);

                        var line1 = name + shift;

                        var tooltip = new TooltipLine(Mod, "ArtifactSetEffect", Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{set.Name}.Effect"));
                        tooltip.OverrideColor = set.Count >= set.Artifacts.Count ? Color.White : Color.Gray;

                        var n = new TooltipLine(Mod, "ArtifactSet", line1);

                        var index = tooltips.LastIndexOf(tooltips.LastOrDefault(t => t.Name != "ArtifactSetBonus" && t.Name.Contains("Artifact")));

                        tooltips.Insert(index == -1 ? 1 : index + 1, tooltip);
                        tooltips.Insert(index == -1 ? 1 : index + 1, n);
                        for (int i = (index == -1 ? 0 : index) + 3; i < tooltips.Count; i++)
                            tooltips[i].Hide();

                        tooltip = new TooltipLine(Mod, "ArtifactSetInfoConsists", /*"\n" + */Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.Info.Consists"));
                        tooltips.Add(tooltip);

                        var names = set.AccessoryNames;
                        foreach (var na in names)
                        {
                            var nam = new TooltipLine(Mod, "ArtifactNames", na);
                            if (!set.CheckEquipped(Main.myPlayer, set.Artifacts[names.IndexOf(na)], out var Item))
                                nam.OverrideColor = Color.Gray;

                                tooltips.Add(nam);
                        }

                        continue;
                    }
                    var text = name + shift;

                    var t = new TooltipLine(Mod, "ArtifactSet", text);

                    tooltips.Insert(1, t);
                }
        }
    }
}