using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (UIAutoScaler))]
public class UIAnimation : MonoBehaviour
{
    private UIAutoScaler scaler;
    private RectTransform my_Rect;
    private bool runningCoroutine = false;

    public Curves[] curves = new Curves[1];
    [System.Serializable]
    public class Curves
    {
        public string name;
        public bool loop = false;
        public AnimationCurve x = new AnimationCurve ();
        public AnimationCurve y = new AnimationCurve();
        public AnimationCurve rot = new AnimationCurve();
    }


    void Awake ()
    {
        my_Rect = GetComponent<RectTransform>();
        if (curves[0] == null)
            curves[0] = new Curves();
        scaler = GetComponent<UIAutoScaler>();
    }


    public void SetKey(float time, int i, Vector2 screenSize)
    {
        curves[i].x.AddKey(time, (my_Rect.anchoredPosition.x / screenSize.x) * 100f);
        curves[i].y.AddKey(time, (my_Rect.anchoredPosition.y / screenSize.y) * 100f);
        curves[i].rot.AddKey(time, my_Rect.eulerAngles.z);
    }


    public void UpdateAnimationState(int i, float time)
    {
        scaler.ChangePosition(new Vector2(curves[i].x.Evaluate(time), curves[i].y.Evaluate(time)));
        my_Rect.eulerAngles = new Vector3(0f, 0f, curves[i].rot.Evaluate(time));
    }


    public void PlayAnimation(int index)
    {
        StartCoroutine(AnimationCoroutine(index));
    }


    public void PlayAnimation(string name)
    {
        for (int i = 0; i < curves.Length; i++)
        {
            if (curves[i].name == name)
            {
                StartCoroutine(AnimationCoroutine(i));
                break;
            }
        }
    }


    public void StopAnimation ()
    {
        StopAllCoroutines();
    }


    public IEnumerator AnimationCoroutine(int i)
    {
        while (runningCoroutine)
        { yield return null; }

        runningCoroutine = true;
        float timer = 0f;
        while (timer < curves[i].rot.keys[curves[i].rot.length - 1].time || curves[i].loop)
        {
            timer += Time.deltaTime;
            UpdateAnimationState(i, timer);
            yield return null;
        }
        runningCoroutine = false;
    }


    void OnDisable()
    {
        StopAllCoroutines();
        runningCoroutine = false;
    }
}
