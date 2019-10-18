using UnityEngine;
using System.Collections;
using AGDev;
using AGDevUnity;
using System;
using System.Collections.Generic;
using AGAsset;
using AGBLang;

namespace AGDevUnity {
	public abstract class MonoBAssetMediator : MonoBehaviour {
		public abstract AssetMediator assetMed { get; }
	}
}
