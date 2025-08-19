using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private SpriteRenderer spriteRenderer;

    private Color originalColor;

    public PlayerHealth playerHealth;
    public float movementSpeed = 1f;

    [Header("Animations")]
    public Animator alienAnim;

    [Header("Normal Gun")]
    public Gun gun;
    public float shootCooldown = 0.5f;

    [Header("Power Shot")]
    public Gun powerGun;
    public float powerShotCooldown = 180f;
    private float powerShotTimer = 0f;

    private Vector2 lastAimDirection = Vector2.right;

    private float shootTimer = 0f;
    private Rigidbody2D rbody;

    public Control controls;
    private Vector2 moveInput;
    private bool shootPressed;
    private bool dashPressed;

    private bool isAttacking = false;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 10f;

    [Header("Rocket")]
    public Sprite normalSprite;
    public Sprite transformedSprite;
    public bool isTransformed = false;
    private float transformationTimer = 0f;
    public float transformationDuration = 60f;
    private float originalMovementSpeed;
    [Header("Rocket Shield")]
    public RocketShield rocketShield;



    private bool canShoot = true;

    private Vector3 originalScale;

    [Header("Shrink Mode")]
    public float shrinkSpeedMultiplier = 1.3f;
    public float shrinkScaleMultiplier = 0.6f;
    public Sprite shrinkSprite;
    public bool isShrunk = false;
    private float normalSpeed;
    private Vector3 normalScale;

    // New variable for rocket movement direction
    private Vector2 rocketMoveDirection = Vector2.up;

    [Header("Card System")]
    public int currentCards = 0;
    public int maxCards = 4;
    [Header("Card UI")]
    public GameObject[] cardImages;  // Assign your card images in the inspector


    [Header("Parry Settings")]
    public float parryWindow = 0.2f;
    public bool isParryActive = false;
    private float parryTimer = 0f;
    private Coroutine parryBlinkCoroutine;

    [Header("Parry Dash")]
    public float parryDashDistance = 3f;
    public float parryDashSpeed = 15f;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private float dashTime;
    private float dashDuration = 0.15f;

    [Header("Parry Shield")]
    public GameObject parryShield; // --- NEW ---

    private void Awake()
    {
        controls = new Control();
        rbody = GetComponent<Rigidbody2D>();
        normalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();


        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void FixedUpdate()
    {
        if (isTransformed)
        {
            float rocketSpeed = 10f;
            Vector2 rocketPos = rbody.position + rocketMoveDirection * rocketSpeed * Time.fixedDeltaTime;
            rbody.MovePosition(rocketPos);
            return;
        }

        Vector2 moveVector = Vector2.zero;

        if (!isAttacking)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
            inputVector = Vector2.ClampMagnitude(inputVector, 1);
            moveVector = inputVector * movementSpeed;
        }

        if (isDashing)
        {
            moveVector = dashDirection * parryDashSpeed;
            dashTime -= Time.fixedDeltaTime;
            alienAnim.SetBool("isDashing",true);
            if (dashTime <= 0f)
            {
                isDashing = false;
                alienAnim.SetBool("isDashing",false);
            }
        }

        Vector2 finalPos = rbody.position + moveVector * Time.fixedDeltaTime;
        rbody.MovePosition(finalPos);
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        powerShotTimer -= Time.deltaTime;
        UpdateTransformation();

        if (isTransformed)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");
            rocketMoveDirection = new Vector2(1f, verticalInput).normalized;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        }

        if (!isTransformed && !isShrunk && controls.Player.Shoot.WasPressedThisFrame() && shootTimer <= 0f)
        {
            Vector2 shootDir = transform.up;
            gun.Shoot(shootDir);
            shootTimer = shootCooldown;
        }

        if (controls.Player.Shoot.WasReleasedThisFrame())
        {
            gun.StopShooting();
        }

        if (controls.Player.PowerShot.WasPressedThisFrame() && !isTransformed && !isShrunk)
        {
            FirePowerShot();
        }

        if (controls.Player.Shrink.IsPressed())
            StartShrink();
        else
            StopShrink();

        if (controls.Player.Parry.WasPressedThisFrame())
            StartParry();

        if (isParryActive)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0f) EndParry();
        }
    }

    public void FirePowerShot()
    {
        gun.StopShooting();

        if (powerGun != null)
        {
            Vector2 shootDir = transform.up;
            powerGun.Shoot(shootDir);
        }

        powerShotTimer = powerShotCooldown;
    }

    public void TransformPlayer()
    {
        if (isTransformed || currentCards < maxCards) return;


        isTransformed = true;
        if (rocketShield != null)
            rocketShield.Activate();
        gun.StopShooting();
        if (powerGun != null) powerGun.StopShooting();
        originalMovementSpeed = movementSpeed;
        originalScale = normalScale;

        movementSpeed = 0f;
        transform.localScale = originalScale * 5.5f;
        transformationTimer = transformationDuration;
        canShoot = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && transformedSprite != null)
            sr.sprite = transformedSprite;
        UpdateColliderToMatchSprite();

        rocketMoveDirection = Vector2.right;
        float angle = Mathf.Atan2(rocketMoveDirection.y, rocketMoveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        currentCards = 0;
        for (int i = 0; i < cardImages.Length; i++)
        {
            if (cardImages[i] != null)
                cardImages[i].SetActive(false);
        }

    }

    public void RevertTransformation()
    {
        isTransformed = false;
        movementSpeed = originalMovementSpeed;
        transform.localScale = normalScale;
        canShoot = true;
        transform.rotation = Quaternion.identity;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
        UpdateColliderToMatchSprite();
        if (rocketShield != null)
            rocketShield.Deactivate();
    }

    public void UpdateTransformation()
    {
        if (isTransformed)
        {
            transformationTimer -= Time.deltaTime;
            if (transformationTimer <= 0f) RevertTransformation();
        }
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (isTransformed && other.CompareTag("Enemy"))
    //     {
    //         RevertTransformation();
    //     }
    // }

    private void StartShrink()
    {
        if (isShrunk) return;

        isShrunk = true;
        normalSpeed = movementSpeed;
        normalScale = transform.localScale;
        movementSpeed *= shrinkSpeedMultiplier;

        gun.StopShooting();
        if (powerGun != null) powerGun.StopShooting();

        if (!isTransformed)
        {
            transform.localScale = normalScale * shrinkScaleMultiplier;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && shrinkSprite != null)
                sr.sprite = shrinkSprite;
            UpdateColliderToMatchSprite();
        }
    }

    private void StopShrink()
    {
        if (!isShrunk) return;

        isShrunk = false;
        movementSpeed = normalSpeed;

        if (!isTransformed)
        {
            transform.localScale = normalScale;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && normalSprite != null)
                sr.sprite = normalSprite;
            UpdateColliderToMatchSprite();
        }
    }

    // --- Parry ---

    private void StartParry()
    {
        if (isParryActive) return;

        isParryActive = true;
        parryTimer = parryWindow;
        Debug.Log("Parry ready!");

        // Activate shield first
        if (parryShield != null) parryShield.SetActive(true);

        // Then disable player collider for invincibility
        playerHealth.EnableInvincibility();

        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputDir == Vector2.zero) inputDir = Vector2.right;
        dashDirection = inputDir.normalized;

        isDashing = true;
        dashTime = dashDuration;
    }


    private void EndParry()
    {
        isParryActive = false;

        // Re-enable collider
        playerHealth.DisableInvincibility();

        // Disable shield
        if (parryShield != null) parryShield.SetActive(false);

        Debug.Log("Parry ended");
    }




    public void SuccessfulParry(GameObject parriedObject)
    {
        Debug.Log("Parry success!");
        Destroy(parriedObject);
        AddCard(1);
        EndParry();
        if (parryBlinkCoroutine != null)
            StopCoroutine(parryBlinkCoroutine);

        parryBlinkCoroutine = StartCoroutine(BlinkPink(0.6f, 0.1f));
    
    }

    void UpdateColliderToMatchSprite()
    {
        var sr = GetComponent<SpriteRenderer>();
        var poly = GetComponent<PolygonCollider2D>();

        if (sr != null && poly != null && sr.sprite != null)
        {
            poly.pathCount = sr.sprite.GetPhysicsShapeCount();

            for (int i = 0; i < poly.pathCount; i++)
            {
                List<Vector2> path = new List<Vector2>();
                sr.sprite.GetPhysicsShape(i, path);
                poly.SetPath(i, path.ToArray());
            }
        }
    }

    public void AddCard(int amount = 1)
    {
        currentCards = Mathf.Clamp(currentCards + amount, 0, maxCards);
        UpdateCardsUI();
    }

    private void UpdateCardsUI()
    {
        for (int i = 0; i < cardImages.Length; i++)
        {
            if (cardImages[i] != null)
                cardImages[i].SetActive(i < currentCards);
        }
    }
    private IEnumerator BlinkPink(float duration = 0.5f, float interval = 0.1f)
    {
        float elapsed = 0f;
        Color pink = new Color(1f, 0.4f, 0.7f); // nice pink tone

        while (elapsed < duration)
        {
            if (spriteRenderer != null)
            {
                // Toggle color between pink and original
                spriteRenderer.color = (spriteRenderer.color == originalColor) ? pink : originalColor;
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        // Restore original color at the end
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }


}



