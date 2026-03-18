using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereController : MonoBehaviour
{
    [Header("Movement")]
    public float moveTorque = 10f;
    public float maxVelocity = 5f;

    [Header("Jump")]
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("References")]
    public Transform cameraTransform;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Jump input is read in Update to ensure button presses aren't lost between frames
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Read movement input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        if (input.sqrMagnitude > 0.0001f)
        {
            // Use camera orientation to move relative to view
            Vector3 camForward = cameraTransform ? cameraTransform.forward : Vector3.forward;
            Vector3 camRight = cameraTransform ? cameraTransform.right : Vector3.right;
            camForward.y = 0f; camRight.y = 0f;
            camForward.Normalize(); camRight.Normalize();

            Vector3 moveDir = (camForward * input.z + camRight * input.x).normalized;

            // Apply torque so the sphere rolls in moveDir. Axis is perpendicular to movement on the XZ plane.
            Vector3 torqueAxis = Vector3.Cross(moveDir, Vector3.up);
            rb.AddTorque(torqueAxis * moveTorque, ForceMode.Force);
        }

        LimitVelocity();
    }

    bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    void LimitVelocity()
    {
        Vector3 horizontal = rb.linearVelocity;
        horizontal.y = 0f;
        if (horizontal.magnitude > maxVelocity)
        {
            Vector3 limited = horizontal.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

