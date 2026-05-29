using System.Collections;
using UnityEngine;

public class ShoesItem : MonoBehaviour
{
    Player playerScript;
    private void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        StartCoroutine(Float());
    }

    IEnumerator Float()
    {
        yield return new WaitForSeconds(1);
        transform.position += Vector3.up * .25f;
        yield return new WaitForSeconds(1);
        transform.position += Vector3.down * .25f;
        StartCoroutine(Float());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerScript.hasRunningShoes)
        {
            playerScript.SetRunningShoes(true);
            Destroy(gameObject);
        }
    }
}
