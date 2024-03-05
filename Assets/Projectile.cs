using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject origin;
    public Vector3 initialDirection = Vector3.up;
    public float restitutionCoefficient = 0.8f;
    public float initialVelocityMagnitude = 10f;
    public float DestroyAfterSeconds = 5f;

    private Rigidbody rb;
    private Vector3 initialVelocity;
    private float timeAlive = 0f;
    private float originalDrag;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.drag;

        GameObject playerObject = GameManager.instance.player;

        if (playerObject != null)
        {
            initialDirection = (playerObject.transform.position - transform.position).normalized;
            initialVelocity = initialDirection.normalized * initialVelocityMagnitude;
            initialVelocity = RemoveVerticalMovment(initialVelocity);
            rb.velocity = initialVelocity;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Adjust the drag based on the GameSpeed
        rb.drag = originalDrag * (1 / Mathf.Max(GameManager.instance.GameSpeed, 0.01f)); // Avoid division by zero

        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * 10f, Color.green, Time.deltaTime);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f))
        {
            if(hit.transform.TryGetComponent<SnapBreakStop>(out SnapBreakStop target) 
                && hit.transform.gameObject != origin)
            {
                target.Kill();
                Destroy(gameObject);
            }
        }

        Vector3 desiredVelocity = initialVelocity * GameManager.instance.GameSpeed;
        Vector3 velocityDifference = desiredVelocity - rb.velocity;
        velocityDifference = RemoveVerticalMovment(velocityDifference);
        rb.AddForce(velocityDifference, ForceMode.Acceleration);

        timeAlive += Time.deltaTime * GameManager.instance.GameSpeed;
        if (DestroyAfterSeconds < timeAlive)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        RaycastHit hit;
        // Perform a raycast directly before the collision happens to get the most accurate normal.
        if (Physics.Raycast(transform.position, rb.velocity.normalized, out hit, Mathf.Infinity))
        {
            Vector3 moreAccurateNormal = hit.normal;
            ReflectProjectile(moreAccurateNormal);
        }
        else
        {
            // Fallback to using the collision normal if the raycast didn't hit
            Vector3 fallbackNormal = collision.contacts[0].normal;
            ReflectProjectile(fallbackNormal);
        }
    }

    private void ReflectProjectile(Vector3 normal)
    {
        Vector3 newDirection = Vector3.Reflect(rb.velocity.normalized, normal);
        float newVelocityMagnitude = rb.velocity.magnitude * restitutionCoefficient;
        rb.velocity = RemoveVerticalMovment(newDirection * newVelocityMagnitude);
        initialVelocity = rb.velocity;
        Debug.DrawRay(transform.position, rb.velocity * 10, Color.yellow, 2.0f);
    }

    private Vector3 RemoveVerticalMovment(Vector3 value)
    {
        value = new Vector3(value.x, 0, value.z);
        return value;
    }
}
