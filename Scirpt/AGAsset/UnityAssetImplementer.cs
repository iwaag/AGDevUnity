using UnityEngine;
using System.Collections.Generic;
using AGDev;
using AGDev.StdUtil;
using AGAsset;

namespace AGDevUnity.StdUtil {
	public interface AssetImplementationHelper {
		void ImplementCommonAsset(AssetReferInterface assetReferInterface);
	}
	public class StdAssetImplementationHelper: AssetImplementationHelper {
		public Taker<TextAsset> erwcTaker;
		public Taker<TextAsset> dictionaryTaker;
		public Taker<GameObject> scenePrepTaker;
		public void ImplementInternal<AssetType>(Taker<AssetType> collecor, AssetReferInterface assetReferInterface, IEnumerable<string> textAssets){
			if (textAssets != null) {
				foreach (var erwcFileName in textAssets) {
					assetReferInterface.PickContent(erwcFileName, collecor);
				}
			}
		}
		void AssetImplementationHelper.ImplementCommonAsset(AssetReferInterface assetReferInterface) {
			//ImplementInternal(erwcTaker, assetReferInterface, commonAssetRef.erws);
			//ImplementInternal(dictionaryTaker, assetReferInterface, commonAssetRef.dictionaries);
			//ImplementInternal(scenePrepTaker, assetReferInterface, commonAssetRef.scenePreps);
		}
	}
	public enum AssetNameType {
		ByFirstAttribute,
		ByAssetType
	}
}