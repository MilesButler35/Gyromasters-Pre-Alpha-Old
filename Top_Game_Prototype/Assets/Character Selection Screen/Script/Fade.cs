using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CharacterSelectionScreen
{
    public class Fade : MonoBehaviour
    {
        private Image fadeImage;
        public static Fade instance = null;
        void Awake ()
        {
            instance = this;
            fadeImage = GetComponent<Image>();
        }


        public IEnumerator StartFade(Color desiredColor, float duration)
        {
            fadeImage.enabled = true;
            Color initialColor = fadeImage.color;
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                fadeImage.color = Color.Lerp(initialColor, desiredColor, t);
                yield return null;
            }
            fadeImage.color = desiredColor;
            if (desiredColor == Color.clear)
                fadeImage.enabled = false;
        }
    }
}
