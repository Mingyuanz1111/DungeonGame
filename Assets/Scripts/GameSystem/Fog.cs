using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    public GameObject playerObject;
    private HealthSystem playerHealth;

    public float fogDamage = 10f;
    public float timeBtwDamage = 1f;
    private float timeUntilDamage;

    private Vector3 targetPos;
    private Vector3 targetScale;
    private float moveSpeed = 40f;
    private float defaultPosX;
    private float defaultScaleX;

    void Start()
    {
        timeUntilDamage = timeBtwDamage;
        playerHealth = playerObject.GetComponent<HealthSystem>();

        defaultPosX = transform.position.x;
        defaultScaleX = transform.localScale.x;
    }

    void Update()
    {
        Rect area = new Rect(transform.position.x - transform.localScale.x/2f, transform.position.y - transform.localScale.y/2f, transform.localScale.x, transform.localScale.y);

        if (!area.Contains((Vector2)playerObject.transform.position))
        {
            if (timeUntilDamage <= 0)
            {
                playerHealth.TakeDamage(fogDamage);
                timeUntilDamage = timeBtwDamage;
            }
            else
            {
                timeUntilDamage -= Time.deltaTime;
            }
        }
        else
        {
            timeUntilDamage = timeBtwDamage;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * moveSpeed);
    }

    public void Move(Vector3 pos, Vector3 scale, float speed = 40f)
    {
        targetPos = pos;
        targetScale = scale;
        moveSpeed = speed;
    }

    public void MoveY(float yPos, float yScale, float speed = 40f)
    {
        targetPos = new Vector3(defaultPosX, yPos, transform.position.z);
        targetScale = new Vector3(defaultScaleX, yScale, transform.localScale.z);
        moveSpeed = speed;
    }
}
