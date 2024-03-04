using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PoolBallController : MonoBehaviour
{
    public LayerMask groundLayer;
    public float range = 3f;
    public float maxForce = 20f; // Maximum force of the push
    public float stopVelocityThreshold = 0.1f;

    [Header("LineSettings")]
    public Color startcolor = Color.green;
    public Color endColor = Color.red;
    public float colorChangeDuration = 2f; // Duration from white to red
    public float textToCameraOffset = 0.5f;

    private LineRenderer lr;
    private Rigidbody rb; // Rigidbody of the GameObject
    private TextMeshPro textMesh;
    private Vector3 pushDirection;
    private float timeButtonDown;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        textMesh = GetComponentInChildren<TextMeshPro>();
        textMesh.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude < stopVelocityThreshold && rb.velocity.magnitude > 0)
        {
            rb.velocity = Vector3.zero;
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;        

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // Calculate the vector pointing from the GameObject's position to the mouse's world position
            Vector3 direction = hit.point - transform.position;

            if (Vector3.Distance(transform.position, hit.point) <= range)
            {
                lr.enabled = true;

                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, hit.point);

                pushDirection = direction.normalized;
            }
            else
            {
                lr.enabled = false;
                textMesh.gameObject.SetActive(false);
                pushDirection = Vector3.zero;
            }
        }

        if (Input.GetMouseButtonDown(0) && lr.enabled)
        {
            timeButtonDown = Time.time;
        }
        else if(Input.GetMouseButton(0) && lr.enabled)
        {
            float lerpFactor = Mathf.Clamp01((Time.time - timeButtonDown) / colorChangeDuration);
            Color currentColor = Color.Lerp(startcolor, endColor, lerpFactor);
            lr.startColor = currentColor;
            lr.endColor = currentColor;
            float forceMagnitude = Mathf.Min(lerpFactor * maxForce, maxForce);
            textMesh.text = $"{forceMagnitude:0}";

            Vector3 midpoint = (transform.position + hit.point) / 2;
            Vector3 directionToCamera = (Camera.main.transform.position - midpoint).normalized;
            Vector3 adjustedMidpoint = midpoint + directionToCamera * textToCameraOffset;
            textMesh.gameObject.SetActive(true);
            textMesh.transform.position = adjustedMidpoint;
            textMesh.transform.rotation = Quaternion.LookRotation(textMesh.transform.position - Camera.main.transform.position);
        }
        else if (Input.GetMouseButtonUp(0) && pushDirection != Vector3.zero)
        {
            float lerpFactor = Mathf.Clamp01((Time.time - timeButtonDown) / colorChangeDuration);
            float forceMagnitude = Mathf.Lerp(0, maxForce, lerpFactor);
            rb.AddForce(pushDirection * forceMagnitude, ForceMode.Impulse);
            lr.startColor = startcolor;
            lr.endColor = startcolor;
            textMesh.gameObject.SetActive(false);
        }
    }
}
