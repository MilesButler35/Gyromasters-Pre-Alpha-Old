using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public int numberOfPlayers = 1;
 
    List<PotraitInfo> potraitList = new List<PotraitInfo>();

    public GameObject potraitCanvas; // the canvas that holds all the potraits

    bool loadLevel; //if we are loading the level  
    public bool bothPlayersSelected;

    CharacterManager charManager;

    GameObject potraitPrefab;

    MySceneManager sceneManager;

    #region Singleton
    public static LoadingManager instance;
    public static LoadingManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }
    #endregion
    private void Start()
    {
        charManager = CharacterManager.GetInstance();
        numberOfPlayers = charManager.numberOfUsers;

       
        sceneManager = gameObject.GetComponent<MySceneManager>();

        charManager.solo = (numberOfPlayers == 1);
       

    }

    private void Update()
    {
         if(bothPlayersSelected)
        {
            Debug.Log("loading");
            StartCoroutine("LoadLevel"); //and start the coroutine to load the level
            loadLevel = true;
            bothPlayersSelected = false;
        }
        else
        {
            if(charManager.players[0].hasCharacter 
                && charManager.players[1].hasCharacter)
            {
                bothPlayersSelected = true;
            }
           
        }
    }
    IEnumerator LoadLevel()
    {

        //if any of the players is an AI, then assign a random character to the prefab
        for (int i = 0; i < charManager.players.Count; i++)
        {
            if (charManager.players[i].playerType == PlayerBase.PlayerType.ai)
            {
                if (charManager.players[i].playerPrefab == null)
                {
                    int ranValue = Random.Range(0, potraitList.Count);

                    charManager.players[i].playerPrefab =
                        charManager.returnCharacterWithID(potraitList[ranValue].characterId).prefab;

                    charManager.players[i].playerPrefab.GetComponent<TopmanPlayerController>().enabled = false;
                    charManager.players[i].playerPrefab.GetComponent<TopmanBarrier>().enabled = false;
                    charManager.players[i].playerPrefab.GetComponent<TopmanDive>().enabled = false;
                    charManager.players[i].playerPrefab.GetComponent<TopmanRush>().enabled = false;
                    charManager.players[i].playerPrefab.GetComponent<AIManager>().enabled = true;
                    charManager.players[i].playerPrefab.GetComponent<AIBarrier>().enabled = true;
                    charManager.players[i].playerPrefab.GetComponent<AIDive>().enabled = true;
                    charManager.players[i].playerPrefab.GetComponent<AIRush>().enabled = true;

                    Debug.Log(potraitList[ranValue].characterId);
                }

            }
            else
            {

                charManager.players[i].playerPrefab.GetComponent<TopmanPlayerController>().enabled = true;
                charManager.players[i].playerPrefab.GetComponent<TopmanBarrier>().enabled = true;
                charManager.players[i].playerPrefab.GetComponent<TopmanDive>().enabled = true;
                charManager.players[i].playerPrefab.GetComponent<TopmanRush>().enabled = true;
                charManager.players[i].playerPrefab.GetComponent<AIManager>().enabled = false;
                charManager.players[i].playerPrefab.GetComponent<AIBarrier>().enabled = false;
                charManager.players[i].playerPrefab.GetComponent<AIDive>().enabled = false;
                charManager.players[i].playerPrefab.GetComponent<AIRush>().enabled = false;
            }
        }

        yield return new WaitForSeconds(2);//after 2 seconds load the level

        if (charManager.solo)
        {
       
            MySceneManager.GetInstance().LoadNextOnProgression();
        }
        else
        {
            MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "WinScreen");
        }


        WinState();

    }

    public void WinState()
    {
        if (charManager.solo)
        {
            sceneManager.progIndex += 1;
            sceneManager.progressionBase += 1;
        }

        if (sceneManager.progressionBase == 2 && charManager.solo)
        {
            MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "WinScreen");
        }
    }
}
