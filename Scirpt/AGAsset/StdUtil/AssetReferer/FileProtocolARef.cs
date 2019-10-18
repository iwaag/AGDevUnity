using AGDev;
using AGAsset;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace iwaag {
	public class FileProtocolARef : MonoBAssetReferer, AssetReferer {
		public override AssetReferer assetReferer => this;
		void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
			if (assetUnitInfo.reference.StartsWith("file:///")) {
				listener.OnBeginRefering();
				if ( ! assetUnitInfo.reference.EndsWith("/") ) {;
					UnityWebRequest www = new UnityWebRequest(assetUnitInfo.reference);
					NetworkUtil.ProcessWebRequest(www, (givenWWW) => {
						if (string.IsNullOrEmpty(givenWWW.error))
							listener.OnRawAssetContentObtained(givenWWW.downloadHandler.data, "");
						listener.OnFinish();
					});
				} else {
					//whole repository
					listener.OnBasicIOObtained(new FileProtocolBasicIO { auInfo = assetUnitInfo });
					listener.OnFinish();
				}
				
			}
			string[] assetRef = assetUnitInfo.reference.Split(':');
			if (System.Collections.CaseInsensitiveComparer.Equals(assetRef[0], "folder://")) {
				
					
			}
		}
	}
	public class FileProtocolBasicIO : AssetUnitBasicIO, AssetUnitBasicIn, AssetUnitBasicOut {
		public AssetUnitInfo auInfo;
		public string baseURI;
		AssetUnitBasicIn AssetUnitBasicIO.assetIn => this;
		AssetUnitBasicOut AssetUnitBasicIO.assetOut => this;
		AssetUnitInfo AssetUnitBasicIO.baseAssetUnitInfo => auInfo;
		void AssetUnitBasicOut.PickAssetAtPath<ContentType>(string path, AssetBasicTaker<ContentType> collector) {
			var www = new UnityWebRequest(auInfo.reference + path);
			NetworkUtil.ProcessWebRequest(www, (givenWWW) => {
				if (string.IsNullOrEmpty(givenWWW.error)) {
					if (typeof(ContentType) == typeof(byte[]))
						collector.CollectAsset((ContentType)(object)givenWWW.downloadHandler.data);
					else if (typeof(ContentType) == typeof(AudioClip))
						collector.CollectAsset((ContentType)(object) DownloadHandlerAudioClip.GetContent( givenWWW ));
					else if (typeof(ContentType) == typeof(Texture2D))
						collector.CollectAsset((ContentType)(object) DownloadHandlerTexture.GetContent( givenWWW ));
					else
						collector.CollectRawAsset(givenWWW.downloadHandler.data);
				}
				collector.OnFinish();
			});
		}

		void AssetUnitBasicIn.SetContent<ContentType>(BasicAssetInParam<ContentType> settingParam, AssetInResultListener<ContentType> listener) {
			listener.OnFail();
		}
	}
}