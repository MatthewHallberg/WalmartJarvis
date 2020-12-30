using System;
using UnityEngine;

[RequireComponent(typeof(CamFeed))]
public class SingleCamImage : Singleton<SingleCamImage> {

    CamFeed cameraFeed;
    Texture2D tempTexture;
    WebCamTexture webCamTex;
    bool intialized;

    void Start() {
        cameraFeed = GetComponent<CamFeed>();
    }

    public int GetImageHeight() {
        return webCamTex.height;
    }

    public int GetImageWidth() {
        return webCamTex.width/2;
    }

    public byte[] GetImage() {

        if (!intialized) {
            return null;
        }

        int sourceWidth = webCamTex.width;
        int sourceHeight = webCamTex.height;

        tempTexture.SetPixels32(webCamTex.GetPixels32());
        //cut iamge in half 
        return CropImageArray(tempTexture.GetRawTextureData(), sourceWidth, 24, sourceWidth / 2, sourceHeight);
    }

    byte[] CropImageArray(byte[] pixels, int sourceWidth, int bitsPerPixel, int newWidth, int newHeight) {
        var blockSize = bitsPerPixel / 8;
        var outputPixels = new byte[newWidth * newHeight * blockSize];

        //Create the array of bytes.
        for (var line = 0; line <= newHeight - 1; line++) {
            var sourceIndex = line * sourceWidth * blockSize;
            var destinationIndex = line * newWidth * blockSize;

            Array.Copy(pixels, sourceIndex, outputPixels, destinationIndex, newWidth * blockSize);
        }

        return outputPixels;
    }
}
