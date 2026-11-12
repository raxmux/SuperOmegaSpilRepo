using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapMovement : MonoBehaviour
{

    public static MapMovement instance;

    //[SerializeField] InputAction controller;
    Vector2 moveDirection;

    RaycastHit2D hit;
    [SerializeField] LayerMask mask;
    public RoomSpawner roomSpawner;
    [SerializeField] CameraManager cameraManager;
    public bool moving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MapMovement.instance = this;
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

                    StartCoroutine(MoveToNextRoom(hit.collider.gameObject.transform.position, !hit.collider.gameObject.GetComponent<RoomType>().discovered, hit.collider.gameObject.GetComponent<RoomType>().roomType));

                    if(hit.collider.gameObject.GetComponent<RoomType>().roomType != RoomType.RoomTypes.EndRoom)
                        hit.collider.gameObject.GetComponent<RoomType>().discovered = true;
                    
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = hit.collider.gameObject.GetComponent<RoomType>().roomSprite;
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
        transform.position = targetPosition;

        yield return new WaitForSeconds(0.2f);

        if (isUnknown)
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

                EventManager.instance.StartEvent(EventType.Dialogue);
                break;
            case RoomType.RoomTypes.EndRoom:

                Debug.Log("End Room Reached");
                EventManager.instance.StartEvent(EventType.end);

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