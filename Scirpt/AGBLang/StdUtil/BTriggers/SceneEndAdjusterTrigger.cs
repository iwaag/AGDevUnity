using AGDevUnity;
using AGAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDev;
using AGDevUnity.StdUtil;

namespace AGDevUnity.StdUtil {
	public class SceneEndAdjusterTrigger : MonoBBehaviorTrigger, ImmediateGiver<BehaviorTrigger, string> {
		public NamedProcessBehaver parent;
		public MonoBBehaviorTrigger rawProcessTrigger;
		public override ImmediateGiver<BehaviorTrigger, string> bTriggerGiver => this;

		BehaviorTrigger ImmediateGiver<BehaviorTrigger, string>.PickBestElement(string key) {
			var rawProcess = rawProcessTrigger.bTriggerGiver.PickBestElement(key);
			if (rawProcess == null)
				return null;
			var def = parent.FindDef(key);
			if (def == null)
				return rawProcess;
			return new PrvtTrigger { rawTrigger = rawProcess, def = def };
		}
		public class PrvtTrigger : BehaviorTrigger {
			public BehaviorTrigger rawTrigger;
			public UnityBehaviorDefinition def;
			void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
				def.rawProcessListener = behaviorListener;
				rawTrigger.BeginBehavior(new PrvtLis { clientListener = behaviorListener, def = def });
			}
			public class PrvtLis : BehaviorListener {
				public UnityBehaviorDefinition def;
				public BehaviorListener clientListener;
				void BehaviorListener.OnFinish() {
					if( ! def.doEndExplicit) {
						clientListener.OnFinish();
					}
				}
			}
		}
	}
}