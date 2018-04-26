using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollision : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {      
        // Find all targets in an area within hitbox and damage them.
        Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();

        if (!targetRigidbody)
            return;

        TopmanStats targetHealth = targetRigidbody.GetComponent<TopmanStats>();

        if (!targetHealth)
            return;

        float damage = targetHealth.m_StartingHealth;

        targetHealth.TakeDamage(damage);       
    }
}
