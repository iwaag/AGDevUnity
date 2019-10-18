using AGDevUnity.StdUtil;
using AGDev;
using AGDev.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessController : MonoBehaviour {
	public string initialScene = "Intro";
	public void SetInitialScene(string scene) { initialScene = scene; }
	public NamedProcessBehaver factory;
	public ObservedMonoBLProcessor syntactic;
	public RootAssetSupplier supplier;
	public HTML_AUSupplier gateAUSupplier;
	public MonoBObservedCSGiver observedCSGiver;
	public HTML_MonoBCSGiver htmlCSGiver;
	public class GateSetting {
		public float elapsedAfterFinishInSec;
		public Gate gate;
	}
	public List<GateSetting> gateSettings = new List<GateSetting>();
	public BehaviorTrigger trigger;
	public float processBeginWaitInSec = 1;
	public BehaviorTrigger GetRootBehaviorTrigger(string rootBehaviorName) {
		if (trigger != null)
			return trigger;
		var rootBehaviorDef = factory.FindDef(rootBehaviorName);
		if (rootBehaviorDef != null) {
			#region gate setting
			var csGateSetting = new GateSetting { elapsedAfterFinishInSec = -1, gate = htmlCSGiver };
			var auSupSetting  = new GateSetting { elapsedAfterFinishInSec = -1, gate = gateAUSupplier.bridgeSupplier };
			gateSettings.Add(csGateSetting);
			gateSettings.Add(auSupSetting);
			//After syntactic analysis, start common sense check
			(syntactic.observeHelper as ObservedProcess).AcceptObserver(new PrvtObserver { setting = csGateSetting });
			//After common sense check, start asset supply
			(observedCSGiver.observeHelper as ObservedProcess).AcceptObserver(new PrvtObserver { setting = auSupSetting });
			#endregion
			return factory.NewBeginTrigger(rootBehaviorDef);
		}
		return null;
	}
	private void Update() {
		for(int i = 0; i < gateSettings.Count; i++) {
			var setting = gateSettings[i];
			if (setting.gate.isOpen || setting.elapsedAfterFinishInSec < 0)
				continue;
			setting.elapsedAfterFinishInSec += Time.deltaTime;
			if (setting.elapsedAfterFinishInSec > processBeginWaitInSec) {
				setting.gate.Open(true);
				setting.elapsedAfterFinishInSec = 0;
			}
		}
	}
	class PrvtObserver : ProcessObserver {
		public GateSetting setting;
		void ProcessObserver.OnGetBusy() {
			setting.elapsedAfterFinishInSec = -1;
		}

		void ProcessObserver.OnGetIdle() {
			setting.elapsedAfterFinishInSec = 0;
		}
	}
	public void BeginReady() {
		trigger = GetRootBehaviorTrigger(initialScene);
		//rootTrigger.BeginBehavior(listener);
	}
	public void StartGame() {
		if (trigger != null) {
			trigger.BeginBehavior(new StubBehaviorListener());
		}
	}
}
