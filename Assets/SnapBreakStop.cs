using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapBreakStop : MonoBehaviour
{
    [Header ("Snap Break Stop")]
    public float stopVelocityThreshold = 0.1f;

    protected Rigidbody rb;
    protected Vector3 startPosition = Vector3.up;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.down, ForceMode.Impulse);
        startPosition = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (rb.velocity.magnitude < stopVelocityThreshold && rb.velocity.magnitude > 0)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void Kill()
    {
        transform.position = startPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Vector3.down, ForceMode.Impulse);
    }
}
