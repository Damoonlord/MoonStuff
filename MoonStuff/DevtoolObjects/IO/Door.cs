using DevInterface;
using MoonStuff.DevtoolObjects.IO;
using RWCustom;
using UnityEngine;
using static Pom.Pom;

namespace MoonStuff.DevtoolObjects
{
    internal class Door : ManagedObjectType
    {
        public Door() : base("Door", Register.IOTab, null, typeof(DoorData), typeof(DoorRepresentation))
        {
        }

        public override UpdatableAndDeletable MakeObject(PlacedObject placedObject, Room room)
        {
            return new DoorObject(placedObject, room);
        }

        class DoorObject : UpdatableAndDeletable, IDrawable
        {
            internal class State : ExtEnum<State>
            {
                public static readonly State Open = new State("Open", true);
                public static readonly State Opening = new State("Opening", true);
                public static readonly State Closing = new State("Closing", true);
                public static readonly State Closed = new State("Closed", true);
                public State(string value, bool register = false) : base(value, register) { }
            }

            public PlacedObject placedObject;
            public DoorObject(PlacedObject placedObject, Room room) : base()
            {
                this.placedObject = placedObject;
            }

            public override void Update(bool eu)
            {
                if (Input.GetKey("K"))
                {

                }

                base.Update(eu);


            }

            public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                sLeaser.sprites = new FSprite[1];
            }

            public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
            {

            }

            public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
            {

            }

            public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
            {
                if (newContatiner == null)
                {
                    newContatiner = rCam.ReturnFContainer("Foreground");
                }

                FSprite[] sprites = sLeaser.sprites;
                foreach (FSprite fSprite in sprites)
                {
                    fSprite.RemoveFromContainer();
                    newContatiner.AddChild(fSprite);
                }
            }
        }   

        class DoorData : IOManager.IOData
        {
            #pragma warning disable 0649
            [BackedByField("Size")]
            public IntVector2 Size;

            [FloatField("Speed", 0f, 1f, 0.5f, 0.1f, displayName: "Speed:")]
            public float Speed;
            #pragma warning restore 0649

            private static ManagedField[] SizeField = new ManagedField[] {
                    new IntVector2Field("Size", new IntVector2(0, -3), IntVector2Field.IntVectorReprType.rect),
            };

            public DoorData(PlacedObject owner) : base(owner, SizeField)
            {
            }
        }
        class DoorRepresentation : IOManager.IORepresentation
        {
            public DoorRepresentation(PlacedObject.Type placedType, DevInterface.ObjectsPage objPage, PlacedObject pObj) : base(placedType, objPage, pObj)
            {
                panel.size.y = 45f;
                Debug.Log(panel.size.x);

                (panel.subNodes[0] as Slider).pos = new Vector2(5f, 25f);
            }
        }
    }
}
