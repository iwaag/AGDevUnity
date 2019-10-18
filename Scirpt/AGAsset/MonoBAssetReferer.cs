using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset;
namespace AGDevUnity {
	public abstract class MonoBAssetReferer : MonoBehaviour {
		public abstract AssetReferer assetReferer { get; }
	}
}