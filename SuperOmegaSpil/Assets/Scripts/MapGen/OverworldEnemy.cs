using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OverworldEnemy : MonoBehaviour
{

    RaycastHit2D hit;
    [SerializeField] LayerMask mask;

    int xdir = 0;
    int ydir = 0;

    bool moved;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Start()
    {
        TriggerManager.Instance.RegisterListener(TriggerType.TimeclockPoint, EnemyMove);
    }
    public void EnemyMove(TriggerContext context)
    {
        moved = false;
        while (moved == false)
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {

                case 0:
                    xdir = 1;
                    ydir = 0;
                    break;
                case 1:
                    xdir = -1;
                    ydir = 0;
                    break;
                case 2:
                    ydir = 1;
                    xdir = 0;
                    break;
                case 3:
                    ydir = -1;
                    xdir = 0;
                    break;
            }

            hit = Physics2D.Raycast(this.transform.position + new Vector3(xdir, ydir, transform.position.z), new Vector2(xdir, ydir), 0.1f, mask);

            if (hit.collider != null)
            {
                if (hit.collider.tag == "Room")
                {
                    if (hit.collider.gameObject.GetComponent<RoomType>().roomType != RoomType.RoomTypes.EmptyRoom)
                    {
                        Debug.Log("larping!");
                        Debug.Log(hit.collider.gameObject.transform.position);
                        //transform.position = hit.collider.gameObject.transform.position;
                        moved = true;
                        StartCoroutine(movewithLerp(hit.collider.gameObject.transform.position));
                    }
                }
            }
            else { }
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
}
