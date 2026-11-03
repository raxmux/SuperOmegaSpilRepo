using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEditor;

public class RoomSpawner : MonoBehaviour
{

    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();
    [SerializeField] GameObject startRoom;
    [SerializeField] List<GameObject> currentRooms = new List<GameObject>();
    RaycastHit2D hit;

    [SerializeField] LayerMask mask;
    
    // Update is called once per frame
    void Update()
    {
        //Invoke("SpawnRooms", 0.1f);
    }

    public void SpawnRooms()
    {

        foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
        {
            Destroy(room);
            if (currentRooms.Count > 0)
            {
                currentRooms.RemoveAt(0);
            }
            Debug.Log("Destroyed");
        }
            currentRooms.Add(Instantiate(startRoom, transform.position, Quaternion.identity, transform));

            while (currentRooms.Count > 0)
            {
                for (int zoop = 0; zoop < 4; zoop++)
                {
                    Transform dir = currentRooms[0].transform;
                    hit = Physics2D.Raycast(currentRooms[0].transform.position + dir.up, dir.up, 0.1f, mask);
                    int rand = UnityEngine.Random.Range(0, 100);
                    Debug.Log(hit.point);
                    if (hit.collider == null)
                    {
                        int roomtype;
                        if (rand > 80)
                        {
                            roomtype = 0;
                        }
                        else
                        {
                            roomtype = 1;
                        }

                        if (MathF.Abs((currentRooms[0].transform.position + dir.up).y) > 3 || MathF.Abs((currentRooms[0].transform.position + dir.up).x) > 4)
                        {
                            Debug.Log("fuck you");
                        }
                        else
                        {
                            currentRooms.Add(Instantiate(roomPrefabs[roomtype], currentRooms[0].transform.position + dir.up, Quaternion.identity, transform));
                        }
                    }
                    else
                    {
                        currentRooms.Remove(hit.collider.gameObject);
                    }
                    dir.rotation = Quaternion.Euler(0, 0, dir.rotation.eulerAngles.z + 90);
                }
                currentRooms.RemoveAt(0);
            }
    }
}