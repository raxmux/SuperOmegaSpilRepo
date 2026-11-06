using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapMovement : MonoBehaviour
{

    //[SerializeField] InputAction controller;
    Vector2 moveDirection;

    RaycastHit2D hit;
    public LayerMask mask;
    public RoomSpawner roomSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //controller = PlayerInput.GetPlayerByIndex(0).actions["Move"];
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
            hit = Physics2D.Raycast(this.transform.position + new Vector3(moveDirection.x, moveDirection.y, this.transform.position.z), new Vector2(moveDirection.x, moveDirection.y), 0.1f, mask);
            if (hit.collider.tag == "Room")
            {
                if (hit.collider.gameObject.GetComponent<RoomType>().roomType != RoomType.RoomTypes.EmptyRoom)
                {
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = hit.collider.gameObject.GetComponent<RoomType>().roomSprite;
                    transform.position += new Vector3(moveDirection.x, moveDirection.y, this.transform.position.z);

                    if (hit.collider.gameObject.GetComponent<RoomType>().roomType == RoomType.RoomTypes.EndRoom)
                    {
                        transform.position = new Vector3(0,0,0);
                        roomSpawner.SpawnRooms();
                    }
                }
            }
        }
    }
}
