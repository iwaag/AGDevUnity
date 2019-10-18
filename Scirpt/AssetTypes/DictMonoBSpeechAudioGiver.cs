using AGDev;
using System.Collections.Generic;
using UnityEngine;

namespace AGDevUnity.StdUtil {
	public class DictMonoBSpeechAudioGiver : MonoBSpeechAudioGiver, Giver<AudioClip, SpeechSetting> {
		public override Giver<AudioClip, SpeechSetting> speechAudioGiver => this;
		public List<NameAndURL> nameAndURL;
		void Giver<AudioClip, SpeechSetting>.Give(SpeechSetting request, Taker<AudioClip> taker) {
			var found = nameAndURL.Find((elem) => request.speech.Equals(elem.name, System.StringComparison.CurrentCultureIgnoreCase) );
			if (!string.IsNullOrEmpty(found.url)) {
				NetworkUtil.DownloadAudio(found.url, taker);
			}
		}
		[System.Serializable]
		public struct NameAndURL {
			public string name;
			public string url;
		}
	}
}
