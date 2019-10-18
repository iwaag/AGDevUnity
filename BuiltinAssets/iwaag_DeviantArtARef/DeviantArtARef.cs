using AGAsset;
using AGDevUnity;
using AGDev.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace iwaag {
	public class DeviantArtARef : MonoBAssetReferer, AssetReferer {
		[System.NonSerialized]
		public string clientID;
		[System.NonSerialized]
		public string clientSecret;
		public override AssetReferer assetReferer => this;
		void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
			if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret) ) {
				return;
			}
			//asume like "deviantart:{GUID}"
			string[] assetRef = assetUnitInfo.reference.Split(':');
			if (System.Collections.CaseInsensitiveComparer.Equals(assetRef[0], "deviantart")) {
				listener.OnBeginRefering();
				var webReq = new UnityWebRequest("https://www.deviantart.com/oauth2/token?grant_type=client_credentials&client_id=" + clientID + "&client_secret=" + clientSecret);
				webReq.downloadHandler = new DownloadHandlerBuffer();
				NetworkUtil.ProcessWebRequest(
					webReq,
					(www) => {
						if (www.error != null) {
							listener.OnFinish();
							return;
						}
						var token = JsonUtility.FromJson<DeviantArtToken>(www.downloadHandler.text);
						var webReq2 = new UnityWebRequest("https://www.deviantart.com/api/v1/oauth2/deviation/download/" + assetRef[1] + "?token=" + token.access_token);
						webReq2.downloadHandler = new DownloadHandlerBuffer();
						NetworkUtil.ProcessWebRequest(
							webReq2,
							(www2) => {
								if (www2.error != null) {
									listener.OnFinish();
									return;
								}
								var dl = JsonUtility.FromJson<DeviantArtDownload>(www2.downloadHandler.text);
								NetworkUtil.ProcessWebRequest(UnityWebRequestTexture.GetTexture(dl.src), (www3) => {
									if (www3.error != null) {
										listener.OnFinish();
										return;
									}
									Debug.Log("Deviant art texture obtained.");
									var texture = DownloadHandlerTexture.GetContent(www3);
									listener.OnAssetContentObtained(texture, "");
									listener.OnFinish();
								});
							});

					}
				);
			}
		}

		[System.Serializable]
		class DeviantArtToken {
			public string access_token = null;
			public string token_type = null;
			public int expires_in = 0;
			public string staus = null;
		}
		[System.Serializable]
		class DeviantArtDownload {
			public string src = null;
			public int width = 0;
			public int height = 0;
			public int filesize = 0;
		}
	}
}
