using UnityEngine;

public class Config : MonoBehaviour {

    public static readonly string KEYWORD = "jarvis";
    public static readonly string GREETING = "yes sir";
    public static readonly string INITIALIZED = "at your service";
    public static readonly float PINCH_THRESHOLD = .7f;

    void Start() {
        Application.targetFrameRate = 60;
    }
}
