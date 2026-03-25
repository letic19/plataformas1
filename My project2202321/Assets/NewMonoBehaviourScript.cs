using UnityEngine;

/// <summary>
/// SphereControllerSimple: simple physics-based controller for a rolling sphere.
/// - Uses Rigidbody.AddTorque to roll the sphere according to Horizontal/Vertical input.
/// - Supports jumping when grounded (uses Input.GetButton("Jump")).
/// - Exposes parameters for tuning in the Inspector.
/// Attach this to your Sphere GameObject (ensure it has a Rigidbody and a collider).
/// This class is intentionally named differently to avoid conflicts with other controllers named "SphereController".
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SphereControllerSimple : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveTorque = 200f;
    [SerializeField] private float maxSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check")]
    [Tooltip("Layers considered ground for jumping")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, -0.5f, 0f);

    private Rigidbody rb;
    private Camera mainCamera;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            // Add a Rigidbody if missing so the script is safer to attach in the editor.
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
        }

        // Allow fast rolling
        rb.maxAngularVelocity = 50f;

        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        // Read input (built-in Input system)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0f, v);

        if (input.sqrMagnitude > 0.0001f)
        {
            // Convert input to world direction relative to camera (if present)
            Vector3 moveDir;
            if (mainCamera != null)
            {
                Vector3 camForward = Vector3.Scale(mainCamera.transform.forward, new Vector3(1f, 0f, 1f)).normalized;
                moveDir = camForward * v + mainCamera.transform.right * h;
            }
            else
            {
                moveDir = transform.forward * v + transform.right * h;
            }

            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 0.0001f) moveDir.Normalize();

            // Compute a torque that will make the sphere roll in moveDir
            // Cross of moveDir with up gives an axis to apply torque around.
            Vector3 torqueAxis = Vector3.Cross(moveDir, Vector3.up);
            float applied = moveTorque * Time.fixedDeltaTime;
            rb.AddTorque(torqueAxis * applied, ForceMode.Acceleration);
        }

        // Limit horizontal speed
        Vector3 horizontalVel = rb.linearVelocity;
        horizontalVel.y = 0f;
        if (horizontalVel.magnitude > maxSpeed)
        {
            Vector3 limited = horizontalVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }

        // Ground check
        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, groundMask);
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
}
