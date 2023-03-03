using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int width = 8;
    private int height = 8;
    private int mineCount = 16;

    private BoardManager board;
    private Cell[,] state;

    private bool gamestarted = false;
    private bool gameover;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        board = GetComponentInChildren<BoardManager>();
    }

    public void NewGame()
    {
        gamestarted = true;

        state = new Cell[width, height];
        gameover = false;

        GenerateCells();
        GenerateMines();
        GenerateCellNumbers();

        Camera.main.transform.position = new Vector3(width / 2, height / 2, -10f);
        board.DrawBoard(state);

    }

    private void GenerateCells()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.cellPosition = new Vector3Int(x, y, 0);
                cell.cellType = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }

    private void GenerateMines()
    {
        for(int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (state[x,y].cellType == Cell.Type.Mine)
            {
                x++;

                if(x >= width) { 
                    x = 0;
                    y++;
                    
                    if(y >= height)
                    {
                        y = 0;
                    }
                }
            }

            state[x, y].cellType = Cell.Type.Mine;
        }
    }

    private void GenerateCellNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                
                if (cell.cellType == Cell.Type.Mine)
                {
                    continue;
                }

                cell.cellNumber = CountMines(x, y);

                if (cell.cellNumber > 0)
                {
                    cell.cellType = Cell.Type.Number;
                }

                state[x, y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                if (GetCell(x, y).cellType == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void Update()
    {
        if(!gameover && gamestarted)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                RevealCell();
            }
        }
    }

    private void RevealCell()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.cellType == Cell.Type.Invalid || cell.isRevealed || cell.isFlagged)
        {
            return;
        }

        switch (cell.cellType)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.isRevealed = true;
                state[cell.cellPosition.x, cell.cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }

        board.DrawBoard(state);
    }

    private void Flood(Cell cell)
    {
        if (cell.isRevealed) return;
        if (cell.cellType == Cell.Type.Mine || cell.cellType == Cell.Type.Invalid) return;

        cell.isRevealed = true;
        state[cell.cellPosition.x, cell.cellPosition.y] = cell;

        if(cell.cellType == Cell.Type.Empty)
        {
            Flood(GetCell(cell.cellPosition.x - 1, cell.cellPosition.y));
            Flood(GetCell(cell.cellPosition.x + 1, cell.cellPosition.y));
            Flood(GetCell(cell.cellPosition.x, cell.cellPosition.y - 1));
            Flood(GetCell(cell.cellPosition.x, cell.cellPosition.y + 1));
        }
    }

    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if(cell.cellType == Cell.Type.Invalid || cell.isRevealed)
        {
            return;
        }

        cell.isFlagged = !cell.isFlagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.DrawBoard(state);
    }

    private void Explode(Cell cell)
    {
        Debug.Log("GameOver!");
        gameover = true;

        cell.isRevealed = true;
        cell.itExploded = true;
        state[cell.cellPosition.x, cell.cellPosition.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if (cell.cellType == Cell.Type.Mine)
                {
                    cell.isRevealed = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.cellType != Cell.Type.Mine && !cell.isRevealed)
                {
                    return;
                }
            }
        }

        Debug.Log("Winner!");
        gameover = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.cellType == Cell.Type.Mine)
                {
                    cell.isFlagged = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y))
        {
            return state[x, y];
        }else
        {
            return new Cell();
        }
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void ChangeDifficulty(int indexValue)
    {
        switch (indexValue)
        {
            case 0:
                width = 8;
                height = 8;
                mineCount = 16;
                break;
            case 1:
                width = 16;
                height = 16;
                mineCount = 32;
                break;
            case 2:
                width = 32;
                height = 32;
                mineCount = 64;
                Camera.main.orthographicSize = 18;
                break;
        }
    }
}
