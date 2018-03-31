using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollision : MonoBehaviour {

    public int m_PlayerNumber = 1;              // Used to identify the different players.
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;
    public float m_MaxLifeTime = 2f;
    public float m_HitStun = 0f;
    [Header("Damage")]
    public float m_MaxDamage = 100f;
    public float m_ExplosionRadius = 5f;
    [Header("Force")]
    public float m_ExplosionForce = 1000f;
    
    public Rigidbody m_OwnerRigidbody;

    public enum HitboxType { STUN, BARRIER, DIVE}

    public HitboxType currentType = HitboxType.STUN;

    private List<GameObject> items;

    private void Start()
    {
        if (currentType == HitboxType.BARRIER)
        {
            Destroy(gameObject, m_MaxLifeTime);
        }    
        transform.localScale = new Vector3(m_ExplosionRadius, m_ExplosionRadius, m_ExplosionRadius);
    }


    private void OnTriggerEnter(Collider other)
    {      
        // Find all targets in an area within hitbox and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.x/2, m_TankMask);

        switch (currentType)
        {
            case HitboxType.STUN:
                break;
            case HitboxType.BARRIER:
                for (int i = 0; i < colliders.Length; i++)
                {       
                    Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                    if (!targetRigidbody || targetRigidbody == m_OwnerRigidbody)
                        continue;

                    TopmanPlayerController targetPlayerController = colliders[i].GetComponent<TopmanPlayerController>();

                    if (!targetPlayerController)
                        continue;

                    if (targetPlayerController.currentState == TopmanPlayerController.StateMachine.DIVE)
                        continue;

                    targetRigidbody.velocity = new Vector3(0f, 0f, 0f);

                    targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

                    TopmanStats targetHealth = targetRigidbody.GetComponent<TopmanStats>();

                    if (!targetHealth)
                        continue;

                    float damage = m_MaxDamage;

                    targetHealth.TakeDamage(damage, m_HitStun);
                }
                break;
            case HitboxType.DIVE:
                for (int i = 0; i < colliders.Length; i++)
                {
                    Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                    if (!targetRigidbody || targetRigidbody == m_OwnerRigidbody)
                        continue;

                    TopmanPlayerController targetPlayerController = colliders[i].GetComponent<TopmanPlayerController>();

                    if (!targetPlayerController)
                        continue;

                    if (targetPlayerController.currentState == TopmanPlayerController.StateMachine.RUSH)
                        continue;

                    targetRigidbody.velocity = new Vector3(0f, 0f, 0f);

                    targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

                    TopmanStats targetHealth = targetRigidbody.GetComponent<TopmanStats>();

                    if (!targetHealth)
                        continue;

                    float damage = CalculateDamage(targetRigidbody.position);

                    targetHealth.TakeDamage(damage, m_HitStun);
                }
                gameObject.GetComponent<SphereCollider>().enabled = false;
                Destroy(gameObject);               
                break;
        }
        
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}
