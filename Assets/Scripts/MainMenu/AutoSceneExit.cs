using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneExit : MonoBehaviour
{
    [SerializeField] private float delay = 4f;
    [SerializeField] private string nextSceneName = "RecruitmentPhase";

    private void OnEnable()
    {
        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        Debug.Log($"[AutoSceneExit] Waiting {delay}s realtime...");
        yield return new WaitForSecondsRealtime(delay);

        Debug.Log($"[AutoSceneExit] Loading scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}