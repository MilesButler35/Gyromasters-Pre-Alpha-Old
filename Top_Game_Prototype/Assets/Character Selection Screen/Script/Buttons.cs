using UnityEngine;

namespace CharacterSelectionScreen
{
    public class Buttons : MonoBehaviour
    {
        // This is the script that will handle everthing related to the buttons in the screen
        // I put these methods in a different script to make it easier to edit in case you want to adapt it to your own liking (effects)

        // I haven't used this anywhere, but, just in case...
        public static Buttons instance = null;
        private void Awake() { instance = this; }

        // A reference to the main script, where all the info and logic are stored
        private CharacterSelection mainScript;
        void Start()
        {
            mainScript = GetComponent<CharacterSelection>();
            if (ScreenManager.instance == null)
                Debug.LogWarning("(Character Selection Screen) Screen manager not found in the scene, check the documentation for more info (null reference exceptions might occur until fixed)");
        }


        // Called on the "Cancel" button from the "Confirm Purchase" screen.
        // Also called when the escape key is pressed.
        public void Button_Return()
        {
            // If the player was confirming the purchase...
            if (mainScript.isConfirmingPurchase)
            {
                mainScript.isConfirmingPurchase = false;

                // Here I am using my animation script to smoothly hide the "Confirm purchase" screen
                // but you can edit it to your own way
                mainScript.confirmPurchaseScreen.GetComponent<UIAnimation>().PlayAnimation("Out");
            }
            // If the player was selecting a character
            else
            {
                // Again, here I had to write a specific logic to make it work in the Example scene
                // but you can edit it to fit in your project
                mainScript.confirmPurchaseScreen.GetComponent<UIAnimation>().PlayAnimation(0);
                ScreenManager.instance.ChangeScreen(ScreenManager.Screens.MainMenu);

                // Don't edit this line
                StartCoroutine(mainScript.ReturnCoroutine());
            }
        }


        // Called on the "Buy" button (the one that shows the price in the screen)
        public void Button_Buy()
        {
            // Just to confirm that we are in the right screen when the button was pressed
            if (!mainScript.isConfirmingPurchase)
            {
                mainScript.isConfirmingPurchase = true;

                // Getting info about the character that the player wants to buy
                CharacterSelection.CharacterSetup selectedCharacter = mainScript.GetHighlightedCharacter();

                // If the player have enough coins
                if (mainScript.IsPurchasePossible())
                {
                    // Set the text asking if the player wants to confirm the purchase
                    mainScript.confirmPurchaseText.text =
                        "Confirm the purchase of " +
                        selectedCharacter.name +
                        " for " +
                        selectedCharacter.price.ToString() +
                        " coins?";
                }

                // If the player doesn't have enough coins, 
                // Set the text to tell how much coins is need to be able to buy the selected character
                else
                {
                    mainScript.confirmPurchaseText.text = "You need more " +
                        (selectedCharacter.price - mainScript.CoinsCollected).ToString() +
                        " coins to be able to buy " +
                        selectedCharacter.name;
                }
                // Olny show the "Confirm Purchase" button if the player have enough coins
                mainScript.confirmPurchaseButton.SetActive(mainScript.CoinsCollected >= selectedCharacter.price);


                // Here I am using my animation script to smoothly show the "Confirm purchase" screen
                // but you can edit it to your own way
                mainScript.confirmPurchaseScreen.GetComponent<UIAnimation>().PlayAnimation("In");
            }
        }


        // Called on the "Buy" button that is in the "Confirm Purchase" screen
        public void Button_ConfirmPurchase()
        {
            mainScript.OnPurchaseConfirmed();


            // Here I'm removing the "Confirm Purchase" screen using my animation script, change if needed
            mainScript.confirmPurchaseScreen.GetComponent<UIAnimation>().PlayAnimation("Out");
        }


        // Called on the "Select" button
        public void Button_Select()
        {
            // You can change this to your own screemanager logic, 
            // Or use my script. Check it, I wrote a little tutorial on how to setup it.
            ScreenManager.instance.ChangeScreen(ScreenManager.Screens.MainMenu);

            mainScript.OnNewCharacterSelected();
        }


        // Just a simple logic to make it work with the keyboard
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)
                && ScreenManager.instance.actualScreen == ScreenManager.Screens.CharacterSelection)
            {
                if (!mainScript.isConfirmingPurchase)
                {
                    if (mainScript.IsHighlightedCharacterUnlocked())
                        Button_Select();
                    else
                        Button_Buy();
                }
                else if (mainScript.IsPurchasePossible())
                    Button_ConfirmPurchase();
            }

            if (Input.GetKeyDown(KeyCode.Escape) && ScreenManager.instance.actualScreen == ScreenManager.Screens.CharacterSelection)
                Button_Return();
        }
    }
}
