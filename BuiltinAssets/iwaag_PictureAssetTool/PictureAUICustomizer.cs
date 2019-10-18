using System;
using UnityEngine;
using AGDev;
using AGAsset.StdUtil;
using AGAsset;

namespace AGDevUnity {
	public class PictureAUICustomizer : MonoBAUICustomizer {
		[Serializable]
		public class PictureAssetGiver : StdAUICustomizer<Texture2D, Texture2D> {};
		public PictureAssetGiver pickerInstance;
		public override ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> assetCustomizer => pickerInstance;
	}
}