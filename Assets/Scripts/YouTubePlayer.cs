using UnityEngine;

public class YouTubePlayer : Singleton<YouTubePlayer> {

    public Transform spawnPoint;
    public GameObject videoPrefab;

    public void PlayVideo(string searchWords) {
        API.Instance.GetYouTubeVideo(searchWords, OnVideoLinkReturned);
    }

    void OnVideoLinkReturned(string videoURL) {
        GameObject prefab = Instantiate(videoPrefab, spawnPoint.position, spawnPoint.rotation);
        VideoScreen videoScreen = prefab.GetComponent<VideoScreen>();
        videoScreen.LoadVideo(videoURL);
    }
}
