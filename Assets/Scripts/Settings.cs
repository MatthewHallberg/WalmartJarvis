using UnityEngine;

public class Settings : Singleton<Settings> {

    public static readonly float PINCH_THRESHOLD = .7f;


    void Start() {
        Application.targetFrameRate = 60;
    }
}
