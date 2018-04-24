using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour {

    float timer;
    bool loadingLevel;
    bool init;

    public int activeElement;
    public GameObject menuObj;
    public ButtonRef[] menuOptions;

    void Start()
    {

    }

	void Update () {
        if (!loadingLevel) //if not already loading the level
        {
            //indicate the selected option
            menuOptions[activeElement].selected = true;

            //change the selected option based on input
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                menuOptions[activeElement].selected = false;

                if (activeElement > 0)
                {

                    activeElement--;
                }
                else
                {
                    activeElement = menuOptions.Length - 1;
                }
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                menuOptions[activeElement].selected = false;

                if (activeElement < menuOptions.Length - 1)
                {
                    activeElement++;
                }
                else
                {
                    activeElement = 0;
                }
            }

            //and if we hit space again
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Submit"))
            {
                //then load the level
                Debug.Log("load");
                loadingLevel = true;
                StartCoroutine("LoadLevel");
                menuOptions[activeElement].transform.localScale *= 1.2f;
            }
        }
	}

    void HandleSelectedOption()
    {
         switch(activeElement)
        {
             case 0:
                CharacterManager.GetInstance().numberOfUsers = 1;
                break;
             case 1:
                CharacterManager.GetInstance().numberOfUsers = 2;
                CharacterManager.GetInstance().players[1].playerType = PlayerBase.PlayerType.user; 
                break;
        }
    }

    IEnumerator LoadLevel()
    {
        HandleSelectedOption();
        yield return new WaitForSeconds(0.6f);

        string nextScene = MySceneManager.GetInstance().mainScenes[2].levelId;

        MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, nextScene);

    }
}
