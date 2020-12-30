using UnityEngine;

public class CamFeed : MonoBehaviour {

    public Material leftMat;
    public Material rightMat;

    WebCamTexture webcamTexture;

    bool initialized;

    void Start() {
        StartWebCam();
    }

    void Update() {

        if (webcamTexture.width > 100) {
            initialized = true;
        }
    }

    void StartWebCam() {
        webcamTexture = new WebCamTexture(GetWebCamDevice(), 2560, 960, 30);
        webcamTexture.Play();
        leftMat.mainTexture = webcamTexture;
        rightMat.mainTexture = webcamTexture;
    }

    string GetWebCamDevice() {
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices) {
            print("WEBCAM: " + device.name);
        }

        WebCamDevice desiredDevice = devices[devices.Length - 1];
        print("CHOSEN: " + desiredDevice.name);
        return desiredDevice.name;
    }
}
