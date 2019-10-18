using AGAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBAssetImplementer : MonoBehaviour {
		public abstract AssetImplementerGetter assetImplGetter { get; }
	}
}
