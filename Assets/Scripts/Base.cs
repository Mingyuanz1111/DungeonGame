using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public bool isMoving = false;

    public float timeBetweenLevels = 90f;
    public float distBetweenRooms = 25f;
    private float timeUntilDest;
    private float timeUntilNext;
    private float moveSpeed;
    public int pathIdx;
    public List<Vector2> path;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(isMoving)
        {
            if(timeUntilNext <= 0)
            {
                pathIdx++;
                timeUntilNext = timeBetweenLevels / path.Count;
            }
            if (pathIdx >= path.Count)
            {
                isMoving = false;
            }
            else
            {
                rb.velocity = (path[pathIdx] - (Vector2)transform.position).normalized * moveSpeed;
                timeUntilDest -= Time.deltaTime;
                timeUntilNext -= Time.deltaTime;
            }
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
        timeUntilDest = timeBetweenLevels;
        timeUntilNext = timeBetweenLevels / path.Count;
        moveSpeed = distBetweenRooms / timeUntilNext;
    }
}
