using AGAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBAssetBasicIO : MonoBehaviour {
		public abstract AssetBasicIO assetIO { get; }
	}
}