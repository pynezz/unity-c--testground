using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;


public class GameManager : MonoBehaviour
{
    public GameObject uiManager;
    public GameObject authManager;

    public void IncreaseScore()
    {
        authManager.GetComponent<AuthManager>().score++;
    }
}
