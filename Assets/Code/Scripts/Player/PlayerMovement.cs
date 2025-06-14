using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float jumpBufferTime = 0.2f;
    public float slopeSpeedMultiplier = 1.5f;
    public LayerMask groundLayer;
    public float rotationSpeed = 200f;

    private Rigidbody2D rb;
    private BoxCollider2D bx;
    private float jumpBufferCounter;
    private Vector2 rotateInput;

    public float uphillMultiplier = 0.5f;
    public float downhillMultiplier = 1.5f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bx = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        UpdateJumpBuffer();

        if (!IsGround())
        {
            Rotate();
        }
    }

    private void FixedUpdate()
    {
        Run();

        if (IsGround() && jumpBufferCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
        }
    }

    private void Run()
    {
        Vector2 slopeNormal = GetGroundNormal();
        Vector2 moveDir = Vector2.right;
        float adjustedSpeed = moveSpeed;

        if (slopeNormal != Vector2.zero)
        {
            Vector2 slopeTangent = new Vector2(slopeNormal.y, -slopeNormal.x).normalized;
            moveDir = slopeTangent;

            float slopeFactor = Vector2.Dot(slopeTangent, Vector2.right);
            Debug.Log("Slope Factor: " + slopeFactor);

            if (slopeFactor > 0.01f) // Downhill
            {
                adjustedSpeed = moveSpeed * (1 + slopeFactor * downhillMultiplier);
                Debug.Log("Downhill speed: " + adjustedSpeed);
            }
            else if (slopeFactor < -0.01f) // Uphill
            {
                adjustedSpeed = moveSpeed * (1 + slopeFactor * uphillMultiplier);
                Debug.Log("Uphill speed: " + adjustedSpeed);
            }
            else
            {
                adjustedSpeed = moveSpeed;
                Debug.Log("Flat speed");
            }

            rb.linearVelocity = new Vector2(moveDir.x * adjustedSpeed, rb.linearVelocity.y);
        }
        else
        {
            // In air or no slope
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }

        Debug.Log("Final Velocity X: " + rb.linearVelocity.x);

    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnJump()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void UpdateJumpBuffer()
    {
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;
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

        Vector2 left = new Vector2(bounds.min.x + 0.05f, bounds.min.y);
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 right = new Vector2(bounds.max.x - 0.05f, bounds.min.y);

        RaycastHit2D leftHit = Physics2D.Raycast(left, Vector2.down, rayLength, groundLayer);
        RaycastHit2D centerHit = Physics2D.Raycast(center, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(right, Vector2.down, rayLength, groundLayer);

        Vector2 sumNormal = Vector2.zero;
        int count = 0;

        if (leftHit) { sumNormal += leftHit.normal; count++; }
        if (centerHit) { sumNormal += centerHit.normal; count++; }
        if (rightHit) { sumNormal += rightHit.normal; count++; }

        return count > 0 ? sumNormal.normalized : Vector2.zero;
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
}
