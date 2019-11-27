using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyBackgroundNoise : MonoBehaviour
{
    [SerializeField] private Vector3 origin;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Enemy.enemies.Count == 0)
        {
            audioSource.volume = 0f;
            return;
        }
        audioSource.volume = 1f;

        // Calculate panning of murmur
        var pan = 0f;
        foreach (var enemy in Enemy.enemies)
        {
            if (Camera.main.WorldToViewportPoint(enemy.transform.position).x < 0.5f) pan -= .5f;
            else pan += .5f;
        }
        audioSource.panStereo = pan;
        
        //Sound depending on enemy count
        if (Enemy.enemies.Count < 5)
        {
            audioSource.volume = 0.6f;
        }
        else if (Enemy.enemies.Count >= 5 && Enemy.enemies.Count < 8)
        {
            audioSource.volume = 0.8f;
        }
        else if (Enemy.enemies.Count >= 8)
        {
            audioSource.volume = 1.0f;
        }
    }
}
