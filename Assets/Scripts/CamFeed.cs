using System;
using UnityEngine;
using UnityEngine.Rendering;

public class CamFeed : Singleton<CamFeed> {

    public Material unDistort;
    public RenderTexture leftEyeRender;
    public RenderTexture leftEyeUndistorted;
    public RenderTexture rightEyeRender;
    public RenderTexture rightEyeUndistorted;

    WebCamTexture webcamTexture;
    byte[] croppedImage;
    Vector2 scale = new Vector2(-.5f, -1);
    Vector2 leftOffset = new Vector2(.5f, 1);
    Vector2 rightOffset = new Vector2(1f, 1);

    bool imageRequested;

    protected override void Awake() {
        base.Awake();
        StartWebCam();
    }

    void Update() {

        if (webcamTexture.didUpdateThisFrame && webcamTexture.width > 100) {
            //use scale and offset to get only second half of image, also it needs flipped on both axes
            Graphics.Blit(webcamTexture, leftEyeRender, scale, leftOffset);
            //next run eye through undistortion shader
            Graphics.Blit(leftEyeRender, leftEyeUndistorted, unDistort);
            //do the same for right eye
            Graphics.Blit(webcamTexture, rightEyeRender, scale, rightOffset);
            Graphics.Blit(rightEyeRender, rightEyeUndistorted, unDistort);

            if (imageRequested) {
                AsyncGPUReadback.Request(leftEyeUndistorted, 0, TextureFormat.RGB24, OnCompleteReadback);
            }
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
