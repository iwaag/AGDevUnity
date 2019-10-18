using AGAsset;
using AGDev;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewableMonoBAUSupplier : MonoBAUSupplier, AssetUnitSupplier {
	public enum AssetSupplyState {
		Seeking,
		Found,
		NotFound
	}
	public class AssetRequestUnitStateLog {
		public AssetSupplyState state = AssetSupplyState.Seeking;
		public AssetRequestUnit unit;
		public GeneralUIContent reqViewer;
		public GeneralUIContent pickViewer;
	}
	public override AssetUnitSupplier assetUnitSupplier => this;
	public MonoBAUSupplier baseSupplier;
	public Transform assetPannelRoot;
	public StdMonoBGeneralPanel pannelPrefab;
	public StdMonoBGeneralPanel requestPannel;
	public StdMonoBGeneralPanel pickPannel;
	public List<AssetRequestUnitStateLog> logs = new List<AssetRequestUnitStateLog>();
	void Awake() {
		requestPannel = Instantiate(pannelPrefab, assetPannelRoot);
		pickPannel = Instantiate(pannelPrefab, assetPannelRoot);
		requestPannel.panel.title = "Requests";
		pickPannel.panel.title = "Provided";
		pickPannel.transform.localPosition = new Vector3(0, 0, 0);
		pickPannel.transform.localPosition = pickPannel.transform.localPosition + new Vector3(500, 0, 0);
	}
	void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit reqUnit, AssetUnitSupplyListener listener) {
		var log = new AssetRequestUnitStateLog { unit = reqUnit };
		logs.Add(log);
		var requestSummery = reqUnit.ID + " ; " + reqUnit.assettype + " ; " + reqUnit.attributes[0];
		log.reqViewer = requestPannel.panel.AddGeneralContent(requestSummery);
		baseSupplier.assetUnitSupplier.SupplyAssetUnit(reqUnit, new PrvtAssetUnitSupplyListener { parent = this, baseLitener = listener, log = log });
	}
	class PrvtAssetUnitSupplyListener : AssetUnitSupplyListener, Taker<AssetUnitInfo> {
		public AssetUnitSupplyListener baseLitener;
		public AssetRequestUnitStateLog log;
		public ViewableMonoBAUSupplier parent;
		Giver<AssetUnitInterface, AssetRequestUnit> AssetUnitSupplyListener.integrantGiver => baseLitener.integrantGiver;
		Taker<AssetUnitInfo> AssetUnitSupplyListener.supplyTaker => this;
		void Taker<AssetUnitInfo>.Take(AssetUnitInfo item) {
			log.state = AssetSupplyState.Found;
			var auInfoSummery = log.unit.ID + " ; " + item.assettype + " ; " + item.distributor + " ; " + item.shortname;
			log.pickViewer = parent.pickPannel.panel.AddGeneralContent(auInfoSummery);
			log.reqViewer.state.text = "Found";
			log.reqViewer.state.color = Color.green;
			baseLitener.supplyTaker.Take(item);
		}
		void Taker<AssetUnitInfo>.None() {
			log.state = AssetSupplyState.NotFound;
			log.reqViewer.state.text = "Not Found";
			log.reqViewer.state.color = Color.red;
			baseLitener.supplyTaker.None();
		}
	}
}
