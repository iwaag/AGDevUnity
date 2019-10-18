using AGAsset;
using UnityEngine;

namespace AGDevUnity {
	public abstract class MonoBAUIntegrator: MonoBehaviour {
		public abstract AssetUnitIntegrator assetUnitInteg { get; }
	}
}
