using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickEvent : MonoBehaviour
{
    public void SceneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GameExit()
    {
#if UNIYT_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#elif UNIYT_ANDROID
    Application.Quit();
#endif
    }
}
