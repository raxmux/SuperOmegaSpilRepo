using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemy : MonoBehaviour
{

    RaycastHit2D hit;
    [SerializeField] LayerMask mask;

    GameObject player;

    int xdir = 0;
    int ydir = 0;

    bool moved;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        TriggerManager.Instance.RegisterListener(TriggerType.TimeclockReset, EnemyMove);
    }

    void OnDestroy()
    {
        if (TriggerManager.Instance != null)
            TriggerManager.Instance.UnregisterListener(TriggerType.TimeclockReset, EnemyMove);
    }
    public void EnemyMove(TriggerContext context)
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 1.1f)
        {
            Vector2 vec = GetClosestDirection(player.transform.position - transform.position);

            StartCoroutine(movewithLerp(transform.position + new Vector3(vec.x, vec.y, transform.position.z)));

            return;
        }

        moved = false;
        List<Vector2> directions = new List<Vector2>() { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        for (int i = 0; i < 4 && !moved; i++)
        {
            int rand = Random.Range(0, directions.Count);
            Vector2 direction = directions[rand];

            hit = Physics2D.Raycast(this.transform.position + new Vector3(direction.x, direction.y, transform.position.z), direction, 0.1f, mask);

            if (hit.collider != null)
            {
                if (hit.collider.tag == "Room")
                {
                    if (hit.collider.gameObject.GetComponent<RoomType>().roomType != RoomType.RoomTypes.EmptyRoom)
                    {
                        moved = true;
                        StartCoroutine(movewithLerp(hit.collider.gameObject.transform.position));
                        break;
                    }
                }
            }

            directions.RemoveAt(rand);

        }
    }

    public IEnumerator movewithLerp(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = targetPos;

        yield return new WaitForSeconds(0.2f);
    }

    public Vector2 GetClosestDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return direction.y > 0 ? Vector2.up : Vector2.down;
        }
    }

}