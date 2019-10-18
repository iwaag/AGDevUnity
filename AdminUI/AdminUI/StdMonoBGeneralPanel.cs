using AGDev;
using AGBLang;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StdMonoBGeneralPanel : MonoBehaviour, GeneralPanel {
	public GeneralPanel panel => this;
	string GeneralPanel.title { get => title.text; set => title.text = value; }
	string GeneralPanel.iconName { set { icon.GetComponent<SeekPicture>().imageName = value; } }
	string _iconName;
	///public List<OvservedProcessContentPack> observedProecsses = new List<OvservedProcessContentPack>();
	public GeneralUIContent contentPrefab;
	public UnityEngine.UI.Image icon;
	public UnityEngine.UI.Text title;
	public Transform contentRoot;
	public StdMonoBGeneralPanel subPannelPrefab;
	public GrammarBlockVisualizer gBlockVisualizer;
	GeneralUIContent GeneralPanel.AddProcessObservePannel(string processName, ObservedProcess process, string iconName) {
		var newText = Instantiate(contentPrefab, contentRoot, false);
		newText.title.text = processName + ": ";
		if (!string.IsNullOrEmpty(iconName)) {
			newText.image.GetComponent<SeekPicture>().imageName = iconName;
		}
		var pack = new OvservedProcessContentPack { guiInstance = newText, processName = processName };
		process.AcceptObserver(pack);
		return newText;
	}

	GeneralUIContent GeneralPanel.AddGeneralContent(string processName, string iconName) {
		var newText = Instantiate(contentPrefab, contentRoot, false);
		newText.title.text = processName + ": ";
		if (!string.IsNullOrEmpty(iconName)) {
			newText.image.GetComponent<SeekPicture>().imageName = iconName;
		}
		return newText;
	}

	void GeneralPanel.ViewGBLock(GrammarBlock block) {
		var viewer = gBlockVisualizer.CreateVisualizeUnit(block, null);
		viewer.transform.SetParent(contentRoot, false);
	}

	GeneralPanel GeneralPanel.AllocateSubSpace() {
		var newSubPanel = Instantiate(subPannelPrefab, contentRoot, false);
		return newSubPanel.panel;
	}

	void GeneralPanel.Clear() {
		foreach (Transform item in contentRoot)
			Destroy(item.gameObject);
	}
}
public interface GeneralPanel {
	string title { set; get; }
	string iconName  { set; }
	GeneralUIContent AddProcessObservePannel(string processName, ObservedProcess process, string iconName = null);
	GeneralUIContent AddGeneralContent(string processName, string iconName = null);
	void ViewGBLock(GrammarBlock block);
	GeneralPanel AllocateSubSpace();
	void Clear();
}
public class OvservedProcessContentPack : ProcessObserver {
	public GeneralUIContent guiInstance;
	public string processName;
	void ProcessObserver.OnGetBusy() {
		guiInstance.state.text = "Busy!!!";
		guiInstance.state.color = Color.red;
	}
	void ProcessObserver.OnGetIdle() {
		guiInstance.state.text = "Idle...";
		guiInstance.state.color = Color.green;
	}
}
