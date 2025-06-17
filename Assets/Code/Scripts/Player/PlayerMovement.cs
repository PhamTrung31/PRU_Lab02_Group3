using Assets.Code.Scripts.Player.Interface;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IUpSideDownSkill
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public float jumpBufferTime = 0.2f;
    public float slopeSpeedMultiplier = 1.5f;
    public LayerMask groundLayer;
    public float rotationSpeed = 200f;

    private Rigidbody2D rb;
    private BoxCollider2D bx;
    private float jumpBufferCounter;
    private Vector2 rotateInput;

    public float uphillMultiplier = 0.4f;
    public float downhillMultiplier = 0.8f;

    private bool isFlipping = false;

    private float flipSpeed = 400f; // Degrees per second
    private float flipProgress = 0f;
    private float targetAngle = 360f;// Can be -360 or 360 depending on skill

    private bool isDashing = false;
    private bool canDash = true;

    public float dashCooldown = 2f;
    public float dashDuration = 0.3f;
    public float dashSpeed = 20f;
    [Header("Score Variables")]
    public int trickScore = 100;
    private bool wasTricking = false;
    public float speedScoreInterval = 0.1f;
    public float speedThreshold = 10f;
    public float speedMultiplier = 1.5f;
    public float speedTimer = 0f;
    [Header("Trick Multiplier")]
    public int currentMultiplier = 1;
    public int maxMultiplier = 5;
    public float trickResetTime = 2f;
    private float trickTimer = 0f;

    [Header("Inspector")]
    public float currentSpeed;

    public SurfaceEffector2D surfaceEffector;
    private Head head;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bx = GetComponent<BoxCollider2D>();
        head = GetComponentInChildren<Head>();
    }

    private void Update()
    {
        UpdateJumpBuffer();

        if (!IsGround())
        {
            Rotate();
        }
        if (currentMultiplier > 1)
        {
            trickTimer += Time.deltaTime;
            if (trickTimer > trickResetTime)
            {
                currentMultiplier = 1;
                trickTimer = 0f;
                ScoreManager.Instance.SetMultiplier(currentMultiplier); // Reset HUD
            }
        }

        if (isFlipping)
        {
            float rotateStep = flipSpeed * Time.deltaTime;
            float step = Mathf.Sign(targetAngle) * rotateStep;

            transform.Rotate(Vector3.forward, step);
            flipProgress += Mathf.Abs(step);

            if (flipProgress >= Mathf.Abs(targetAngle))
            {
                isFlipping = false;
                flipProgress = 0f;
                transform.rotation = Quaternion.identity; // Optional: reset rotation\
                if (wasTricking)
                {
                    int scoreToAdd = trickScore * currentMultiplier;
                    ScoreManager.Instance.AddScore(scoreToAdd);

                    // tăng multiplier nếu chưa đến max
                    if (currentMultiplier < maxMultiplier)
                    {
                        currentMultiplier++;
                        ScoreManager.Instance.SetMultiplier(currentMultiplier);
                    }


                    trickTimer = 0f;
                    wasTricking = false;
                }
            }

        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Run();

            if (IsGround() && jumpBufferCounter > 0)
            {
                Jump();
                jumpBufferCounter = 0;
            }
        }
        speedTimer += Time.fixedDeltaTime;
        if (speedTimer >= speedScoreInterval)
        {
            currentSpeed = rb.linearVelocity.magnitude;
            if (currentSpeed > speedThreshold)
            {
                int speedScore = Mathf.RoundToInt((currentSpeed - speedThreshold) * speedMultiplier);
                ScoreManager.Instance.AddScore(speedScore);
            }
            speedTimer = 0f;
        }
    }

    private void Run()
    {
        Vector2 groundNormal = GetGroundNormal();

        if (groundNormal != Vector2.zero)
        {
            Vector2 slopeTangent = new Vector2(groundNormal.y, -groundNormal.x).normalized;
            float slopeFactor = Vector2.Dot(slopeTangent, Vector2.right * transform.localScale.x);

            float adjustedSpeed = moveSpeed;

            if (slopeFactor > 0.01f)
            {
                adjustedSpeed = moveSpeed * (1 + slopeFactor * downhillMultiplier);
                //Debug.Log("Downhill Slope Detected: " + slopeFactor);
            }
            else if (slopeFactor < -0.01f)
            {
                adjustedSpeed = moveSpeed * (1 + slopeFactor * uphillMultiplier);
                //Debug.Log("Uphill Slope Detected: " + slopeFactor);
            }

            // Smoothly change speed
            float currentSpeed = surfaceEffector.speed;
            surfaceEffector.speed = Mathf.Lerp(currentSpeed, adjustedSpeed, Time.fixedDeltaTime * 5f);
        }
        else
        {
            // No slope or in air
            surfaceEffector.speed = Mathf.Lerp(surfaceEffector.speed, moveSpeed, Time.fixedDeltaTime * 5f);
            //Debug.Log(" nomal speed" + surfaceEffector.speed);
        }

    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnJump()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    public void OnSkillUpSideDown(InputValue value)
    {
        if (value.isPressed && !IsGround() && !isFlipping)
        {
            UpsideDown();
        }
    }

    public void OnSkillDownSideUp(InputValue value)
    {
        if (value.isPressed && !IsGround() && !isFlipping)
        {
            DownSideUp();
        }
    }
    // Press Q to flip 360° clockwise
    public void UpsideDown()
    {
        isFlipping = true;
        flipProgress = 0f;
        targetAngle = 360f;
        wasTricking = true;
    }

    // Press E to flip 360° counter-clockwise
    public void DownSideUp()
    {
        isFlipping = true;
        flipProgress = 0f;
        targetAngle = -360f;
        wasTricking = true;
    }

    private void UpdateJumpBuffer()
    {
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;
    }

    // click shift to dash
    private void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isFlipping && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; // Stop gravity during dash

        float dashTime = 0f;
        while (dashTime < dashDuration)
        {
            rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
            dashTime += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private bool IsGround()
    {
        Bounds bounds = bx.bounds;
        float rayLength = 0.1f;

        Vector2 left = new Vector2(bounds.min.x + 0.05f, bounds.min.y);
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 right = new Vector2(bounds.max.x - 0.05f, bounds.min.y);

        bool leftHit = Physics2D.Raycast(left, Vector2.down, rayLength, groundLayer);
        bool centerHit = Physics2D.Raycast(center, Vector2.down, rayLength, groundLayer);
        bool rightHit = Physics2D.Raycast(right, Vector2.down, rayLength, groundLayer);

        return leftHit || centerHit || rightHit;
    }

    private Vector2 GetGroundNormal()
    {
        Bounds bounds = bx.bounds;
        float rayLength = 0.2f;

        Vector2 downDirection = -transform.up;

        Vector2 left = new Vector2(bounds.min.x + 0.05f, bounds.min.y);
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 right = new Vector2(bounds.max.x - 0.05f, bounds.min.y);

        RaycastHit2D leftHit = Physics2D.Raycast(left, downDirection, rayLength, groundLayer);
        RaycastHit2D centerHit = Physics2D.Raycast(center, downDirection, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(right, downDirection, rayLength, groundLayer);

        Vector2 sumNormal = Vector2.zero;
        int count = 0;

        if (leftHit) { sumNormal += leftHit.normal; count++; }
        if (centerHit) { sumNormal += centerHit.normal; count++; }
        if (rightHit) { sumNormal += rightHit.normal; count++; }

        return count > 0 ? (sumNormal / count).normalized : Vector2.zero;
    }

    public void OnRotate(InputValue value)
    {
        rotateInput = value.Get<Vector2>();
    }

    private void Rotate()
    {
        float rotationAmount = -rotationSpeed * rotateInput.x * Time.deltaTime;
        transform.Rotate(Vector3.forward * rotationAmount);
    }

    private void OnDrawGizmos()
    {
        if (bx == null) return;

        Bounds bounds = bx.bounds;
        float rayLength = 0.2f;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector2.down * rayLength);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            head?.OnHeadHitGround();
        }
    }
}
