using UnityEngine;

namespace CharacterSelectionScreen
{
    public class VisibilityTrigger : MonoBehaviour
    {
        private void OnBecameVisible()
        {
            CharacterSelection.visibleCharacters.Add(transform);
        }

        private void OnBecameInvisible()
        {
            CharacterSelection.visibleCharacters.Remove(transform);
        }
    }
}
