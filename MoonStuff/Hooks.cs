using DevInterface;
using MoonStuff.DevtoolObjects;
using RWCustom;
using UnityEngine;

namespace MoonStuff
{
    internal class Hooks
    {
        public static void EnableHooks()
        {
            On.MoreSlugcats.OEsphere.AddToContainer += OESphereFix;

            On.SpotLight.ctor += SpotLightTurnOn;
            On.SpotLight.DrawSprites += SpotLightOnOff;

            On.LightSource.ctor_Vector2_bool_Color_UpdatableAndDeletable += LightSourceTurnOn;
            On.LightSource.DrawSprites += LightSourceOnOff;

            On.LightBeam.ctor += LightBeamTurnOn;
            On.LightBeam.DrawSprites += LightBeamOnOff;

            On.DevInterface.AddObjectButton.Clicked += MSCCheck;
        }

        public static WarningLabel MSCWarning;
        public class WarningLabel : UpdatableAndDeletable
        {
            public int Duration;
            public int MaxDuration;
            public FLabel label;
            public Panel panel;

            public WarningLabel(string text, int duration, Panel panel)
            {
                Debug.Log("[Moon's Stuff] " + text);

                Duration = duration;
                MaxDuration = duration;
                this.panel = panel;

                label = new FLabel(Custom.GetFont(), text);
                label.color = Color.red;
                label.anchorX = 1;
                label.anchorY = 0;

                panel.fLabels.Add(label);
                Futile.stage.AddChild(label);
            }

            public override void Update(bool eu)
            {
                base.Update(eu);

                if (Duration == 0)
                {
                    panel.fLabels.Remove(label);
                    Futile.stage.RemoveChild(label);
                    this.Destroy();
                }
                else
                {
                    Duration--;
                }

                Vector2 pos = new Vector2(panel.pos.x + panel.size.x - 33f, panel.pos.y + panel.size.y + 4f);
                label.SetPosition(pos);
                label.alpha = (float)Duration / (float)MaxDuration;
            }
        }

        public static void MSCCheck(On.DevInterface.AddObjectButton.orig_Clicked orig, AddObjectButton self)
        {
            if (!ModManager.MSC && self.type == Register.PlacedObjects.ColoredOESphere || self.type == Register.PlacedObjects.WarmSpot)
            {
                self.owner.room.AddObject(MSCWarning = new WarningLabel("You need MSC to use this object!", 100, (self.Page as ObjectsPage).objectsPanel));
                return;
            }

            orig(self);
        }

        private static void LightBeamOnOff(On.LightBeam.orig_DrawSprites orig, LightBeam self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[0].isVisible = Register.GetCustomLightBeamData(self).On;
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }

        private static void LightBeamTurnOn(On.LightBeam.orig_ctor orig, LightBeam self, PlacedObject placedObject)
        {
            orig(self, placedObject);
            Register.GetCustomLightBeamData(self).On = true;
        }

        private static void LightSourceOnOff(On.LightSource.orig_DrawSprites orig, LightSource self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[0].isVisible = Register.GetCustomLightSourceData(self).On;
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }

        private static void LightSourceTurnOn(On.LightSource.orig_ctor_Vector2_bool_Color_UpdatableAndDeletable orig, LightSource self, Vector2 initPos, bool environmentalLight, Color color, UpdatableAndDeletable tiedToObject)
        {
            orig(self, initPos, environmentalLight, color, tiedToObject);
            Register.GetCustomLightSourceData(self).On = true;
        }

        private static void SpotLightOnOff(On.SpotLight.orig_DrawSprites orig, SpotLight self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[0].isVisible = Register.GetCustomSpotLightData(self).On;
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }

        private static void SpotLightTurnOn(On.SpotLight.orig_ctor orig, SpotLight self, PlacedObject placedObject)
        {
            orig(self, placedObject);
            Register.GetCustomSpotLightData(self).On = true;
        }

        public static void OESphereFix(On.MoreSlugcats.OEsphere.orig_AddToContainer orig, MoreSlugcats.OEsphere self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            rCam.ReturnFContainer("GrabShaders").AddChild(sLeaser.sprites[0]);
            rCam.ReturnFContainer("GrabShaders").AddChild(sLeaser.sprites[1]);
            rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[2]);
            sLeaser.sprites[1].MoveInFrontOfOtherNode(sLeaser.sprites[0]);
            sLeaser.sprites[0].MoveToBack();
        }
    }
}
