using AGAsset;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleImgARef : MonoBAssetReferer, AssetReferer {
	public override AssetReferer assetReferer => this;
	[System.Serializable]
	public class GoogleImageItem {
		public GoogleImageImage image;
	}
	[System.Serializable]
	public class GoogleImageImage {
		public string thumbnailLink;
	}
	[System.Serializable]
	public class GoogleImageSearchResult {
		public List<GoogleImageItem> items;
	}
	[System.Serializable]
	public class SearchCache{
		public AssetUnitInfo auInfo;
		public Sprite sprite;
	}
	[System.NonSerialized]
	public string apiKey;
	public List<SearchCache> caches;
	void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
		var refElements = assetUnitInfo.reference.Split(':');
		if (string.Compare(refElements[0], "GSearch", true) != 0) {
			return;
		}
		listener.OnBeginRefering();
		var chache = caches.Find((cache) => string.Compare( cache.auInfo.reference, assetUnitInfo.reference, true) == 0);
		if (chache != null) {
			listener.OnAssetContentObtained(chache.sprite,"");
			listener.OnFinish();
			return;
		}
		NetworkUtil.ProcessWebRequest(
			new UnityWebRequest("https://www.googleapis.com/customsearch/v1?key=" + "&searchType=image&q=" + refElements[1]),
			(doneWWW1) => {
				if (doneWWW1.error == null) {
					var result = JsonUtility.FromJson<GoogleImageSearchResult>(doneWWW1.downloadHandler.text);
					NetworkUtil.ProcessWebRequest(
						new UnityWebRequest(result.items[0].image.thumbnailLink),
						(doneWWW2) => {
							if (doneWWW2.error == null) {
								var texture = DownloadHandlerTexture.GetContent(doneWWW2);
								var sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(0.5f, 0.5f), System.Math.Max(texture.width, texture.height));
								caches.Add(new SearchCache { auInfo = assetUnitInfo, sprite = sprite });
								listener.OnAssetContentObtained(sprite, "");
							}
							listener.OnFinish();
						}
					);
				} else
					listener.OnFinish();
			}
		);
	}
}
