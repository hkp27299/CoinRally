using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    
    public static FirebaseApp App;
    public static FirebaseAuth Auth;
    public static FirebaseUser CurrentUser;
    public static DatabaseReference Db;

    private string SHA_keystore = "68:35:50:98:4B:1C:F0:6C:77:A5:64:54:88:6F:DD:73:E8:6D:7D:9D";

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
        var result = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (result == DependencyStatus.Available)
        {
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            Db = FirebaseDatabase.DefaultInstance.RootReference;
            
            Debug.Log("AUTH : " + JsonUtility.ToJson(Auth));

            Auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }
        else
        {
            Debug.Log("Error");
        }
    }

    private void Start()
    {
        // UpdateFirebaseDatabase();
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs) {
        if (Auth.CurrentUser != CurrentUser) {
            Debug.Log("##################### Inside Auth.CurrentUser != CurrentUser");
            bool signedIn = CurrentUser != Auth.CurrentUser && Auth.CurrentUser != null;
            if (!signedIn && CurrentUser != null) {
                Debug.Log("Signed out ");
                UpdateFirebaseDatabase();
                ChangeScene("LoginScene");
            }

            if (signedIn)
            {
                Debug.Log("Signed in " + CurrentUser.UserId);
                UpdateFirebaseDatabase();
                ChangeScene("StartingPoint");
            }
        }
    }

    public void UpdateFirebaseDatabase()
    {
        StartCoroutine(UpdateRoutine());
    }

    private IEnumerator UpdateRoutine()
    {
        var updateNameTask = Db.Child("users").Child(CurrentUser.UserId).Child("name").SetValueAsync(CurrentUser.DisplayName);

        yield return new WaitUntil(() => updateNameTask.IsCompleted);
        
        var updateScore = Db.Child("users").Child(CurrentUser.UserId).Child("score").SetValueAsync(PlayerPrefs.GetInt("HighScore"));
        
        yield return new WaitUntil(() => updateScore.IsCompleted);

        if (updateNameTask.Exception != null)
        {
            Debug.Log("Cannot update name");
        }
        else if (updateScore.Exception != null)
        {
            Debug.Log("Cannot Update score");
        }
        else
        {
            Debug.Log("Name and score updated");
        }
    }

    public void SetMyUserScore(int score)
    {
        PlayerPrefs.SetInt("HighScore", score);
    }

    public void GetUserScore()
    {
        StartCoroutine(GetUserScoreRoutine());
    }

    private IEnumerator GetUserScoreRoutine()
    {
        var getScoreTask = Db.Child("users").Child(CurrentUser.UserId).Child("score").GetValueAsync();

        yield return new WaitUntil(() => getScoreTask.IsCompleted);
        Debug.Log("Inside Get User Score");
        if (getScoreTask.Exception != null && getScoreTask.Result != null)
        {
            SetMyUserScore(int.Parse(getScoreTask.Result.Value.ToString()));
        }
        
        ChangeScene("StartingPoint");
    }

    public async Task<Dictionary<string,int>> GetAllUserLeaderBoards()
    {
        var data = new Dictionary<string, int>();

        var allUserDataTask = Db.Child("users").GetValueAsync();

        while (!allUserDataTask.IsCompleted)
        {
            await Task.Delay(100);
        }
        
        if (allUserDataTask.Exception == null)
        {
            Debug.Log("Inside all user Data: " + JsonUtility.ToJson(allUserDataTask));
            foreach (var childSnapSnapshot in allUserDataTask.Result.Children)
            {
                data.Add(childSnapSnapshot.Child("name").Value.ToString(),int.Parse(childSnapSnapshot.Child("score").Value.ToString()));
                Debug.Log($"Data: {childSnapSnapshot.Child("name").Value} , {childSnapSnapshot.Child("score").Value}");
            }
        }
        else
        {
            Debug.LogError($"$$$$$######$$$$$$$ => {allUserDataTask.Exception.Message}");
        }
        
        return data;
    }
    
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
