using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    #region Variables
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
    #endregion

    #region Unity Methods
    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            } 
            else
            {
                Debug.LogError("Something went wrong with Firebase Dependencies: " + dependencyStatus);
            }
        });
    }
    #endregion

    #region Custom Methods

    void InitializeFirebase()
    {
        Debug.Log("Firing up Firebase..");
        auth = FirebaseAuth.DefaultInstance;
    }

    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegField.text, passwordRegField.text, usernameRegField.text));
    }

    #endregion

    #region IENumerators

    IEnumerator Login(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            // Login error
            Debug.LogWarning(message: $"Failed to register with {loginTask.Exception}");
            FirebaseException firebaseEx = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login failed";
            switch(errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Enter email";
                    break;
                case AuthError.MissingPassword:
                    message = "Enter a password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong email or password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Email is invalid";
                    break;
                case AuthError.UserNotFound:
                    message = "Wrong email or password";
                    break;
                default:
                    message = "Error! Wtf?";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User logged in no error
            user = loginTask.Result;
            Debug.LogFormat("User signed in sucessfully: {0} ({1})", user.DisplayName, user.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged in!";
        }
    }

    IEnumerator Register(string email, string password, string username)
    {
        if (username == "")
        {
            warningRegText.text = "No username";
        }
        else if (passwordRegField.text != passwordRegVerifyField.text)
        {
            warningRegText.text = "Password donesn't match";
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                // Register error
                Debug.LogWarning(message: $"Failed to register with {registerTask.Exception}");
                FirebaseException firebaseEx = (FirebaseException)registerTask.Exception.GetBaseException();
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Failed to register";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Enter email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Enter a password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Password too weak";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email already in use";
                        break;
                    default:
                        message = "Error idk why";
                        break;
                }
                warningRegText.text = message;
            }
            else
            {
                // User created!
                user = registerTask.Result;
                if (user != null)
                {
                    // Create user profile and set username
                    UserProfile profile = new UserProfile { DisplayName = username };
                    var profileTask = user.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

                    if (profileTask.Exception != null)
                    {
                        // Any errors?
                        Debug.LogWarning(message: $"Failed to register task with {profileTask.Exception}");
                        FirebaseException fireBaseEx = (FirebaseException)profileTask.Exception.GetBaseException();
                        AuthError errorCode = (AuthError)fireBaseEx.ErrorCode;
                        warningRegText.text = "Setting username error!";
                    }
                    else
                    {
                        Debug.Log("IT WORKED!" + " Username: " + username);
                        warningRegText.text = "";
                    }
                }
            }
        }
    }

    #endregion
}
