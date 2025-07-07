using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool isFollow = true;
    //private GameObject[] playerObjects;
    private GameObject playerObject;
    private GameObject baseObject;
    private Transform currentlyFollow;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Start()
    {
        /*playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject eachPlayer in players)
        {
            if (eachPlayer.name == "Base")
            {
                baseObject = eachPlayer;
            }
            else
            {
                playerObject = eachPlayer;
            }
        }*/
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        /*if (playerObject.activeSelf && !gamemode.gameOver)
        {
            currentlyFollow = playerObject.transform;
        }
        else
        {
            currentlyFollow = baseObject.transform;
        }*/
        currentlyFollow = playerObject.transform;
        if (isFollow)
        {
            Vector3 desiredPosition = currentlyFollow.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}