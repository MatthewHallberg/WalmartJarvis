using UnityEngine;

public class CamFeed : MonoBehaviour {

    public Material camMaterial;

    void Start() {
        StartWebCam();
    }

    void StartWebCam() {
        //logitech 1920 by 1080
        WebCamTexture webcamTexture = new WebCamTexture(GetWebCamDevice(), 1280, 960, 90);
        camMaterial.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    string GetWebCamDevice() {
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices) {
            print("WEBCAM: " + device.name);
        }
        return devices[devices.Length - 1].name;
    }
}
