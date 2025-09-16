using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MoonStuff
{
    static class RegionThings
    {
        public static void RegionHooks()
        {
            IL.World.LoadMapConfig_Timeline += RegionProperties;
        }

        public static Dictionary<Region, HSLColor> CrystalColor = new Dictionary<Region, HSLColor>();
        public static Dictionary<Region, float> OESphereHue = new Dictionary<Region, float>();

        public static void RegionProperties(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(
                MoveType.After,
                x => x.MatchStloc(14)
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, 14);
                c.EmitDelegate<Action<World, string[]>>(LoadRegionProperties);
            }
        }

        public static void LoadRegionProperties(World world, string[] property)
        {
            if (property[0].ToLower() == "defaultcrystalcolor")
            {
                string[] vals = Regex.Split(property[1].Trim(), ",");

                HSLColor col = new HSLColor(0.87f, 0.9f, 0.6f);
                col.hue = float.TryParse(vals[0], out float h) ? h : 0.87f;
                col.saturation = float.TryParse(vals[1], out float s) ? s : 0.9f;
                col.lightness = float.TryParse(vals[2], out float l) ? l : 0.6f;

                if (!CrystalColor.ContainsKey(world.region))
                {
                    CrystalColor.Add(world.region, col);
                }

            }
            else if (property[0].ToLower() == "defaultcolouredoespherecolour" || property[0].ToLower() == "defaultcoloredoespherecolor")
            {
                if (!OESphereHue.ContainsKey(world.region))
                {
                    OESphereHue.Add(world.region, float.TryParse(property[1], out float h) ? h : 0.06f);
                }
            }
        }
    }
}
