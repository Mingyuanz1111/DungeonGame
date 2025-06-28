using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;

    public float dashDistance = 3f;
    public float dashDuration = 0.25f;
    private float dashTimeLeft = 0;
    private Vector3 dashDirection;

    public string state = "Normal";
    private Vector3 direction;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - transform.position;
        direction.z = 0.0f;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (state == "Normal")
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(horizontal, vertical).normalized * speed;

            if (Input.GetMouseButtonDown(1))
            {
                dashTimeLeft = dashDuration;
                dashDirection = direction;
                state = "Dash";
            }
        }
        else if (state == "Dash")
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                rb.velocity = Vector3.zero;
                state = "Normal";
            }
            rb.velocity = dashDirection * dashDistance / dashDuration;
        }
        // weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
