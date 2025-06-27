using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;

    public float dashDistance = 3f;
    public float dashDuration = 0.25f;
    private float dashUntil;
    private Vector3 dashDirection;

    private string state = "Normal";
    public Vector3 direction;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
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

            if (Input.GetMouseButton(1))
            {
                dashUntil = Time.time + dashDuration;
                dashDirection = direction;
                state = "Dash";
            }
        }
        else if (state == "Dash")
        {
            rb.velocity = dashDirection * dashDistance / dashDuration;
            if (Time.time >= dashUntil)
            {
                state = "Normal";
            }
        }
        // weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
