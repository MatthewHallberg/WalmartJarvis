using UnityEngine.Video;
using UnityEngine;
using System.Collections;

public class VideoScreen : ScreenBehavior {

    public VideoPlayer videoPlayer;

    public void LoadVideo(string url) {
        Reset();
        StartCoroutine(PlayVideoRoutine(url));
    }

    IEnumerator PlayVideoRoutine(string url) {
        videoPlayer.url = url;
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared) {
            yield return new WaitForSeconds(.3f);
        }

        videoPlayer.Play();
        Open();
    }
}
