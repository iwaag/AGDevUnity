using AGAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class FixedMonoBAUInterface : MonoBAUInterface {
		public MonoBAssetInterface assetInterface;
		public AssetUnitInfo erwcAUInfo;
		public override AssetUnitInterface auInterface {
			get {
				if (_auInterface == null) {
					_auInterface = assetInterface.assetInterface.PickBestElement(erwcAUInfo);
				}
				return _auInterface;
			}
		}
		public AssetUnitInterface _auInterface;
	}
}