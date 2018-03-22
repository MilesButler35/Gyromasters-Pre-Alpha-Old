using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveTargetController : MonoBehaviour
{
    [HideInInspector] public int m_PlayerNumber = 1;
    [HideInInspector] public float speed;

    private float moveHorizontal;
    private float moveVertical;
    private string h_MovementAxisName;
    private string v_MovementAxisName;

    void Start()
    {
        h_MovementAxisName = "Horizontal" + m_PlayerNumber;
        v_MovementAxisName = "Vertical" + m_PlayerNumber;
        moveHorizontal = 0f;
        moveVertical = 0f;
        gameObject.SetActive(false);
    }

    void Update()
    {
        moveHorizontal = Input.GetAxis(h_MovementAxisName) * speed * Time.deltaTime;
        moveVertical = Input.GetAxis(v_MovementAxisName) * speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        transform.Translate(moveHorizontal, 0, moveVertical);
    }
}