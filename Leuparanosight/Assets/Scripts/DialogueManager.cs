using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Settings")]
    public float fadeSpeed = 2f;
    public float displayTime = 2f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartDialogue(DialogueData data)
    {
        StopAllCoroutines();
        StartCoroutine(RunDialogue(data));
    }

    private IEnumerator RunDialogue(DialogueData data)
    {
        dialoguePanel.SetActive(true);

        CanvasGroup cg = dialoguePanel.GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = 0f;

        foreach (string sentence in data.sentences)
        {
            yield return StartCoroutine(FadeInSentence(sentence));
            yield return new WaitForSeconds(displayTime);
        }

        // Fade-out ตอนจบ (optional)
        if (cg != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                cg.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
        }

        dialoguePanel.SetActive(false);
    }

    private IEnumerator FadeInSentence(string sentence)
    {
        dialogueText.text = sentence;

        CanvasGroup cg = dialoguePanel.GetComponent<CanvasGroup>();
        if (cg == null)
            yield break;

        cg.alpha = 0f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            cg.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
    }


}
