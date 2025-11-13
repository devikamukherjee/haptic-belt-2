using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ContinueButton : MonoBehaviour
{
    [Tooltip("Reference to the gamemanager in the scene")]
    public gamemanager gameManager;

    [Tooltip("Tag that the hand colliders have (e.g. 'Hand')")]
    public string handTag = "Hand";

    [Tooltip("Delay between allowed presses (seconds)")]
    public float debounceSeconds = 0.5f;

    private float lastPressTime = -10f;

    [Header("Optional Feedback")]
    public Animator animator;
    public AudioSource audioSource;

    [Tooltip("Animation name to return to after press (e.g. 'RoundComplete_Idle')")]
    public string idleAnimationName = "RoundComplete_Idle";

    [Tooltip("Time to wait before resetting (seconds)")]
    public float resetDelay = 1.0f;

    private void Reset()
    {
        // Ensure collider works as trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastPressTime < debounceSeconds) return;

        if (other.CompareTag(handTag))
        {
            lastPressTime = Time.time;

            // Play feedback animation and sound
            if (animator != null)
                animator.SetTrigger("Pressed");

            if (audioSource != null)
                audioSource.Play();

            // Trigger continue in GameManager
            if (gameManager != null)
            {
                gameManager.TriggerContinue();
            }
            else
            {
                Debug.LogWarning("ContinueButton: No GameManager assigned!");
            }

            // ✅ Start reset coroutine
            if (animator != null && !string.IsNullOrEmpty(idleAnimationName))
                StartCoroutine(ResetAfterDelay());
        }
    }

    private System.Collections.IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);
        animator.Play(idleAnimationName);
        Debug.Log("Button reset to idle animation");
    }
}
