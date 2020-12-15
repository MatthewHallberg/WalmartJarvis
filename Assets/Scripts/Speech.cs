using SpeechLib;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Speech : MonoBehaviour {

    const string GREETING = "yes sir";

    SpVoice voice = new SpVoice();
    KeywordRecognizer keywordRecognizer;
    DictationRecognizer m_DictationRecognizer;
    bool isSpeaking;

    void Start() {
        //setup keyword reecognizer
        keywordRecognizer = new KeywordRecognizer(new string[] { "jarvis" }, ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

        //setup dictation recognizer
        m_DictationRecognizer = new DictationRecognizer();
        m_DictationRecognizer.DictationResult += (text, confidence) => {
            StartCoroutine(OnSpeechResult(text));
        };
        m_DictationRecognizer.DictationComplete += (completionCause) => {
            if (completionCause != DictationCompletionCause.Complete)
                StartCoroutine(OnSpeechResult("error"));
        };
        m_DictationRecognizer.DictationError += (error, hresult) => {
            StartCoroutine(OnSpeechResult("error"));
        };

        //start listening for first keyword
        ListenForKeyword();
    }

    void SpeakWords(string words) {
        voice.Speak(words, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        isSpeaking = true;
    }

    // this gets called late so we cant start listening here
    void OnDoneSpeaking() {
        isSpeaking = false;
    }

    void ListenForKeyword() {
        PhraseRecognitionSystem.Restart();
        keywordRecognizer.Start();
        print("Waiting...");
    }

    void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
        SpeakWords(GREETING);
        PhraseRecognitionSystem.Shutdown();
        ListenForSpeech();
    }

     void ListenForSpeech() {
        m_DictationRecognizer.Start();
        print("Listening...");
    }

    IEnumerator OnSpeechResult(string speech) {

        if (speech.Length > 1 && speech != "error") {
            speech = RemoveGreeting(speech);
            print(speech);
        } else {
            print("try again");
        }

        m_DictationRecognizer.Stop();

        yield return new WaitForSeconds(1f);
        ListenForKeyword();

    }

    string RemoveGreeting(string text) {
        int index = text.IndexOf(GREETING);
        return (index < 0) ? text : text.Remove(index, GREETING.Length);
    }

    void Update() {
        if (isSpeaking && voice.Status.RunningState == SpeechRunState.SRSEDone) {
            OnDoneSpeaking();
        }
    }
}
