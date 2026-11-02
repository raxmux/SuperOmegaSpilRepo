using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEditor;

public class RoomSpawner : MonoBehaviour
{

    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();
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
            Transform dir = currentRooms[0].transform;

            for (int zoop = 0; zoop < 4; zoop++)
            {
                hit = Physics2D.Raycast(currentRooms[0].transform.position + dir.up, dir.up, 0.1f, mask);
                Debug.Log(dir.up);
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

                if (hit.point.y > 5 || hit.point.y < -5 || hit.point.x > 5 || hit.point.x < -5)
                {
                    
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
