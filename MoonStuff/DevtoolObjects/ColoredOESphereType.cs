using DevInterface;
using RWCustom;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using static Pom.Pom;

namespace MoonStuff.DevtoolObjects
{
    public class ColoredOESphereType : ManagedObjectType
    {
        public ColoredOESphereType() : base("Colored OE Sphere", Register.GeneralTab, null, typeof(ColoredOESphereData), ModManager.MSC ? typeof(ColoredOESphereRepresentation) : null) { }
        public override UpdatableAndDeletable MakeObject(PlacedObject placedObject, Room room) => ModManager.MSC ? new ColoredOESphere(placedObject, room) : null;

        public class ColoredOESphereData : ManagedData
        {
            #pragma warning disable 0649
            [FloatField("light", 0f, 100f, 100f, 1f, ManagedFieldWithPanel.ControlType.slider, "Light:")]
            public float light;

            [IntegerField("depth", 0, 30, 0, ManagedFieldWithPanel.ControlType.slider, "Depth:")]
            public int depth;

            [BackedByField("rad")]
            public Vector2 rad;
            #pragma warning restore 0649

            private static ManagedField[] customFields = new ManagedField[] { new Vector2Field("rad", new Vector2(0, 100), Vector2Field.VectorReprType.circle) };
            public float Hue = -1f;

            public ColoredOESphereData(PlacedObject owner) : base(owner, customFields) { }

            public override string ToString() => base.ToString() + "~" + Hue;

            public override void FromString(string s)
            {
                base.FromString(s);
                string[] arr = Regex.Split(s, "~");
                try
                {
                    Hue = float.Parse(arr[base.FieldsWhenSerialized + 0]);
                }
                catch { }
            }
        }

        public class ColoredOESphereRepresentation : ManagedRepresentation, IDevUISignals
        {
            public class ColoredOESphereSlider : Slider
            {
                public PlacedObject pObj;
                public ColoredOESphereSlider(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, string title, bool inheritButton, float titleWidth)
                    : base(owner, IDstring, parentNode, pos, title, inheritButton, titleWidth)
                {
                    pObj = (parentNode.parentNode as ColoredOESphereRepresentation).pObj;
                    subNodes[0].fSprites[0].scaleX = 100f;
                    subNodes[1].fSprites[0].scaleX = 24f;
                    (subNodes[1] as DevUILabel).pos.x = titleWidth;
                }

                public override void Refresh()
                {
                    base.Refresh();
                    float nubPos = 0f;


                    if (IDstring == "CrystalHue")
                    {
                        if ((pObj.data as ColoredOESphereData).Hue == -1f)
                        {
                            NumberText = "Def";
                        }
                        else
                        {
                            nubPos = (pObj.data as ColoredOESphereData).Hue;
                            NumberText = (Mathf.Round((pObj.data as ColoredOESphereData).Hue * 100) / 100).ToString();
                        }
                    }

                    RefreshNubPos(nubPos);
                }

                public override void NubDragged(float nubPos)
                {
                    RefreshNubPos(nubPos);

                    if (IDstring == "CrystalHue")
                    {
                        (pObj.data as ColoredOESphereData).Hue = nubPos;
                    }

                    parentNode.parentNode.Refresh();
                    Refresh();
                }
            }

            public Button Reset;
            public ColoredOESphereSlider Hue;
            public ColoredOESphereRepresentation(PlacedObject.Type placedType, DevInterface.ObjectsPage objPage, PlacedObject pObj) : base(placedType, objPage, pObj)
            {
                for (int i = 0; i < panel.subNodes.Count; i++)
                {
                    (panel.subNodes[i] as Slider).pos = new Vector2(5, i * 20f + 45f);
                }

                panel.subNodes.Add(Reset = new Button(owner, "Default", panel, new Vector2(5, 25), 64f, "Default"));
                panel.subNodes.Add(Hue = new ColoredOESphereSlider(owner, "CrystalHue", panel, new Vector2(5, 5), "Hue:", false, 110f));

                panel.size = new Vector2(250f, 85f);
            }

            public void Signal(DevUISignalType type, DevUINode sender, string message)
            {
                if (sender.IDstring == "Default")
                {
                    (pObj.data as ColoredOESphereData).Hue = -1f;
                }
            }
        }
    }
}
