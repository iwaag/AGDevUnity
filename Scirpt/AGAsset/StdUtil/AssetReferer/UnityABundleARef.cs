using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDevUnity;
using UnityEngine.Networking;
using AGAsset.StdUtil;
using AGAsset;

namespace AGDevUnity.StdUtil{
	public class UnityABundleARef : MonoBAssetReferer, AssetReferer {
		public class AssetBundleRef {
			public string uri;
			public string assetPath;
		};
		public override AssetReferer assetReferer => this;
		void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
			if (assetUnitInfo.reference.StartsWith("unityab:")) {
				listener.OnBeginRefering();
				var json = assetUnitInfo.reference.Substring(8);
				var abRef = RequiredFuncs.FromJson<AssetBundleRef>(json);
				NetworkUtil.ProcessWebRequest(
					UnityWebRequestAssetBundle.GetAssetBundle(abRef.uri, 0),
					(www) => {
						var bundle = DownloadHandlerAssetBundle.GetContent( www );
						if (string.IsNullOrEmpty(abRef.assetPath)) {
							//stub
						} else {
							var bundleAsset = bundle.LoadAsset(abRef.assetPath);
							if (bundleAsset != null) {
								listener.OnObjectObtained(bundleAsset, abRef.assetPath);
							}
						}
						listener.OnFinish();
					}
				);
			}
		}
	}
}