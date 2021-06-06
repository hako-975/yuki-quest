using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAsync(PlayerPrefsManager.instance.GetNextScene()));
    }

    IEnumerator LoadAsync(string nextScene)
    {
        AsyncOperation sync = SceneManager.LoadSceneAsync(nextScene);
        
        if (sync == null)
        {
            SceneManager.LoadScene("Main Menu");
        }
        else
        {
            while (!sync.isDone)
            {
                float progress = Mathf.Clamp01(sync.progress);
                slider.value = progress;

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
