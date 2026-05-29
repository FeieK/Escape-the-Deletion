using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Serializable]
    public struct PlatformPoint
    {
        public Vector2 position;
        public float stayTime;
    }


    public float platformSpeed;
    public PlatformPoint[] platformPoints;

    Dictionary<Transform, Transform> originalParents = new Dictionary<Transform, Transform>();
    private bool isMoving = false;

    Vector2 targetPos;

    IEnumerator Moving(int index, int nextIndex = 1)
    {
        targetPos = platformPoints[index].position;

        while (Vector2.Distance(transform.position, targetPos) > .01f)
        {

            transform.position = Vector2.MoveTowards(transform.position, targetPos, platformSpeed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = targetPos;
        yield return new WaitForSeconds(platformPoints[index].stayTime);
        StartCoroutine(Moving(nextIndex, index));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity entity = collision.gameObject.GetComponent<Entity>();
        if (entity != null && !originalParents.ContainsKey(collision.transform))
        {
            originalParents.Add(collision.transform, collision.transform.parent);
            collision.transform.parent = transform;
            if (!isMoving)
            {
                StartCoroutine(Moving(0));
                isMoving = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity entity = collision.gameObject.GetComponent<Entity>();
        if (entity != null && originalParents.ContainsKey(collision.transform))
        {
            collision.transform.parent = originalParents[collision.transform];
            originalParents.Remove(collision.transform);
        }
    }
}
