using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
namespace AGDevUnity {
	public abstract class MonoBBehaverEventHub : MonoBehaviour {
		public abstract BehaverEventHub behaverEventHub { get; }
	}
	public interface BehaverEventListener {
		void OnAllAgentGObjSpecified(IEnumerable<MonoBBehaverAgent> behaver);
	}
	public class ClusterBehaverEventListener : BehaverEventListener {
		public List<BehaverEventListener> listeners = new List<BehaverEventListener>();
		void BehaverEventListener.OnAllAgentGObjSpecified(IEnumerable<MonoBBehaverAgent> behaver) {
			foreach (var listener in listeners) {
				listener.OnAllAgentGObjSpecified(behaver);
			}
		}
	}
	public interface BehaverEventHub {
		void ListenBehaverEvent(GrammarBlock grammarBlock, BehaverEventListener listener);
		BehaverEventListener GetEventListener(GrammarBlock grammarBlock);
	}
}