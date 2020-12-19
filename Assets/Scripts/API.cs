using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;

public class API : Singleton<API> {

    const string ENDPOINT = "http://127.0.0.1:3000/";
    const string YOUTUBE_ENDPOINT = "http://ec2-3-135-248-32.us-east-2.compute.amazonaws.com/GetYouTubeLink.php/?url=";

    [Serializable]
    public class VideoInfo {
        public string quality;
        public string url;
        public string itag;
        public string type;
    }

    [Serializable]
    public class VideoInfoCollection {
        public VideoInfo[] videoInfoCollection;
    }

    public void LoadVideo(string url, UnityAction<string> callback) {
        StartCoroutine(GetYouTubeLinkRoutine(url, callback));
    }

    IEnumerator GetYouTubeLinkRoutine(string url, UnityAction<string> callback) {
        UnityWebRequest www = UnityWebRequest.Get(YOUTUBE_ENDPOINT + url);
        yield return www.SendWebRequest();
        string videoURL = www.downloadHandler.text;
        print("YT RESPONSE: " + videoURL);
        if (videoURL.Contains("https")) {
            callback(www.downloadHandler.text);
        } else {
            Debug.Log("Can't play copyright videos...");
        }
    }

    public void MakeChatBotRequest(string message, UnityAction<BotData> callback) {
        StartCoroutine(GetResponseFromChatbot(message, callback));
    }

    IEnumerator GetResponseFromChatbot(string message, UnityAction<BotData> callback) {
        if (message.Length > 1) {
            Debug.Log("Making Request...");
            UnityWebRequest www = UnityWebRequest.Post(ENDPOINT, message);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string response = www.downloadHandler.text;
                print("RESPONSE: " + response);
                if (response.Length > 0) {
                    callback(JsonUtility.FromJson<BotData>(response));
                }
            }
        }
    }
}
