using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float jumpForce = 12f;
    public float jumpBufferTime = 0.2f;
    public LayerMask groundLayer;
    public float rotationSpeed = 200f;

    [SerializeField] private Rigidbody2D rb;
    private CapsuleCollider2D cap;
    private float jumpBufferCounter;
    private Vector2 rotateInput;

    private bool isDashing = false;
    private bool canDash = true;
    private bool isJumping = false;

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

    [SerializeField] private float maxSpeed = 90f;
    [SerializeField] private float baseAccelRate = 50f;
    [SerializeField] private float baseDecelRate = 3f;

    [SerializeField] private float slideBackForce = 2f; // lực đẩy ngược lại
    [SerializeField] private float stopThreshold = 0.1f; // nếu tốc độ quá nhỏ

    [SerializeField] private TrailRenderer trail;

    private Head head;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = false;
        cap = GetComponent<CapsuleCollider2D>();
        head = GetComponentInChildren<Head>();
    }

    private void Update()
    {
        UpdateJumpBuffer();
        bool isGrounded = IsGround();
        if (!isGrounded)
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

            bool isGrounded = IsGround();

            if (isGrounded && jumpBufferCounter > 0)
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
        if (groundNormal == Vector2.zero) return;

        float slopeDir = Vector2.Perpendicular(groundNormal).x;
        float slopeAmt = Mathf.Abs(slopeDir);

        // Dựa vào độ nghiêng để tăng hoặc giảm lực
        Vector2 slideDir = Vector2.Perpendicular(groundNormal).normalized;

        float currentSpeed = rb.linearVelocity.magnitude;

        if (slopeDir > 0.05f)
        {
            // Xuống dốc → tăng tốc
            float accel = baseAccelRate * slopeAmt * Mathf.Pow(slopeAmt, 4f);
            rb.AddForce(slideDir * accel, ForceMode2D.Force);
        }
        else if (slopeDir < -0.05f)
        {
            // Lên dốc → giảm tốc bằng lực ngược
            float decel = baseDecelRate * slopeAmt * 0.2f;

            // Nếu tốc độ nhỏ → trượt ngược lại
            if (currentSpeed <= stopThreshold)
            {
                rb.AddForce(-slideDir * slideBackForce, ForceMode2D.Force);
            }
            else
            {
                rb.AddForce(-slideDir * decel, ForceMode2D.Force);
            }
        }

        // Clamp vận tốc lại để tránh vượt quá giới hạn
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnJump()
    {
        jumpBufferCounter = jumpBufferTime;
        isJumping = true;
    }

    private void OnJumpCanceled()
    {
        isJumping = false;
        if (rb.linearVelocity.y > 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f); // cut jump short
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
        if (value.isPressed && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        trail.emitting = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        Vector2 dashDir = GetDashDirection();
        rb.AddForce(dashDir * dashSpeed, ForceMode2D.Impulse);

        float dashTime = 0f;
        while (dashTime < dashDuration)
        {
            dashTime += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = originalGravity;
        isDashing = false;
        trail.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private Vector2 GetDashDirection()
    {
        Vector2 dashDir;

        // Nếu đang có input (di chuyển)
        if (rotateInput.x != 0)
        {
            dashDir = new Vector2(Mathf.Sign(rotateInput.x), 0);
        }
        else
        {
            // Nếu không có input thì dash theo hướng đang nhìn (scale)
            dashDir = transform.right;
        }

        return dashDir.normalized;
    }

    private bool IsGround()
    {
        float rayLength = 0.1f;
        Vector2 origin = transform.position;

        // Hướng raycast phải theo hướng -transform.up vì nhân vật có thể bị xoay
        Vector2 down = -transform.up;

        // Các điểm raycast lệch sang trái và phải
        Vector2 left = origin + (Vector2)(-transform.right * 0.3f);
        Vector2 right = origin + (Vector2)(transform.right * 0.3f);

        Debug.DrawRay(origin, down * rayLength, Color.green);
        Debug.DrawRay(left, down * rayLength, Color.red);
        Debug.DrawRay(right, down * rayLength, Color.red);

        bool centerHit = Physics2D.Raycast(origin, down, rayLength, groundLayer);
        bool leftHit = Physics2D.Raycast(left, down, rayLength, groundLayer);
        bool rightHit = Physics2D.Raycast(right, down, rayLength, groundLayer);

        return centerHit || leftHit || rightHit;
    }

    private Vector2 GetGroundNormal()
    {
        float rayLength = 0.2f;
        Vector2 origin = transform.position;
        Vector2 down = -transform.up;

        Vector2 left = origin + (Vector2)(-transform.right * 0.3f);
        Vector2 right = origin + (Vector2)(transform.right * 0.3f);

        RaycastHit2D centerHit = Physics2D.Raycast(origin, down, rayLength, groundLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(left, down, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(right, down, rayLength, groundLayer);

        Vector2 sumNormal = Vector2.zero;
        int count = 0;

        if (centerHit) { sumNormal += centerHit.normal; count++; }
        if (leftHit) { sumNormal += leftHit.normal; count++; }
        if (rightHit) { sumNormal += rightHit.normal; count++; }

        return count > 0 ? (sumNormal / count).normalized : Vector2.zero;
    }

    private void Rotate()
    {
        if (IsGround()) return;

        Debug.Log($"On ground: {IsGround()}");
        float torque = -rotationSpeed * rotateInput.x;
        rb.AddTorque(torque);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -60f, 60f);

    }

    public void OnRotate(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        rotateInput = new Vector2(input.x, 0); // Chỉ dùng trục X để xoay
    }

    private void OnDrawGizmos()
    {
        if (cap == null) return;

        Bounds bounds = cap.bounds;
        float rayLength = 0.2f;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector2.down * rayLength);
    }
}