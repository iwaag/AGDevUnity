using AGAsset;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset.StdUtil;
namespace AGDevUnity.StdUtil {
	public class RootAssetIntegrator : MonoBAUIntegrator, AssetUnitIntegrator {
		public override AssetUnitIntegrator assetUnitInteg {
			get {
				return this;
			}
		}
		public MonoBAssetMediator assetMed;
		void AssetUnitIntegrator.IntegrateAssetUnit(AssetRequestUnit reqUnit, AssetUnitIntegrateListener listener) {
			
			var mnoBIntegs = assetMed.assetMed.GetImplementedAssets<MonoBAUIntegrator>();
			if (mnoBIntegs == null) {
				return;
			}
			var integrators = new List<AssetUnitIntegrator>();
			foreach (var integrator in mnoBIntegs) {
				integrators.Add(integrator.assetUnitInteg);
			}
			AssetUnitIntegrator assetIntegrator = new ClusterAssetUnitIntegrator { Integrators = integrators };
			assetIntegrator.IntegrateAssetUnit(reqUnit, listener);
			
		}
	}
}