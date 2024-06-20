using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }
    public AudioSource audioSource;
    public AudioClip shotgunSound;
    public AudioClip sniperSound;
    public AudioClip uziSound;
    public AudioClip coltSound;
    public AudioClip ak47Sound;
    public AudioClip reloadingSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
