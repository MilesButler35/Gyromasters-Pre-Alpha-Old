using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopmanAnim : MonoBehaviour
{
    public TopmanDive m_Dive;

    Animator anim;
    int chargeHash = Animator.StringToHash("DiveCharge");
    int jumpHash = Animator.StringToHash("Jump");

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void IsChargingDive (bool charge)
    {
        anim.SetBool(chargeHash, charge);
    }

    public void Jump(bool jump)
    {
        anim.SetBool(jumpHash, jump);
    }

    public void MoveToTarget()
    {
        m_Dive.MoveToTarget();
    }

    public void DiveBomb()
    {
        m_Dive.OnGround();
    }
}
