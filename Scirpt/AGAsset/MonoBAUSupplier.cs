using AGAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBAUSupplier : MonoBehaviour {
		public abstract AssetUnitSupplier assetUnitSupplier { get; }
	}
}