using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Block block;

    float fall = 0;
    float accel = 1;

    TetManager tetManager;

    void Start()
    {
        block = FindObjectOfType<Block>();
        tetManager = FindObjectOfType<TetManager>();
    }

    void Update()
    {
        if(!tetManager.gameOver && tetManager.gameStart)
            UserInput();
    }

    void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            block.transform.position += Vector3.right;

            if (CheckIsValidPosition()) { tetManager.UpdateMap(block); }
            else { block.transform.position += Vector3.left; }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            block.transform.position += Vector3.left;
            if (CheckIsValidPosition()) { tetManager.UpdateMap(block); }
            else { block.transform.position += Vector3.right; }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (block.allowRotation)
            {
                if (block.limitRotation)
                {
                    if (block.transform.eulerAngles.z >= 90)
                        block.transform.Rotate(0, 0, -90);
                    else
                        block.transform.Rotate(0, 0, 90);
                }
                else
                {
                    block.transform.Rotate(0, 0, 90);
                }

                if (CheckIsValidPosition()) { tetManager.UpdateMap(block); }
                else
                {
                    if (block.limitRotation)
                    {
                        if (block.transform.eulerAngles.z >= 90)
                            block.transform.Rotate(0, 0, -90);
                        else
                            block.transform.Rotate(0, 0, 90);
                    }
                    else
                        block.transform.Rotate(0, 0, -90);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - fall >= accel)
        {
            block.transform.position += Vector3.down;

            if (CheckIsValidPosition()) { tetManager.UpdateMap(block); }
            else
            {
                block.transform.position += Vector3.up;
                if (tetManager.GameOver(block.transform.position.y))
                {
                    tetManager.gameOver = true;
                    UIManager.Instance.DisplayGameoverText("Lose", new Color(1, 0, 0, 1));
                    NetworkManager.Instance.InformGameOver();
                    return;
                }
                tetManager.DeleteRow();
                tetManager.SpawnNextBlock();

                
                NetworkManager.Instance.OnUserMapDisplay();
            }
            fall = Time.time;

        }
    }

    bool CheckIsValidPosition()
    {
        foreach(Transform b in block.transform)
        {
            Vector2 pos = tetManager.Round(b.transform.position);

            if (tetManager.CheckIsInside(pos) == false)
                return false;

            if (tetManager.GetTransformAtMapPosition(pos) != null && tetManager.GetTransformAtMapPosition(pos).parent != block.transform)
                return false;
        }

        return true;
    }

    
}
