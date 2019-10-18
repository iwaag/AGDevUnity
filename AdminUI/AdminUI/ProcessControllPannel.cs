using AGDevUnity.StdUtil;
using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
public class ProcessControllPannel : MonoBehaviour
{
	public StdMonoBGeneralPanel pannlPrefab;
	public StdMonoBGeneralPanel analysisProcessPannel;
	public StdMonoBGeneralPanel assetProcessPannel;
	public ObservedMonoBLProcessor syntactic;
	public RootAssetSupplier supplier;
	public MonoBObservedCSGiver csGiver;
	public RootAssetReferer referer;
	private void Awake() {
		analysisProcessPannel = Instantiate(pannlPrefab, transform);
		analysisProcessPannel.panel.title = "Analytic Processes";
		analysisProcessPannel.panel.iconName = "AnalysisIcon.png";
		assetProcessPannel = Instantiate(analysisProcessPannel, transform);
		assetProcessPannel.panel.title = "Asset Processes";
		assetProcessPannel.panel.iconName = "AssetIcon.png"; 
		analysisProcessPannel.panel.AddProcessObservePannel("Syntax", syntactic.observeHelper, "SProcessIcon.png");
		analysisProcessPannel.panel.AddProcessObservePannel("Common Sense", csGiver.observeHelper, "CommonSenseIcon.png");
		assetProcessPannel.panel.AddProcessObservePannel("Search", supplier.observeHelper, "SearchIcon.png");
		assetProcessPannel.panel.AddProcessObservePannel("Download", referer.helper, "DownloadIcon.png");
	}
}
