using AGDevUnity.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEvent : MonoBehaviour {
	public StdMonoBGeneralPanel textNameListPanel;
	public StdMonoBGeneralPanel grammarBlockViewPanel;
	public UnityEngine.UI.Text sourceTextView;
	public void AddAnalysisResult(AnalysisResult result) {
		var content = textNameListPanel.panel.AddGeneralContent(result.name);
		var subPanel = grammarBlockViewPanel.panel.AllocateSubSpace();
		content.toggle.isOn = false;
		content.toggle.gameObject.SetActive(true);
		content.toggle.onValueChanged.AddListener((value) => {
			if (value)
				ViewResult(subPanel, result);
			else
				subPanel.Clear();
		});
	}
	public void ViewResult(GeneralPanel panel, AnalysisResult result) {
		panel.Clear();
		panel.ViewGBLock(result.gBlock);
		sourceTextView.text = result.sourceText;
	}
}
