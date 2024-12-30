using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.UI
{
    public class SplashImageUI : MonoBehaviour
    {
        [Scene]
        [SerializeField] private string menuScene;

        private AsyncOperation loadSceneOperation;


        private void Start()
        {
            loadSceneOperation = SceneManager.LoadSceneAsync(menuScene);
        }

        public float GetProgress()
        {
            if (loadSceneOperation != null)
                return loadSceneOperation.progress;

            return 0.0f;
        }
    }
}