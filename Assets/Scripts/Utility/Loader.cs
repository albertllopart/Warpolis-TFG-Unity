using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour //aquesta classe serveix per iniciar una co-rutina per carregar una escena amb LoadAsync
    {

    }

    public enum Scene
    {
        intro, loading, title, main_menu, game, gym
    };

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene scene)
    {
        //set the loader callback action to load the target scene
        onLoaderCallback = () =>
        {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };

        //load loading scene
        SceneManager.LoadScene(Scene.loading.ToString());
    }

    public static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null; //deixarà passar un frame abans d'executar el codi de sota

        Application.backgroundLoadingPriority = ThreadPriority.Low;
        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAsyncOperation.isDone)
        {
            yield return null; //esperem fins al proper frame
        }

        //quan surti del while carregarà l'escena
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
            return loadingAsyncOperation.progress;

        return 1f;
    }

    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
