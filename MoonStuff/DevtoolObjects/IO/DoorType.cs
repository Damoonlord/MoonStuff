using DevInterface;
using MoonStuff.DevtoolObjects.IO;
using RWCustom;
using UnityEngine;
using static Pom.Pom;

namespace MoonStuff.DevtoolObjects
{
    internal class DoorType : ManagedObjectType
    {
        public DoorType() : base("Door", Register.IOTab, null, typeof(DoorData), typeof(DoorRepresentation))
        {
        }

        public override UpdatableAndDeletable MakeObject(PlacedObject placedObject, Room room)
        {
            return new Door(placedObject, room);
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
                panel.size = new Vector2(195f, 45f);

                (panel.subNodes[0] as Slider).pos = new Vector2(5f, 25f);
            }
        }
    }
}
