using UnityEngine;

public class Jarvis : MonoBehaviour {

    public Speech speech;
    public Terminal terminal;
    bool intialized;

    void OnEnable() {
        Speech.keywordRecognized += OnKeywordRecognized;
        Speech.speechRecognized += OnSpeechRecognized;
        Terminal.initialized += OnInitializedStatusRecieved;
    }

    void OnDisable() {
        Speech.keywordRecognized -= OnKeywordRecognized;
        Speech.speechRecognized -= OnSpeechRecognized;
        Terminal.initialized -= OnInitializedStatusRecieved;
    }

    void Start() {
        speech.StartSpeechRoutine();
    }

    void OnChatBotRequestReceived(BotData botData) {

        switch (botData.intent) {
            case "YouTube":
                speech.SpeakWords("loading video...");
                YouTubePlayer.Instance.PlayVideo(botData.value);
                break;
            case "website":
                speech.SpeakWords("loading website...");
                break;
            default:
                speech.SpeakWords(botData.speech);
                break;
        }
    }

    void OnInitializedStatusRecieved(bool status) {
        intialized = status;
        string feedback = status ? Config.INITIALIZED : Config.FAILED;
        speech.SpeakWords(feedback);
    }

    void OnKeywordRecognized() {
        if (intialized) {
            speech.SpeakWords(Config.GREETING);
        } else {
            speech.SpeakWords(Config.LOADING);
            terminal.StartTerminal();
        }
    }

    void OnSpeechRecognized(string speech) {
        print(speech);
        API.Instance.MakeChatBotRequest(speech, OnChatBotRequestReceived);
    }

    string RemoveGreeting(string text) {
        int index = text.IndexOf(Config.GREETING);
        return (index < 0) ? text : text.Remove(index, Config.GREETING.Length);
    }
}
