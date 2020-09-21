using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.Assertions.Must;

public class AuthManager : MonoBehaviour
{
    #region Variables

    public GameObject uiManager;
    public GameObject gameManager;

    public DatabaseReference reference;

    public int score = 0;

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
        Debug.Log("Firing up Firebase..");
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
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Firebase Auth active");
        Debug.Log("Initializing Firebase Database");
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-firebase-prototype.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Firebase Database active");
    }

    /*public void LoadData()
    {
        //var task = FirebaseDatabase.DefaultInstance.GetReference("user_1").
            
        FirebaseDatabase.DefaultInstance.GetReference("users").Child("user_1").Child("score").GetValueAsync().ContinueWith(task => {
          if (task.IsFaulted)
          {
                Debug.LogError("Task failed " + task);
          }
          else if (task.IsCompleted)
          {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.Value);
                uiManager.GetComponent<UIManager>().userScore.text = task.Result.Value.ToString();
                Debug.Log("Score: " + score.ToString() + snapshot.Value.GetHashCode().ToString());
          }
      });
        Debug.Log("Data loaded");
    }*/

    /*private void GameManager_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        uiManager.GetComponent<UIManager>().userScore.text = e.Snapshot.Child("users").Child("score").GetValue(true).ToString();
    }*/

    public void SaveData()
    {
        reference.Child("users").Child("user_1").Child("score").SetValueAsync(score);
        Debug.Log("Saved score: " + score + " to database");
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
            var uiMan = uiManager.GetComponent<UIManager>();
            uiMan.EnableProfilePage();
            uiMan.InitiateProfilePage(user.DisplayName, user.Email);
            warningRegText.text = "";
            Debug.Log("Trying to load data..");
            StartCoroutine(LoadScore());
        }
    }

    IEnumerator LoadScore()
    {
        var getTask = FirebaseDatabase.DefaultInstance.GetReference("users").Child("user_1").Child("score").GetValueAsync();
        yield return new WaitUntil(() => getTask.IsCompleted || getTask.IsFaulted);
        if (getTask.IsCompleted)
        {
            uiManager.GetComponent<UIManager>().userScore.text = getTask.Result.Value.ToString();
            Debug.Log("Data loaded successfully!");
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
                        var uiMan = uiManager.GetComponent<UIManager>();
                        uiMan.EnableProfilePage();
                        uiMan.InitiateProfilePage(username, email);
                        Debug.Log("IT WORKED!" + " Username: " + username);
                        warningRegText.text = "";
                    }
                }
            }
        }
    }

    #endregion
}
