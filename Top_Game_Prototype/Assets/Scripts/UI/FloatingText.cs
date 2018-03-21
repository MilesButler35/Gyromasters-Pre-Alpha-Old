using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour {
    // Called by Animation event in PopupText animation
    void AnimationComplete()
    {
        Destroy(transform.gameObject);
    }
}
