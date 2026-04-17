using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadRecruitmentPhase()
    {
        Debug.Log("[SceneLoader] Loading RecruitmentPhase");
        SceneManager.LoadScene("RecruitmentPhase");
    }

    public void LoadEnemyArmyShowcase()
    {
        //Debug.Log("[SceneLoader] Loading EnemyArmyShowcase");
        //SceneManager.LoadScene("EnemyArmyShowcase");
        PlayAgain();
    }

    public void LoadBattlePhase()
    {
        Debug.Log("[SceneLoader] Loading BattlePhase");
        SceneManager.LoadScene("BattlePhase");
    }

    public void PlayAgain()
    {
        if (EnemyArmyScript.Instance != null)
            EnemyArmyScript.Instance.ResetAndGenerate();

        if (PlayerArmyScript.Instance != null)
            PlayerArmyScript.Instance.ResetTeam();

        SceneManager.LoadScene("EnemyArmyShowcase");
    }

    public void MainMenu()
    {
        Debug.Log("[SceneLoader] Loading MainMenu");
        SceneManager.LoadScene("MainMenu");
    }
}