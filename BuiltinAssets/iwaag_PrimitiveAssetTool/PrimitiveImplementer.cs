using AGAsset;
using AGBLang;
using AGDevUnity;
using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset.StdUtil;
namespace AGDevUnity.StdUtil {
	public class PrimitiveImplementer : MonoBAssetImplementer, AssetImplementerGetter {
		public override AssetImplementerGetter assetImplGetter => this;
		[System.Serializable]
		public class NamedPrimitive: NamedAsset<SerializedPrimitive> {}
		[System.Serializable]
		public class PictureCustomImpl : SimpleAssetImplementCustomizer<SerializedPrimitive, NamedPrimitive> {
			public override NamedPrimitive IntegrateImplementType<ParamAssetType>(ParamAssetType asset, AssetUnitInfo auInfo, AssetRequestUnit assetReqUnit) {
				if (typeof(ParamAssetType) == typeof(SerializedPrimitive)) {
					return new NamedPrimitive { asset = (SerializedPrimitive)(object)asset, name = AssetUtils.ExtractPackedAssetAttribute(auInfo.attributes)[0] };
				}
				return null;
			}
		}
		public PictureCustomImpl customzier;
		AssetImplementer<AssetType> AssetImplementerGetter.GetAssetImplementer<AssetType>(GrammarBlock gBlock) {
			if (typeof(AssetType) == typeof(SerializedPrimitive)) {
				return new StdAssetImplementer<AssetType, NamedPrimitive> { customizer = customzier };
			}
			return null;
		}
	}
}
