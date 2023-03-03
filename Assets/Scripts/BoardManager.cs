using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile tileUknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNumber1;
    public Tile tileNumber2;
    public Tile tileNumber3;
    public Tile tileNumber4;
    public Tile tileNumber5;
    public Tile tileNumber6;
    public Tile tileNumber7;
    public Tile tileNumber8;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void DrawBoard(Cell[,] gameStatus)
    {
        int width = gameStatus.GetLength(0);
        int height = gameStatus.GetLength(1);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Cell cell = gameStatus[x, y];
                tilemap.SetTile(cell.cellPosition, GetTile(cell));
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.isRevealed)
        {
            return GetRevealedTile(cell);
        }
        else if (cell.isFlagged)
        {
            return tileFlag;
        }
        else
        {
            return tileUknown;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch(cell.cellType)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.itExploded ? tileExploded : tileMine;
            case Cell.Type.Number: return GetTileNumber(cell);
            default: return null;
        }
    }

    private Tile GetTileNumber(Cell cell)
    {
        switch(cell.cellNumber)
        {
            case 1: return tileNumber1;
            case 2: return tileNumber2;
            case 3: return tileNumber3; 
            case 4: return tileNumber4;
            case 5: return tileNumber5;
            case 6: return tileNumber6;
            case 7: return tileNumber7;
            case 8: return tileNumber8;
            default: return null;
        }
    }
}
    
