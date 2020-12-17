using UnityEngine;

public class AdjustCamera : MonoBehaviour {

    public Transform leftCam;
    public Transform rightCam;

    [Header("X-SHIFT")]
    [Range(1, 6f)]
    public float X;

    [Header("Y-SHIFT")]
    [Range(-.4f, .4f)]
    public float Y;

    [Header("Z-SHIFT")]
    [Range(0f, -5f)]
    public float Z;

    void Start() {
        LoadValues();
    }

    void OnApplicationQuit() {
        SaveValues();
    }

    void OnValidate() {
        SetValues();
    }

    void SetValues() {
        //leftCam.localPosition = new Vector3(-X, Y, Z);
        //rightCam.localPosition = new Vector3(X, Y, Z);
    }

    void LoadValues() {
        if (PlayerPrefs.HasKey("x")) {
            X = PlayerPrefs.GetFloat("x");
            Y = PlayerPrefs.GetFloat("y");
            Z = PlayerPrefs.GetFloat("z");
        }
        SetValues();
    }

    void SaveValues() {
        //PlayerPrefs.SetFloat("x", X);
        PlayerPrefs.SetFloat("y", Y);
        //PlayerPrefs.SetFloat("z", Z);
    }
}
