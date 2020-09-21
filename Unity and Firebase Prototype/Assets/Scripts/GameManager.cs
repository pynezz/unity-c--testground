using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;


public class GameManager : MonoBehaviour
{
    public GameObject uiManager;
    public GameObject authManager;
    private int _internalScore = 0;
    private int _globalScore;

    public void LoadScore()
    {
        _globalScore = authManager.GetComponent<AuthManager>().score;
    }

    public void IncreaseScore()
    {
        _internalScore += _globalScore;
        _internalScore++;
        authManager.GetComponent<AuthManager>().score++;
        uiManager.GetComponent<UIManager>().UpdateUserScore(_internalScore);
    }
}
