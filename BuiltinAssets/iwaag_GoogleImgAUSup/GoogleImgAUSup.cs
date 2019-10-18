using AGAsset;
namespace AGDevUnity {
	public class GoogleImgAUSup : MonoBAUSupplier, AssetUnitSupplier {
		public override AssetUnitSupplier assetUnitSupplier => this;
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			if (assetRequest.assettype != "Picture") {
				listener.supplyTaker.None();
				return;
			}
			//check cache
			var temporaryAUInfo = new AssetUnitInfo {
				assettype = assetRequest.assettype,
				attributes = assetRequest.attributes[0],
				distributor = "GSearch",
				reference = "GSearch:" + assetRequest.attributes[0],
				shortname = "GSearch-" + assetRequest.attributes[0]
			};
			listener.supplyTaker.Take(temporaryAUInfo);
		}
	}
}
