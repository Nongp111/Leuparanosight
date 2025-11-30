using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 2f;

    private Coroutine currentMessage;

    public void ShowMessage(string text)
    {
        if (currentMessage != null)
            StopCoroutine(currentMessage);

        currentMessage = StartCoroutine(DisplayMessage(text));
    }

    private IEnumerator DisplayMessage(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        messageText.gameObject.SetActive(false);
    }
}
