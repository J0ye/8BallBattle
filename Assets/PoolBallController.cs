using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PoolBallController : SnapBreakStop
{
    public float mag = 1f;
    [Header ("Player Ball")]
    public LayerMask groundLayer;
    public float range = 3f;
    public float maxForce = 20f; // Maximum force of the push

    [Header("LineSettings")]
    public Color startcolor = Color.green;
    public Color endColor = Color.red;
    public float colorChangeDuration = 2f; // Duration from white to red
    public float textToCameraOffset = 0.5f;

    private LineRenderer lr;
    private TextMeshPro textMesh;
    private Vector3 pushDirection;
    private float timeButtonDown;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        lr = GetComponent<LineRenderer>();
        textMesh = GetComponentInChildren<TextMeshPro>();
        textMesh.gameObject.SetActive(false);

        GameManager.instance.SetPlayer(gameObject);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Ray ray;
        #region mouse
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

        if (Input.GetMouseButtonDown(0) && lr.enabled && rb.velocity.magnitude == 0)
        {
            timeButtonDown = Time.time;
        }
        else if(Input.GetMouseButton(0) && lr.enabled && rb.velocity.magnitude == 0)
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
        else if (Input.GetMouseButtonUp(0) && pushDirection != Vector3.zero && rb.velocity.magnitude == 0)
        {
            float lerpFactor = Mathf.Clamp01((Time.time - timeButtonDown) / colorChangeDuration);
            float forceMagnitude = Mathf.Lerp(0, maxForce, lerpFactor);
            rb.AddForce(pushDirection * forceMagnitude, ForceMode.Impulse);
            lr.startColor = startcolor;
            lr.endColor = startcolor;
            textMesh.gameObject.SetActive(false);
        }

        if(Input.GetMouseButtonUp(1) && pushDirection != Vector3.zero && rb.velocity.magnitude == 0)
        {
            rb.AddForce(pushDirection * maxForce, ForceMode.Impulse);
            textMesh.gameObject.SetActive(false);
        }
        #endregion

        #region touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch
            ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                // Calculate the vector pointing from the GameObject's position to the touch's world position
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

            if (rb.velocity.magnitude <= 0)
            {

                if (touch.phase == TouchPhase.Began && lr.enabled)
                {
                    timeButtonDown = Time.time;
                }
                else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved && lr.enabled)
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
                else if (touch.phase == TouchPhase.Ended && pushDirection != Vector3.zero)
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
        #endregion

        mag = rb.velocity.magnitude;
    }
}
