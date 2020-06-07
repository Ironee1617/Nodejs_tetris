using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChattingManager : MonoBehaviour
{
    public InputField inputText;
    public Text[] displayText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SendMessageBtn();
    }

    public void SendMessageBtn()
    {
        if (inputText.text == "")
            return;
        SendToChatWindow(inputText.text);
    }

    void SendToChatWindow(string msg)
    {
        for(int i = displayText.Length - 1; i > 0; --i)
        {
            displayText[i].text = displayText[i - 1].text;
        }
        displayText[0].text = " 나 : " + msg;
        inputText.text = string.Empty;
        NetworkManager.Instance.SendMessageToServer(msg);
    }

    public void ReceiveMessage(string msg)
    {
        for (int i = displayText.Length - 1; i > 0; --i)
        {
            displayText[i].text = displayText[i - 1].text;
        }
        displayText[0].text = " 낯선 상대 : " + msg;
        inputText.text = string.Empty;
    }


}
