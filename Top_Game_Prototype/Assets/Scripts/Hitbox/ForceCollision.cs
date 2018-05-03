using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCollision : MonoBehaviour
{

    public float force;

    private void OnTriggerEnter(Collider other)
    {
        // Find all targets in an area within hitbox and damage them.
        Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();

        if (!targetRigidbody)
            return;

        targetRigidbody.constraints = RigidbodyConstraints.None;

        targetRigidbody.AddForce(Vector3.up * force);
    }
}
