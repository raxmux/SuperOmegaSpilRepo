using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapMovement : MonoBehaviour
{

    //[SerializeField] InputAction controller;
    Vector2 moveDirection;

    RaycastHit2D hit;
    [SerializeField] LayerMask mask;
    [SerializeField] RoomSpawner roomSpawner;
    [SerializeField] CameraManager cameraManager;
    bool moving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //controller = PlayerInput.GetPlayerByIndex(0).actions["Move"];
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (moving) return;

            moveDirection = context.ReadValue<Vector2>();
            hit = Physics2D.Raycast(this.transform.position + new Vector3(moveDirection.x, moveDirection.y, this.transform.position.z), new Vector2(moveDirection.x, moveDirection.y), 0.1f, mask);

            if (hit.collider != null && hit.collider.CompareTag("Room"))
            {
                if (hit.collider.gameObject.GetComponent<RoomType>().roomType != RoomType.RoomTypes.EmptyRoom)
                {
                    moving = true;



                    StartCoroutine(MoveToNextRoom(hit.collider.gameObject.transform.position, hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite.name == "Rom_0", hit.collider.gameObject.GetComponent<RoomType>().roomType));

                    hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = hit.collider.gameObject.GetComponent<RoomType>().roomSprite;

                    /*if (hit.collider.gameObject.GetComponent<RoomType>().roomType == RoomType.RoomTypes.EndRoom)
                    {
                        transform.position = new Vector3(0, 0, 0);
                        roomSpawner.SpawnRooms();
                    }*/
                }
            }
        }
    }
    public IEnumerator MoveToNextRoom(Vector3 targetPosition, bool isUnknown, RoomType.RoomTypes type)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.2f);

        if (isUnknown && hit.collider.gameObject.GetComponent<RoomType>().roomType != RoomType.RoomTypes.EndRoom)
        {
            EnterRoom(type);
        }
        else
        {
            moving = false;
        }
    }

    void EnterRoom(RoomType.RoomTypes type)
    {
        cameraManager.switchCam();

        switch (type)
        {
            case RoomType.RoomTypes.ShopRoom:

                EventManager.instance.StartEvent(EventType.Shop);
                break;
            case RoomType.RoomTypes.EventRoom:

                int randomEvent = UnityEngine.Random.Range(0, 4);
                if (randomEvent == 0) EventManager.instance.StartEvent(EventType.Enemy); 
                else EventManager.instance.StartEvent(EventType.Dialogue);
                break;
            case RoomType.RoomTypes.EndRoom:
                //do nothing
                break;
            default:

                EventManager.instance.StartEvent(EventType.Dialogue);
                break;
        }
        //more logic needed ye

    }
    public void ExitRoom()
    {
        cameraManager.switchCam();
        moving = false;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
    }
}