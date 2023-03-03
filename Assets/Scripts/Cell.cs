using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    public enum Type
    {
        Invalid,
        Empty,
        Mine,
        Number
    }

    public Vector3Int cellPosition;
    public Type cellType;
    public int cellNumber;
    public bool isRevealed;
    public bool isFlagged;
    public bool itExploded;
}
