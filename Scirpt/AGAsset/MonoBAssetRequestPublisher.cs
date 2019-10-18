using AGAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBAssetRequestPublisher : MonoBehaviour {
		public abstract AssetRequestPublisher assetRequestPublisher { get; }
	}
}