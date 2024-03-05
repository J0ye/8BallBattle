using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform barrelEnd;
    public Vector3 rotationOffset;
    public Vector3 positionOffset = new Vector3(0, 0.7f, -0.25f);
    public float shootEverySecond = 1f;

    private float timer = 0f;
    void Update()
    {
        if (GameManager.instance.player != null)
        {
            // Calculate the position with offset
            Vector3 targetPosition = GameManager.instance.player.transform.position + rotationOffset;
            
            // Make the GameObject look at the player with the offset
            transform.LookAt(targetPosition);
        }

        transform.position = new Vector3(transform.parent.position.x + positionOffset.x, 
            positionOffset.y, transform.parent.position.z + positionOffset.z);

        timer += Time.deltaTime * GameManager.instance.GameSpeed;
        if (timer >= shootEverySecond && GameManager.instance.player != null)
        {
            ShootProjectile();
            timer = 0f;
        }
    }

    protected void ShootProjectile()
    {
        GameObject newBullet = Instantiate(projectilePrefab, barrelEnd.position, Quaternion.identity);
        newBullet.GetComponent<Projectile>().origin = transform.parent.gameObject;
    }
}
