using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        bool pause = GameRules.WinCharge > 0f || GameRules.DeathCharge > 0f || GameRules.Paused;
        if (pause && audioSource.isPlaying) {
            audioSource.Stop();
        }
        if (!pause && !audioSource.isPlaying) {
            audioSource.Play();
        }
    }
}
