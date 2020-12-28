using UnityEngine;

public class CamFeed : MonoBehaviour {

    public Material camMaterial;

    void Start() {
        StartWebCam();
    }

    void StartWebCam() {
        WebCamTexture webcamTexture = new WebCamTexture(GetWebCamDevice(), 2560, 960, 30);
        camMaterial.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    string GetWebCamDevice() {
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices) {
            print("WEBCAM: " + device.name);
        }

        WebCamDevice desiredDevice = devices[devices.Length - 1];
        print(desiredDevice.availableResolutions);
        return desiredDevice.name;
    }
}
