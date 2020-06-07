using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TetManager : MonoBehaviour
{
    public static int width = 10;
    public static int height = 20;

    public Transform[,] map = new Transform[width, height];

    public Block[] blocks;
    public bool gameOver = false;
    public bool gameStart = false;
    public List<Block> blockQ = new List<Block>();
    public List<Vector3> spawnLocal = new List<Vector3>();
    
    public bool ready = false;

    InputManager inputManager;
    
    void Start()
    {
        spawnLocal.Add(new Vector3(5, 20, 0));
        spawnLocal.Add(new Vector3(15, 20, 0));
        spawnLocal.Add(new Vector3(15, 15, 0));

        inputManager = GetComponent<InputManager>();

        NetworkManager.Instance.handler += GameStart;
    }

    void GameStart()
    {
        for (int i = 0; i < 3; ++i)
            SpawnNextBlock();
        gameStart = true;
    }

    public bool CheckFullRow(int _y)
    {
        for(int i = 0; i<width; ++i)
        {
            if (map[i, _y] == null)
                return false;
        }

        return true;
    }

    public void DeleteTile(int _y)
    {
        for(int i = 0; i < width; ++i)
        {
            Destroy(map[i, _y].gameObject);
            map[i, _y] = null;
        }
    }

    public void DeleteRow()
    {
        for (int i = 0; i < height; ++i)
        {
            if(CheckFullRow(i))
            {
                DeleteTile(i);
                AllRowDown(i + 1);
                --i;
            }
        }
    }

    public void RowDown(int _y)
    {
        for (int i = 0; i < width; ++i)
        {
            if(map[i,_y] != null)
            {
                map[i, _y - 1] = map[i, _y];
                map[i, _y] = null;
                map[i, _y - 1].position += Vector3.down;
            }
        }
    }

    public void AllRowDown(int _y)
    {
        for (int i = _y; i < height; ++i)
        {
            RowDown(i);
        }
    }

    public void UpdateMap(Block _block)
    {
        for(int y = 0; y < height; ++y)
        { 
            for (int x = 0; x < width; ++x)
            {
                if (map[x,y] != null)
                {
                    if (map[x, y].parent == _block.transform)
                        map[x, y] = null;
                }
            }
        }

        foreach(Transform tile in _block.transform)
        {
            Vector2 pos = Round(tile.position);

            if (pos.y < height)
                map[(int)pos.x, (int)pos.y] = tile;
        }
    }

    public Transform GetTransformAtMapPosition(Vector2 _pos)
    {
        if (_pos.y > height - 1)
            return null;
        else
            return map[(int)_pos.x, (int)_pos.y];
    }

    public void SpawnNextBlock()
    {
        int randomBlock = Random.Range(0, blocks.Length);
        Block spawnBlock = Instantiate(blocks[randomBlock], spawnLocal[2], transform.rotation);
        blockQ.Add(spawnBlock);

        if(blockQ.Count.Equals(3))
        {
            for (int i = 0; i < blockQ.Count; ++i)
            {
                blockQ[i].transform.position = spawnLocal[i];
            }
            NextBlock();
        }
    }

    public void NextBlock()
    {
        Block nextBlock = blockQ[0];
        blockQ.RemoveAt(0);

        inputManager.block = nextBlock;
    }

    public bool CheckIsInside(Vector2 _pos)
    {
        return ((int)_pos.x >= 0 && (int)_pos.x < width && (int)_pos.y > -1);
    }

    public bool GameOver(float _y)
    {
        return (int)_y >= height;
    }

    public Vector2 Round(Vector2 _pos)
    {
        return new Vector2(Mathf.Round(_pos.x), Mathf.Round(_pos.y));
    }

    
}
