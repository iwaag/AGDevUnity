using UnityEngine;
using System.Collections;
using AGDev;
using AGBLang;
using AGDevUnity;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using AGAsset.StdUtil;
using AGDevUnity.StdUtil;
using AGDev.StdUtil;
using AGAsset;

namespace AGDevUnity {

	public class StdMonoBAssetMediator : MonoBAssetMediator, AssetMediator {
		public MonoBAssetImplementer implementerGetter;
		public MonoBAUSupplier assetSupplier;
		public MonoBAssetInterface assetInterface;
		public AssetRequest integrantReq = new AssetRequest();
		public List<MonoBAUICustomizer> auiCustomizers;
		public List<MonoBAUIntegrator> auIntegrators;
		public List<MonoBAssetImplementer> assetImplementers;
		public List<TextAsset> dictionaries;
		public StdAssetRequestHolder _reqHolder;
		[Serializable]
		public class StdAssetRequestHolder : AssetRequestHolder, Taker<AssetRequestUnit>, ImmediateGiver<AssetRequestUnit, int> {
			public AssetRequest assetReq = new AssetRequest();
			public AssetRequest referenceAssetReq;

			AssetRequest AssetRequestHolder.request { get { return assetReq; } }
			ImmediateGiver<AssetRequestUnit, int> AssetRequestHolder.unitGiver => this;

			void Taker<AssetRequestUnit>.Take(AssetRequestUnit newElement) {
				AssetRequestUnit existing = null;
				if (referenceAssetReq != null)
					existing = AssetUtils.FindEquivelent(referenceAssetReq, newElement);
				if (existing != null) {
					if (assetReq.units.Find((elem) => elem.ID == existing.ID) == null) {
						assetReq.units.Add(existing);
					}
				} else
					assetReq.units.Add(newElement);
			}

			AssetRequestUnit ImmediateGiver<AssetRequestUnit, int>.PickBestElement(int key) {
				return assetReq.units.Find((request) => request.ID == key);
			}

			AssetRequestUnit AssetRequestHolder.AddOrMergeRequest_GetAdded(AssetRequestUnit newReqUnit) {
				AssetRequestUnit existing = null;
				if (referenceAssetReq != null)
					existing = AssetUtils.FindEquivelent(referenceAssetReq, newReqUnit);
				if (existing != null) {
					var old = assetReq.units.Find((elem) => elem.ID == existing.ID);
					if (old == null) {
						assetReq.units.Add(existing);
						return existing;
					}
					return existing;
				}
				existing = AssetUtils.FindEquivelent(assetReq, newReqUnit);
				if (existing == null) {
					assetReq.units.Add(newReqUnit);
					return newReqUnit;
				}
				return existing;
			}

			void Taker<AssetRequestUnit>.None() {}
		}
		AssetRequestHolder reqHolder {
			get {
				if (_reqHolder == null)
					_reqHolder = new StdAssetRequestHolder { };
				return _reqHolder;
			}
		}

		public override AssetMediator assetMed => this;

		ContentType AssetMediator.GetImplementedAsset<ContentType>(GrammarBlock gBlock) {
			var implementer = implementerGetter.assetImplGetter.GetAssetImplementer<ContentType>(gBlock);
			if (implementer != null) {
				return implementer.PickImplementedAsset(gBlock);
			}
			return default(ContentType);
		}

		void AssetMediator.SeekAsset<ContentType>(GrammarBlock gBlock, Taker<ContentType> collLis) {
			var implementer = implementerGetter.assetImplGetter.GetAssetImplementer<ContentType>(gBlock);
			if (implementer != null) {
				implementer.SeekAsset(gBlock, new PrvtSeekLis<ContentType> { collLis = collLis, parent = this});
				return;
			}
			//stub: should seek asset tool
			else
				collLis.None();
		}
		class PrvtSeekLis<AssetType> : AssetSeekSupportListener<AssetType>, Giver<AssetUnitInterface, AssetRequestUnit> {
			public StdMonoBAssetMediator parent;
			public Taker<AssetType> collLis;
			Taker<AssetType> AssetSeekSupportListener<AssetType>.collectorOnImplement => collLis;
			Giver<AssetUnitInterface, AssetRequestUnit> AssetSeekSupportListener<AssetType>.auInterfaceGiver => this;
			void Giver<AssetUnitInterface, AssetRequestUnit>.Give(AssetRequestUnit key, Taker<AssetUnitInterface> processor) {
				parent.assetSupplier.assetUnitSupplier.SupplyAssetUnit(key, new PrvtSupplyListener { parent = this, auIntefaceTaker = processor });
			}
			class PrvtSupplyListener : AssetUnitSupplyListener, Taker<AssetUnitInfo>, Giver<AssetUnitInterface, AssetRequestUnit> {
				public PrvtSeekLis<AssetType> parent;
				public Taker<AssetUnitInterface> auIntefaceTaker;
				Taker<AssetUnitInfo> AssetUnitSupplyListener.supplyTaker => this;
				Giver<AssetUnitInterface, AssetRequestUnit> AssetUnitSupplyListener.integrantGiver => this;

				void Taker<AssetUnitInfo>.Take(AssetUnitInfo newElement) {
					var assetInterface = parent.parent.assetInterface.assetInterface.PickBestElement(newElement);
					if(assetInterface != null) {
						auIntefaceTaker.Take(assetInterface);
					}
				}

				void Taker<AssetUnitInfo>.None() {
					auIntefaceTaker.None();
				}

				void Giver<AssetUnitInterface, AssetRequestUnit>.Give(AssetRequestUnit key, Taker<AssetUnitInterface> processor) {
					var integrantSupLis = new PrvtSupplyListener { parent = parent, auIntefaceTaker = processor };
					parent.parent.assetSupplier.assetUnitSupplier.SupplyAssetUnit(key, integrantSupLis);
				}
			}
		}
		IEnumerable<AssetType> AssetMediator.GetImplementedAssets<AssetType>() {
			if(typeof(TextAsset) == typeof(AssetType)) {
				return new ConvertingEnumarable<AssetType, TextAsset> { convertFunc = (elem) => (AssetType)(object)elem, sourceEnumerable = dictionaries };
			}
			else if (typeof(MonoBAssetImplementer) == typeof(AssetType)) {
				return new ConvertingEnumarable<AssetType, MonoBAssetImplementer> { convertFunc = (elem) => (AssetType)(object)elem, sourceEnumerable = assetImplementers };
			}
			else if(typeof(MonoBAUIntegrator) == typeof(AssetType)) {
				return new ConvertingEnumarable<AssetType, MonoBAUIntegrator> { convertFunc = (elem) => (AssetType)(object)elem, sourceEnumerable = auIntegrators };
			}
			else if(typeof(MonoBAUICustomizer) == typeof(AssetType)) {
				return new ConvertingEnumarable<AssetType, MonoBAUICustomizer> { convertFunc = (elem) => (AssetType)(object)elem, sourceEnumerable = auiCustomizers };
			} else  {
				var impl = implementerGetter.assetImplGetter.GetAssetImplementer<AssetType>(null);
				if (impl != null)
					return impl.implementedAssets;
			}
			return null;
		}
		AssetType AssetMediator.GetImplementedModule<AssetType>() {
			return default;
		}

		void AssetMediator.SeekModule<AssetType>(Taker<AssetType> taker) {
			taker.None();
		}
	}

}