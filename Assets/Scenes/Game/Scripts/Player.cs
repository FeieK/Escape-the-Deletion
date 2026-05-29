using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class Player : Entity
{
    [Header("Player Atributes")]
    public float maxJumpTime;
    public float jumpTime;
    public int maxJumps;
    public int jumps;
    public float maxStamina;
    public float stamina;
    public float staminaSpeed;
    public float staminaRechargeSpeed;
    public bool invertControlls = false;
    public bool canMove = true;


    Vector2 walkInputDirection;
    float sprintInputBit;
    bool staminaStun = false;


    [Header("Camera Settings")]
    public float camSlerp;
    public float camDirectionMultiplier;
    public Transform camTarget;
    public Vector2 camOffset;
    float originalCamZPosition;

    Coroutine jumpCoroutine;

    [Header("Player Components")]
    public InputActionReference walkInput;
    public InputActionReference jumpInput;
    public InputActionReference sprintInput;
    public Transform cameraTransform;
    public GameObject staminaSliderObj;
    public SpriteRenderer deadSpriteRenderer;
    Slider staminaSlider;
    Image staminaBarImage;
    Image staminaFillImage;

    private void Start()
    {
        originalCamZPosition = cameraTransform.position.z;
        camTarget = transform;
        stamina = maxStamina;

        staminaSlider = staminaSliderObj.GetComponent<Slider>();
        staminaBarImage = staminaSliderObj.GetComponent<Image>();
        staminaFillImage = staminaSliderObj.transform.GetChild(0).GetComponent<Image>();

        staminaSlider.maxValue = maxStamina;
        staminaBarImage.color = new Color(0, .6f, 0);
        staminaFillImage.color = Color.green;

        Init();
    }

    private void Update()
    {
        if (health > 0 && canMove)
        {
            walkInputDirection = invertControlls ? -walkInput.action.ReadValue<Vector2>() : walkInput.action.ReadValue<Vector2>();
            sprintInputBit = !staminaStun ? sprintInput.action.ReadValue<float>() : 0;


            if (walkDirectionSpeed < 0) deadSpriteRenderer.flipX = true;
            if (walkDirectionSpeed > 0) deadSpriteRenderer.flipX = false;
            if (walkDirectionSpeed != 0) GameManager.doTimeTick = true;

            Vector2 camDirection = walkInputDirection * camDirectionMultiplier;
            Vector3 newCamPos = camTarget.position + new Vector3(camDirection.x, camDirection.y, originalCamZPosition) + (Vector3) camOffset;
            Vector3 slerpPos = Vector3.Slerp(cameraTransform.position, newCamPos, camSlerp);

            cameraTransform.position = new(slerpPos.x, slerpPos.y, originalCamZPosition);
        }
        else
        {
            walkInputDirection = Vector2.zero;
            sprintInputBit = 0;
        }

        staminaSlider.value = stamina;

    }

    private void FixedUpdate()
    {
        walkDirectionSpeed = walkInputDirection.x * (movementSpeed * (sprintInputBit == 1 ? sprintMultiplier : 1));
        if (sprintInputBit == 1)
        {
            stamina = Mathf.Clamp(stamina - staminaSpeed, 0, maxStamina);
        }
        else {
            stamina = Mathf.Clamp(stamina + staminaRechargeSpeed * (hasRunningShoes ? 2 : 1), 0, maxStamina);
        }
        if (stamina == 0)
        {
            staminaStun = true;
            staminaBarImage.color = new Color(.6f, 0, 0);
            staminaFillImage.color = Color.red;
        }
        if (staminaStun && stamina == maxStamina)
        {
            staminaStun = false;
            staminaBarImage.color = new Color(0, .6f, 0);
            staminaFillImage.color = Color.green;
        }

        UpdateVelocity();
    }

    void OnEnable()
    {
        jumpInput.action.started += JumpStart;
        jumpInput.action.canceled += JumpCancel;

    }

    void OnDisable()
    {
        jumpInput.action.started -= JumpStart;
        jumpInput.action.canceled -= JumpCancel;
    }

    void JumpStart(InputAction.CallbackContext obj)
    {
        if (health > 0)
        {
            if (jumps > 0)
            {
                jumps--;
                if (gameObject.activeSelf) jumpCoroutine = StartCoroutine(Jump());
            }
        }
    }
    public void ForceJump()
    {
        if (gameObject.activeSelf) jumpCoroutine = StartCoroutine(Jump());
    }
    void JumpCancel(InputAction.CallbackContext obj)
    {
        if (jumpCoroutine != null) {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
            jumpTime = 0;
        }
    }

    IEnumerator Jump()
    {
        float originalVelocityY = rb.linearVelocityY > 0 ? rb.linearVelocityY : 0;
        rb.linearVelocityY += jumpStrenght;
        while(jumpTime < maxJumpTime)
        {
            jumpTime++;
            rb.linearVelocityY = originalVelocityY + jumpStrenght;
            yield return new WaitForFixedUpdate();
        }
    }

    public override void Die()
    {
        if (!isDead)
        {
            GlitchTakeOver.doMove = false;
            isDead = true;
            deadSpriteRenderer.enabled = true;
            base.Die();
            StartCoroutine(DieAnimation());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("KillBox"))
        {
            Die();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != gameObject) {
            if (collision.CompareTag("Ground") || collision.CompareTag("Enemy"))
            {
                jumpTime = 0;
                jumps = maxJumps;
            }
        }
    }

    IEnumerator DieAnimation()
    {
        float progress = -1;
        float startYPos = transform.position.y;
        while (progress < 5)
        {
            progress += 0.1f;
            float pos = -Mathf.Pow(progress, 2) + 1;
            transform.position = new Vector2(transform.position.x, pos + startYPos);
            yield return new WaitForFixedUpdate();
        }
        GlitchTakeOver.doMove = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
