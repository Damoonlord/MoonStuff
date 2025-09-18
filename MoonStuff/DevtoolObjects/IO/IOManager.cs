using DevInterface;
using Pom;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static Pom.Pom;

namespace MoonStuff.DevtoolObjects.IO
{

    public class IOManager
    {
        public class Inputs : ExtEnum<Inputs>
        {
            public static readonly Inputs Open = new Inputs("Open", true);
            public Inputs(string value, bool register = false) : base(value, register) { }
        }

        public class IODataHolder
        {
            public bool InputType;
            public string MessageID;
            public float Delay;

            public IODataHolder(bool input, string messageID, float delay)
            {
                InputType = input;
                MessageID = messageID;
                Delay = delay;
            }
        }
        public class IOData : ManagedData
        {
            public List<IODataHolder> IOHolder;

            public IOData(PlacedObject owner, ManagedField[] customFields) : base(owner, customFields)
            {
                IOHolder = new List<IODataHolder>();
            }

            public override string ToString()
            {
                StringBuilder data = new StringBuilder();

                for (int d = 0; d < IOHolder.Count; d++)
                {
                    data.Append(IOHolder[d].InputType + "=" + IOHolder[d].MessageID + "=" + IOHolder[d].Delay);
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

                        IOHolder.Add
                        (
                            new IODataHolder(false, null, 0f)
                            {
                                InputType = bool.Parse(Items[0].ToString()),
                                MessageID = Items[1].ToString(),
                                Delay = float.Parse(Items[2].ToString())
                            }

                        );
                    }
                }
                catch { }
            }
        }

        public class IOPanel : Panel, IDevUISignals
        {
            public class IOButton : Button
            {
                public bool Enabled;
                public IOButton(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, float width, string text) : base(owner, IDstring, parentNode, pos, width, text)
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

                        fLabels[f].alignment = FLabelAlignment.Center;

                        if (fLabels[f].text == "+" || fLabels[f].text == "-")
                        {
                            fLabels[f].SetPosition(absPos.x + size.x / 2, absPos.y - 4f);
                            fLabels[f].scale = 1.75f;
                        }
                        else
                        {
                            fLabels[f].SetPosition(absPos.x + size.x / 2, absPos.y);
                            fLabels[f].scale = 1f;
                        }
                    }
                }
            }

            public class DeleteButton : IOButton
            {
                FSprite trashicon;
                public DeleteButton(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, float width, string text) : base(owner, IDstring, parentNode, pos, width, text)
                {
                    fSprites.Add(trashicon = new FSprite("Trash"));

                    fSprites[0].scale = 20f;
                    trashicon.anchorX = 0f;
                    trashicon.anchorY = 0f;

                    if (owner != null)
                    {
                        Futile.stage.AddChild(trashicon);
                    }
                }

                public override void Update()
                {
                    base.Update();
                    trashicon.color = MouseOver ? colorA : colorB;
                }

                public override void Refresh()
                {
                    base.Refresh();
                    MoveSprite(fSprites.IndexOf(trashicon), absPos);
                }
            }

            public class InputOutputPanel : RectangularDevUINode, IDevUISignals
            {
                public class IOMessageBox : PositionedDevUINode
                {
                    public StringControl Inputbox;
                    public IOMessageBox(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, float width, string text) : base(owner, IDstring, parentNode, pos)
                    {
                        subNodes.Add(Inputbox = new StringControl(this.owner, IDstring + "-Inputbox", this, pos, width, text, IsValidMethod));
                    }

                    public static bool IsValidMethod(StringControl self, string value) => true;
                }

                public bool Enabled;

                public IOMessageBox messageID;
                public float delay = 0f;
                public bool input;

                public IOButton DeleteButton;
                public FLabel label;

                public IODataHolder Data;
                public InputOutputPanel(bool input, DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, Vector2 size, string text = "Message1") : base(owner, IDstring, parentNode, pos, size)
                {
                    if ((parentNode as IOPanel).IO.Contains(this) && ((parentNode as IOPanel).placedObject.data as IOData).IOHolder.Count >= (parentNode as IOPanel).IO.IndexOf(this))
                    {
                        Data = ((parentNode as IOPanel).placedObject.data as IOData).IOHolder[(parentNode as IOPanel).IO.IndexOf(this)];
                    }
                    else
                    {
                        Data = new IODataHolder(input, null, 0f);
                        ((parentNode as IOPanel).placedObject.data as IOData).IOHolder.Add(Data);
                    }

                    Enabled = false;
                    parentNode.subNodes.Add(this);
                    this.input = input;

                    subNodes.Add(DeleteButton = new DeleteButton(this.owner, IDstring + "-Delete", this, new Vector2(size.x - 5f, size.y - 20f), 16, ""));
                    fLabels.Add(label = new FLabel(Custom.GetFont(), input ? "I" : "O"));
                    fLabels[0].color = Color.red;
                    fLabels[0].alignment = FLabelAlignment.Center;

                    subNodes.Add(messageID = new IOMessageBox(this.owner, IDstring + "-ID", this, new Vector2(11f, 0f), 128f, text));

                    fSprites.Add(new FSprite("pixel"));
                    fSprites[0].color = new Color(1f, 1f, 1f);
                    fSprites[0].alpha = 0.5f;

                    for (int i = 0; i < fSprites.Count; i++)
                    {
                        fSprites[i].anchorX = 0f;
                        fSprites[i].anchorY = 0f;
                        if (owner != null)
                        {
                            Futile.stage.AddChild(fSprites[i]);
                        }
                    }

                    for (int i = 0; i < fLabels.Count; i++)
                    {
                        fLabels[i].anchorX = 0f;
                        fLabels[i].anchorY = 0f;
                        if (owner != null)
                        {
                            Futile.stage.AddChild(fLabels[i]);
                        }
                    }
                }

                public override void Update()
                {
                    if (Enabled)
                    {
                        base.Update();
                    }

                    DeleteButton.Enabled = Enabled;
                }

                public override void Refresh()
                {
                    base.Refresh();

                    if (fSprites.Count == 0 && fLabels.Count == 0)
                    {
                        return;
                    }

                    fSprites[0].scaleX = size.x - 10f;
                    fSprites[0].scaleY = size.y;

                    MoveSprite(0, absPos);
                    MoveLabel(0, absPos + new Vector2(input ? 7.5f : 5f, 3f));

                    for (int f = 0; f < fSprites.Count; f++)
                    {
                        fSprites[f].isVisible = Enabled;
                    }

                    for (int f = 0; f < fLabels.Count; f++)
                    {
                        fLabels[f].isVisible = Enabled;
                    }

                    ((parentNode as IOPanel).placedObject.data as IOData).IOHolder[(parentNode as IOPanel).IO.IndexOf(this)].MessageID = messageID.Inputbox.Text;
                }

                public void Signal(DevUISignalType type, DevUINode sender, string message)
                {
                    if (sender.IDstring == IDstring + "-Delete")
                    {
                        Delete();
                    }
                }

                public void Delete()
                {
                    parentNode.subNodes.Remove(this);
                    (parentNode as IOPanel).IO.Remove(this);
                    ClearSprites();

                    (parentNode as IOPanel).UpdateIO();
                }
            }

            public bool Enabled;
            public FSprite line;
            public bool IsInput;

            public PlacedObject placedObject;

            public IOButton AddItem;
            public IOButton ItemType;
            public HorizontalDivider Divide;

            public List<InputOutputPanel> IO = new List<InputOutputPanel>();
            public int lastIOCount;

            public IOPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, string name, PlacedObject pObj) : base(owner, IDstring, parentNode, pos, new Vector2(185f, 25f), name)
            {
                placedObject = pObj;

                Enabled = false;

                fSprites.Add(line = new FSprite("pixel"));
                line.anchorY = 0f;

                subNodes.Add(AddItem = new IOButton(this.owner, "AddItem", this, new Vector2(5f, size.y - 20f), 85f, "+"));
                subNodes.Add(ItemType = new IOButton(this.owner, "Type", this, new Vector2(95f, size.y - 20f), 85f, "Input"));
                subNodes.Add(Divide = new HorizontalDivider(this.owner, "Divide", this, 5f));

                BuildIOPanels();
            }

            public void BuildIOPanels()
            {
                foreach(IODataHolder data in (placedObject.data as IOData).IOHolder)
                {
                    IO.Add
                    (
                        new InputOutputPanel(data.InputType, this.owner, "IO" + (IO.Count + 1), this, new Vector2(5f, 5f + (25f * IO.Count)), new Vector2(size.x - 25f, 20f), data.MessageID)
                        {
                            delay = data.Delay,
                        }
                    );
                }

                UpdateIO();
            }

            public override void Update()
            {
                if (Enabled)
                {
                    base.Update();
                }

                size = new Vector2(185f, 30f * (IO.Count + 1));
                Divide.pos = new Vector2(Divide.pos.x, size.y - 30f);

                AddItem.pos = new Vector2(5f, size.y - 20f);
                ItemType.pos = new Vector2(95f, size.y - 20f);
            }

            public void UpdateIO()
            {
                if (IO.Count - lastIOCount > 0)
                {
                    pos.y = pos.y + (25f * (IO.Count - lastIOCount - 1));
                }
                else
                {
                    pos.y = pos.y + (25f * (IO.Count - lastIOCount + 1));
                }

                for (int i = 0; i < IO.Count; i++)
                {
                    IO[i].pos = new Vector2(5f, 5f + (25f * i));
                }

                lastIOCount = IO.Count;
            }

            public void Signal(DevUISignalType type, DevUINode sender, string message)
            {
                if (sender.IDstring == "AddItem")
                {
                    IO.Add(new InputOutputPanel(IsInput, this.owner, "IO" + (IO.Count + 1), this, new Vector2(5f, 5f + (25f * IO.Count)), new Vector2(size.x - 25f, 20f)));
                    UpdateIO();
                }

                if (sender.IDstring == "Type")
                {
                    IsInput = !IsInput;
                }
            }

            public override void Refresh()
            {
                base.Refresh();

                ItemType.Text = IsInput ? "Input" : "Output";

                MoveSprite(fSprites.IndexOf(line), (this.parentNode as Panel).pos);
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

                AddItem.Enabled = Enabled;
                ItemType.Enabled = Enabled;

                Divide.fSprites[0].isVisible = Enabled && IO.Count != 0;

                foreach (InputOutputPanel panel in IO)
                {
                    panel.Enabled = Enabled;
                }
            }
        }

        public class IORepresentation : ManagedRepresentation, IDevUISignals
        {
            public IOPanel IOPanel;
            public Button IOButton;
            public IORepresentation(PlacedObject.Type placedType, DevInterface.ObjectsPage objPage, PlacedObject pObj) : base(placedType, objPage, pObj)
            {
                subNodes.Add(IOPanel = new IOPanel(owner, "IOPanel", this.panel, new Vector2(0f, -145), "I/O", pObj));
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
