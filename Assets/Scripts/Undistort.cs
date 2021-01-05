using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;
using ArucoUnity.Cameras.Parameters;
using ArucoUnity.Plugin;
using Unity.Mathematics;
using System.Collections;

public class Undistort : MonoBehaviour {

    const string CalibrationFilePath = "Assets/ArucoUnity/CameraParameters/calibration.xml";

    public Material undistortMat;
    public Camera viewingCamera;

    void Start() {
        ReadCalibrationFile();
    }

    void ReadCalibrationFile() {
        StreamReader reader = null;
        try {
            reader = new StreamReader(CalibrationFilePath);
            XmlSerializer serializer = new XmlSerializer(typeof(ArucoCameraParameters));
            ArucoCameraParameters camCalibration = serializer.Deserialize(reader) as ArucoCameraParameters;

            // Populate non-serialized properties
            camCalibration.CameraMatrices = CreateProperty(camCalibration.CameraMatricesType, camCalibration.CameraMatricesValues);
            camCalibration.DistCoeffs = CreateProperty(camCalibration.DistCoeffsType, camCalibration.DistCoeffsValues);
            camCalibration.OmnidirXis = CreateProperty(camCalibration.OmnidirXisType, camCalibration.OmnidirXisValues);

            StartCoroutine(GetOptimalCameraMatrix(camCalibration));
        } catch (Exception ex) {
            throw new ArgumentException("Couldn't load the camera parameters file path '" + CalibrationFilePath + ". ",
              "cameraParametersFilePath", ex);
        } finally {
            if (reader != null) {
                reader.Close();
            }
        }
    }

    internal static Cv.Mat[] CreateProperty(Cv.Type propertyType, double[][][] propertyValues) {
        int cameraNumber = propertyValues.Length;

        var property = new Cv.Mat[cameraNumber];
        for (int cameraId = 0; cameraId < cameraNumber; cameraId++) {
            int rows = propertyValues[cameraId].Length,
                cols = (rows > 0) ? propertyValues[cameraId][0].Length : 0;
            property[cameraId] = new Cv.Mat(rows, cols, propertyType);
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    property[cameraId].AtDouble(i, j, propertyValues[cameraId][i][j]);
                }
            }
        }

        return property;
    }

    IEnumerator GetOptimalCameraMatrix(ArucoCameraParameters camCalibration) {

        //get dimensions of camera image
        Cv.Size imageSize = new Cv.Size(camCalibration.ImageWidths[0], camCalibration.ImageHeights[0]);

        //wait a frame before reading FOV incase another source is resetting it (i.e. VR SDK)
        yield return new WaitForEndOfFrame();

        //convert Unity FOV to openCV friendly version
        float cameraFocalLength = imageSize.Height / (2f * Mathf.Tan(0.5f * viewingCamera.fieldOfView * Mathf.Deg2Rad));

        //this is what normally gets passed to opencv function initUndistortRectifyMap()
        float3x3 rectifiedCamMatrices = new float3x3(cameraFocalLength,0, (float)imageSize.Width / 2,
                                                    0, cameraFocalLength, (float)imageSize.Height / 2,
                                                    0,0,1);

        //we get the inverse here rather than doing it in shader since we only have to do it once
        float3x3 inputMatrix = math.inverse(rectifiedCamMatrices);

        SetCameraMatrixValues(camCalibration, inputMatrix);
    }

    void SetCameraMatrixValues(ArucoCameraParameters camCalibration, float3x3 inputMatrix) {

        undistortMat.SetVector("_InverseMatrix", new Vector4(inputMatrix[0][0], inputMatrix[2][0], inputMatrix[2][1], 0));

        undistortMat.SetFloat("_F1", (float)camCalibration.CameraMatricesValues[0][0][0]);
        undistortMat.SetFloat("_F2", (float)camCalibration.CameraMatricesValues[0][1][1]);

        undistortMat.SetFloat("_OX", (float)camCalibration.CameraMatricesValues[0][0][2]);
        undistortMat.SetFloat("_OY", (float)camCalibration.CameraMatricesValues[0][1][2]);

        undistortMat.SetFloat("_S", (float)camCalibration.CameraMatricesValues[0][0][1]);

        undistortMat.SetFloat("_K1", (float)camCalibration.DistCoeffsValues[0][0][0]);
        undistortMat.SetFloat("_K2", (float)camCalibration.DistCoeffsValues[0][0][1]);
        undistortMat.SetFloat("_P1", (float)camCalibration.DistCoeffsValues[0][0][2]);
        undistortMat.SetFloat("_P2", (float)camCalibration.DistCoeffsValues[0][0][3]);
        undistortMat.SetFloat("_Omni", (float)camCalibration.OmnidirXisValues[0][0][0]);
    }
}

