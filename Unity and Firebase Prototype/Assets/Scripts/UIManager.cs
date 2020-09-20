using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Color palette 
    // 
    // Yellow               FFCB2A
    // Dark Orange          F5820C
    // Light Orange         FFA610
    // Background Blue      051E34
    // Light Blue           1A73E8
    //

    public GameObject registerForm, loginForm, statusForm;

    public void EnableRegisterForm()
    {
        registerForm.SetActive(true);
        loginForm.SetActive(false);
        statusForm.SetActive(false);
    }

    public void EnableLoginForm()
    {
        registerForm.SetActive(false);
        loginForm.SetActive(true);
        statusForm.SetActive(false);
    }

    public void EnableStatusForm()
    {
        registerForm.SetActive(false);
        loginForm.SetActive(false);
        statusForm.SetActive(true);
    }

    public void RegisterSuccessful()
    {
        Debug.Log("Holy shit it worked!");
    }
}
