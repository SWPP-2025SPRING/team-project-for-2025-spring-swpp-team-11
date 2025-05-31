using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1.5f;
    
    private IEnumerator FadeOut()
    {
        float alpha = 0;
        fadeImage.color = new Color(0, 0, 0, 0);

        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
    }

    private IEnumerator FadeIn()
    {
        float alpha = 1;
        fadeImage.color = new Color(0, 0, 0, 1);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
    }

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut());

        // Load the new scene
        SceneManager.LoadScene(sceneName);
        Debug.Log("ASDG");

        StartCoroutine(FadeIn());
    }
}
