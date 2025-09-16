using UnityEngine;
using static Pom.Pom;

namespace MoonStuff.DevtoolObjects
{
    public class WarmSpotType : ManagedObjectType
    {
        public WarmSpotType() : base("WarmSpot", Register.GeneralTab, null, typeof(WarmSpotData), ModManager.MSC ? typeof(ManagedRepresentation) : null) { }
        public override UpdatableAndDeletable MakeObject(PlacedObject placedObject, Room room) => ModManager.MSC ? new WarmSpot(placedObject, room) : null;

        public class WarmSpotData : ManagedData
        {
            #pragma warning disable 0649
            [FloatField("warmth", 0.01f, 1f, 0.05f, 0.01f, ManagedFieldWithPanel.ControlType.slider, "Warmth:")]
            public float warmth;

            [BackedByField("rad")]
            public Vector2 rad;
            #pragma warning restore 0649

            private static ManagedField[] customFields = new ManagedField[] { new Vector2Field("rad", new Vector2(0, 50), Vector2Field.VectorReprType.circle) };
            public float Hue = -1f;

            public WarmSpotData(PlacedObject owner) : base(owner, customFields) { }
        }
    }
}
