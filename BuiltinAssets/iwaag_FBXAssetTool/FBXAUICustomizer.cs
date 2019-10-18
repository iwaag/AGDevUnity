using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using AGDevUnity;
using AGAsset;
using System.Collections;
using AGDev;
using AGAsset.StdUtil;

namespace iwaag {
	public class FBXAUICustomizer : MonoBAUICustomizer {
		[Serializable]
		public class FBXAssetGiver : StdAUICustomizer<GameObject, GameObject> {};
		public FBXAssetGiver pickerInstance;
		public override ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> assetCustomizer => pickerInstance;
	}
}