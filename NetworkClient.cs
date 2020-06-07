using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkClient : MonoBehaviour
{
    public Transform[,] otherClientMap = new Transform[TetManager.width, TetManager.height];
    public GameObject tilePrefab;

    public void OtherMapDisplay(string[] _strArr)
    {
        int a = 0;
        for(int i = 0; i < otherClientMap.GetLength(0); ++i)
        {
            for (int j = 0; j < otherClientMap.GetLength(1); ++j)
            {
                if (_strArr[a] == "false")
                {
                    if (otherClientMap[i, j] != null)
                    {
                        Destroy(otherClientMap[i, j].gameObject);
                        otherClientMap[i, j] = null;
                    }
                }
                else if (_strArr[a] == "true")
                {
                    if (otherClientMap[i, j] == null)
                    {
                        GameObject tile = Instantiate(tilePrefab, otherClientMap[i, j]);
                        otherClientMap[i, j] = tile.transform;
                        tile.transform.position = new Vector3(-19 + i, j, 0);
                    }
                }

                a++;
            }
        }
    } 
}
