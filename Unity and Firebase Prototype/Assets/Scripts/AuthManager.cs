using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    // Firebase

    [Header("Firebase")]

    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Header("Login")]

    public InputField emailLoginField;
    public InputField passwordLoginField;
    public Text warningLoginText;
    public Text confirmLoginText;

    [Header("Register")]

    public InputField usernameRegField;
    public InputField emailRegField;
    public InputField passwordRegField;
    public InputField passwordRegVerifyField;
    public Text warningRegText;
    
}
