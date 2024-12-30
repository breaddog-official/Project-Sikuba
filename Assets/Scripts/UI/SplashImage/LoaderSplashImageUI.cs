using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoaderSplashImageUI : MonoBehaviour
{
    [SerializeField] private SplashImageUI splashImage;
    [SerializeField] private Image loadingImage;

    private void Update()
    {
        if (splashImage == null || loadingImage == null)
            return;

        loadingImage.fillAmount = splashImage.GetProgress();
    }
}
