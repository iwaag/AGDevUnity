using AGBLang;
using AGDev;
using AGAsset;
using System.Collections.Generic;
using AGDev.StdUtil;

class CountingTaker<ItemType> : Taker<ItemType> {
	public Taker<ItemType> clientTaker;
	public ObservedProcessHelper helper;
	void Taker<ItemType>.Take(ItemType item) {
		clientTaker.Take(item);
		helper.CountDown();
	}

	void Taker<ItemType>.None() {
		clientTaker.None();
		helper.CountDown();
	}
}
class ObservedSyntacticProcessor : NaturalLanguageProcessor {
	public ObservedProcessHelper helper;
	public NaturalLanguageProcessor clientProcessor;
	void NaturalLanguageProcessor.PerformSyntacticProcess(string behaviorExpression, Taker<GrammarBlock> listener) {
		helper.CountUp();
		clientProcessor.PerformSyntacticProcess(behaviorExpression, new CountingTaker<GrammarBlock> { helper = helper, clientTaker = listener });
	}
}
class ObservedCSGiver : CommonSenseGiver, Giver<NounCommonSenseUnit, string> {
	public ObservedProcessHelper helper;
	public CommonSenseGiver clientGiver;
	Giver<NounCommonSenseUnit, string> CommonSenseGiver.nounCSGiver => this;
	void Giver<NounCommonSenseUnit, string>.Give(string key, Taker<NounCommonSenseUnit> colletor) {
		helper.CountUp();
		clientGiver.nounCSGiver.Give(key, new CountingTaker<NounCommonSenseUnit> { clientTaker = colletor, helper = helper});
	}
}
class ObservedAssetSupplier : AssetUnitSupplier {
	public ObservedProcessHelper helper;
	public AssetUnitSupplier clientSupllier;
	void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
		helper.CountUp();
		clientSupllier.SupplyAssetUnit(assetRequest, new CountingAssetSupplyListener { helper = helper, clientListener = listener });
	}

	class CountingAssetSupplyListener : AssetUnitSupplyListener, Taker<AssetUnitInfo> {
		public AssetUnitSupplyListener clientListener;
		public ObservedProcessHelper helper;
		Taker<AssetUnitInfo> AssetUnitSupplyListener.supplyTaker => this;
		Giver<AssetUnitInterface, AssetRequestUnit> AssetUnitSupplyListener.integrantGiver => clientListener.integrantGiver;
		void Taker<AssetUnitInfo>.Take(AssetUnitInfo item) {
			helper.CountDown();
			clientListener.supplyTaker.Take(item);
		}
		void Taker<AssetUnitInfo>.None() {
			helper.CountDown();
			clientListener.supplyTaker.None();
			
		}
	}
}
