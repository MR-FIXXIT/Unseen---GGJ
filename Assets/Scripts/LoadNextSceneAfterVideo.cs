using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LoadNextSceneAfterVideo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;           // Drag your VideoPlayer here (or auto-find below)

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "Level1";   // Must match exactly a scene in Build Settings
    // OR use build index (safer against typos/renames):
    // [SerializeField] private int nextSceneBuildIndex = 1;

    [Header("Options")]
    [SerializeField] private bool allowSkipWithKey = true;      // Press any key to skip video
    [SerializeField] private float extraDelayAfterEnd = 0.5f;   // Optional small buffer before loading

    private bool hasTransitioned = false;

    void Awake()
    {
        // Auto-find if not assigned in Inspector
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer == null)
        {
            Debug.LogError("No VideoPlayer assigned or found on this GameObject!", this);
            LoadNextScene(); // fallback — load anyway
            return;
        }

        // Make sure looping is OFF (important!)
        videoPlayer.isLooping = false;
    }

    void Start()
    {
        // Subscribe to the end-of-video event
        videoPlayer.loopPointReached += OnVideoEnd;

        // Optional: if video is not set to Play on Awake, start it now
        if (!videoPlayer.playOnAwake)
        {
            videoPlayer.Play();
        }
    }

    void Update()
    {
        // Optional skip feature
        // if (allowSkipWithKey && Input.anyKeyDown && !hasTransitioned)
        // {
        //     Debug.Log("Video skipped by player input");
        //     LoadNextScene();
        // }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (hasTransitioned) return;
        hasTransitioned = true;

        Debug.Log("Video reached end → starting scene transition");

        if (extraDelayAfterEnd > 0f)
        {
            Invoke(nameof(LoadNextScene), extraDelayAfterEnd);
        }
        else
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        hasTransitioned = true; // prevent double calls

        // Clean up event (good practice)
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        // else if (nextSceneBuildIndex >= 0)
        // {
        //     SceneManager.LoadScene(nextSceneBuildIndex);
        // }
        else
        {
            Debug.LogError("No next scene name or index defined!");
        }
    }

    void OnDestroy()
    {
        // Safety cleanup
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}