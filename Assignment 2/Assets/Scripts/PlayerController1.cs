using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float movementSpeed = 1f;

    [Header("Normal Gun")]
    public Gun gun;
    public float shootCooldown = 0.5f;

    [Header("Power Shot")]
    public Gun powerGun;                        // Different gun for the special attack
    public float powerShotCooldown = 180f;      // 3 minutes
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

    [Header("Transformation")]
    public Sprite normalSprite;
    public Sprite transformedSprite;
    public bool isTransformed = false;
    private float transformationTimer = 0f;
    public float transformationDuration = 60f;
    private float originalMovementSpeed;

    private bool canShoot = true;

    private Vector3 originalScale;

    [Header("Shrink Mode")]
    public float shrinkSpeedMultiplier = 1.3f;   // move faster while shrunk

    public float shrinkScaleMultiplier = 0.6f;   // size relative to normal

    public Sprite shrinkSprite;                  // sprite when shrunk

    public bool isShrunk = false;

    private float normalSpeed;

    private Vector3 normalScale;

    // New variable for rocket movement direction
    private Vector2 rocketMoveDirection = Vector2.up;

    [Header("Card System")]
    public int currentCards = 0;
    public int maxCards = 4;

    [Header("Parry Settings")]
    public float parryWindow = 0.2f; // Time in seconds parry is active
    public bool isParryActive = false;
    private float parryTimer = 0f;

    [Header("Parry Dash")]
    public float parryDashDistance = 3f;

    public float parryDashSpeed = 15f;

    private bool isDashing = false;

    private Vector2 dashDirection;

    private float dashTime;

    private float dashDuration = 0.15f; 


    private void Awake()
    {
        controls = new Control();
        rbody = GetComponent<Rigidbody2D>();
        normalScale = transform.localScale;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

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

        // Dash overrides normal movement
        if (isDashing)
        {
            moveVector = dashDirection * parryDashSpeed;
            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0f)
                isDashing = false;
        }

        // Final movement
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

        

        // Normal shooting (still handled here)
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

        if (controls.Player.PowerShot.WasPressedThisFrame() &&!isTransformed && !isShrunk)
        {
           FirePowerShot();
        }



 
        // (Handled in PlayerCardManager)

        if (controls.Player.Shrink.IsPressed())
        {
            StartShrink();
        }
        else
        {
            StopShrink();
        }

        if (controls.Player.Parry.WasPressedThisFrame())
        {
            StartParry();
        }

        if (isParryActive)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0f) EndParry();
        }
    }

    public void FirePowerShot()
    {
        // if (currentCards <= 0) return; // Not enough cards

        gun.StopShooting();

        if (powerGun != null)
        {
            Vector2 shootDir = transform.up;
            powerGun.Shoot(shootDir);
        }

        // currentCards--; // Consume a card
        powerShotTimer = powerShotCooldown;
    }


    public void TransformPlayer()
    {
        if (isTransformed || currentCards < maxCards) return; // Need full cards

        isTransformed = true;
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

        currentCards = 0; // Consume all cards
    }










    private void RevertTransformation()
    {
        isTransformed = false;
        movementSpeed = originalMovementSpeed;
        transform.localScale = normalScale;
        canShoot = true;

        // Reset rotation
        transform.rotation = Quaternion.identity;

        // Revert sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
        UpdateColliderToMatchSprite();
    }

    private void UpdateTransformation()
    {
        if (isTransformed)
        {
            transformationTimer -= Time.deltaTime;
            if (transformationTimer <= 0f)
            {
                RevertTransformation();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransformed && other.CompareTag("Enemy"))
        {
            RevertTransformation();
        }

        if (isParryActive && other.CompareTag("Parry"))
        {
            SuccessfulParry(other.gameObject);
        }
    }

    private void StartShrink()
    {
        if (isShrunk) return;

        isShrunk = true;
        normalSpeed = movementSpeed;
        normalScale = transform.localScale;

        movementSpeed *= shrinkSpeedMultiplier;

        // Stop shooting immediately
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

        if (!isTransformed) // Only revert size/sprite if not transformed
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

        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputDir == Vector2.zero) inputDir = Vector2.right; // default forward
        dashDirection = inputDir.normalized;

        // Start dash
        isDashing = true;
        dashTime = dashDuration;
    
        // Optional: play parry animation here
    }

    private void EndParry()
    {
        isParryActive = false;
        Debug.Log("Parry ended");
    }

    private void SuccessfulParry(GameObject parriedObject)
    {
        Debug.Log("Parry success!");
        Destroy(parriedObject);
        AddCard(1);
        // Optional: parry success animation/sound
        EndParry();
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (isTransformed && other.CompareTag("Enemy"))
    //     {
    //         RevertTransformation();
    //     }

    //     if (isParryActive && other.CompareTag("Parryable"))
    //     {
    //         SuccessfulParry(other.gameObject);
    //     }
    // }



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
    }

}



