using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;

public class API : Singleton<API> {

    const string CHATBOT = "http://127.0.0.1:3000/";
    const string YOUTUBE = "http://127.0.0.1:3001/";

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

    public void GetYouTubeVideo(string videoSearch, UnityAction<string> callback) {
        StartCoroutine(GetYouTubeLinkRoutine(videoSearch, callback));
    }

    IEnumerator GetYouTubeLinkRoutine(string videoSearch, UnityAction<string> callback) {
        UnityWebRequest www = UnityWebRequest.Post(YOUTUBE, videoSearch);
        yield return www.SendWebRequest();
        string videoURL = www.downloadHandler.text;
        if (videoURL.Contains("manifest")) {
            print("ERROR: " + videoURL);
        } else {
            callback(videoURL);
        }
    }

    public void MakeChatBotRequest(string message, UnityAction<BotData> callback) {
        StartCoroutine(GetResponseFromChatbot(message, callback));
    }

    IEnumerator GetResponseFromChatbot(string message, UnityAction<BotData> callback) {
        if (message.Length > 1) {
            Debug.Log("Making Request...");
            UnityWebRequest www = UnityWebRequest.Post(CHATBOT, message);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string response = www.downloadHandler.text;
                print("RESPONSE: " + response);
                callback(JsonUtility.FromJson<BotData>(response));
            }
        }
    }
}
