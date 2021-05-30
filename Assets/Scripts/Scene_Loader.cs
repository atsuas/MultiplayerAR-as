using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Loader : Singleton<Scene_Loader>
{
    private string sceneNameToBeLoaded;

    public void LoadScene(string _sceneName)
    {
        sceneNameToBeLoaded = _sceneName;

        StartCoroutine(InitializeSceneLoading());
    }

    IEnumerator InitializeSceneLoading()
    {
        //まず、Loadingシーンを読み込む
        yield return SceneManager.LoadSceneAsync("Scene_Loading");

        //実際のシーンを読み込む
        StartCoroutine(LoadActualyScene());

    }

    IEnumerator LoadActualyScene()
    {
        var asyncSceneLoading = SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

        //この値は、ロード中のシーンの表示を停止する
        asyncSceneLoading.allowSceneActivation = false;

        while (!asyncSceneLoading.isDone)
        {
            Debug.Log(asyncSceneLoading.progress);

            if (asyncSceneLoading.progress >= 0.9f)
            {
                //最後にシーンを表示する
                asyncSceneLoading.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    
}
