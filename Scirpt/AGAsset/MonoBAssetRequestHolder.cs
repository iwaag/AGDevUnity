using AGAsset;
using UnityEngine;

namespace AGDevUnity {
	public abstract class MonoBAssetRequestHolder : MonoBehaviour {
		public abstract AssetRequest assetRequest { get; }
	}
}
