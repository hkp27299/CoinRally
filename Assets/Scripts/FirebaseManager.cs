using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    
    private FirebaseApp app;
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseDatabase db;


    public FirebaseApp App
    {
        get => app;
        set => app = value;
    }

    public FirebaseAuth Auth
    {
        get => auth;
        set => auth = value;
    }

    public FirebaseDatabase Db
    {
        get => db;
        set => db = value;
    }

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
            app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;

            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }
        else
        {
            Debug.Log("Error");
        }
    }
    
    void AuthStateChanged(object sender, System.EventArgs eventArgs) {
        if (auth.CurrentUser != user) {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null) {
                Debug.Log("Signed out " + user.UserId);
            }
            
            user = auth.CurrentUser;
            
            if (signedIn) {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
