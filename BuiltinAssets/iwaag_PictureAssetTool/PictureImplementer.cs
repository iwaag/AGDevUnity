using AGAsset;
using AGDevUnity;
using AGDev;
using AGBLang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset.StdUtil;
namespace AGDevUnity.StdUtil {
	public class PictureImplementer : MonoBAssetImplementer, AssetImplementerGetter {
		public override AssetImplementerGetter assetImplGetter => this;
		[System.Serializable]
		public class NamedPicture : NamedAsset<Sprite> {}
		[System.Serializable]
		public class PictureCustomImpl : SimpleAssetImplementCustomizer<Sprite, NamedPicture> {
			public override NamedPicture IntegrateImplementType<ParamAssetType>(ParamAssetType asset, AssetUnitInfo auInfo, AssetRequestUnit assetReqUnit) {
				if (typeof(ParamAssetType) == typeof(Sprite)) {
					return new NamedPicture { asset = (Sprite)(object)asset, name = AssetUtils.ExtractPackedAssetAttribute(auInfo.attributes)[0] };
				}
				return null;
			}
		}
		public PictureCustomImpl customzier;
		AssetImplementer<AssetType> AssetImplementerGetter.GetAssetImplementer<AssetType>(GrammarBlock gBlock) {
			if (typeof(AssetType) == typeof(Sprite)) {
				return new StdAssetImplementer<AssetType, NamedPicture> { customizer = customzier };
			}
			return null;
		}
	}
}
