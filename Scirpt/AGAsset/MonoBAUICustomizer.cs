using AGAsset;
using AGDev;
using System.Collections.Generic;
using UnityEngine;

namespace AGDevUnity {
	public abstract class MonoBAUICustomizer : MonoBehaviour {
		public abstract ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> assetCustomizer { get; }
	}
}
