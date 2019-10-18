using System.Collections.Generic;
using AGDev;
using AGBLang;
using AGBLang.StdUtil;

namespace AGDevUnity.StdUtil {
	public class PackedBTrigger : MonoBBehaviorTrigger, ImmediateGiver<BehaviorTrigger, string> {
		public MonoBAUInterface auInterface;
		public override ImmediateGiver<BehaviorTrigger, string> bTriggerGiver => this;
		public MonoBBIntegTriggerFactory triggerIntegrator;
		public List<BehaviorPackingSetting> settings;
		public MonoBSyntacticProcessor LProcessor;
		public MonoBBehaviorAnalyzer bAnalyzer;
		public StrEvent OnUnreadableFound;
		BehaviorTrigger ImmediateGiver<BehaviorTrigger, string>.PickBestElement(string key) {
			var setting = settings.Find((item) => string.IsNullOrEmpty(item.behaviorName) ? string.Compare(item.sourceEWCs[0], key, false) == 0 : string.Compare(item.behaviorName, key, false) == 0);
			if(setting != null) {
				var trigger =  new DelayBTrigger { parent = this, setting = setting };
				trigger.BeginActualTriggerCreation();
				return trigger;
			} else {
				var newSettting = new BehaviorPackingSetting { behaviorName = key };
				newSettting.sourceEWCs.Add(key);
				var trigger = new DelayBTrigger { parent = this, setting = setting };
				trigger.BeginActualTriggerCreation();
				return trigger;
			}
		}
		public class DelayBTrigger : BehaviorTrigger {
			public PackedBTrigger parent;
			public IntegrableBTrigger actualTrigger;
			public BehaviorPackingSetting setting;
			public List<System.Action> tasks = new List<System.Action>();
			void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
				if (actualTrigger == null)
					tasks.Add(() => actualTrigger.BeginBehavior(behaviorListener));
				else
					actualTrigger.BeginBehavior(behaviorListener);
			}
			public void BeginActualTriggerCreation() {
				string fileName = setting.sourceEWCs[0] + ".txt";
				parent.auInterface.auInterface.referer.PickContent<byte[]>(fileName, new PrvtColl { fileName = fileName, parent = this });
			}
			public class PrvtColl : Taker<byte[]>, Taker<GrammarBlock>, Taker<BehaviorTriggerSet> {
				public DelayBTrigger parent;
				public string fileName;
				void Taker<byte[]>.Take(byte[] newElement) {
					parent.parent.LProcessor.LProcessor.PerformSyntacticProcess(System.Text.Encoding.UTF8.GetString(newElement), this);
				}

				void Taker<GrammarBlock>.Take(GrammarBlock item) {
					GrammarBlockUtils.ForEach(item, "Unreadable", (gBlock) => {
						string result = fileName + " : ";
						GrammarBlockUtils.ForEachUnits((gBlock), (gUnit) => result += " " + gUnit.word);
						parent.parent.OnUnreadableFound.Invoke(result);
					});
					parent.parent.bAnalyzer.bAnlys.AnalyzeBehavior(item, this);
				}

				void Taker<BehaviorTriggerSet>.Take(BehaviorTriggerSet item) {
					if (item.namedTriggers != null) {
						if(parent.actualTrigger == null)
							parent.actualTrigger = parent.parent.triggerIntegrator.NewIntegrableBTrigger();
						foreach (var namedTrigger in item.namedTriggers) {
							parent.setting.triggers.Add(new NamedBTrigger { name = namedTrigger.name, bTrigger = namedTrigger.bTrigger });
							parent.actualTrigger.AddSubTrigger(namedTrigger);
						}
						foreach (var task in parent.tasks) {
							task();
						}
						parent.tasks.Clear();
					}
				}

				void Taker<byte[]>.None() {}
				void Taker<GrammarBlock>.None() {}
				void Taker<BehaviorTriggerSet>.None() {}
			}
		}
	}
	[System.Serializable]
	public class NamedBTrigger : NameAndBTrigger { }
	[System.Serializable]
	public class BehaviorPackingSetting {
		public string behaviorName;
		public List<string> sourceEWCs = new List<string>();
		public List<NamedBTrigger> triggers = new List<NamedBTrigger>();
	}
}