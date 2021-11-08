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
    private CoroutineWrapper blinkCoroutine;
    private CoroutineWrapper typeCoroutine;
    private GameObject cursor;

    private void Awake()
    {
        textMesh = GetComponent<TMPro.TextMeshProUGUI>();
        EventManager.Instance.AddListener("Unpause", ContinueTyping);
        cursor = Instantiate(cursorPrefab, transform);
        cursor.SetActive(false);
        blinkCoroutine = new CoroutineWrapper(() => CurosrBlinkCoroutine());
        typeCoroutine = new CoroutineWrapper(() => TypeTextCoroutine());
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
        blinkCoroutine.Run(this);
        typeCoroutine.Run(this);
    }

    public void StopTyping()
    {
        if (!typing)
        {
            return;
        }
        blinkCoroutine.StopIfRunning(this);
        typeCoroutine.StopIfRunning(this);
        OnTypingEnd();
    }

    private void ContinueTyping()
    {
        if (typing)
        {
            blinkCoroutine.Reset(this);
            typeCoroutine.Reset(this);
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
