using UnityEngine;
using UnityEditor.SceneManagement;

public class WinScreen : MonoBehaviour {

    public void restart()
    {
        EditorSceneManager.LoadScene(3);
    }
    public void select()
    {
        EditorSceneManager.LoadScene(1);
    }
    public void title()
    {
        EditorSceneManager.LoadScene(0);
    }
}

