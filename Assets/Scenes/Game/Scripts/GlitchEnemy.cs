using System;
using UnityEngine;

public class GlitchEnemy : Entity
{

    [Header("Enemy AI")]
    public bool aiEnabled = true;
    public float objectDetectDistance;
    public float activatedAIRange;
    public float playerDistanceJumpRange;
    public int maxJumps;
    public int jumps;

    [Header("Enemy Stats")]
    public int damage;

    [Header("Debug")]
    [SerializeField]
    private bool showActivatedAIRange;

    float jumpTime;

    GameObject playerObj;
    Player playerScript;
    GameObject shaderObj;
    GlitchAction glitchAction;



    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerObj.GetComponent<Player>();
        shaderObj = GameObject.FindGameObjectWithTag("Shaders");
        glitchAction = shaderObj.GetComponent<GlitchAction>();

        if (playerScript == null)
        {
            Debug.LogError("PlayerObj does not contain component \"Player\" ");
        }
    }

    private void Update()
    {
        if (!isDead)
        {
            Vector2 origin = (Vector2)transform.position + (Vector2.up * .5f);
            Vector2 size = new Vector2(.25f, .05f);
            RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, Vector2.up, size.y);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Die();
            }
        }
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, playerObj.transform.position) < activatedAIRange && aiEnabled)
        {
            float gotoPlayerDir = playerObj.transform.position.x < transform.position.x ? -1 : 1;
            walkDirectionSpeed = gotoPlayerDir * movementSpeed;

            Vector2 rayStartPos = (Vector2)transform.position + (Vector2.up * .1f);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * gotoPlayerDir, objectDetectDistance);

            if (hit.collider != null)
            {
                if (!(hit.collider.CompareTag("Player") || hit.collider.CompareTag("Border")))
                {
                    jumpTime++;
                    if (jumpTime >= 20)
                    {
                        jumpTime = 0;
                        Jump();
                    }
                }
            }

            if (Vector2.Distance(transform.position, playerObj.transform.position) < playerDistanceJumpRange)
            {
                if (transform.position.y + .5f < playerObj.transform.position.y && playerScript.rb.linearVelocityY > 1)
                {
                    jumpTime++;
                    if (jumpTime >= 20)
                    {
                        jumpTime = 0;
                        Jump();
                    }
                }
            }
        }

        UpdateVelocity();
    }

    void Jump()
    {
        if (jumps > 0)
        {
            jumps--;
            float originalVelocityY = rb.linearVelocityY > 0 ? rb.linearVelocityY : 0;
            rb.linearVelocityY += jumpStrenght;
        }
    }

    public void Attack()
    {
        Array values = Enum.GetValues(typeof(GlitchAction.Action));
        GlitchAction.Action action = (GlitchAction.Action)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        glitchAction.PreformAction(action);
        playerScript.Damage(damage);
    }

    public override void Die()
    {
        if (!isDead)
        {
            isDead = true;
            playerScript.ForceJump();
            gameObject.SetActive(false);
            base.Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != gameObject)
        {
            if (collision.CompareTag("Ground"))
            {
                jumps = maxJumps;
                jumpTime = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != gameObject)
        {
            if (collision.CompareTag("Ground"))
            {
                if (jumps == maxJumps)
                {
                    Jump();
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (playerScript.imunityFrames == 0)
            {
                Attack();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showActivatedAIRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, activatedAIRange);

            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, playerDistanceJumpRange);
        }
    }
}
