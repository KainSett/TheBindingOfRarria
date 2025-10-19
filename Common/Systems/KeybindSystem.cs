using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace TheBindingOfRarria.Common.Config;

public class KeybindSystem : ModSystem
{
    public static ModKeybind ZaWardoKey { get; private set; }
    public static ModKeybind BloodDripKey { get; private set; }
    public static ModKeybind AbsorbingKey { get; private set; }
    //public static ModKeybind CrystalDashKey { get; private set; }
    public static ModKeybind BloodhoundDashKey { get; private set; }
    public static ModKeybind TreeSentinelKey { get; private set; }
    public static ModKeybind ParryKey { get; private set; }

    public override void Load()
    {
        ZaWardoKey = KeybindLoader.RegisterKeybind(Mod, "BrokenWatch", Keys.P);
        BloodDripKey = KeybindLoader.RegisterKeybind(Mod, "CursedBlood", Keys.K);
        AbsorbingKey = KeybindLoader.RegisterKeybind(Mod, "AbsorbingLiquid", Keys.O);
        //CrystalDashKey = KeybindLoader.RegisterKeybind(Mod, "CrystalHeart", Keys.V);
        BloodhoundDashKey = KeybindLoader.RegisterKeybind(Mod, "BloodhoundDash", Keys.C);
        TreeSentinelKey = KeybindLoader.RegisterKeybind(Mod, "GoldenRetaliation", Keys.L);
        ParryKey = KeybindLoader.RegisterKeybind(Mod, "Parry", Keys.N);
    }
}
