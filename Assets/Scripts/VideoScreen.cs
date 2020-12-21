using UnityEngine.Video;
using UnityEngine;
using System.Collections;

public class VideoScreen : MonoBehaviour {

    const float SCALE_SPEED = 6f;

    public VideoPlayer videoPlayer;

    Vector3 startScale;
    Vector3 desiredScale;

    void Awake() {
        startScale = transform.localScale;
        desiredScale = Vector3.zero;
        transform.localScale = Vector3.zero;
    }

    public void LoadVideo(string url) {
        StartCoroutine(PlayVideoRoutine(url));
    }

    IEnumerator PlayVideoRoutine(string url) {
        videoPlayer.url = url;
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared) {
            yield return new WaitForSeconds(.3f);
        }

        videoPlayer.Play();
        desiredScale = startScale;
    }


    void Update() {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * SCALE_SPEED);
    }
}
