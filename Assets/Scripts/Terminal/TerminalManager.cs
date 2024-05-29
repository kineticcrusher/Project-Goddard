using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public TMP_InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect scrollRect;
    public GameObject messageList;

    TerminalInterpreter interpreter;

    private void Start()
    {
        interpreter = GetComponent<TerminalInterpreter>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            scrollRect.gameObject.SetActive(!scrollRect.gameObject.activeSelf);
            if (scrollRect.gameObject.activeSelf)
            {
                terminalInput.ActivateInputField();
                terminalInput.Select();
            }
        }
    }

    private void OnGUI()
    {
        if(terminalInput.isFocused && !string.IsNullOrEmpty(terminalInput.text) && Input.GetKeyDown(KeyCode.Return))
        {
            var input = terminalInput.text;

            ClearInputField();

            AddDirectoryLine(input);
            var lines = AddInterpreterLines(interpreter.Interpret(input));

            ScrollToBottom(lines);

            userInputLine.transform.SetAsLastSibling();
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    private void ScrollToBottom(int lines)
    {
        if(lines > 4)
        {
            scrollRect.velocity = new Vector2(0, 450);
        }
        else
        {
            scrollRect.verticalNormalizedPosition = 0;
        }
    }

    private void AddDirectoryLine(string input)
    {
        Vector2 messageListSize = messageList.GetComponent<RectTransform>().sizeDelta;

        messageList.GetComponent<RectTransform>().sizeDelta = new Vector2(messageListSize.x, messageListSize.y + 35.0f);

        GameObject message = Instantiate(directoryLine, messageList.transform);
        message.transform.SetSiblingIndex(messageList.transform.childCount - 1);
        message.GetComponentsInChildren<TMP_Text>()[1].text = input;
    }

    private void ClearInputField()
    {
        terminalInput.text = string.Empty;
    }

    private int AddInterpreterLines(List<string> interpreterLines)
    {
        foreach (var line in interpreterLines)
        {
            GameObject res = Instantiate(responseLine, messageList.transform);
            res.transform.SetAsLastSibling();
            Vector2 messageListSize = messageList.GetComponent<RectTransform>().sizeDelta;
            messageList.GetComponent<RectTransform>().sizeDelta = new Vector2(messageListSize.x, messageListSize.y + 35.0f);
            res.GetComponentInChildren<TMP_Text>().text = line;
        }

        return interpreterLines.Count;
    }
}
