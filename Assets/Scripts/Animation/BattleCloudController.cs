using System.Collections;
using UnityEngine;

public class BattleCloud : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Animator state names (must match exactly the state names in Animator)
    private static readonly int StartBattle = Animator.StringToHash("StartBattle");
    private static readonly int EndBattle   = Animator.StringToHash("EndBattle");

    private static readonly int StateIntro = Animator.StringToHash("Cloud_intro");
    private static readonly int StateLoop  = Animator.StringToHash("Cloud_loop");
    private static readonly int StateOutro = Animator.StringToHash("Cloud_outro");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public IEnumerator PlayIntroAndEnterLoop()
    {
        gameObject.SetActive(true);

        animator.ResetTrigger(EndBattle);
        animator.SetTrigger(StartBattle);

        // Wait until we actually enter Intro
        yield return WaitUntilState(StateIntro);

        // Then wait until we reach Loop (Intro finished and transitioned)
        yield return WaitUntilState(StateLoop);
    }

    public IEnumerator PlayOutroAndDestroy()
    {
        animator.ResetTrigger(StartBattle);
        animator.SetTrigger(EndBattle);

        // Wait until we actually enter Outro
        yield return WaitUntilState(StateOutro);

        // Wait until Outro clip finishes (normalizedTime >= 1)
        while (true)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            if (st.shortNameHash == StateOutro && st.normalizedTime >= 1f)
                break;

            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator WaitUntilState(int stateHash)
    {
        // Safety timeout to avoid infinite loops if Animator is misconfigured
        float timeout = 2f;
        float t = 0f;

        while (t < timeout)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            if (st.shortNameHash == stateHash)
                yield break;

            t += Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning($"[BattleCloud] Timeout waiting for state {stateHash}. Check Animator transitions/states.");
    }
}
