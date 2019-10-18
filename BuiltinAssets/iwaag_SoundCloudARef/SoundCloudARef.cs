using AGAsset;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace iwaag {
	public class SoundCloudARef : MonoBAssetReferer, AssetReferer {
		[System.NonSerialized]
		public string clientID;
		public override AssetReferer assetReferer => this;
		void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
			string[] assetRefParts = assetUnitInfo.reference.Split(':');
			if (System.Collections.CaseInsensitiveComparer.Equals(assetRefParts[0], "soundcloud")) {
				listener.OnBeginRefering();
				NetworkUtil.ProcessWebRequest(
					new UnityWebRequest("https://api.soundcloud.com/tracks/" + assetRefParts[1] + "/download?client_id=" + clientID),
					(www) => {
						if (www.error == null) {
							listener.OnAssetContentObtained(DownloadHandlerAudioClip.GetContent(www), "");
						}
						listener.OnFinish();
					}
				);
			}
		}
	}
}
