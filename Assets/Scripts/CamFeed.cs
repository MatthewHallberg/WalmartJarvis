using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CamFeed : Singleton<CamFeed> {

    public RenderTexture renderTexture;
    public Material leftMat;
    public Material rightMat;

    WebCamTexture webcamTexture;
    byte[] croppedImage;
    Vector2 scale = new Vector2(.5f, 1);
    Vector2 offset = new Vector2(.5f, 0);
    bool imageRequested;

    protected override void Awake() {
        base.Awake();
        StartWebCam();
    }

    void Update() {
        if (imageRequested && webcamTexture.didUpdateThisFrame && webcamTexture.width > 100) {
            //use scale and offset to get only second half of image. 
            Graphics.Blit(webcamTexture, renderTexture, scale, offset);
            AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
        }
    }

    public WebCamTexture GetWebCamTexture() {
        return webcamTexture;
    }

    public byte[] GetLeftImageBytes() {
        imageRequested = true;
        return croppedImage;
    }

    void StartWebCam() {
        //set up webcam and cam textures for each eye
        webcamTexture = new WebCamTexture(GetWebCamDevice(), 2560, 960, 30);
        webcamTexture.Play();
        leftMat.mainTexture = webcamTexture;
        rightMat.mainTexture = webcamTexture;
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request) {
        if (request.hasError) {
            Debug.Log("GPU readback error detected.");
            return;
        }

        croppedImage = request.GetData<byte>().ToArray();
        imageRequested = false;
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
