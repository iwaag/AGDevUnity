using AGDev;
using AGDev.StdUtil;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	[System.Serializable]
	public class TriggerSettingUnit {
		public string mainTriggerName;
		public List<string> subTriggerNames;
	}
	[System.Serializable]
	public class CompositeTriggerSetting {
		public string processName;
		public List<TriggerSettingUnit> serialBehaviorSettings = new List<TriggerSettingUnit>();
	}
	public class IntegratedBTrigger : MonoBBehaviorTrigger, ImmediateGiver<BehaviorTrigger, string> {
		public static IEnumerable<BehaviorTrigger> TriggerNameListToTriggerList(ImmediateGiver<BehaviorTrigger, string> baseTrigger, IEnumerable<string> triggerNames) {
			var triggerList = new List<BehaviorTrigger>();
			foreach (var triggerName in triggerNames) {
				var unitProcess = baseTrigger.PickBestElement(triggerName);
				triggerList.Add(unitProcess);
			}
			return triggerList;
		}
		public override ImmediateGiver<BehaviorTrigger, string> bTriggerGiver => this;
		public MonoBBehaviorTrigger baseTrigger;
		public List<CompositeTriggerSetting> settings;
		public Dictionary<string, BehaviorTrigger> savedTriggers = new Dictionary<string, BehaviorTrigger>(System.StringComparer.OrdinalIgnoreCase);
		BehaviorTrigger ImmediateGiver<BehaviorTrigger, string>.PickBestElement(string key) {
			savedTriggers.TryGetValue(key, out var integrated);
			if(integrated != null) {
				return integrated;
			}
			var setting = settings.Find((elem) => string.Compare(key, elem.processName, true) == 0);
			if(setting == null) {
				return baseTrigger.bTriggerGiver.PickBestElement(key);
			}
			var composite = new CompositeBTrigger {};
			foreach (var unit in setting.serialBehaviorSettings) {
				CompositeBTriggerUnit triggerUnit = new CompositeBTriggerUnit();
				triggerUnit.mainTrigger = baseTrigger.bTriggerGiver.PickBestElement(unit.mainTriggerName);
				foreach (var subName in unit.subTriggerNames) {
					triggerUnit.subTriggers.Add(baseTrigger.bTriggerGiver.PickBestElement(subName));
				}
				composite.serialTriggers.Add(triggerUnit);
			}
			savedTriggers[key] = composite;
			return composite;
		}
		public class CompositeBTriggerUnit : BehaviorTrigger {
			public BehaviorTrigger mainTrigger;
			public List<BehaviorTrigger> subTriggers = new List<BehaviorTrigger>();
			void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
				mainTrigger.BeginBehavior(behaviorListener);
				foreach (var sub in subTriggers) {
					sub.BeginBehavior(new StubBehaviorListener());
				}
			}
		}
		public class CompositeBTrigger : BehaviorTrigger {
			public List<BehaviorTrigger> serialTriggers = new List<BehaviorTrigger>();
			void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
				var enumerator =  serialTriggers.GetEnumerator();
				if (enumerator.MoveNext())
					enumerator.Current.BeginBehavior(new PrvtListener { clientListener  = behaviorListener , enumerator = enumerator });
			}
			public class PrvtListener : BehaviorListener {
				public BehaviorListener clientListener;
				public IEnumerator<BehaviorTrigger> enumerator;
				void BehaviorListener.OnFinish() {
					if (enumerator.MoveNext())
						enumerator.Current.BeginBehavior(this);
					else
						clientListener.OnFinish();
				}
			}
		}
	}
}
