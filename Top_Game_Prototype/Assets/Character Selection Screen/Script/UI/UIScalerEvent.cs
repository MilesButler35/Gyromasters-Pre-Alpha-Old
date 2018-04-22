using UnityEngine;

[ExecuteInEditMode]
public class UIScalerEvent : MonoBehaviour
{
    public delegate void UIAction();
    public static event UIAction UIEvent;

    private Vector2 lastFrameScreenSize;
    private Vector2 actualFrameScreenSize;


	void Start ()
    {
        lastFrameScreenSize.x = Screen.width;
        lastFrameScreenSize.y = Screen.height;
	}
	

	void Update ()
    {
        actualFrameScreenSize.x = Screen.width;
        actualFrameScreenSize.y = Screen.height;
        if (lastFrameScreenSize != actualFrameScreenSize || !Application.isPlaying)
        {
            if (UIEvent != null)
                UIEvent();
        }
        lastFrameScreenSize = actualFrameScreenSize;
	}
}