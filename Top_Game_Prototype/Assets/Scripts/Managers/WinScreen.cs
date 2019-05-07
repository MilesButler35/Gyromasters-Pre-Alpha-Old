using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    private MySceneManager sceneManager;

    private void Start()
    {
        sceneManager = gameObject.GetComponent<MySceneManager>();
    }
   /* public void Update()
    {
        if (Input.GetButton("Submit"))
        {
            select();
            

        }

        else if (Input.GetButton("Cancel"))
        {
            title();
        }
    }*/

  /*  public void restart()
    {
        EditorSceneManager.LoadScene(3);
        sceneManager.progressionBase = 0;
    }*/
    public void select()
    {
        SceneManager.LoadScene(2);
        sceneManager.progressionBase = 0;
    }
    public void title()
    {
        SceneManager.LoadScene(0);
        sceneManager.progressionBase = 0;
    }

    public void CharSelect()
    {
        SceneManager.LoadScene(3);
        sceneManager.progressionBase = 0;
    }
    public void StageSelect()
    {
        SceneManager.LoadScene(1);
        
    }


}

