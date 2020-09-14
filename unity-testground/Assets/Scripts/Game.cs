using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    int _gameNumber = 1;
    bool _gameReady;

    private void Start()
    {
        _gameReady = true;
        JoystickActivation();
    }

    void Update()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (_gameReady)
        {
            for (int i = 0; i < JoystickActivation().Count; i++)
            {
                if (Input.GetKeyDown(JoystickActivation()[i]))
                {
                    Debug.Log("Keycode: " + JoystickActivation()[i]);
                    _gameReady = false;
                    break;
                }
            }
        }
    }

    private List<KeyCode> JoystickActivation()
    {
        List<KeyCode> keys = new List<KeyCode>();

        for (int i = 0; i < 20; i++)
        {
            KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode),"Joystick" + _gameNumber + "Button" + i);
            keys.Add(key);
        }
        keys.Add(KeyCode.K); //For testing
        return keys;
    }
}
