using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GlitchTakeOver : MonoBehaviour
{
    public static bool doMove = true;

    public float speed;
    public float speedDistanceMultiplier;
    public float minDistance;
    public Transform cameraPos;

    [SerializeField]
    private Transform target;

    float distanceFromPlayer;

    private void Start()
    {
        doMove = true;
    }

    private void Update()
    {
        if (doMove)
        {
            distanceFromPlayer = Vector2.Distance(transform.position, target.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (doMove)
        {
            float moveSpeed = speed * (distanceFromPlayer * speedDistanceMultiplier * (distanceFromPlayer > minDistance ? 10 : 1));
            Vector2 newPos = new(Vector2.MoveTowards(transform.position, cameraPos.position, moveSpeed).x, cameraPos.position.y);
            transform.position = newPos;
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && doMove)
        {
            collider.GetComponent<Entity>().Die();
        }
    }
}
