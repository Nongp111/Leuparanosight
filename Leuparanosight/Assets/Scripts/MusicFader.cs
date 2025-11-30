using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFader : MonoBehaviour
{
    private AudioSource audioSource;
    public float fadeDuration = 2f;
    public bool playOnStart = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (playOnStart)
            StartCoroutine(FadeInCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        audioSource.volume = 0f;
        if (!audioSource.isPlaying)
            audioSource.Play();

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 0.5f, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0.5f;
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
