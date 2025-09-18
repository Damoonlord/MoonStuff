using UnityEngine;

namespace MoonStuff.DevtoolObjects
{
    class Door : UpdatableAndDeletable, IDrawable
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
        public Door(PlacedObject placedObject, Room room) : base()
        {
            this.placedObject = placedObject;
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
}
