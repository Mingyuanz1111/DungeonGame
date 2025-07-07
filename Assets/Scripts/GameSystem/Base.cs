using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public bool isMoving = false;
    public bool isBoosted = false;
    
    public float distBetweenRooms = 25f;
    private float timeBetweenRooms;
    private float timeUntilNext;

    private float moveSpeed;
    public float boostMultiplier = 2f;

    public float boostDuration = 1f;
    private float boostTimeLeft = 0f;

    public int pathIdx;
    public List<Vector2> path;

    private Rigidbody2D rb;
    public DungeonGenerator dungeonGen;
    public Gamemode gamemode;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(isMoving)
        {
            if (timeUntilNext <= 0)
            {
                pathIdx++;
                timeUntilNext = timeBetweenRooms;
            }
            if (pathIdx >= path.Count)
            {
                rb.velocity = Vector3.zero;
                isMoving = false;
            }
            else
            {
                rb.velocity = (path[pathIdx] - (Vector2)transform.position).normalized * moveSpeed * ((isBoosted)?boostMultiplier:1f);
                timeUntilNext -= Time.deltaTime * ((isBoosted) ? boostMultiplier : 1f);
            }
        }
        if (boostTimeLeft > 0)
        {
            isBoosted = true;
            boostTimeLeft -= Time.deltaTime;
        }
        else
        {
            isBoosted = false;
        }
    }

    public void ResetPath()
    {
        pathIdx = 0;
        path.Clear();
    }

    public void StartMove()
    {
        isMoving = true;
        timeBetweenRooms = gamemode.timeUntilNextPhase / path.Count;
        timeUntilNext = timeBetweenRooms;
        moveSpeed = distBetweenRooms / timeBetweenRooms;
    }
}
