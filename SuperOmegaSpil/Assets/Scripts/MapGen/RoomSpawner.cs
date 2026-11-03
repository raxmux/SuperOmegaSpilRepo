using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEditor;
using UnityEngine.Rendering;

public class RoomSpawner : MonoBehaviour
{

    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();
    [SerializeField] GameObject startRoom;
    [SerializeField] GameObject endRoom;
    [SerializeField] List<GameObject> currentRooms = new List<GameObject>();
    RaycastHit2D hit;

    [SerializeField] LayerMask mask;
    [SerializeField] int mapSize;
    [SerializeField] int mapX;
    [SerializeField] int mapY;

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
        }

        currentRooms.Add(Instantiate(startRoom, transform.position, Quaternion.identity, transform));

        for (int i = 0; i < mapSize ; i++)
        {
            int randRoom = UnityEngine.Random.Range(0, currentRooms.Count);
            for (int zoop = 0; zoop < 4; zoop++)
            {
                Transform dir = currentRooms[randRoom].transform;
                hit = Physics2D.Raycast(currentRooms[randRoom].transform.position + dir.up, dir.up, 0.1f, mask);
                int rand = UnityEngine.Random.Range(0, 100);
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

                    if (!(MathF.Abs((currentRooms[randRoom].transform.position + dir.up).y) > mapY || MathF.Abs((currentRooms[randRoom].transform.position + dir.up).x) > mapX+1))
                    {
                        if (roomtype == 0)
                        {
                            Instantiate(roomPrefabs[roomtype], currentRooms[randRoom].transform.position + dir.up, Quaternion.identity, transform);
                        }
                        else
                        {
                            currentRooms.Add(Instantiate(roomPrefabs[roomtype], currentRooms[randRoom].transform.position + dir.up, Quaternion.identity, transform));

                        }
                    }
                }
                dir.rotation = Quaternion.Euler(0, 0, dir.rotation.eulerAngles.z + 90);
            }
        }
        int endRoomRand = UnityEngine.Random.Range(1, currentRooms.Count);
        Vector2 endRoomPos = currentRooms[endRoomRand].transform.position;
        Instantiate(endRoom, endRoomPos, Quaternion.identity, transform);
        Destroy(currentRooms[endRoomRand]);
    }
}