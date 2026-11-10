using UnityEngine;

public class RoomType : MonoBehaviour
{
    public bool discovered = false;
    public enum RoomTypes
        {
            StartRoom,
            EndRoom,
            EmptyRoom,
            UnknownRoom,
            ShopRoom,
            EventRoom,
        }
        public RoomTypes roomType;

    public Sprite roomSprite;
}


