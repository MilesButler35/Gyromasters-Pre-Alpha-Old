using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveTargetController : MonoBehaviour
{
    [HideInInspector] public int m_PlayerNumber = 1;
    [HideInInspector] public float speed;
    Rigidbody m_Rigidbody;

    private float moveHorizontal;
    private float moveVertical;
    private string h_MovementAxisName;
    private string v_MovementAxisName;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        h_MovementAxisName = "Horizontal" + m_PlayerNumber;
        v_MovementAxisName = "Vertical" + m_PlayerNumber;
        moveHorizontal = 0f;
        moveVertical = 0f;
        //gameObject.SetActive(false);
        Physics.IgnoreLayerCollision(10, 9);
    }

    void FixedUpdate()
    {
        moveHorizontal = Input.GetAxis(h_MovementAxisName);
        moveVertical = Input.GetAxis(v_MovementAxisName);
        float topSpeed = speed;

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        if (m_Rigidbody.velocity.magnitude > topSpeed)
        {
            SlowDownVelocity(0.95f);
        }
        else if (movement == Vector3.zero)
        {
            SlowDownVelocity(0.20f);
        }
        else
        {
            m_Rigidbody.AddForce(movement * speed * 2);
        }

    }

    private void SlowDownVelocity(float slowdownRate)
    {
        // Gradually lower velocity at a rate of slowdownRate
        m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x * slowdownRate, m_Rigidbody.velocity.y * slowdownRate, m_Rigidbody.velocity.z * slowdownRate);
    }
}