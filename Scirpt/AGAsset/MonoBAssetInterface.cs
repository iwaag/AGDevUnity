using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDev;
using AGAsset;

namespace AGDevUnity {
	public abstract class MonoBAssetInterface : MonoBehaviour {
		public abstract ImmediateGiver<AssetUnitInterface, AssetUnitInfo> assetInterface { get; }
	}
}
