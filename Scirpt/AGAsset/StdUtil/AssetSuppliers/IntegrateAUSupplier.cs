using AGAsset;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset.StdUtil;
namespace AGDevUnity.StdUtil {
	public class IntegrateAUSupplier : MonoBAUSupplier {
		public MonoBAUIntegrator assetInteg;
		public MonoBAssetInterface assetInterface;
		public MonoBAssetBasicIO generatedIO;
		public override AssetUnitSupplier assetUnitSupplier {
			get {
				if (_assetUnitSupplier == null) {
					_assetUnitSupplier = new AssetUnitIntegrateSupplier {
						Integrator = assetInteg.assetUnitInteg,
						generatedAUInterfaceGiver = assetInterface.assetInterface,
						generatedIO = generatedIO
					};
				}
				return _assetUnitSupplier;
			}
		}
		AssetUnitIntegrateSupplier _assetUnitSupplier;
	}
}