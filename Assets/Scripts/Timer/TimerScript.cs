using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    public float totalTime = 10f;
    public bool timerOn = false;

    [SerializeField] private PlayerManager playerManager;

    [SerializeField] TextMeshProUGUI timerText;
    public string nextSceneName = "BattleScene";

    public bool IsTimeUp => timerOn == false;

    void Start()
    {
        timerOn = true;
    }

    void Update()
    {
        if (!timerOn) return;

        if (totalTime > 0)
        {
            totalTime -= Time.deltaTime;
            timerText.text = GetTime();
        }
        else
        {
            totalTime = 0;
            timerOn = false;

            EndPhase();
        }
    }

    public void EndPhase()
    {
        if (playerManager != null)
        {
            playerManager.SaveTeamToGlobal();
        }

        SceneManager.LoadScene(nextSceneName);
    }

    string GetTime()
    {
        float minutes = Mathf.FloorToInt(totalTime / 60);
        float seconds = Mathf.FloorToInt(totalTime % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}
