using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextControl : MonoBehaviour {
    public Text PopupText;

    public void CreateText(float playerNumber, float amount, Vector3 location)
    {
        if (amount >= 1)
        {
            //Vector2 screenPosition = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));//Camera.main.WorldToScreenPoint(location);
            Text instance = Instantiate(PopupText);
            instance.transform.SetParent(GameObject.Find("Player" + playerNumber + "/DamageCanvas").transform, false);
            instance.text = "-" + Mathf.Round(amount);
            //instance.transform.position = screenPosition;
        }
    }
}
