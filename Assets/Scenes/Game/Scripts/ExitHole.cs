using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ExitHole : MonoBehaviour
{
    public float randomMoveSpeed;
    public float randomMoveRange;
    public float randomMoveSlerp;
    public float spinSpeed;
    public float playerSpinSpeed;
    public float shrinkSpeed;
    public Volume enterHoleShader;
    public Volume fadeShader;

    GameObject playerDragObj;
    List<GameObject> swirls = new List<GameObject>();

    GameObject playerObj;
    Player playerScript;

    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerObj.GetComponent<Player>();

        playerDragObj = transform.GetChild(0).gameObject;
        for (int i = 1; i < transform.childCount; i++)
        {
            swirls.Add(transform.GetChild(i).gameObject);
        }
        if (gameObject.activeSelf) StartCoroutine(RandomPos(transform.position));
    }

    private void FixedUpdate()
    {
        foreach (GameObject swirl in swirls)
        {
            Vector3 offset = new(0, 0, swirl.transform.lossyScale.z);
            Vector3 rotation = swirl.transform.eulerAngles + Vector3.forward * spinSpeed;
            swirl.transform.eulerAngles = offset + rotation;
        }
    }

    IEnumerator RandomPos(Vector2 center)
    {
        Vector2 targetPos = new Vector2(Random.Range(-randomMoveRange, randomMoveRange), Random.Range(-randomMoveRange, randomMoveRange)) + center;
        while ((Vector2) transform.position != targetPos)
        {
            transform.position = Vector3.Slerp(transform.position, Vector2.MoveTowards(transform.position, targetPos, randomMoveSpeed), randomMoveSlerp);
            yield return new WaitForEndOfFrame();
        }
        if (gameObject.activeSelf) StartCoroutine(RandomPos(center));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript.rb.constraints = RigidbodyConstraints2D.FreezeAll;

            playerObj.transform.SetParent(playerDragObj.transform, true);
            if (gameObject.activeSelf) StartCoroutine(PlayerExit());
        }
    }


    IEnumerator PlayerExit()
    {
        GlitchTakeOver.doMove = false;
        playerScript.canMove = false;
        GameManager.doTimeTick = false;
        playerScript.camTarget = transform;
        GameObject.FindGameObjectsWithTag("Enemy").ToList().ForEach(obj => {
            GlitchEnemy enemy = obj.GetComponent<GlitchEnemy>();
            enemy.aiEnabled = false;
            enemy.walkDirectionSpeed = 0;
            enemy.jumpDirectionSpeed = 0;
        });
        while (0 <= playerDragObj.transform.localScale.x) {
            float invertedScale = -playerDragObj.transform.localScale.x + 1;

            Vector3 offset = new(0, 0, playerDragObj.transform.lossyScale.z);
            Vector3 rotation = playerDragObj.transform.eulerAngles + Vector3.forward * (playerSpinSpeed * playerDragObj.transform.localScale.x);
            playerDragObj.transform.eulerAngles = offset + rotation;
            playerDragObj.transform.localScale -= Vector3.one * shrinkSpeed;
            enterHoleShader.weight = invertedScale;
            fadeShader.weight = invertedScale * 2 - 1;


            yield return new WaitForEndOfFrame();
        }
        playerDragObj.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(2.5f);
        GameManager.NextScene();
    }
}
