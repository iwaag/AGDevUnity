using UnityEngine;
using System.Collections.Generic;
using AGDev.StdUtil;
using AGDev;
using AGDevUnity;
using AGBLang;
using AGBLang.StdUtil;

namespace AGDevUnity {
	public class StdMonoBBAnalyzer : MonoBBehaviorAnalyzer, ConfigurableBehaviorAnalyzer {
		public override BehaviorAnalyzer bAnlys => this;
		public MonoBBahaverGiver behaverGiver;
		public MonoBAssetMediator assetMediator;
        TimeBehaver timeBehaver;
		public StdBehaverGiver stdGiver {
			get {
				if (_stdGiver == null) {
					_stdGiver = new StdBehaverGiver {  clientBehaverGiver = behaverGiver.behaverGiver };
                    timeBehaver = new TimeBehaver();
                    _stdGiver.behavers.Add(timeBehaver);
				}
				return _stdGiver;
			}
		}
		StdBehaverGiver _stdGiver;
		public StdBSetCheck bSetCheck {
			get {
				if( _bSetCheck == null) {
					_bSetCheck = new StdBSetCheck { behaverGiver = stdGiver };
				}
				return _bSetCheck;
			}
		}
		StdBSetCheck _bSetCheck;

		bool BehaviorAnalyzer.AskForFloatAnswer(GrammarBlock question, AnswerListener<float> listener) {
			return false;//stub
		}

		void BehaviorAnalyzer.AnalyzeBehavior(GrammarBlock behaviorExpressionByGBlock, Taker<BehaviorTriggerSet> listener) {
			//ensure behaver
			{
				var metaDepVis = new MetaInfoDependentGrammarBlockVisitor { doDeepSeek = true, doDeepSeekModifier = true };
				metaDepVis.metaToVis[StdMetaInfos.nominalBlock.word] = new EnsureBehaver { behaverGiver = stdGiver };
				GrammarBlockUtils.VisitGrammarBlock(behaviorExpressionByGBlock, metaDepVis);
			}
			//cereate behavior trigger
			{
				var rootTrigger = new StdCompositeBehaviorTrigger();
				var namedCBTrigger = new Dictionary<string, CompositeBehaviorTrigger>();
				var processor = new SentenceBlockRecursiveProcessor { subSentenceBehaviorTaker = rootTrigger, behaverSetCheck = bSetCheck, namedCBTriggers = namedCBTrigger, support = new StdBehaviorReadySupport { assetMediator = assetMediator.assetMed } };
				processor.GrammarBlockCommon(behaviorExpressionByGBlock);
				if (rootTrigger.bTriggers.Count > 0 || namedCBTrigger.Count > 0) {
					List<NameAndBTrigger> namedTriggers = null;
					if (namedCBTrigger.Count > 0) {
						namedTriggers = new List<NameAndBTrigger>();
						foreach (var pair in namedCBTrigger) {
							namedTriggers.Add(new NameAndBTrigger { name = pair.Key, bTrigger = pair.Value });
						}
					}
					var result = new BehaviorTriggerSet { rootTrigger = rootTrigger.bTriggers.Count > 0 ? rootTrigger : null, namedTriggers = namedTriggers };
					listener.Take(result);
				}
			}
		}
        private void Update() {
            if(timeBehaver != null) {
                timeBehaver.Update();
            }
        }
        void ConfigurableBehaviorAnalyzer.AddBehaver(Behaver behaver) {
			stdGiver.behavers.Add(behaver);
		}

		class EnsureBehaver : GrammarBlockVisitor {
			public ImmediateGiver<Behaver, GrammarBlock> behaverGiver;
			void GrammarBlockVisitor.IfClusterGrammarBlock(ClusterGrammarBlock cluster) {
				foreach (var subBlock in cluster.blocks) {
					GrammarBlockUtils.VisitGrammarBlock(subBlock, this);
				}
			}

			void GrammarBlockVisitor.IfGrammarUnit(GrammarUnit unit) {
				behaverGiver.PickBestElement(unit);
			}

			void GrammarBlockVisitor.IfHasMetaInfo(GrammarBlock meta) { }

			void GrammarBlockVisitor.IfHasModifier(GrammarBlock mod) { }
		}
	}
}
