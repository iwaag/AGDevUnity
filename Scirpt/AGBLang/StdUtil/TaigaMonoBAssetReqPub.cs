using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset;
using System;
using AGAsset.StdUtil;
namespace AGDevUnity.StdUtil {
	public class TaigaMonoBAssetReqPub : MonoBAssetRequestPublisher {
		public TaigaIOAssetRequestPublisher impl;
		public override AssetRequestPublisher assetRequestPublisher { get { return impl; } }
	}
}