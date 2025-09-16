using DevInterface;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using RWCustom;
using UnityEngine;
using static Pom.Pom;

namespace MoonStuff.DevtoolObjects.IO
{
    public class IOManager
    {
        internal class Inputs : ExtEnum<Inputs>
        {
            public static readonly Inputs Open = new Inputs("Open", true);
            public Inputs(string value, bool register = false) : base(value, register) { }
        }

        public class IODataHolder<T1, T2, T3> : List<(T1, T2, T3)> { }
        public class IOData : ManagedData
        {
            public IODataHolder<string, string, float> Data;

            public IOData(PlacedObject owner, ManagedField[] customFields) : base(owner, customFields)
            {
                Data = new IODataHolder<string, string, float>();
            }

            public override string ToString()
            {
                StringBuilder data = new StringBuilder();

                for (int d = 0; d < Data.Count; d++)
                {
                    data.Append(Data[d].Item1 + "=" + Data[d].Item2 + "=" + Data[d].Item3);
                    data.Append("~");
                }

                return base.ToString() + "~" + data.ToString();
            }

            public override void FromString(string s)
            {
                base.FromString(s);
                string[] Datas = Regex.Split(s, "~");

                try
                {
                    for (int d = FieldsWhenSerialized; d < Datas.Length; d++)
                    {
                        string[] Items = Regex.Split(Datas[d], "=");
                        (string, string, float) T = (Items[0].ToString(), Items[1].ToString(), float.Parse(Items[2].ToString()));
                        Data.Add(T);
                    }
                }
                catch { }
            }
        }

        public class IOPanel : Panel
        {
            public class ItemButton : Button
            {
                public bool Enabled;
                public ItemButton(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, float width, string text) : base(owner, IDstring, parentNode, pos, width, text)
                {
                    Enabled = false;
                }

                public override void Update()
                {
                    if (Enabled)
                    {
                        base.Update();
                    }
                }

                public override void Refresh()
                {
                    base.Refresh();

                    for (int f = 0; f < fSprites.Count; f++)
                    {
                        fSprites[f].isVisible = Enabled;
                    }

                    for (int f = 0; f < fLabels.Count; f++)
                    {
                        fLabels[f].isVisible = Enabled;
                        fLabels[f].scale = 1.5f;
                    }
                }
            }

            public bool Enabled;
            public FSprite line;

            public ItemButton AddItem;
            public IOPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, string name) : base(owner, IDstring, parentNode, pos, new Vector2(190f, 100f), name)
            {
                Enabled = false;

                fSprites.Add(line = new FSprite("pixel"));
                line.anchorY = 0f;

                subNodes.Add(AddItem = new ItemButton(this.owner, "AddItem", this, new Vector2(5f, size.y - 20f), 32f, "+"));
            }

            public override void Update()
            {
                if (Enabled)
                {
                    base.Update();
                }

                AddItem.Enabled = Enabled;
            }

            public override void Refresh()
            {
                base.Refresh();

                MoveSprite(fSprites.IndexOf(line), absPos);
                line.scaleY = pos.magnitude;
                line.scaleX = 16f;
                line.rotation = Custom.VecToDeg(-pos);

                for (int f = 0; f < fSprites.Count; f++)
                {
                    fSprites[f].isVisible = Enabled;
                }

                for (int f = 0; f < fLabels.Count; f++)
                {
                    fLabels[f].isVisible = Enabled;
                }
            }
        }

        public class IORepresentation : ManagedRepresentation, IDevUISignals
        {
            public IOPanel IOPanel;
            public Button IOButton;
            public IORepresentation(PlacedObject.Type placedType, DevInterface.ObjectsPage objPage, PlacedObject pObj) : base(placedType, objPage, pObj)
            {
                subNodes.Add(IOPanel = new IOPanel(owner, "IOPanel", this.panel, new Vector2(0f, -145), "I/O"));
                subNodes.Add(IOButton = new Button(owner, "IOButton", this.panel, new Vector2(5, 5), panel.size.x - 5, "I/O"));

                subNodes[subNodes.IndexOf(IOButton)].fLabels[0].x = subNodes[subNodes.IndexOf(IOButton)].fSprites[0].width / 2;
            }

            public void Signal(DevUISignalType type, DevUINode sender, string message)
            {
                if (sender.IDstring == IOButton.IDstring)
                {
                    IOPanel.Enabled = !IOPanel.Enabled;
                }
            }
        }
    }
}
