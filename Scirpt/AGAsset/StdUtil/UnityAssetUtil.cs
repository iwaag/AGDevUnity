using AGAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AGDevUnity.StdUtil{
	class UnityAssetUtil {
		public static Transform _inSceneAssetWorkspace;
		public static Transform inSceneAssetWorkspace {
			get {
				if(_inSceneAssetWorkspace == null) {
					_inSceneAssetWorkspace = new GameObject().transform;
					_inSceneAssetWorkspace.name = "AssetWorkspace";
					_inSceneAssetWorkspace.gameObject.SetActive(false);
				}
				return _inSceneAssetWorkspace;
			}
		}
		public static void PickFromDownloadLink<ContentType>(string url, AssetBasicTaker<ContentType> collector) {
			UnityWebRequest www = null;
			if (typeof(ContentType) == typeof(Texture2D)) {
				www = UnityWebRequestTexture.GetTexture(url);
			}
			else {
				www = UnityWebRequest.Get(url);
			}
			NetworkUtil.ProcessWebRequest(www, (givenWWW) => {
				if (string.IsNullOrEmpty(givenWWW.error)) {
					if (typeof(ContentType) == typeof(byte[]))
						collector.CollectAsset((ContentType)(object)givenWWW.downloadHandler.data);
					else if (typeof(ContentType) == typeof(AudioClip))
						collector.CollectAsset((ContentType)(object)DownloadHandlerAudioClip.GetContent(givenWWW));
					else if (typeof(ContentType) == typeof(Texture2D)) {
						var texture = DownloadHandlerTexture.GetContent(givenWWW);
						Debug.Log(texture);
						collector.CollectAsset((ContentType)(object)texture);
					}
					else
						collector.CollectRawAsset(givenWWW.downloadHandler.data);
				}
				collector.OnFinish();
			});
		}
	}
	public class DirectLinkReferer : AssetReferer {
		void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
			if (assetUnitInfo.reference.StartsWith("http")) {
				listener.OnBeginRefering();
				UnityAssetUtil.PickFromDownloadLink(assetUnitInfo.reference, new PrvtColl{ listener = listener });
			}
		}
		public class PrvtColl : AssetBasicTaker<byte[]> {
			public AssetReferenceListener listener;
			void AssetBasicTaker<byte[]>.CollectAsset(byte[] assetType) {
				listener.OnRawAssetContentObtained(assetType, "");
			}

			void AssetBasicTaker<byte[]>.CollectRawAsset(byte[] assetType) {
				listener.OnRawAssetContentObtained(assetType, "");
			}

			void AssetBasicTaker<byte[]>.OnFinish() {
				listener.OnFinish();
			}
		}
	}
	public class StdArchiveReferer : AssetReferer {
		void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
			if (assetUnitInfo.reference.Contains("agas.zip")) {
				listener.OnBeginRefering();
				var www = new UnityWebRequest(assetUnitInfo.reference);
				NetworkUtil.ProcessWebRequest(www, (doneWWW)=> {
					listener.OnStdArchiveObtained(doneWWW.downloadHandler.data);
					listener.OnFinish();
				});
			} else {
				listener.OnFinish();
			}
		}
	}
}