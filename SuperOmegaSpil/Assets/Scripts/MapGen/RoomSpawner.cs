using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEditor;
using UnityEngine.Rendering;
using Unity.VisualScripting;


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

        /*foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
        {
            Destroy(room);
            if (currentRooms.Count > 0)
            {
                currentRooms.RemoveAt(0);
            }
        }

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
            if (enemies.Count > 0)
            {
                enemies.RemoveAt(0);
            }
        }*/

        currentRooms.Add(Instantiate(startRoom, transform.position, Quaternion.identity, transform));

        for (int i = 0; i < mapSize ; i++)
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

                    if (!(MathF.Abs((currentRooms[randRoom].transform.position + dir.up).y) > mapY || MathF.Abs((currentRooms[randRoom].transform.position + dir.up).x) > mapX+1))
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
        int endRoomRand = UnityEngine.Random.Range(1, currentRooms.Count);
        Vector2 endRoomPos = currentRooms[endRoomRand].transform.position;
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
            int enemyPosRand = UnityEngine.Random.Range(1, currentRooms.Count);
            Vector2 enemyPos = currentRooms[enemyPosRand].transform.position;
            enemies.Add(Instantiate(this.gameObject, enemyPos, Quaternion.identity, currentRooms[enemyPosRand].transform));
            currentRooms.RemoveAt(enemyPosRand);
        }
    }
}