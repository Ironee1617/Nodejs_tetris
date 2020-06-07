using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            if (!_instance)
                _instance = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;
            return _instance;
        }
    }

    public delegate void GameStartHandler();
    public event GameStartHandler handler; 

    private SocketIOComponent socket;
    private TetManager tetManager;
    private NetworkClient n_Client;
    private ChattingManager chatManager;
    
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        tetManager = GetComponent<TetManager>();
        n_Client = GetComponent<NetworkClient>();
        chatManager = GetComponent<ChattingManager>();

        StartCoroutine(ConnectToServer());
        socket.On("user_connected", OnUserConnected);
        socket.On("play_game", OnUserPlay);
        socket.On("other_map_display", OnOtherMapDisplay);
        socket.On("gameover", OnClientInformGameOver);
        socket.On("receive_message", OnReceiveMessage);
    }

    public void JoinGame()
    {
        StartCoroutine(ConnectToServer());
    }

    IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);

        socket.Emit("user_connect");
    }

    public void Ready()
    {
        socket.Emit("ready", new JSONObject(1));
    }

    public void ReadyCancel()
    {
        socket.Emit("ready_cancel", new JSONObject(-1));
    }

    public void OnUserMapDisplay()
    {
        JSONObject dataArr = new JSONObject(JSONObject.Type.ARRAY);
        JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
        data.AddField("value", dataArr);

        for (int i = 0; i < tetManager.map.GetLength(0); ++i)
        {
            for (int j = 0; j < tetManager.map.GetLength(1); ++j)
            {
                if (tetManager.map[i, j] != null)
                    dataArr.Add("true"); 
                else
                    dataArr.Add("false");
            }
        }

        socket.Emit("display", data);
    }

    private void OnUserConnected(SocketIOEvent evt)
    {
        Debug.Log("from server msg " + evt.data.Print());
    }

    private void OnUserPlay(SocketIOEvent evt)
    {
        handler();
    }

    private void OnOtherMapDisplay(SocketIOEvent evt)
    {
        string data = evt.data.Print();
        string[] arr = data.Split(new char[] { ',' });
        string t = "true";
        string f = "false";
        for(int i = 0; i < arr.Length; ++i)
        {
            if (arr[i].Contains(t))
                arr[i] = "true";
            else if (arr[i].Contains(f))
                arr[i] = "false";
        }

        n_Client.OtherMapDisplay(arr);
    }

    public void InformGameOver()
    {
        socket.Emit("gameover");
    }

    private void OnClientInformGameOver(SocketIOEvent evt)
    {
        tetManager.gameOver = true;
        UIManager.Instance.DisplayGameoverText("Win", new Color(0, 0, 1, 1));
    }

    public void SendMessageToServer(string _message)
    {
        JSONObject data = new JSONObject(JSONObject.Type.STRING);
        data.AddField("msg", _message);
        socket.Emit("send_message", data);
    }

    private void OnReceiveMessage(SocketIOEvent evt)
    {

        JSONObject obj = evt.data.list[0];
        string msg = obj.Print().Replace("\"","");
        
        chatManager.ReceiveMessage(msg);
    }
}
