using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTextController : MonoBehaviour
{
    [SerializeField]
    private string cursor;
    [SerializeField]
    private float letterTypeDuration;
    [SerializeField]
    private float cursorVisibleTime;
    [SerializeField]
    private float cursorInvisibleTime;
    [HideInInspector]
    public event System.EventHandler displayEndEvent;

    private TMPro.TextMeshProUGUI textMesh;
    private string textToType;
    private System.CharEnumerator enumerator;
    private string typedText;
    private bool cursorState;
    private bool typing;
    private float endDelay;
    private bool paused;
    private (Coroutine type, Coroutine blink) coroutines;

    private void Awake()
    {
        textMesh = GetComponent<TMPro.TextMeshProUGUI>();
        paused = false;
        EventManager.Instance.AddListener("Unpause", ContinueTyping);
    }

    public void TypeText(string text, float hideDelay)
    {
        StopTyping();
        textToType = text;
        enumerator = textToType.GetEnumerator();
        textMesh.text = cursor;
        typedText = "";
        typing = true;
        cursorState = true;
        textMesh.enabled = true;
        endDelay = hideDelay;
        coroutines.type = StartCoroutine(TypeTextCoroutine());
        coroutines.blink = StartCoroutine(CurosrBlinkCoroutine());
    }

    public void StopTyping()
    {
        if(!typing)
        {
            return;
        }
        if (coroutines.blink != null)
        {
            StopCoroutine(coroutines.blink);
        }
        if (coroutines.type != null)
        {
            StopCoroutine(coroutines.type);
        }
        OnTypingEnd();
    }

    private void ContinueTyping()
    {
        if (typing)
        {
            if (coroutines.blink != null)
            {
                StopCoroutine(coroutines.blink);
            }
            if (coroutines.type != null)
            {
                StopCoroutine(coroutines.type);
            }
            coroutines.type = StartCoroutine(TypeTextCoroutine());
            coroutines.blink = StartCoroutine(CurosrBlinkCoroutine());
        }
    }

    private IEnumerator TypeTextCoroutine()
    {
        while (enumerator.MoveNext())
        {
            typedText += enumerator.Current;
            UpdateText();
            yield return new WaitForSecondsRealtime(letterTypeDuration);
        }
        textMesh.text = textToType;
        typing = false;
        yield return new WaitForSecondsRealtime(letterTypeDuration);
        yield return new WaitForSecondsRealtime(endDelay);
        OnTypingEnd();
    }

    private IEnumerator CurosrBlinkCoroutine()
    {
        while (typing)
        {
            if (cursorState)
            {
                yield return new WaitForSecondsRealtime(cursorVisibleTime);
            }
            else
            {
                yield return new WaitForSecondsRealtime(cursorInvisibleTime);
            }
            cursorState = !cursorState;
            UpdateText();
        }
        cursorState = false;
        UpdateText();
    }

    private void UpdateText()
    {
        textMesh.text = typedText + (cursorState ? cursor : "");
    }

    private void OnTypingEnd()
    {
        textMesh.enabled = false;
        if (displayEndEvent != null)
        {
            displayEndEvent(this, null);
        }
    }
}
