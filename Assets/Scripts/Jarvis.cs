using UnityEngine;

public class Jarvis : MonoBehaviour {

    public Speech speech;
    void OnEnable() {
        Speech.keywordRecognized += OnKeywordRecognized;
        Speech.speechRecognized += OnSpeechRecognized;
    }

    void OnDisable() {
        Speech.keywordRecognized -= OnKeywordRecognized;
        Speech.speechRecognized -= OnSpeechRecognized;
    }

    void Start() {
        speech.StartSpeechRoutine();
    }

    void OnChatBotRequestReceived(BotData botData) {
        print(botData.speech);
        speech.SpeakWords(botData.speech);
    }

    void OnKeywordRecognized() {
        speech.SpeakWords(Config.GREETING);
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
