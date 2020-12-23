using UnityEngine;

public class AdjustCamera : MonoBehaviour {

    public Transform leftCam;
    public Transform rightCam;

    [Header("Eye Distance")]
    [Range(2.2f, 2.5f)]
    public float eyeDistance;

    [Header("X-Shift")]
    [Range(-1f, 1f)]
    public float xShift;

    [Header("Y-SHIFT")]
    [Range(-.4f, .4f)]
    public float yShift;

    [Header("Z-SHIFT")]
    [Range(-2f, 0f)]
    public float zShift;

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
        leftCam.localPosition = new Vector3(-eyeDistance + xShift, yShift, zShift);
        rightCam.localPosition = new Vector3(eyeDistance + xShift, yShift, zShift);
    }

    void LoadValues() {
        if (PlayerPrefs.HasKey("x")) {
            xShift = PlayerPrefs.GetFloat("x");
            yShift = PlayerPrefs.GetFloat("y");
            zShift = PlayerPrefs.GetFloat("z");
            eyeDistance = PlayerPrefs.GetFloat("eyeDistance");

        }
        SetValues();
    }

    void SaveValues() {
        PlayerPrefs.SetFloat("x", xShift);
        PlayerPrefs.SetFloat("y", yShift);
        PlayerPrefs.SetFloat("z", zShift);
        PlayerPrefs.SetFloat("eyeDistance", eyeDistance);
    }
}
