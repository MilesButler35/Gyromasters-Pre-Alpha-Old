using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour
{
    // If you decided to use this optional script, there are some things here that you need to check...

    // The first one is this enum
    public enum Screens { MainMenu, CharacterSelection };
    // Add ALL your screens here


    // And now this array of rect
    public RectTransform[] rects;
    // Add ALL the main rect from each screen here
    // IMPORTANT: Make sure they are in the EXACT same order from the enum above
    // ALSO IMPORTANT: The first screen that appears in your game MUST be the first in the enum

    
    // Now, this event
    public delegate void ScreenAction();
    public static event ScreenAction onScreenChanged;
    // It will be fired every time a screen is changed, can be helpful


    public enum Sides {Left, Right, Up, Down};
    public Screens lastScreen = Screens.MainMenu;
    public Screens actualScreen = Screens.MainMenu;

    private bool runningCoroutine = false;
    private RectTransform activeRect;


    public static ScreenManager instance = null;
    private void Awake ()
    {
        instance = this;
    }


    private void Start ()
    {
        activeRect = rects[0];
        rects[0].gameObject.SetActive(true);
        for (int i = 1; i < rects.Length; i++)
            rects[i].gameObject.SetActive(false);
    }


    // This is the best place to handle the return/escape button
    // Use the switch case to make the logic in each screen
    private void Update ()
    {
    }


    /// <summary>
    /// Switch to a new game screen
    /// </summary>
    /// <param name="newScreen">The new screen</param>
    public void ChangeScreen (Screens newScreen)
    {
        if (!runningCoroutine)
        {
            StartCoroutine(ScreenTransition((Sides)Random.Range(0, 4), rects[(int)newScreen], 0.5f));
            lastScreen = actualScreen;
            actualScreen = newScreen;
            if (onScreenChanged != null)
                onScreenChanged();
            if (newScreen == Screens.MainMenu)
                PlayerPrefs.Save();
        }
    }


    private IEnumerator ScreenTransition (Sides side, RectTransform newScreen, float duration)
    {
        runningCoroutine = true;
        AnimationCurve transition = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        Vector2 posOld = Vector2.zero, posNew = Vector2.zero;
        if (side == Sides.Left)
            posNew.x = Screen.width;
        else if (side == Sides.Right)
            posNew.x = -Screen.width;
        else if (side == Sides.Up)
            posNew.y = -Screen.height;
        else
            posNew.y = Screen.height;
        posOld = -posNew;

        newScreen.anchoredPosition = posNew;
        newScreen.gameObject.SetActive(true);

        for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
        {
            newScreen.anchoredPosition = Vector2.Lerp(posNew, Vector2.zero, transition.Evaluate (t));
            activeRect.anchoredPosition = Vector2.Lerp(Vector2.zero, posOld, transition.Evaluate(t));
            yield return null;
        }
        newScreen.anchoredPosition = Vector2.zero;
        activeRect.gameObject.SetActive(false);
        activeRect = newScreen;
        runningCoroutine = false;
    }
}
