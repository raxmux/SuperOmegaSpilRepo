using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class RoomSpawner : MonoBehaviour
{

    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();
    [SerializeField] GameObject startRoom;
    [SerializeField] GameObject endRoom;
    public List<GameObject> currentRooms = new List<GameObject>();

    public List<GameObject> enemies = new List<GameObject>();
    [SerializeField] GameObject overworldEnemy;

    int enemyCount = 0;

    [SerializeField] Sprite unknownRoom;

    RaycastHit2D hit;

    [SerializeField] LayerMask mask;
    [SerializeField] int mapSize;
    [SerializeField] int mapX;
    [SerializeField] int mapY;
    public void Start()
    {
        SpawnRooms();
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

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("EnemyMap"))
        {
            Destroy(enemy);
        }
        enemies.Clear();

        currentRooms.Add(Instantiate(startRoom, transform.position, Quaternion.identity, transform));

        for (int i = 0; i < mapSize; i++)
        {
            int randRoom = UnityEngine.Random.Range(0, currentRooms.Count);
            for (int zoop = 0; zoop < 4; zoop++)
            {
                Transform dir = currentRooms[randRoom].transform;
                hit = Physics2D.Raycast(currentRooms[randRoom].transform.position + dir.up, dir.up, 0.1f, mask);
                int rand = UnityEngine.Random.Range(0, 11);
                if (hit.collider == null)
                {
                    int roomtype;
                    if (rand > 8)
                    {
                        roomtype = 0;
                    }
                    else
                    {
                        int roomrand = UnityEngine.Random.Range(0, 11);
                        if (roomrand > 9)
                        {
                            //shoproom
                            roomtype = 1;
                        }
                        else
                        {
                            //eventroom
                            roomtype = 2;
                        }
                    }

                    if (!(MathF.Abs((currentRooms[randRoom].transform.position + dir.up).y) > mapY || MathF.Abs((currentRooms[randRoom].transform.position + dir.up).x) > mapX + 1))
                    {
                        if (roomtype == 0)
                        {
                            Instantiate(roomPrefabs[roomtype], currentRooms[randRoom].transform.position + dir.up, Quaternion.identity, transform);

                        }
                        else
                        {
                            GameObject instantatedRoom = Instantiate(roomPrefabs[roomtype], currentRooms[randRoom].transform.position + dir.up, Quaternion.identity, transform);
                            currentRooms.Add(instantatedRoom);
                            instantatedRoom.GetComponent<SpriteRenderer>().sprite = unknownRoom;
                        }
                    }
                }
                dir.rotation = Quaternion.Euler(0, 0, dir.rotation.eulerAngles.z + 90);
            }
            if (i == 0)
            {
                currentRooms.RemoveAt(0);
            }
        }


        int endRoomRand = 0;
        Vector2 endRoomPos = Vector2.zero;

        int successes = 0;
        while (successes != 4)
        {
            successes = 0;
            endRoomRand = UnityEngine.Random.Range(0, currentRooms.Count);
            endRoomPos = currentRooms[endRoomRand].transform.position;

            for (int zoop = 0; zoop < 4; zoop++)
            {
                Transform dir = currentRooms[endRoomRand].transform;
                hit = Physics2D.Raycast(currentRooms[endRoomRand].transform.position + dir.up, dir.up, 0.1f, mask);

                if (hit.collider == null)
                {
                    successes++;
                    Debug.Log("hit null");
                }
                else
                {
                    if (hit.collider.gameObject.GetComponent<RoomType>().roomType == RoomType.RoomTypes.StartRoom)
                    {
                        successes = 0;
                        Debug.Log("failed");
                    }
                    else
                    {
                        successes++;
                        Debug.Log("success");
                    }
                }

                dir.rotation = Quaternion.Euler(0, 0, dir.rotation.eulerAngles.z + 90);
            }
        }


        GameObject endRoomInstance = Instantiate(endRoom, endRoomPos, Quaternion.identity, transform);
        endRoomInstance.GetComponent<SpriteRenderer>().sprite = unknownRoom;
        Destroy(currentRooms[endRoomRand]);
        currentRooms.RemoveAt(endRoomRand);
        spawnEnemies();
    }

    public void spawnEnemies()
    {
        enemyCount = Mathf.RoundToInt(currentRooms.Count / 6);
        for (int i = 0; i < enemyCount; i++)
        {
            int enemyPosRand = 0;
            Vector2 enemyPos = Vector2.zero;

            int successes = 0;
            while (successes != 4)
            {
                successes = 0;
                enemyPosRand = UnityEngine.Random.Range(0, currentRooms.Count);
                enemyPos = currentRooms[enemyPosRand].transform.position;

                for (int zoop = 0; zoop < 4; zoop++)
                {
                    Transform dir = currentRooms[enemyPosRand].transform;
                    hit = Physics2D.Raycast(currentRooms[enemyPosRand].transform.position + dir.up, dir.up, 0.1f, mask);

                    if (hit.collider == null)
                    {
                        successes++;
                        Debug.Log("hit null");
                    }
                    else
                    {
                        if (hit.collider.gameObject.GetComponent<RoomType>().roomType == RoomType.RoomTypes.StartRoom)
                        {
                            successes = 0;
                            Debug.Log("failed");
                        }
                        else
                        {
                            successes++;
                            Debug.Log("success");
                        }
                    }

                    dir.rotation = Quaternion.Euler(0, 0, dir.rotation.eulerAngles.z + 90);
                }
            }

            enemies.Add(Instantiate(overworldEnemy, currentRooms[enemyPosRand].transform.position, Quaternion.identity));
            currentRooms.RemoveAt(enemyPosRand);
        }
    }
}