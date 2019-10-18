using AGAsset;
using AGDev.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class ObservedMonoASupp : MonoBAUSupplier {
		public MonoBAUSupplier clientSupplier;
		public ObservedProcessHelper observeHelper = new ObservedProcessHelper();
		public override AssetUnitSupplier assetUnitSupplier => _assetUnitSupplier ?? (_assetUnitSupplier = new ObservedAssetSupplier { clientSupllier = clientSupplier.assetUnitSupplier, helper = observeHelper });
		AssetUnitSupplier _assetUnitSupplier;
	}
}