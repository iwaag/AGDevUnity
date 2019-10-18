using AGBLang;
using AGDev;
using AGDev.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class StdMonoBIBTriggerFactory : MonoBBIntegTriggerFactory {
		public List<PrvtIntegrableBTrigger> createdTriggers = new List<PrvtIntegrableBTrigger>();
		public override IntegrableBTrigger NewIntegrableBTrigger() {
			var newTrigger = new PrvtIntegrableBTrigger();
			createdTriggers.Add(newTrigger);
			return newTrigger;
		}
		void Update() {
			if (Input.GetKeyDown(KeyCode.A)) {
				foreach (var createdTrigger in createdTriggers) {
					foreach (var sesssion in createdTrigger.sessions) {
						(sesssion as BehaviorListener).OnFinish();
					}
					createdTrigger.sessions.RemoveAll((session) => session.didAllFinish);
				}

			}
		}
		public class PrvtIntegrableBTrigger : IntegrableBTrigger {
			public List<NameAndBTrigger> triggers = new List<NameAndBTrigger>();
			public List<PrvtLis> sessions = new List<PrvtLis>();
			void IntegrableBTrigger.AddSubTrigger(NameAndBTrigger namedTrigger) {
				triggers.Add(namedTrigger);
			}
			void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
				var newLis = new PrvtLis { parent = this, triggers = triggers.GetEnumerator(), clientLis = behaviorListener };
				sessions.Add(newLis);
				triggers[0].bTrigger.BeginBehavior(new StubBehaviorListener());
			}
			public class PrvtLis : BehaviorListener {
				public PrvtIntegrableBTrigger parent;
				public BehaviorListener clientLis;
				public IEnumerator<NameAndBTrigger> triggers;
				public bool didAllFinish = false;
				void BehaviorListener.OnFinish() {
					if (triggers.MoveNext()) {
						triggers.Current.bTrigger.BeginBehavior(this);
					} else {
						clientLis.OnFinish();
						didAllFinish = true;
					}
				}
				
			}
		}
	}
}