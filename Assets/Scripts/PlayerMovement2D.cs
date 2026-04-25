using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Collider2D boundaryZone;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Bounds bounds;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (boundaryZone != null)
        {
            bounds = boundaryZone.bounds;
        }
        else
        {
            Debug.LogWarning("PlayerMovement2D: No boundary zone assigned. Movement bounds will not be enforced.", this);
        }
    }

    void Update()
    {
        // Read input using the new Input System - keyboard first, then gamepad
        float x = 0f;
        float y = 0f;
        
        // Keyboard input (WASD/Arrow keys)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed) x = 1f;
            if (Keyboard.current.aKey.isPressed) x = -1f;
            if (Keyboard.current.wKey.isPressed) y = 1f;
            if (Keyboard.current.sKey.isPressed) y = -1f;
        }
        
        // Gamepad input (only if no keyboard input)
        if (x == 0 && y == 0 && Gamepad.current != null)
        {
            Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
            x = stickInput.x;
            y = stickInput.y;
        }
        
        moveInput = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        // Apply movement via physics
        rb.linearVelocity = moveInput * moveSpeed;

        // Clamp position within bounds and stop velocity only if pushing into bounds
        if (boundaryZone != null)
        {
            Collider2D playerCollider = GetComponent<Collider2D>();
            Vector2 clampedPos = rb.position;
            Vector2 clampedVelocity = rb.linearVelocity;
            
            if (playerCollider != null)
            {
                // Get player collider bounds
                Bounds playerBounds = playerCollider.bounds;
                float playerExtentX = playerBounds.extents.x;
                float playerExtentY = playerBounds.extents.y;
                
                float minX = bounds.min.x + playerExtentX;
                float maxX = bounds.max.x - playerExtentX;
                float minY = bounds.min.y + playerExtentY;
                float maxY = bounds.max.y - playerExtentY;
                
                // Only kill velocity if it would push further out of bounds
                if ((clampedPos.x <= minX && clampedVelocity.x < 0) || (clampedPos.x >= maxX && clampedVelocity.x > 0))
                {
                    clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
                    clampedVelocity.x = 0;
                }
                if ((clampedPos.y <= minY && clampedVelocity.y < 0) || (clampedPos.y >= maxY && clampedVelocity.y > 0))
                {
                    clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
                    clampedVelocity.y = 0;
                }
            }
            else
            {
                // Fallback: clamp center position only
                if ((clampedPos.x <= bounds.min.x && clampedVelocity.x < 0) || (clampedPos.x >= bounds.max.x && clampedVelocity.x > 0))
                {
                    clampedPos.x = Mathf.Clamp(clampedPos.x, bounds.min.x, bounds.max.x);
                    clampedVelocity.x = 0;
                }
                if ((clampedPos.y <= bounds.min.y && clampedVelocity.y < 0) || (clampedPos.y >= bounds.max.y && clampedVelocity.y > 0))
                {
                    clampedPos.y = Mathf.Clamp(clampedPos.y, bounds.min.y, bounds.max.y);
                    clampedVelocity.y = 0;
                }
            }
            
            rb.position = clampedPos;
            rb.linearVelocity = clampedVelocity;
        }
    }
}