using AGAsset;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class SQLAssetSupplier : MonoBAUSupplier, AssetUnitSupplier {
		public override AssetUnitSupplier assetUnitSupplier => throw new System.NotImplementedException();
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			listener.supplyTaker.None();
		}
		/*public SQLiteLocalAssetGiver assetDatabase {
get{
if (_assetDatabase == null)
_assetDatabase = new SQLiteLocalAssetGiver();
return _assetDatabase;
}
}
public SQLiteLocalAssetGiver _assetDatabase;
public override AssetUnitSupplier assetUnitSupplier => this;

void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
(assetDatabase as AssetInfoDatabase).SupplyAssetUnit(assetRequest, listener);
}*/

	}
}