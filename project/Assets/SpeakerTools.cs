using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice.Tool;
using Crosstales.RTVoice;

public class SpeakerTools : MonoBehaviour {
	public SpeechText SpeechText;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SpeechText.Speak();
		}
	}
}
