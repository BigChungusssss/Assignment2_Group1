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

    // private Animator animator;
    // public float interactRange = 2f;
    // public LayerMask interactableLayer;

    private Vector2 lastAimDirection = Vector2.right;

    private float shootTimer = 0f;
    private Rigidbody2D rbody;

    private Control controls;
    private Vector2 moveInput;
    private bool shootPressed;
    private bool dashPressed;

    // public float dashSpeed = 8f;
    // public float dashDuration = 0.25f;
    // public float dashCooldown = 0.75f;

    // private bool isDashing = false;
     private bool isAttacking = false;

    // private float dashTimer = 0f;
    // private float dashCooldownTimer = 0f;
    // private Vector2 dashDirection;

    // public float lockOnRange = 5f;
    // public LayerMask enemyLayer;
    // private Transform lockedOnTarget;

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


    private void Awake()
    {
        controls = new Control();
        rbody = GetComponent<Rigidbody2D>();
        normalScale = transform.localScale; 
        //animator = GetComponentInChildren<Animator>();
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
        Vector2 currentPos = rbody.position;
        Vector2 movement = Vector2.zero;

        // if (isDashing)
        // {
        //     dashTimer -= Time.fixedDeltaTime;
        //     movement = dashDirection * dashSpeed;

        //     if (dashTimer <= 0f)
        //     {
        //         isDashing = false;
        //         gameObject.tag = "Player";
        //         // if (animator != null)
        //         //     animator.SetBool("isDashing", false);
        //     }

        //     // if (animator != null)
        //     //     animator.SetFloat("xVelocity", 0f);
        // }
        if (!isAttacking)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
            inputVector = Vector2.ClampMagnitude(inputVector, 1);

            movement = inputVector * movementSpeed;

            // if (animator != null)
            //     animator.SetFloat("xVelocity", inputVector.magnitude);
        }
        else
        {
            // if (animator != null)
            //     animator.SetFloat("xVelocity", 0f);
        }

        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;
        rbody.MovePosition(newPos);
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        //dashCooldownTimer -= Time.deltaTime;
        powerShotTimer -= Time.deltaTime;

        UpdateTransformation();

        // if (!isDashing && dashCooldownTimer <= 0f && controls.Player.Dash.WasPressedThisFrame())
        // {
        //     gameObject.tag = "Enemy";
        // }

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
        // Stop normal gun
        gun.StopShooting();

        // Fire from power gun
        if (powerGun != null)
        {
            Vector2 shootDir = transform.up;
            powerGun.Shoot(shootDir);
        }

        // Reset cooldown
        powerShotTimer = powerShotCooldown;
    }

    // private void RotateTowardsTaggedTarget(string targetTag)
    // {
    //     GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
    //     if (targets.Length == 0) return;

    //     GameObject closestTarget = null;
    //     float closestDistanceSqr = Mathf.Infinity;
    //     Vector3 currentPosition = -transform.position;

    //     foreach (GameObject target in targets)
    //     {
    //         Vector3 directionToTarget = target.transform.position - currentPosition;
    //         float dSqrToTarget = directionToTarget.sqrMagnitude;
    //         if (dSqrToTarget < closestDistanceSqr)
    //         {
    //             closestDistanceSqr = dSqrToTarget;
    //             closestTarget = target;
    //         }
    //     }

    //     if (closestTarget != null)
    //     {
    //         Vector3 direction = closestTarget.transform.position - transform.position;
    //         float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //         transform.rotation = Quaternion.Euler(0f, 0f, angle);
    //         transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    //         transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    //     }
    // }


    public void TransformPlayer()
    {
        if (isTransformed) return; // Already transformed

        isTransformed = true;
        gun.StopShooting();
        if (powerGun != null) powerGun.StopShooting();
        originalMovementSpeed = movementSpeed;
        originalScale = normalScale;
        movementSpeed = originalMovementSpeed* 0.5f; // half speed
        transform.localScale = originalScale * 5.5f;  //change 5.5f if needed (this increases size of player object)
        transformationTimer = transformationDuration;

        isTransformed = true;
        canShoot = false; // disable shooting

        // Change sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && transformedSprite != null)
            sr.sprite = transformedSprite;
            UpdateColliderToMatchSprite();


    }

    private void RevertTransformation()
    {
        isTransformed = false;
        movementSpeed = originalMovementSpeed;
        transform.localScale = normalScale;
        isTransformed = false;
        canShoot = true;

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


