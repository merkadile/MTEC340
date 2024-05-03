using UnityEngine;

//this script is a singleton that holds the unchanging values in the game
//that should be accessible by any script at any time.
public class StaticBehavior : MonoBehaviour
{
    public static StaticBehavior Instance;
    
    public KeyCode UpKey = KeyCode.W;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode DownKey = KeyCode.S;
    public KeyCode RightKey = KeyCode.D;
    public KeyCode PauseKey = KeyCode.P;
    public KeyCode EnterKey = KeyCode.Return;
    public KeyCode QuitKey = KeyCode.Escape;

    public float FlickerInterval = 1.0f;

    void Awake()
    {
        //allow the Static Properties Manager to exist beyond the TitleScreen scene.
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //the following if-else statement is necessary when using a static instance of a class...
        if (Instance != this && Instance != null)
            Destroy(this);
        else
            Instance = this;
    }
}