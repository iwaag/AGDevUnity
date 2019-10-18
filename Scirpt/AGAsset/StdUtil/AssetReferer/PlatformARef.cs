using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDevUnity;
using AGAsset.StdUtil;
using AGAsset;

namespace AGDevUnity.StdUtil{
	public class PlatformRef {
		public string source;
		public string defaultBuild;
	};
	public class PlatformARef : MonoBAssetReferer, AssetReferer {
		public override AssetReferer assetReferer => this;
		public MonoBAssetReferer baseReferer;
		void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
			if (assetUnitInfo.reference.StartsWith("platform:")) {
				var json = assetUnitInfo.reference.Substring(9);
				var pRef = RequiredFuncs.FromJson<PlatformRef>(json);
				var auInfoClone = AssetUtils.CloneAssetRef(assetUnitInfo);
				auInfoClone.reference = pRef.defaultBuild;
				baseReferer.assetReferer.ReferAsset(auInfoClone, listener);
			}
		}
	}
}