using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void SelectDifficulty(int indexValue)
    {
        switch (indexValue)
        {
            case 0:
                gameManager.ChangeDifficulty(indexValue);
                Debug.Log("0");
                break;
            case 1:
                gameManager.ChangeDifficulty(indexValue);
                Debug.Log("1");
                break;
            case 2:
                gameManager.ChangeDifficulty(indexValue);
                Debug.Log("2");
                break;
        }
    }
}
