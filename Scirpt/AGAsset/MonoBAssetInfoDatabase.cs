using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGAsset {
	public abstract class MonoBAssetInfoDatabase : MonoBehaviour {
		public abstract AssetInfoDatabase assetInfoDatabase { get; }
	}
}