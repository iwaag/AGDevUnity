using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset.StdUtil;
using AGAsset;

namespace AGDevUnity.StdUtil {
	public class HTML_AUSupplier : MonoBAUSupplier, AssetUnitSupplier {
		class PrvtLis : AssetUnitSupplyListener, Taker<AssetUnitInfo> {
			public HTML_AUSupplier parent;
			public AssetUnitSupplyListener listener;
			Taker<AssetUnitInfo> AssetUnitSupplyListener.supplyTaker => this;

			Giver<AssetUnitInterface, AssetRequestUnit> AssetUnitSupplyListener.integrantGiver => listener.integrantGiver;

			void Taker<AssetUnitInfo>.Take(AssetUnitInfo newElement) {
				parent.obtainedAssets.Add(newElement);
				listener.supplyTaker.Take(newElement);
			}
			void Taker<AssetUnitInfo>.None() {
				listener.supplyTaker.None();
			}
		}
		public HTMLAssetSupplier supplier;
		public List<AssetUnitInfo> obtainedAssets;
		public override AssetUnitSupplier assetUnitSupplier => bridgeSupplier;
		public BridgeUnitAssetSupplier bridgeSupplier {
			get {
				if(_bridgeSupplier.supplier == null) {
					_bridgeSupplier.supplier = supplier;
				}
				return _bridgeSupplier;
			}
		}
		public BridgeUnitAssetSupplier _bridgeSupplier;
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			assetUnitSupplier.SupplyAssetUnit(assetRequest, new PrvtLis { listener = listener, parent = this });
		}
	}
}