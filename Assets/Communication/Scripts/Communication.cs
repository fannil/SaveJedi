using System;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Communication : MonoBehaviour {

	public KeywordRecognizer speachApi = null;
	public string[] keywords;

	// Use this for initialization
	void Start () {
		if(!this.InitializeCommunication()){
			return;
		}
		Debug.Log("Communication initialized");
	}

	bool InitializeCommunication(){
		if(!PhraseRecognitionSystem.isSupported){
			return false;
		}
		if(this.keywords == null){
			return false;
		}
		foreach(string element in this.keywords){
			Debug.Log(element);
		}
		this.speachApi = new KeywordRecognizer(this.keywords);
		this.speachApi.OnPhraseRecognized += OnVoiceRecognition;
		this.speachApi.Start();
		return true;
	}

	void OnVoiceRecognition(PhraseRecognizedEventArgs args){
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        stringBuilder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        stringBuilder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        Debug.Log(stringBuilder.ToString());
	}

	void OnDestroy(){
		this.speachApi.OnPhraseRecognized -= OnVoiceRecognition;
		this.speachApi.Stop();
	}
}
