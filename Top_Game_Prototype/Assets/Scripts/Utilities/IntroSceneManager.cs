using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour {

    float timer;
    bool loadingLevel;
    bool init;
    int maxCollumn;

    public int activeY;
    public bool hitInputOnce;
    public float timerToReset;
    public int activeElement;
    public GameObject menuObj;
    public ButtonRef[] menuOptions;

    private float moveVertical;
    private string v_MovementAxisName;

    void Start()
    {
        v_MovementAxisName = "Vertical1";
        maxCollumn = menuOptions.Length;
    }

	void Update () {
        if (!loadingLevel) //if not already loading the level
        {
            //indicate the selected option
            menuOptions[activeElement].selected = true;

            moveVertical = Input.GetAxis(v_MovementAxisName);

            //change the selected option based on input
            HandleSelectScreenInput();
        }
	}

    void HandleSelectScreenInput()
    {
        float vertical = Input.GetAxis("Vertical1");

        if (vertical != 0)
        {
            if (!hitInputOnce)
            {
                if (vertical > 0)
                {
                    activeY = (activeY > 0) ? activeY - 1 : maxCollumn;
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
                else
                {
                    activeY = (activeY < maxCollumn) ? activeY + 1 : 0;
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

                hitInputOnce = true;
            }
        }

        if (vertical == 0)
        {
            hitInputOnce = false;
        }

        if (hitInputOnce)
        {
            timerToReset += Time.deltaTime;

            if (timerToReset > 0.8f)
            {
                hitInputOnce = false;
                timerToReset = 0;
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


    void HandleSelectedOption()
    {
         switch(activeElement)
        {
             case 0:
                CharacterManager.GetInstance().numberOfUsers = 1;
                CharacterManager.GetInstance().players[1].playerType = PlayerBase.PlayerType.ai;
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
