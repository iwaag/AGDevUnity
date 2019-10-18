using AGAsset;
using AGDev.StdUtil;
using System.Collections;
using System.Collections.Generic;
using AGAsset.StdUtil;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class RootAssetSupplier : MonoBAUSupplier {
		public ObservedProcessHelper observeHelper = new ObservedProcessHelper();
		public override AssetUnitSupplier assetUnitSupplier {
			get {
				if (rootSup == null) {
					var suppliers = new List<MonoBAUSupplier>();
					foreach (var supplier in GetComponentsInChildren<MonoBAUSupplier>()) {
						if (supplier != this)
							suppliers.Add(supplier);
					}
					var supplierEnumerable = new ConvertingEnumarable<AssetUnitSupplier, MonoBAUSupplier> { convertFunc = (elem) => elem.gameObject.activeInHierarchy ? elem.assetUnitSupplier : null, sourceEnumerable = suppliers };
					var lineUp = new AssetUnitSupplierLineup { suppliers = supplierEnumerable };
					var observed = new ObservedAssetSupplier { clientSupllier = lineUp, helper = observeHelper };
					//var recSup = new RecursiveAssetUnitSupplier { integrantSupplier = lineUp };
					rootSup = observed;
				}
				return rootSup;
			}
		}
		public AssetUnitSupplier rootSup;
	}
}
