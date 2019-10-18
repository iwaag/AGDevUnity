using AGBLang.StdUtil;
using AGDev;
using System.Collections.Generic;
namespace AGDevUnity.StdUtil {
	public class GroupingBTrigger : MonoBBehaviorTrigger, ImmediateGiver<BehaviorTrigger, string> {
		public NamedProcessBehaver rootSystem;
		public override ImmediateGiver<BehaviorTrigger, string> bTriggerGiver => this;
		public MonoBBehaviorTrigger baseTrigger;
		public List<ProcessGroupSetting> settings;
		public List<UnitGroupTrigger> groupTriggers = new List<UnitGroupTrigger>();
		BehaviorTrigger ImmediateGiver<BehaviorTrigger, string>.PickBestElement(string key) {
			var trigger = groupTriggers.Find((elem) => string.Compare(key, elem.processName, true) == 0);
			var grouptSetting = settings.Find((elem) => elem.members.Find((member) => string.Compare(member, key, true) == 0) != null);
			if (trigger == null) {
				if (grouptSetting == null)
					return baseTrigger.bTriggerGiver.PickBestElement(key);
				else {
					if(grouptSetting.preProcessTrigger == null && !string.IsNullOrEmpty(grouptSetting.preProcess)) {
						grouptSetting.preProcessTrigger = baseTrigger.bTriggerGiver.PickBestElement(grouptSetting.preProcess);
					}
					if (grouptSetting.postProcessTrigger == null && !string.IsNullOrEmpty(grouptSetting.postProcess)) {
						grouptSetting.postProcessTrigger = baseTrigger.bTriggerGiver.PickBestElement(grouptSetting.postProcess);
					}
					/*foreach (var member in grouptSetting.members) {
						grouptSetting.memberProcesses[member] = baseTrigger.bTriggerGiver.PickBestElement(member);
					}*/
					
					var newGroupTrigger = new UnitGroupTrigger { groupSetting = grouptSetting, processName = key };
					groupTriggers.Add(newGroupTrigger);
					trigger = newGroupTrigger;
				}
			}
			grouptSetting.memberProcesses[key] = baseTrigger.bTriggerGiver.PickBestElement(key);
			return trigger;
		}
		public class UnitGroupTrigger : BehaviorTrigger {
			public string processName;
			public ProcessGroupSetting groupSetting;
			void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
				groupSetting.currentGivenProcessListeners[processName] = behaviorListener;
				groupSetting.Do(processName);
			}
		}
	}
}
