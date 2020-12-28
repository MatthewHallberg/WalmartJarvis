using UnityEngine;

public class Config : MonoBehaviour {

    public static readonly string KEYWORD = "jarvis";
    public static readonly string GREETING = "yes sir";
    public static readonly string LOADING = "loading services...";
    public static readonly string INITIALIZED = "at your service sir";
    public static readonly string FAILED = "Mr. Stark I don't feel so good";

    //if these dont match its possible the chatbot is running but not loaded properly
    public static readonly string TEST_QUESTION = "What is one plus one?";
    public static readonly string TEST_ANSWER = "Two.";

    public static readonly float PINCH_THRESHOLD = .7f;
    public static readonly float TRASH_DISTANCE = .5f;

    void Start() {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }
}
