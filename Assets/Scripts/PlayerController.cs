using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 6f;

    Rigidbody rb;
    Vector3 moveInput;
    bool jumpQueued;

    readonly HashSet<Collider> groundContacts = new();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveInput = Vector3.ClampMagnitude(
            new Vector3(horizontal, 0f, vertical), 1f);

        if (groundContacts.Count > 0 &&
            Input.GetKeyDown(KeyCode.Space))
        {
            jumpQueued = true;
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = moveInput * speed;

        // Change horizontal movement while preserving gravity/jump velocity.
        rb.linearVelocity = new Vector3(
            movement.x, rb.linearVelocity.y, movement.z);

        if (jumpQueued)
        {
            if (groundContacts.Count > 0)
            {
                rb.AddForce(
                    Vector3.up * jumpForce, ForceMode.Impulse);

                groundContacts.Clear();
            }

            jumpQueued = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        UpdateGroundContact(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        UpdateGroundContact(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        groundContacts.Remove(collision.collider);
    }

    void UpdateGroundContact(Collision collision)
    {
        // Ignore contacts while rising after a jump.
        if (rb.linearVelocity.y > 0.1f)
        {
            groundContacts.Remove(collision.collider);
            return;
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            // Upward-facing surfaces count as ground; walls don't.
            if (contact.normal.y > 0.5f)
            {
                groundContacts.Add(collision.collider);
                return;
            }
        }

        groundContacts.Remove(collision.collider);
    }

    void OnDisable()
    {
        groundContacts.Clear();
        jumpQueued = false;
    }
}