using AGDevUnity;
using AGDev;
using AGBLang;
using AGAsset;

namespace AGDevUnity.StdUtil {
	public class RootAssetImplementer : MonoBAssetImplementer, AssetImplementerGetter {
		public MonoBAssetMediator assetMed;

		public override AssetImplementerGetter assetImplGetter => this;

		AssetImplementer<AssetType> AssetImplementerGetter.GetAssetImplementer<AssetType>(GrammarBlock gBlock) {
			var assetImpls = assetMed.assetMed.GetImplementedAssets<MonoBAssetImplementer>();
			foreach (var assetImpl in assetImpls) {
				var impl = assetImpl.assetImplGetter.GetAssetImplementer<AssetType>(gBlock);
				if (impl != null) {
					return impl;
				}
			}
			foreach (var assetImpl in GetComponentsInChildren<MonoBAssetImplementer>()) {
				if (assetImpl != this) {
					var impl = assetImpl.assetImplGetter.GetAssetImplementer<AssetType>(gBlock);
					if (impl != null) {
						return impl;
					}
				}
			}
			return null;

		}
	}
}