using TMPro;
using UnityEngine;

public class TeamCountScript : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TextMeshProUGUI recruitedText;

    private void Start()
    {
        UpdateText(playerManager.RecruitedCount);

        playerManager.OnRecruitedCountChanged += UpdateText;
    }

    private void OnDestroy()
    {
        if (playerManager != null)
            playerManager.OnRecruitedCountChanged -= UpdateText;
    }

    private void UpdateText(int count)
    {
        recruitedText.text = $"Team: {count}";
    }
}