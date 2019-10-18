using UnityEngine;
using AGDev;
using AGAsset;

namespace AGDevUnity {
	public abstract class MonoBRequestAssetInterface : MonoBehaviour {
		public abstract Giver<AssetUnitInterface, AssetRequestUnit> reqAssetItfcGiver { get; }
	}
}
