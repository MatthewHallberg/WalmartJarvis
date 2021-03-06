﻿using UnityEngine;

public class AdjustCamera : Singleton<AdjustCamera> {

    public Transform leftCam;
    public Transform rightCam;

    [Header("Eye Distance")]
    [Range(2.5f, 3.5f)]
    public float eyeDistance;

    [Header("X-Shift")]
    [Range(-1f, 1f)]
    public float xShift;

    [Header("Y-SHIFT")]
    [Range(-.4f, .4f)]
    public float yShift;

    [Header("Z-SHIFT")]
    [Range(-1f, -.3f)]
    public float zShift;

    //returns IPD in millimeters
    public float GetIPD() {
        return (eyeDistance % 1) + Mathf.Abs(xShift);
    }

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
