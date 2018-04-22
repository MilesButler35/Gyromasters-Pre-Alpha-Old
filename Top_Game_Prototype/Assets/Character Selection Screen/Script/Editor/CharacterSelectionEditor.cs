using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelectionScreen
{
    [CustomEditor (typeof (CharacterSelection))]
    public class CharacterSelectionEditor : Editor
    {
        private string[] menuNames = new string[] { "UI", "Global", "Specific" };
        private enum Menus {UI, Global, Specific}
        private Menus actualMenu = Menus.Global;
        private int actualCharacterIndex = 0;

        public override void OnInspectorGUI()
        {
            CharacterSelection t = (CharacterSelection)target;
            EditorGUILayout.Space();
            actualMenu = (Menus)GUILayout.SelectionGrid((int)actualMenu, menuNames, 3);

            switch (actualMenu)
            {
                //
                //
                // UI
                //
                //
                case Menus.UI:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Main Screen");
                    EditorGUIUtility.labelWidth = 150;
                    t.coinsText = EditorGUILayout.ObjectField("Coins Text", t.coinsText, typeof(Text), true) as Text;
                    t.nameText = EditorGUILayout.ObjectField("Name Text", t.nameText, typeof(Text), true) as Text;
                    t.descriptionText = EditorGUILayout.ObjectField("Description Text", t.descriptionText, typeof(Text), true) as Text;
                    t.descriptionTextSize = EditorGUILayout.IntField("Description Text Size", t.descriptionTextSize);
                    t.selectButton = EditorGUILayout.ObjectField("Select Button", t.selectButton, typeof(GameObject), true) as GameObject;
                    t.buyButton = EditorGUILayout.ObjectField("Buy Button", t.buyButton, typeof(GameObject), true) as GameObject;
                    t.priceText = EditorGUILayout.ObjectField("Price Text", t.priceText, typeof(Text), true) as Text;

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Confirm Purchase Screen");
                    t.confirmPurchaseScreen = EditorGUILayout.ObjectField("Screen Rect", t.confirmPurchaseScreen, typeof(RectTransform), true) as RectTransform;
                    t.confirmPurchaseText = EditorGUILayout.ObjectField("Confirm Text", t.confirmPurchaseText, typeof(Text), true) as Text;
                    t.confirmPurchaseButton = EditorGUILayout.ObjectField("Buy Button", t.confirmPurchaseButton, typeof(GameObject), true) as GameObject;
                    break;
                //
                //
                // GLOBAL
                //
                //
                case Menus.Global:
                    EditorGUILayout.Space();
                    EditorGUIUtility.labelWidth = 107;
                    t.prefabContainer = EditorGUILayout.ObjectField(new GUIContent(
                        "Prefab Container", 
                        "Here all the prefab from the characters will be stored. Is also the reference position to where the characters will be shown in the scene"), 
                        t.prefabContainer, typeof(Transform), true) as Transform;

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(
                        "Camera Offset", 
                        "How far from the characters should the Camera be"), 
                        GUILayout.MaxWidth(90));
                    t.cameraOffset = EditorGUILayout.Vector3Field("", t.cameraOffset, GUILayout.MinWidth(10));
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUIUtility.labelWidth = 175;
                    t.dragDeltaMultiplty = EditorGUILayout.FloatField(new GUIContent(
                        "Drag Multiply", 
                        "The bigger the number, the faster the camera will move when a finger/mouse is draged in the screen"), 
                        t.dragDeltaMultiplty);

                    t.distanceBetweenCharacters = EditorGUILayout.FloatField(new GUIContent (
                        "Distance Between Characters", 
                        "How far should the character be from each other"), 
                        t.distanceBetweenCharacters);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Highlighted State");
                    EditorGUIUtility.labelWidth = 10;

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(
                        "Scale",
                        "The scale of the highlighted character"), 
                        GUILayout.MaxWidth(50));
                    t.highlightedScale = EditorGUILayout.Vector3Field("", t.highlightedScale);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(
                        "Offset",
                        "The position of the highlighted character relative to its original position"), 
                        GUILayout.MaxWidth(50));
                    t.highlightedOffsetPosition = EditorGUILayout.Vector3Field("", t.highlightedOffsetPosition);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUIUtility.labelWidth = 50;
                    t.rotate = (CharacterSelection.Rotate)EditorGUILayout.EnumPopup("Rotate", t.rotate, GUILayout.MaxWidth(175));
                    if (t.rotate != CharacterSelection.Rotate.None)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent(
                            "Speed", 
                            "How many angles per second should the character rotate"), 
                            GUILayout.MaxWidth(50));
                        EditorGUIUtility.labelWidth = 0;
                        t.rotationSpeed = EditorGUILayout.Vector3Field("", t.rotationSpeed);
                        EditorGUILayout.EndHorizontal();
                    }
                    if (t.rotate == CharacterSelection.Rotate.OnlyHighlighted)
                    {
                        EditorGUIUtility.labelWidth = 100;
                        t.returnSpeed = EditorGUILayout.FloatField(new GUIContent(
                            "Return Speed", 
                            "How fast should the character returns to its original rotation after another one was selected"), 
                            t.returnSpeed, GUILayout.MaxWidth(150));
                    }
                    break;
                    //
                    //
                    // SPECIFIC
                    //
                    //
                case Menus.Specific:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField((actualCharacterIndex + 1).ToString() + " / " + t.characterSetup.Length.ToString());

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("<"))
                    {
                        if (actualCharacterIndex > 0)
                            actualCharacterIndex--;
                    }
                    if (GUILayout.Button(">"))
                    {
                        if (actualCharacterIndex < t.characterSetup.Length - 1)
                            actualCharacterIndex++;
                    }
                    if (GUILayout.Button("+"))
                    {
                        System.Array.Resize(ref t.characterSetup, t.characterSetup.Length + 1);
                        t.characterSetup[t.characterSetup.Length - 1] = new CharacterSelection.CharacterSetup();
                        actualCharacterIndex = t.characterSetup.Length - 1;
                    }
                    if (GUILayout.Button("-"))
                    {
                        if (t.characterSetup.Length > 1)
                            System.Array.Resize(ref t.characterSetup, t.characterSetup.Length - 1);
                        if (actualCharacterIndex > t.characterSetup.Length - 1)
                            actualCharacterIndex = t.characterSetup.Length - 1;
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUIUtility.labelWidth = 115;
                    t.characterSetup[actualCharacterIndex].preFabScreen = EditorGUILayout.ObjectField("Prerab (Screen)",
                        t.characterSetup[actualCharacterIndex].preFabScreen, typeof(GameObject), true) as GameObject;

                    EditorGUIUtility.labelWidth = 115;
                    t.characterSetup[actualCharacterIndex].prefabGameplay = EditorGUILayout.ObjectField("Prerab (Gameplay)",
                        t.characterSetup[actualCharacterIndex].prefabGameplay, typeof(GameObject), true) as GameObject;

                    EditorGUIUtility.labelWidth = 50;
                    t.characterSetup[actualCharacterIndex].name = EditorGUILayout.TextField("Name",
                        t.characterSetup[actualCharacterIndex].name);

                    EditorGUIUtility.labelWidth = 50;
                    t.characterSetup[actualCharacterIndex].price = 
                        EditorGUILayout.IntField(new GUIContent(
                            "Price", 
                            "If the price is set as 0 or less the character will be considered unlocked"), 
                            t.characterSetup[actualCharacterIndex].price, GUILayout.MaxWidth(115));

                    

                    EditorGUILayout.LabelField("Description");
                    EditorStyles.textField.wordWrap = true;
                    t.characterSetup[actualCharacterIndex].description = EditorGUILayout.TextArea(
                        t.characterSetup[actualCharacterIndex].description, GUILayout.MinHeight(250));
                    break;
            }

            Undo.RecordObject(t, "Character Selection");
            EditorUtility.SetDirty(t);
        }
    }
}
