using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private readonly Queue<Unit> recruitQueue = new Queue<Unit>();
    private readonly List<UnitType> recruitedTypes = new List<UnitType>();

    private Unit currentTarget;

    public int maxRecruits;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float stopDistance; // stop area
    [SerializeField] private RecruitController recruitController;
    [SerializeField] private KingWalkAnimator kingAnim;

    public event Action<int> OnRecruitedCountChanged;

    public int RecruitedCount => recruitedTypes.Count;
    private void Start()
    {
        if (PlayerArmyScript.Instance != null)
        {
            maxRecruits = PlayerArmyScript.Instance.GetTeamSize();
        }

        OnRecruitedCountChanged?.Invoke(RecruitedCount);
    }

    void Update()
    {
        if (currentTarget == null && recruitQueue.Count > 0)
        {
            currentTarget = recruitQueue.Dequeue();
        }
        
        bool isMoving = false;

        if (currentTarget != null)
        {
            Vector2 current = transform.position;
            Vector2 next = currentTarget.transform.position;

            transform.position = Vector2.MoveTowards(
                current, next, moveSpeed * Time.deltaTime);
            isMoving = true;

            if (Vector2.Distance(transform.position, next) <= stopDistance)
            {
                recruitController.RecruitUnit(currentTarget);

                recruitedTypes.Add(currentTarget.Type);

                OnRecruitedCountChanged?.Invoke(RecruitedCount);

                currentTarget = null;
            }
        }
        kingAnim.SetMoving(isMoving); // say for kingAnim that kins is moving
    }

    public void EnqueueTarget(Unit unit)
    {
        if (unit == null) return;
        if (unit.isRecruited) return;
        if (currentTarget == unit) return;
        if (recruitQueue.Contains(unit)) return;

        recruitQueue.Enqueue(unit);
    }

    public void SaveTeamToGlobal()
    {
        PlayerArmyScript.Instance.SetTeam(recruitedTypes);
        Debug.Log("[PlayerManager] Saved TYPES: " + recruitedTypes.Count);
    }
}