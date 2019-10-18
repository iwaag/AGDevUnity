using AGAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBAUBasicIO : MonoBehaviour {
		public abstract AssetUnitBasicIO assetUnitIO { get; }
	}
}