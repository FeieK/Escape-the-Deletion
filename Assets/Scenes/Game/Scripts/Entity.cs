using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Entity : MonoBehaviour
{
    [Header("Entity Stats")]
    public int maxHP = 100;
    public int health;
    public float movementSpeed;
    public float sprintMultiplier;
    public float jumpStrenght;
    public float runningShoesSpeed;

    [Header("Entity Components")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public Animator animator;

    public GameObject runningShoes;
    public SpriteRenderer shoesSr;
    public Animator shoesAnimator;

    [Header("Entity Movement")]
    public float walkDirectionSpeed;
    public float jumpDirectionSpeed;

    public bool canWalkLeft = true;
    public bool canWalkRight = true;

    public int imunityFrames;
    public bool hasRunningShoes = false;
    public bool isDead = false;



    Coroutine wallSlideCoroutine;
    Coroutine imunityFramesCoroutine;


    private void Start()
    {
        Init();
    }

    public void Init()
    {
        health = maxHP;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        if (runningShoes != null)
        {
            if (shoesSr == null) shoesSr = runningShoes.GetComponent<SpriteRenderer>();
            if (shoesAnimator == null) shoesAnimator = runningShoes.GetComponent<Animator>();
        }
    }

    public void UpdateVelocity()
    {
        if (!canWalkLeft) walkDirectionSpeed = Mathf.Clamp(walkDirectionSpeed, 0, float.MaxValue);
        if (!canWalkRight) walkDirectionSpeed = Mathf.Clamp(walkDirectionSpeed, float.MinValue, 0);
        if (walkDirectionSpeed < 0)
        {
            walkDirectionSpeed += hasRunningShoes ? -runningShoesSpeed : 0;
        }
        if (walkDirectionSpeed > 0)
        {
            walkDirectionSpeed += hasRunningShoes ? runningShoesSpeed : 0;
        }
        rb.linearVelocityX = walkDirectionSpeed;


        if (walkDirectionSpeed == 0) animator.SetBool("IsWalking", false);
        else animator.SetBool("IsWalking", true);
        if (walkDirectionSpeed < 0) sr.flipX = true;
        if (walkDirectionSpeed > 0) sr.flipX = false;
        if (shoesSr != null && shoesAnimator != null)
        {
            if (walkDirectionSpeed == 0) shoesAnimator.SetBool("IsWalking", false);
            else shoesAnimator.SetBool("IsWalking", true);
            if (walkDirectionSpeed < 0) shoesSr.flipX = true;
            if (walkDirectionSpeed > 0) shoesSr.flipX = false;
        }
    }

    public enum WallSide
    {
        LEFT, RIGHT
    };

    public void ToutchWall(float delay, WallSide side, bool isToutching)
    {
        if (!isToutching && wallSlideCoroutine != null) StopCoroutine(wallSlideCoroutine);
        if (gameObject.activeSelf) wallSlideCoroutine = StartCoroutine(ToutchWallCoroutine(delay, side, isToutching));
    }

    public void ToutchBorder(WallSide side, bool isToutching)
    {
        canWalkLeft = !isToutching;
    }


    IEnumerator ToutchWallCoroutine(float delay, WallSide side, bool isToutching)
    {
        yield return new WaitForSeconds(delay);
        switch (side)
        {
            case WallSide.LEFT:
                {
                    canWalkLeft = !isToutching;
                    break;
                }
            case WallSide.RIGHT:
                {
                    canWalkRight = !isToutching;
                    break;
                }
        }
    }

    public void Damage(int amount)
    {
        if (imunityFrames == 0)
        {
            if (hasRunningShoes) SetRunningShoes(false);
            health -= amount;
            if (health > 0)
            {
                imunityFrames = 10;
                if (gameObject.activeSelf) imunityFramesCoroutine = StartCoroutine(ImunityFrames());
            }
            else Die();
        }
    }

    public void SetRunningShoes(bool equiped)
    {
        shoesSr.enabled = equiped;
        hasRunningShoes = equiped;
    }

    IEnumerator ImunityFrames()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = Color.white;

        while (imunityFrames > 0)
        {
            imunityFrames--;
            if (imunityFrames % 2 == 0)
            {
                sr.enabled = true;
                if (hasRunningShoes) shoesSr.enabled = true;
                if (shoesSr != null && hasRunningShoes) shoesSr.enabled = true;
            }
            else
            {
                sr.enabled = false;
                if (hasRunningShoes) shoesSr.enabled = false;
                if (shoesSr != null && hasRunningShoes) shoesSr.enabled = false;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    public virtual void Die()
    {
        if (imunityFramesCoroutine != null)
        {
            StopCoroutine(imunityFramesCoroutine);
        }
        health = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != gameObject)
        {
            if (collision.CompareTag("KillBox"))
            {
                Die();
            }
        }
    }
}
