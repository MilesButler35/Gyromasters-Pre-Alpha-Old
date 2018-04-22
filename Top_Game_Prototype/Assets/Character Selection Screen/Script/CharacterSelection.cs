using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace CharacterSelectionScreen
{
    public class CharacterSelection : MonoBehaviour
    {
        // Since you probaly already have at least your coin counter in another script, 
        // I put these values right at the beginning so you can easily change them
        public int CoinsCollected
        {
            get { return ExampleSceneController.coinsCollected; }
            set { ExampleSceneController.coinsCollected = value; }
        }
        private int SelectedCharacter
        {
            get { return ExampleSceneController.selectedCharacter; }
            set { ExampleSceneController.selectedCharacter = value; }
        }
        private int UnlockProgression
        {
            get { return ExampleSceneController.characterUnlocked; }
            set { ExampleSceneController.characterUnlocked = value; }
        }
        // Yeah, I'm using an int instead of a bool array to save what is unlocked or not.
        // Basicaly I use one bit for each character, being 0 as locked and 1 as unlocked.
        // I did this to make it easier to use the PlayerPrefs.
        // As long as there are less than 32 characters it will work fine,
        // If you have more than 32 characters in your game, e-mail me and I'll write a version using a boll array instead

        
        // This is the event that will trigger when a new character is selected,
        // You can use this to update the character that is being shown in your scene
        public delegate void CharacterAction();
        public static event CharacterAction onNewCharacterSelected;


        //GLOBAL SETTINGS
        //
        // The bigger the number, the faster the camera will move when a finger/mouse is draged in the screen
        public float dragDeltaMultiplty = 1f;

        // How far should the character be from each other
        public float distanceBetweenCharacters = 5f;

        // Here all the prefab from the characters will be stored
        // Is also the reference position to where the characters will be shown in the scene
        public Transform prefabContainer;

        // How far from the "prefabContainer" should the Camera be
        public Vector3 cameraOffset;

        // The scale of the highlighted character
        public Vector3 highlightedScale;

        // The position of the highlighted character relative to its original position
        public Vector3 highlightedOffsetPosition;

        // Options to rotate the characters
        public enum Rotate { None, OnlyHighlighted, All }
        public Rotate rotate;

        // How many angles per second should the character rotate
        public Vector3 rotationSpeed;

        // Used in the "OnlyHighlighted" option
        // How fast should the character returns to its original rotation after another one was selected
        public float returnSpeed = 5f;


        //UI
        //
        // Take a look at the template for reference
        public GameObject selectButton;
        public GameObject buyButton;
        public GameObject confirmPurchaseButton;
        public Text priceText;
        public Text nameText;
        public Text descriptionText;
        public Text confirmPurchaseText;
        public Text coinsText;
        public RectTransform confirmPurchaseScreen;
        public int descriptionTextSize = 5;


        //SETUP CLASS
        // Class that store info about each character
        //
        public CharacterSetup[] characterSetup = new CharacterSetup[1];
        [System.Serializable]
        public class CharacterSetup
        {
            public GameObject prefabGameplay;
            public GameObject preFabScreen;
            public string name;
            public string description;
            public int price; // If the price is set as 0 or less the character will be considered unlocked
        }


        //PUBLIC VARIABLES
        //
        // Is the player in the "Comfirm Purchase" screen
        public bool isConfirmingPurchase = false;

        // List that contains only the characters that are visible.
        // Used to avoid unnecessary processing
        public static List<Transform> visibleCharacters = new List<Transform>();


        //PRIVATE VARIABLES
        //
        // The index of the currently highlited character
        private int highlightedCharacterIndex = 0;

        // The State of the camera before entering in the character selection screen
        private Vector3 previousCameraPosition;
        private Quaternion previousCameraRotation;


        // INSTANCE and initialization
        //
        public static CharacterSelection instance;
        private Transform myCamera;
        void Awake ()
        {
            InitializeCharacters();
            if (Camera.main != null)
            myCamera = Camera.main.transform;
            else
                Debug.LogWarning("(Character Selection Screen) There is no Camera tagged as \"Main Camera\", please tag one");

            enabled = false;
        }
        // Just referencing the instance
        CharacterSelection()
        {
            instance = this;
        }


        private void Update()
        {
            RotateCharacters();
            CheckArrowInput();
        }


        //////////////
        // EDITABLE //
        //////////////
        private IEnumerator SelectionStartedCoroutine()
        {
            // What you can edit here are the lines where I use my fade and animation script
            yield return Fade.instance.StartFade(Color.black, 0.2f);


            // DO NOT EDIT ANYTHING FROM HERE
            previousCameraPosition = myCamera.position;
            previousCameraRotation = myCamera.rotation;
            myCamera.position = prefabContainer.position + cameraOffset;
            myCamera.position += Vector3.right * distanceBetweenCharacters * SelectedCharacter;
            myCamera.rotation = Quaternion.identity;
            prefabContainer.gameObject.SetActive(true);
            highlightedCharacterIndex = SelectedCharacter;
            UpdateUI();
            yield return new WaitForEndOfFrame();
            UpdateCharacterState();
            // TO HERE


            confirmPurchaseScreen.GetComponent<UIAnimation>().PlayAnimation(0);
            yield return Fade.instance.StartFade(Color.clear, 0.2f);
        }


        //////////////
        // EDITABLE //
        //////////////
        public IEnumerator ReturnCoroutine ()
        {
            // Same as before, just edit the lines related to my fade script
            // If you end up not using any "yield", just put "return null" at the end to avoid compiling errors
            yield return Fade.instance.StartFade(Color.black, 0.2f);


            prefabContainer.gameObject.SetActive(false);
            myCamera.position = previousCameraPosition;
            myCamera.rotation = previousCameraRotation;
            if (onNewCharacterSelected != null)
                onNewCharacterSelected();


            yield return Fade.instance.StartFade(Color.clear, 0.2f);


            enabled = false;
        }


        // Here I Instantiate all the character and put them in line
        private void InitializeCharacters ()
        {
            if (prefabContainer == null)
            {
                Debug.LogWarning("(Character Selection Screen) Creating a prefab container because it was let as null in the script");
                prefabContainer = new GameObject("Character Prefab Container").transform;
            }
            GameObject newCharacter;
            for (int i = 0; i < characterSetup.Length; i++)
            {
                newCharacter = Instantiate(characterSetup[i].preFabScreen) as GameObject;
                newCharacter.transform.parent = prefabContainer;
                newCharacter.transform.localPosition = Vector3.right * distanceBetweenCharacters * i;
                // This is the script that will adding and removing the characters from the "visibleCharacters" list.
                // This won't change anything in the prefab
                newCharacter.AddComponent<VisibilityTrigger>();
            }
            prefabContainer.gameObject.SetActive(false);
        }


        /// <summary>
        /// MUST be called when the game is about to show this screen
        /// </summary>
        public void OnSelectionStarted ()
        {
            enabled = true;
            StartCoroutine(SelectionStartedCoroutine()); 
        }


        /// <summary>
        /// Called by the "Event Trigger" component of the "Touch Zone" GameObject, do NOT call this from another script
        /// </summary>
        /// <param name="baseData"></param>
        public void OnDrag(BaseEventData baseData)
        {
            PointerEventData pointerData = baseData as PointerEventData;

            // Getting the next position the camera is supposed to be
            Vector3 newPosition = myCamera.position + (Vector3.left * (pointerData.delta.x / Screen.width) * dragDeltaMultiplty);

            // Making sure the camera isn't out of bounds
            if (newPosition.x < prefabContainer.GetChild(0).position.x - distanceBetweenCharacters + cameraOffset.x)
                newPosition.x = prefabContainer.GetChild(0).position.x - distanceBetweenCharacters + cameraOffset.x;
            else if (newPosition.x > prefabContainer.GetChild(prefabContainer.childCount - 1).position.x + distanceBetweenCharacters + cameraOffset.x)
                newPosition.x = prefabContainer.GetChild(prefabContainer.childCount - 1).position.x + distanceBetweenCharacters + cameraOffset.x;

            // Setting the new position
            myCamera.position = newPosition;

            // Checking if the player moved enough to qualify the previous/next character as the highlighted one
            if (newPosition.x < 
                ((prefabContainer.GetChild(highlightedCharacterIndex).position.x - (distanceBetweenCharacters / 2f) + cameraOffset.x)) && 
                highlightedCharacterIndex > 0)
            {
                highlightedCharacterIndex--;
                UpdateUI();
            }
            else if (newPosition.x > 
                ((prefabContainer.GetChild(highlightedCharacterIndex).position.x + (distanceBetweenCharacters / 2f) + cameraOffset.x)) && 
                highlightedCharacterIndex < (prefabContainer.childCount - 1))
            {
                highlightedCharacterIndex++;
                UpdateUI();
            }

            UpdateCharacterState();
        }


        /// <summary>
        /// Called by the "Event Trigger" component of the "Touch Zone" GameObject, do NOT call this from another script
        /// </summary>
        /// <param name="baseData"></param>
        public void OnPointerUp(BaseEventData baseData)
        {
            StartCoroutine(CenterHighlightedCharacter(0.1f));
        }


        // Used to apply the "Highlighted State" options
        private void UpdateCharacterState ()
        {
            // Used to cache data
            Vector3 positionTemp;
            float distance = 0f;

            // Here I limit to update only the ones that are visible to avoid unnecessary processing
            foreach (Transform character in visibleCharacters)
            {
                // Here I take the original position of each character
                positionTemp = Vector3.right * distanceBetweenCharacters * character.GetSiblingIndex();

                // Now I need to know the distance between the camera and the character
                // to apply those "Highlighted State" options
                distance = (myCamera.position.x - cameraOffset.x) - character.position.x;

                // Making sure its always positive
                if (distance < 0f)
                    distance *= -1f;

                // Since the Lerp only work between 0 and 1 I need to divide this
                distance /= distanceBetweenCharacters;

                // Now I just Lerp the position and Scale between the default and the highlighted State
                character.localPosition = Vector3.Lerp(positionTemp + highlightedOffsetPosition, positionTemp, distance);
                character.localScale = Vector3.Lerp(highlightedScale, Vector3.one, distance);
            }
        }


        // Called when the highlighted character is changed to update all the UI info about the new character
        private void UpdateUI ()
        {
            coinsText.text = CoinsCollected.ToString();
            nameText.text = characterSetup[highlightedCharacterIndex].name;
            descriptionText.text = characterSetup[highlightedCharacterIndex].description + "\n";
            descriptionText.rectTransform.anchoredPosition = Vector2.zero;
            descriptionText.fontSize = descriptionTextSize * (Screen.height / 100);

            // If the character is already bought we show the "Select" button
            if (IsHighlightedCharacterUnlocked())
            {
                selectButton.SetActive(true);
                buyButton.SetActive(false);
            }
            // If not we show the "Buy" button
            else
            {
                selectButton.SetActive(false);
                buyButton.SetActive(true);
                priceText.text = characterSetup[highlightedCharacterIndex].price.ToString();
            }
        }


        // Used to make the camera center at the highlighted character
        private IEnumerator CenterHighlightedCharacter (float duration)
        {
            // Caching data
            Vector3 initialCameraPosition = myCamera.position;
            Vector3 finalCameraPosition = initialCameraPosition;
            finalCameraPosition.x = prefabContainer.GetChild(highlightedCharacterIndex).position.x + cameraOffset.x;

            // Smoothly lerping the camera
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                myCamera.position = Vector3.Lerp(initialCameraPosition, finalCameraPosition, t);
                UpdateCharacterState();
                yield return null;
            }
            myCamera.position = finalCameraPosition;
            UpdateCharacterState();
        }


        // Rotate the characters based on the selected option
        private void RotateCharacters()
        {
            if (rotate == Rotate.All)
            {
                foreach (Transform character in visibleCharacters)
                {
                    character.localEulerAngles = rotationSpeed * Time.time;
                }
            }

            else if (rotate == Rotate.OnlyHighlighted)
            {
                Transform highlighted = prefabContainer.GetChild(highlightedCharacterIndex);
                foreach (Transform character in visibleCharacters)
                {
                    if (character == highlighted)
                        character.Rotate(rotationSpeed * Time.deltaTime);
                    else
                        character.rotation = Quaternion.Slerp(character.rotation, Quaternion.identity, Time.deltaTime * returnSpeed);
                }
            }
        }


        // Change the highlighted character based on the keyboard input
        private void CheckArrowInput ()
        {
            if (Input.GetKeyDown (KeyCode.LeftArrow) && highlightedCharacterIndex > 0 && !isConfirmingPurchase)
            {
                highlightedCharacterIndex--;
                UpdateUI();
                StartCoroutine(CenterHighlightedCharacter(0.2f));
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && highlightedCharacterIndex < prefabContainer.childCount - 1 && !isConfirmingPurchase)
            {
                highlightedCharacterIndex++;
                UpdateUI();
                StartCoroutine(CenterHighlightedCharacter(0.2f));
            }
        }


        /// <summary>
        /// Get the info from the actual SELECTED character
        /// </summary>
        /// <returns>Return the info from the actual SELECTED character</returns>
        public CharacterSetup GetSelectedCharacter()
        {
            return characterSetup[SelectedCharacter];
        }


        /// <summary>
        /// Get the info from the actual HIGHLIGHTED character
        /// </summary>
        /// <returns>Return the info from the actual HIGHLIGHTED character</returns>
        public CharacterSetup GetHighlightedCharacter()
        {
            return characterSetup[highlightedCharacterIndex];
        }


        /// <summary>
        /// Check if there is enough coins to purchase the highlighted character
        /// </summary>
        /// <returns>Returns true if there is enough coins to complete the purchase</returns>
        public bool IsPurchasePossible ()
        {
            return CoinsCollected >= characterSetup[highlightedCharacterIndex].price ? true : false;
        }


        /// <summary>
        /// Check if the actual highlighted character is already unlocked
        /// </summary>
        /// <returns>Returns true if the actual highlighted character is already unlocked</returns>
        public bool IsHighlightedCharacterUnlocked ()
        {
            return (BoolCasting.IntBitToBool(UnlockProgression, highlightedCharacterIndex) ||
                characterSetup[highlightedCharacterIndex].price <= 0) ? true : false;
        }


        /// <summary>
        /// Intended to be used by the "Buttons" script only
        /// </summary>
        public void OnPurchaseConfirmed()
        {
            CoinsCollected -= characterSetup[highlightedCharacterIndex].price;
            UnlockProgression = BoolCasting.BoolToIntBit(UnlockProgression, highlightedCharacterIndex, true);
            isConfirmingPurchase = false;
            UpdateUI();
        }


        /// <summary>
        /// Intended to be used by the "Buttons" script only
        /// </summary>
        public void OnNewCharacterSelected()
        {
            SelectedCharacter = highlightedCharacterIndex;
            StartCoroutine(ReturnCoroutine());
        }
    }
}
