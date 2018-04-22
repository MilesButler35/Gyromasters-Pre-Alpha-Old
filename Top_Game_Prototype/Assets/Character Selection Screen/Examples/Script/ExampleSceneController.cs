using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace CharacterSelectionScreen
{
    public class ExampleSceneController : MonoBehaviour
    {
        public static int coinsCollected = 700;
        public static int characterUnlocked = 0;
        public static int selectedCharacter = 0;

        public Transform player;
        public Text actualCharacterText;
        public enum SceneOrientation { Portrait, Landscape }
        public SceneOrientation sceneOrientation;


        private void Start()
        {
            // USEFUL TO MAKE SOME PROCESS AUTOMATIZED
            CharacterSelection.onNewCharacterSelected += UpdateActualCharacter;

            player = GameObject.FindWithTag("Player").transform;
            GameObject newCharacter = Instantiate(
                // GET THE PREFAB FROM THE SELECTED CHARACTER
                CharacterSelection.instance.GetSelectedCharacter().prefabGameplay,
                player.position,
                player.rotation) as GameObject;
            newCharacter.transform.parent = player;

            actualCharacterText.text =
                "Actual character: " + CharacterSelection.instance.characterSetup[selectedCharacter].name.ToString();

            StartCoroutine(AdaptScreenResolution());
        }


        public void Button_ChangeCharacter()
        {
            ScreenManager.instance.ChangeScreen(ScreenManager.Screens.CharacterSelection);
            // USE THIS TO INITIALIZE THE SCREEN
            CharacterSelection.instance.OnSelectionStarted();
        }


        private void UpdateActualCharacter()
        {
            Destroy(player.GetChild(0).gameObject);
            GameObject newCharacter = Instantiate(
                // GET THE PREFAB FROM THE SELECTED CHARACTER
                CharacterSelection.instance.GetSelectedCharacter().prefabGameplay,
                player.position,
                player.rotation) as GameObject;
            newCharacter.transform.parent = player;
            actualCharacterText.text =
                "Actual character: " + CharacterSelection.instance.characterSetup[selectedCharacter].name.ToString();
        }


        IEnumerator AdaptScreenResolution()
        {
            // I was having some wierd errors when trying to change the screen resolution in the inicialization of the game
            // Asking the engine to wait two frames solved the problem somehow
            yield return null;
            yield return null;
            Vector2 newRes = Vector2.zero;
            switch (sceneOrientation)
            {
                case SceneOrientation.Portrait:
                    newRes.y = Screen.currentResolution.height * 0.75f;
                    newRes.x = newRes.y * (9f / 16f);
                    break;
                case SceneOrientation.Landscape:
                    newRes.x = Screen.currentResolution.width * 0.75f;
                    newRes.y = newRes.x * (9f / 16f);
                    break;
            }
            Screen.SetResolution((int)newRes.x, (int)newRes.y, false);
        }


        private void Update()
        {
            if (Input.GetKeyDown (KeyCode.R))
            {
                switch (CharacterSelection.instance.rotate)
                {
                    case CharacterSelection.Rotate.OnlyHighlighted:
                        CharacterSelection.instance.rotate = CharacterSelection.Rotate.All;
                        break;
                    case CharacterSelection.Rotate.All:
                        CharacterSelection.instance.rotate = CharacterSelection.Rotate.OnlyHighlighted;
                        break;
                }
            }
        }


        public void Button_ChangeScene (int newSceneIndex)
        {
#if UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(newSceneIndex);
#else
            Application.LoadLevel(newSceneIndex);
#endif
        }


        private void OnDestroy()
        {
            CharacterSelection.onNewCharacterSelected -= UpdateActualCharacter;
        }
    }
}
