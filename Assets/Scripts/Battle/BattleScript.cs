using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum BattleResult
{
    Draw,
    PlayerWins,
    EnemyWins
}

public class BattleScript : MonoBehaviour
{
    [SerializeField] private DiceSystem diceSystem;
    [SerializeField] private PlayerArmyShowcase playerArmyShowcase;
    [SerializeField] private EnemyArmyShowcase enemyArmyShowcase;
    [SerializeField] private TMP_Text scoreText;

    [Header("Result UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject gazeFollow;
    [SerializeField] private Image diceKingPlaceholder;
    [SerializeField] private Sprite enemyKing;
    [SerializeField] private Sprite playerKing;

    [Header("Time settings")]
    [SerializeField] private BattleCloud battleCloudPrefab;
    [SerializeField] private float meetMoveDuration = 1f;   // how long units move to meet point
    [SerializeField] private float beforeFightPause = 0.2f; // small pause after reaching meet point
    [SerializeField] private float fightDuration = 1.0f;    // how long the "fight" lasts while cloud loops
    [SerializeField] private float afterDuelPause = 0.3f;   // pause before next duel

    private List<GameObject> playerTeam = new List<GameObject>();
    private List<GameObject> enemyTeam = new List<GameObject>();

    private bool battleStarted = false;

    private int playerScore = 0;

    private void Start()
    {
        resultPanel.SetActive(false);
        gazeFollow.SetActive(false);

        // get teams from showcases
        playerTeam = playerArmyShowcase.GetArmyList();
        enemyTeam = enemyArmyShowcase.GetArmyList();

        Debug.Log($"[BattleScript] Player team count: {playerTeam?.Count ?? 0}");
        Debug.Log($"[BattleScript] Enemy  team count: {enemyTeam?.Count ?? 0}");

        if (playerTeam == null || enemyTeam == null)
        {
            Debug.LogError("[BattleScript] One of the teams is null!");
            return;
        }

        scoreText.text = $"Battle Score : {playerScore}";

        // rerun battle if not started
        if (!battleStarted)
        {
            battleStarted = true;
            StartCoroutine(RunBattle());
        }
    }

    private IEnumerator RunBattle()
    {
        int pairCount = Mathf.Min(playerTeam.Count, enemyTeam.Count);

        for (int i = 0; i < pairCount; i++)
        {
            GameObject playerObj = playerTeam[i];
            GameObject enemyObj = enemyTeam[i];

            // if the player is missing a unit, penalty
            if (playerObj == null && enemyObj != null)
            {
                Debug.Log($"[Duel {i}] Player is missing a unit → -2 penalty!");
                playerScore -= 2;
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // if both dont have units, skip
            if (playerObj == null && enemyObj == null)
            {
                continue;
            }

            // play duel
            yield return StartCoroutine(Duel(playerObj, enemyObj));
        }

        Debug.Log($"=== FINAL SCORE: {playerScore} ===");
        ShowResult(playerScore);
    }

    private void ShowResult(int finalScoreValue)
    {
        if (resultPanel == null || resultText == null)
        {
            Debug.LogError("Result UI not assigned!");
            return;
        }

        resultPanel.SetActive(true);
        gazeFollow.SetActive(true);
        
        if(finalScoreValue > 0)
            diceKingPlaceholder.sprite = playerKing;
        else
            diceKingPlaceholder.sprite = enemyKing;

        if (finalScoreValue > 0)
        {
            resultText.text = "<color=green>YOUR ARMY WON</color>";
        }
        else
        {
            resultText.text = "<color=red>ENEMY ARMY WON</color>";
        }
    }

    private IEnumerator Duel(GameObject playerObj, GameObject enemyObj)
    {
        if (playerObj == null || enemyObj == null)
            yield break;

        // Cache start positions
        Vector3 startP = playerObj.transform.position;
        Vector3 startE = enemyObj.transform.position;

        // Compute ONE shared meet point (midpoint between start positions)
        Vector3 meetPoint = (startP + startE) * 0.5f;

        // Small separation so units don't overlap visually at the same pixel
        float meetSeparation = 0.6f; // tweak in Inspector later if needed
        Vector3 dir = (startP - startE).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.up;

        Vector3 meetPointP = meetPoint + dir * (meetSeparation * 0.5f);
        Vector3 meetPointE = meetPoint - dir * (meetSeparation * 0.5f);

        // Move both units to meet points
        float t = 0f;
        float dur = Mathf.Max(0.0001f, meetMoveDuration);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            float k = Mathf.Clamp01(t);

            if (playerObj != null)
                playerObj.transform.position = Vector3.Lerp(startP, meetPointP, k);

            if (enemyObj != null)
                enemyObj.transform.position = Vector3.Lerp(startE, meetPointE, k);

            yield return null;
        }

        // Snap exactly
        if (playerObj != null) playerObj.transform.position = meetPointP;
        if (enemyObj != null) enemyObj.transform.position = meetPointE;

        yield return new WaitForSeconds(beforeFightPause);

        // ---- Get Unit components (must exist) ----
        Unit pUnit = playerObj.GetComponent<Unit>();
        Unit eUnit = enemyObj.GetComponent<Unit>();

        if (pUnit == null || eUnit == null)
        {
            Debug.LogWarning("[Duel] Missing Unit component.");
            yield return new WaitForSeconds(afterDuelPause);
            yield break;
        }

        // 1) Base result from types
        BattleResult baseResult = Resolve(pUnit.Type, eUnit.Type);

        // 2) Fail chance depends on base result
        float failChance = 50f;
        switch (baseResult)
        {
            case BattleResult.PlayerWins: failChance = 30f; break;
            case BattleResult.EnemyWins: failChance = 70f; break;
            case BattleResult.Draw: failChance = 60f; break;
        }

        // 3) Spawn cloud and roll dice одновременно
        BattleCloud cloud = null;

        int roll = 0;
        bool isFail = false; // TRUE = player loses (fails)

        Coroutine cloudRoutine = null;

        if (battleCloudPrefab != null)
        {
            cloud = Instantiate(battleCloudPrefab, meetPoint, Quaternion.identity);
            cloudRoutine = StartCoroutine(cloud.PlayIntroAndEnterLoop());
        }

        // IMPORTANT: set chance BEFORE rolling
        diceSystem.SetFailChance(failChance);

        // Roll dice and wait until animation finished
        yield return diceSystem.RollAndWait((r, fail) =>
        {
            roll = r;
            isFail = fail;
        });

        // Optional: если хочешь дождаться окончания intro cloud (обычно да)
        if (cloudRoutine != null)
            yield return cloudRoutine;

        // (опционально) подождать "файт" фазу, если тебе нужно
        if (fightDuration > 0f)
            yield return new WaitForSeconds(fightDuration);

        Debug.Log($"[Duel] BaseResult={baseResult} | FailChance={failChance}% | roll={roll} | playerFailed={isFail}");

        // 4) Final outcome from dice
        bool playerActuallyWins = !isFail;

        if (playerActuallyWins)
        {
            playerScore += 1;
            scoreText.text = $"Battle Score : <color=green>{playerScore}</color>";
            Debug.Log($"[Duel] PLAYER wins (dice) | {pUnit.Type} vs {eUnit.Type} | roll={roll} failChance={failChance}%");
            Destroy(enemyObj);
        }
        else
        {
            playerScore -= 1;
            scoreText.text = $"Battle Score : <color=red>{playerScore}</color>";
            Debug.Log($"[Duel] ENEMY wins (dice) | {eUnit.Type} vs {pUnit.Type} | roll={roll} failChance={failChance}%");
            Destroy(playerObj);
        }

        // Always play outro
        if (cloud != null)
            yield return cloud.PlayOutroAndDestroy();

        yield return new WaitForSeconds(afterDuelPause);
    }

    public static BattleResult Resolve(UnitType player, UnitType enemy)
    {
        if (player == enemy)
            return BattleResult.Draw;

        switch (player)
        {
            case UnitType.Lance:
                if (enemy == UnitType.Shield) return BattleResult.PlayerWins;
                if (enemy == UnitType.Archer) return BattleResult.EnemyWins;
                break;

            case UnitType.Archer:
                if (enemy == UnitType.Lance) return BattleResult.PlayerWins;
                if (enemy == UnitType.Shield) return BattleResult.EnemyWins;
                break;

            case UnitType.Shield:
                if (enemy == UnitType.Archer) return BattleResult.PlayerWins;
                if (enemy == UnitType.Lance) return BattleResult.EnemyWins;
                break;
        }

        return BattleResult.Draw;
    }
}
