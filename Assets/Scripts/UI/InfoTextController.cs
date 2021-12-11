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
    [HideInInspector]
    public event System.EventHandler neededLinesChanged;
    [HideInInspector]
    public bool hideAfterDisplayEnd = true;

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

    public int TypeText(string text, float hideDelay)
    {
        StopTyping();
        var info = textMesh.GetTextInfo(text);
        for (int i = 0; i < info.lineCount - 1; i++)
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
        int lines = textMesh.GetTextInfo(textToType).lineCount;
        textMesh.text = "";
        return lines;
    }

    public void StopTyping()
    {
        blinkCoroutine.StopIfRunning(this);
        typeCoroutine.StopIfRunning(this);
        if (typing)
        {
            OnTypingEnd();
        }
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
            yield return new WaitForSeconds(letterTypeDuration);
        }
        textMesh.text = textToType;
        typing = false;
        yield return new WaitForSeconds(letterTypeDuration);
        yield return new WaitForSeconds(endDelay);
        OnTypingEnd();
    }

    private IEnumerator CurosrBlinkCoroutine()
    {
        while (typing)
        {
            if (cursorState)
            {
                yield return new WaitForSeconds(cursorVisibleTime);
            }
            else
            {
                yield return new WaitForSeconds(cursorInvisibleTime);
            }
            cursorState = !cursorState;
            UpdateText();
        }
        cursorState = false;
        UpdateText();
    }

    private void UpdateText()
    {
        string text = typedText;
        textMesh.text = text;
        var info = textMesh.GetTextInfo(text);
        float x = info.lineInfo[info.lineCount - 1].length;
        float y = -info.lineInfo[0].lineHeight * info.lineCount;
        cursor.transform.localPosition = new Vector2(x, y) + offset;
        cursor.SetActive(cursorState);
    }

    private void OnTypingEnd()
    {
        if (hideAfterDisplayEnd)
        {
            textMesh.enabled = false;
        }
        if (displayEndEvent != null)
        {
            displayEndEvent(this, null);
        }
    }
}
