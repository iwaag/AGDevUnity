using AGDevUnity.StdUtil;
using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AGAsset;
using AGAsset.StdUtil;
using AGDev.StdUtil;
using AGDevUnity;
#if false
public class DockEvent : MonoBehaviour {
	public UnityEngine.UI.Button createGameButton;
	public Transform subPanelRoot;
	public AssetManagementPanel assetManagementPanel;
	public Transform unredableListPanelRoot;
	public bool didStart = false;
	public bool doSeekAsset = false;
	public UnityEngine.UI.Text textContentPrefab;
	public class PrvtLis : ModuleInterceptListener {
		public UnityEngine.UI.Text text;
		public DockEvent parent;
		void ModuleInterceptListener.OnAllProcessDone() {
			text.text = "Complete!";
			text.color = Color.green;
			if (parent.doSeekAsset) {
				//FindObjectOfType<HTML_AUSupplier>().bridgeSupplier.BeginAssetSupply();
			}
			else if (parent != null ? parent.didStart : false) {
				//FindObjectOfType<RootMonoBBahaverFrontFactory>().BeginBehavior();
			}
		}
		void ModuleInterceptListener.OnBeginWorking() {
			text.text = "Working...";
			text.color = Color.blue;
		}
	}
	public class PrvtAssetSeekLis : ModuleInterceptListener {
		public UnityEngine.UI.Text text;
		void ModuleInterceptListener.OnAllProcessDone() {
			text.text = "Complete!";
			text.color = Color.green;
		}
		void ModuleInterceptListener.OnBeginWorking() {
			text.text = "Working...";
			text.color = Color.blue;
		}
	}
	void OnIdeaEdited(string idea) {
		createGameButton.interactable = ! string.IsNullOrWhiteSpace(idea);
	}
	public void OnUnreadableSentenceFound(string sentence) {
		var newText = Instantiate(textContentPrefab, unredableListPanelRoot, false);
		newText.gameObject.SetActive(true);
		newText.text = sentence;
	}
	public void ActivateSubPanel(GameObject panel) {
		bool previousActive = panel.activeSelf;
		var subPanels = subPanelRoot.Cast<Transform>();
		foreach(var subPanel in subPanels) {
			subPanel.gameObject.SetActive(false);
		}
		panel.SetActive(!previousActive);
	}
}
#endif
public class ViewableMonoBBTriggerGiver : MonoBBehaviorTrigger, ImmediateGiver<BehaviorTrigger, string> {
	public ProcessController controller;
	public Transform pannelRoot;
	public MonoBBehaviorTrigger baseTriggerGiver;
	public StdMonoBGeneralPanel panelPrefab;
	public StdMonoBGeneralPanel triggerStatePannel;
	public Dictionary<BehaviorTrigger, ViewableTrigger> triggerDict = new Dictionary<BehaviorTrigger, ViewableTrigger>();
	public override ImmediateGiver<BehaviorTrigger, string> bTriggerGiver => this;
	public void Awake() {
		triggerStatePannel = Instantiate(panelPrefab, pannelRoot);
		triggerStatePannel.panel.title = "Behaviors";
	}
	BehaviorTrigger ImmediateGiver<BehaviorTrigger, string>.PickBestElement(string key) {
		BehaviorTrigger baseTrigger = null;
		baseTrigger = baseTriggerGiver.bTriggerGiver.PickBestElement(key);
		if (triggerDict.ContainsKey(baseTrigger))
			return baseTrigger;
		var newTrigger = new ViewableTrigger { trigger = baseTrigger };
		triggerDict[baseTrigger] = newTrigger;
		triggerStatePannel.panel.AddProcessObservePannel(key, newTrigger.helper);
		return newTrigger;
	}
	public class ViewableTrigger : BehaviorTrigger {
		public BehaviorTrigger trigger;
		int sessionCount = 0;
		public ObservedProcessHelper helper = new ObservedProcessHelper();
		void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
			sessionCount++;
			helper.CountUp();
			trigger.BeginBehavior(new ViewingListener { clientListener = behaviorListener, parent = this });
		}
		class ViewingListener : BehaviorListener {
			public ViewableTrigger parent;
			public BehaviorListener clientListener;
			void BehaviorListener.OnFinish() {
				parent.helper.CountDown();
				clientListener.OnFinish();
			}
		}
	}
}
