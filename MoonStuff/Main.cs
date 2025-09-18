using BepInEx;
using MonoMod.Cil;
using MoonStuff.DevtoolObjects;
using System;
using System.IO;
using System.Security.Permissions;
using UnityEngine;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace MoonStuff
{
    [BepInDependency("rwmodding.coreorg.pom")]

    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "damoonlord.stuff";
        public const string PLUGIN_NAME = "Moon's Stuff";
        public const string PLUGIN_VERSION = "1.0.0";
        public const bool IOModule = false;

        public bool Registered = false;

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += IniReg;
            Hooks.EnableHooks();

            Register.RegisterObjects();
        }

        public void IniReg(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            if (Registered) { return; }
            Registered = true;

            try
            {
                RegionThings.RegionHooks();

                Futile.atlasManager.LoadAtlas("assets" + Path.DirectorySeparatorChar + "Sprites");
                Logger.LogInfo("Sprites loaded!");

                AssetBundle assetBundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("assets/moonstuff", false));
                self.Shaders.Add("ColoredOESphereBase", FShader.CreateShader("ColoredOESphereBase", assetBundle.LoadAsset<Shader>("assets/shaders/ColoredOESphereBase.shader")));
                self.Shaders.Add("ColoredOESphereLight", FShader.CreateShader("ColoredOESphereLight", assetBundle.LoadAsset<Shader>("assets/shaders/ColoredOESphereLight.shader")));
                self.Shaders.Add("SandFall", FShader.CreateShader("SandFall", assetBundle.LoadAsset<Shader>("assets/shaders/SandFall.shader")));
                Logger.LogInfo("Shaders loaded!");
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}