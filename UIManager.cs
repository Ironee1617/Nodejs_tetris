using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (!_instance)
                _instance = FindObjectOfType(typeof(UIManager)) as UIManager;
            return _instance;
        }
    }
    public Text text;
    public Button readyBtn;

    private TetManager tetManager;

    private void Start()
    {
        tetManager = GetComponent<TetManager>();
        NetworkManager.Instance.handler += GameStart; 
    }

    public void DisplayGameoverText(string _text, Color _color)
    {
        text.gameObject.SetActive(true);
        text.text = _text;
        text.color = _color;
    }

    public void ReadyBtn()
    {
        if (tetManager.ready == false)
        {
            tetManager.ready = true;
            NetworkManager.Instance.Ready();
            readyBtn.transform.GetComponentInChildren<Text>().text = "Wait...";
        }
        else
        {
            tetManager.ready = false;
            NetworkManager.Instance.ReadyCancel();
            readyBtn.transform.GetComponentInChildren<Text>().text = "Ready";
        }
    }

    void GameStart()
    {
        readyBtn.gameObject.SetActive(false);
        readyBtn.transform.GetComponentInChildren<Text>().text = "Go";
    }
}
