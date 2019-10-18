using AGDev;
using UnityEngine;

namespace AGDevUnity.StdUtil {
	public abstract class MonoBSpeechAudioGiver : MonoBehaviour {
		public abstract Giver<AudioClip, SpeechSetting> speechAudioGiver { get; }
	}
}
