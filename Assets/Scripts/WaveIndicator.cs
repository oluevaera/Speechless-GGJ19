using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WaveIndicator : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.StopPlayback();
    }

    private void OnWaveComplete()
    {
        animator.StartPlayback();
    }
}
