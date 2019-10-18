using AGAsset;
using AGDevUnity.StdUtil;
using AGDev;
using UnityEngine;
using AGDevUnity;
public class AssetManagementPanel : MonoBehaviour, AssetSupplyListener, Taker<AssetPick> {
	public UnityEngine.UI.Text textContentPrefab;
	public MonoBConfigurableField supplierConfigField;
	public MonoBConfigurableField integratorConfigField;
	public GameObject integratorRoot;
	public GameObject supplierRootRoot;
	public GameObject reqViewerContentRoot;
	public GameObject pickViewerContentRoot;
	float countdown = 0;
	void Start() {
		foreach( var configurable in supplierRootRoot.GetComponentsInChildren<Configurable>(true)) {
			supplierConfigField.configurableField.Take(configurable);
		}
		foreach (var configurable in integratorRoot.GetComponentsInChildren<Configurable>(true)) {
			integratorConfigField.configurableField.Take(configurable);
		}
	}
	void Update () {
		if ((countdown -= Time.deltaTime) < 0) {
			countdown = 4;
			foreach (Transform reqViewerContent in reqViewerContentRoot.transform) {
				Destroy(reqViewerContent.gameObject);
			}
			foreach (var reqUnit in FindObjectOfType<HTML_AUSupplier>().bridgeSupplier.req.units) {
				var requestSummery = reqUnit.ID + " ; " + reqUnit.assettype + " ; " + reqUnit.attributes[0];
				var newText = Instantiate(textContentPrefab, reqViewerContentRoot.transform, false);
				newText.gameObject.SetActive(true);
				newText.text = requestSummery;
			}
		}
	}
	Taker<AssetPick> AssetSupplyListener.supplyTaker => this;
	void Taker<AssetPick>.Take(AssetPick item) {
		foreach (var pickUnit in item.units) {
			foreach (var unit in pickUnit.assetInfo.units) {
				var requestSummery = pickUnit.reqID + " ; " + unit.assettype + " ; " + unit.distributor + " ; " + unit.shortname + " ; " + unit.reference;
				var newText = Instantiate(textContentPrefab, pickViewerContentRoot.transform, false);
				newText.gameObject.SetActive(true);
				newText.text = requestSummery;
			}
		}
	}


	void Taker<AssetPick>.None() {}
}
