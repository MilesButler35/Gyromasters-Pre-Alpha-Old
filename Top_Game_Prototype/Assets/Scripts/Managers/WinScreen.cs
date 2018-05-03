using UnityEngine;
using UnityEditor.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetButton("Submit"))
        {
            restart();
        }

        else if (Input.GetButton("Cancel"))
        {
            select();
        }
    }

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

