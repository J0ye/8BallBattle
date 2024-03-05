using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float speed = 1f;
    public GameObject playerPrefab;
    public GameObject player
    {
        get { return _player; }
        private set { _player = value; }
    }
    public float GameSpeed 
    { 
        get { return GetGameSpeed(); }
    }

    private GameObject _player;  //backing field
    private Vector3 playerStartPosition = Vector3.up;

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        speed = GameSpeed;
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        playerStartPosition = newPlayer.transform.position;
    }

    public void RespawnPlayer()
    {
        Instantiate(playerPrefab, playerStartPosition, playerPrefab.transform.rotation);
    }

    private float GetGameSpeed()
    {
        float ret = 1f;
        if(player != null)
        {
            float currentSpeed = player.GetComponent<Rigidbody>().velocity.magnitude;
            ret = Mathf.Clamp(currentSpeed / player.GetComponent<PoolBallController>().maxForce, 0f, 1f);
        }

        return ret;
    }
}
