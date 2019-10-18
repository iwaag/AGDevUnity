using AGDev;
using AGAsset;
using AGDevUnity;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AGAsset.StdUtil;

namespace iwaag {
	public class FBXAUIntegrator : MonoBAUIntegrator, AssetUnitIntegrator {
		public override AssetUnitIntegrator assetUnitInteg => this;

		void AssetUnitIntegrator.IntegrateAssetUnit(AssetRequestUnit request, AssetUnitIntegrateListener listener) {
			if (!AssetUtils.IsRequestedType(request, "BAgent")) {
				return;
			}
			var support = listener.OnBeginIntegrate();
			var coll = new PrvtColl { support = support, listener = listener, request = request };
			support.integrantGiver.Give(AssetUtils.ChangeAssetRequestType(request, "FBX"), coll);

		}
		class PrvtColl : Taker<AssetUnitInterface>, Taker<GameObject> {
			public AssetUnitIntegrateSupport support;
			public AssetUnitIntegrateListener listener;
			public AssetRequestUnit request;
			void Taker<AssetUnitInterface>.Take(AssetUnitInterface auInterface) {
				auInterface.referer.PickContent("", this as Taker<GameObject>);
			}
			void Taker<GameObject>.Take(GameObject fbxModelPrefab) {
				var modelInstance = GameObject.Instantiate(fbxModelPrefab);
				support.generatedAssetInterface.modifier.SetContent(
					new AssetContentSettingParam<GameObject> {
						content = modelInstance,
						contentPath = "",
						doOverwrite = true
					},
					new StubAssetInResultListener<GameObject>()
				);
				//GameObject.DestroyImmediate(modelInstance);
				support.OnSucceed();
			}

			void Taker<AssetUnitInterface>.None() {
				support.OnFail();
			}

			void Taker<GameObject>.None() {
				support.OnFail();
			}
		}
	}
}
