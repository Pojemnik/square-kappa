using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTextController : MonoBehaviour
{
    [Header("Cursor settings")]
    [SerializeField]
    private float cursorVisibleTime;
    [SerializeField]
    private float cursorInvisibleTime;
    [SerializeField]
    private GameObject cursorPrefab;
    [SerializeField]
    private Vector2 offset;

    [Space]
    [SerializeField]
    private float letterTypeDuration;
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
    private GameObject cursor;

    private void Awake()
    {
        textMesh = GetComponent<TMPro.TextMeshProUGUI>();
        paused = false;
        EventManager.Instance.AddListener("Unpause", ContinueTyping);
        cursor = Instantiate(cursorPrefab, transform);
        cursor.SetActive(false);
    }

    public void TypeText(string text, float hideDelay)
    {
        StopTyping();
        var info = textMesh.GetTextInfo(text);
        for (int i = 0; i < info.lineCount; i++)
        {
            TMPro.TMP_LineInfo line = info.lineInfo[i];
            text = text.Remove(line.lastCharacterIndex, 1).Insert(line.lastCharacterIndex, "\n");
        }
        textToType = text;
        enumerator = textToType.GetEnumerator();
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
        if (!typing)
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
        string text = typedText;// + (cursorState ? cursorString : "");
        textMesh.text = text;
        var info = textMesh.GetTextInfo(text);
        float x = info.lineInfo[info.lineCount - 1].length;
        float y = -info.lineInfo[0].lineHeight * info.lineCount;
        cursor.transform.localPosition = new Vector2(x, y) + offset;
        cursor.SetActive(cursorState);
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
