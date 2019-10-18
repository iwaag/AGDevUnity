using AGAsset;
using AGBLang;
using AGDev;
using System.Collections.Generic;
using AGBLang.StdUtil;

namespace AGDevUnity.StdUtil {
	[System.Serializable]
	public class StrEvent : UnityEngine.Events.UnityEvent<string>{};
	[System.Serializable]
	public class AnalysisResult {
		public string name;
		public string sourceText;
		public GrammarBlock gBlock;
	}
	[System.Serializable]
	public class ResultEvent : UnityEngine.Events.UnityEvent<AnalysisResult> { };
	public class StdMonoBBehaviorTrigger : MonoBBehaviorTrigger, ImmediateGiver<BehaviorTrigger, string> {
		public MonoBAUInterface assetUnitInterface;
		public MonoBSyntacticProcessor LProcessor;
		public MonoBBehaviorAnalyzer bAnalyzer;
		public StrEvent OnUnreadableFound;
		public ResultEvent OnSucceedEvent;
		AssetUnitInterface _erwcAUInfo;
		public Dictionary<string, BehaviorTrigger> triggers = new Dictionary<string, BehaviorTrigger>();
		public override ImmediateGiver<BehaviorTrigger, string> bTriggerGiver => this;
		BehaviorTrigger ImmediateGiver<BehaviorTrigger, string>.PickBestElement(string key) {
			triggers.TryGetValue(key, out var trigger);
			if(trigger == null) {
				var delayTrigger = new DelayBTrigger { parent = this, ewcName = key };
				triggers.Add(key, trigger = delayTrigger);
				delayTrigger.BeginActualTriggerCreation();
			}
			return trigger;
		}
		public class DelayBTrigger : BehaviorTrigger {
			public StdMonoBBehaviorTrigger parent;
			public BehaviorTrigger actualTrigger;
			public string ewcName;
			public List<System.Action> tasks = new List<System.Action>();
			void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
				if(actualTrigger == null)
					tasks.Add(() => actualTrigger.BeginBehavior(behaviorListener));
				else
					actualTrigger.BeginBehavior(behaviorListener);
			}
			public void BeginActualTriggerCreation() {
				string fileName = ewcName + ".txt";
				parent.assetUnitInterface.auInterface.referer.PickContent<byte[]>(fileName, new PrvtColl { parent = this, fileName = fileName });
			}
			public class PrvtColl : Taker<byte[]>, Taker<GrammarBlock>, Taker<BehaviorTriggerSet> {
				public DelayBTrigger parent;
				public string fileName;
				public string sourceText;
				void Taker<byte[]>.Take(byte[] newElement) {
					parent.parent.LProcessor.LProcessor.PerformSyntacticProcess(System.Text.Encoding.UTF8.GetString(newElement), this);
					sourceText = System.Text.Encoding.UTF8.GetString(newElement);
					/*parent.actualTrigger = parent.parent.interpreter.ewIptr.InterpretERWordsAsBehavior(newElement);
					foreach(var task in parent.tasks) {
						task();
					}
					parent.tasks.Clear();*/
				}

				void Taker<GrammarBlock>.Take(GrammarBlock item) {
					GrammarBlockUtils.ForEach(item, "Unreadable", (gBlock) => {
						string result = "";
						GrammarBlockUtils.ForEachUnits((gBlock), (gUnit) => result += " " + gUnit.word);
						parent.parent.OnUnreadableFound.Invoke(result);
					});
					parent.parent.OnSucceedEvent.Invoke(new AnalysisResult { gBlock = item , name = fileName , sourceText = sourceText}) ;

					parent.parent.bAnalyzer.bAnlys.AnalyzeBehavior(item, this);
					
				}

				void Taker<BehaviorTriggerSet>.Take(BehaviorTriggerSet item) {
					if (item.namedTriggers != null) {
						//stub
					}
					if (item.rootTrigger != null) {
						parent.actualTrigger = item.rootTrigger;
						foreach (var task in parent.tasks) {
							task();
						}
					}
					parent.tasks.Clear();
				}
				void Taker<byte[]>.None() {}
				void Taker<GrammarBlock>.None() {}
				void Taker<BehaviorTriggerSet>.None() {}
			}
		}
	}
}