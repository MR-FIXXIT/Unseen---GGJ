    using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonLoadScene : MonoBehaviour
{
    [Tooltip("Scene name as in Build Settings OR the exact scene path. Prefer name.")]
    [SerializeField] private string sceneToLoad;

    // Hook this to the Button's OnClick() in the Inspector
    public void LoadSelectedScene()
    {
        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogWarning("UIButtonLoadScene: sceneToLoad is empty.", this);
            return;
        }

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    // Optional: set scene name at runtime then load
    public void SetSceneAndLoad(string sceneName)
    {
        sceneToLoad = sceneName;
        LoadSelectedScene();
    }
}