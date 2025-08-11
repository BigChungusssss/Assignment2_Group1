using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
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

    private Control controls;
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
    private bool isTransformed = false;
    private float transformationTimer = 0f;
    public float transformationDuration = 60f;
    private float originalMovementSpeed;

    private bool canShoot = true;

    private Vector3 originalScale;

    [Header("Shrink Mode")]
    public float shrinkSpeedMultiplier = 1.3f;   // move faster while shrunk

    public float shrinkScaleMultiplier = 0.6f;   // size relative to normal

    public Sprite shrinkSprite;                  // sprite when shrunk

    private bool isShrunk = false;

    private float normalSpeed;

    private Vector3 normalScale;

    // New variable for rocket movement direction
    private Vector2 rocketMoveDirection = Vector2.up;

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
            // Move rocket automatically in rocketMoveDirection at a set rocket speed
            float rocketSpeed = 10f;  // Adjust speed as needed
            Vector2 newPos = rbody.position + rocketMoveDirection * rocketSpeed * Time.fixedDeltaTime;
            rbody.MovePosition(newPos);
            return; // Skip normal movement while transformed
        }

        Vector2 currentPos = rbody.position;
        Vector2 movement = Vector2.zero;

        if (!isAttacking)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
            inputVector = Vector2.ClampMagnitude(inputVector, 1);

            movement = inputVector * movementSpeed;
        }

        Vector2 newPosNormal = currentPos + movement * Time.fixedDeltaTime;
        rbody.MovePosition(newPosNormal);
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        powerShotTimer -= Time.deltaTime;

        UpdateTransformation();
        if (isTransformed)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");
            // Always move right (x=1), vertical guided by player input
            rocketMoveDirection = new Vector2(1f, verticalInput);

            // Normalize to keep speed consistent
            rocketMoveDirection = rocketMoveDirection.normalized;
        }
        else
        {
            // Optional: reset rotation when not transformed
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        }

        // Normal shooting
        if (!isTransformed && controls.Player.Shoot.WasPressedThisFrame() && shootTimer <= 0f)
        {
            Vector2 shootDir = transform.up;
            gun.Shoot(shootDir);
            shootTimer = shootCooldown;
        }

        if (controls.Player.Shoot.WasReleasedThisFrame())
        {
            gun.StopShooting();
        }

        // Power Shot (Left Trigger)
        if (!isTransformed && controls.Player.PowerShot.WasPressedThisFrame() && powerShotTimer <= 0f)
        {
            FirePowerShot();
        }

        if (controls.Player.RocketTransform.WasPressedThisFrame())
        {
            TransformPlayer();
        }

        if (controls.Player.Shrink.IsPressed())
        {
            StartShrink();
        }
        else
        {
            StopShrink();
        }
    }

    private void FirePowerShot()
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
        if (isTransformed) return; // Already transformed

        isTransformed = true;
        gun.StopShooting();
        if (powerGun != null) powerGun.StopShooting();
        originalMovementSpeed = movementSpeed;
        originalScale = normalScale;

        movementSpeed = 0f; // Disable player movement while rocket
        transform.localScale = originalScale * 5.5f;  // Scale up for rocket
        transformationTimer = transformationDuration;

        canShoot = false;

        // Change sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && transformedSprite != null)
            sr.sprite = transformedSprite;
        UpdateColliderToMatchSprite();

        // Initialize rocket direction with last aiming direction or default
        rocketMoveDirection = Vector2.right;

        // Rotate rocket to face initial direction
        float angle = Mathf.Atan2(rocketMoveDirection.y, rocketMoveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
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
    }

    private void StartShrink()
    {
        if (isShrunk) return;

        isShrunk = true;
        normalSpeed = movementSpeed;
        normalScale = transform.localScale;

        movementSpeed *= shrinkSpeedMultiplier;

        if (!isTransformed) // Only change size/sprite if not transformed
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
}



