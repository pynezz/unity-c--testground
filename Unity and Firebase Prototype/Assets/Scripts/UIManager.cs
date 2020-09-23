using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject registerForm, loginForm, profilePage;
    public Text userName, userMail, userScore, titleText, outerTitleText;
    public int UIscore;

    public void EnableRegisterForm()
    {
        registerForm.SetActive(true);
        loginForm.SetActive(false);
        profilePage.SetActive(false);
    }

    public void EnableLoginForm()
    {
        registerForm.SetActive(false);
        loginForm.SetActive(true);
        profilePage.SetActive(false);
    }

    public void EnableProfilePage()
    {
        registerForm.SetActive(false);
        loginForm.SetActive(false);
        profilePage.SetActive(true);
    }

    public void InitiateProfilePage(string username, string email)
    {
        outerTitleText.gameObject.SetActive(false);
        userName.text = username;
        userMail.text = email;
        titleText.text = "Welcome " + username + "!";
    }

    public void UpdateUserScore(int score)
    {
        userScore.text = score.ToString();
    }
}
