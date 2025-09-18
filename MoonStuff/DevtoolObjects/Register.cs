using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Pom.Pom;

namespace MoonStuff.DevtoolObjects
{
    public static class Register
    {
        public const string GeneralTab = "Moon's Stuff";
        public const string IOTab = "Moon's I/O";

        public static class PlacedObjects
        {
            public static PlacedObject.Type SandFall = new PlacedObject.Type("SandFall", true);
            public static PlacedObject.Type LightSourceFlicker = new PlacedObject.Type("Light Source Flicker", true);
            public static PlacedObject.Type Crystal = new PlacedObject.Type("Crystal", true);
            public static PlacedObject.Type ColoredOESphere = new PlacedObject.Type("Colored OE Sphere", true);
            public static PlacedObject.Type WarmSpot = new PlacedObject.Type("WarmSpot", true);
        }

        static ConditionalWeakTable<SpotLight, SpData> Sptable = new ConditionalWeakTable<SpotLight, SpData>();
        public static SpData GetCustomSpotLightData(this SpotLight self) => Sptable.GetOrCreateValue(self);
        public class SpData
        {
            public bool On;
        }

        static ConditionalWeakTable<LightSource, SData> Stable = new ConditionalWeakTable<LightSource, SData>();
        public static SData GetCustomLightSourceData(this LightSource self) => Stable.GetOrCreateValue(self);
        public class SData
        {
            public bool On;
        }

        static ConditionalWeakTable<LightBeam, BData> Btable = new ConditionalWeakTable<LightBeam, BData>();
        public static BData GetCustomLightBeamData(this LightBeam self) => Btable.GetOrCreateValue(self);
        public class BData
        {
            public bool On;
        }

        public static void RegisterObjects()
        {
            RegisterManagedObject(new CrystalType());

            RegisterManagedObject(new SandfallType());

            RegisterManagedObject(new LightSourceFlickerType());

            RegisterManagedObject(new ColoredOESphereType());

            RegisterManagedObject(new WarmSpotType());

            if (Main.IOModule)
            {
                RegisterManagedObject(new DoorType());
            }
        }
    }
}
