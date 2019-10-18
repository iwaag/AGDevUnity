using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
namespace AGDevUnity.StdUtil {
	public class StdMonoBBehaverEventHub : MonoBBehaverEventHub, BehaverEventHub {
		public override BehaverEventHub behaverEventHub => this;

		public Dictionary<string, ClusterBehaverEventListener> eventListenerDict = new Dictionary<string, ClusterBehaverEventListener>();
		BehaverEventListener BehaverEventHub.GetEventListener(GrammarBlock grammarBlock) {
			return GetCluster(grammarBlock);
		}
		void BehaverEventHub.ListenBehaverEvent(GrammarBlock grammarBlock, BehaverEventListener listener) {
			GetCluster(grammarBlock).listeners.Add(listener);
		}
		ClusterBehaverEventListener GetCluster(GrammarBlock grammarBlock) {
			if (grammarBlock.unit != null) {
				eventListenerDict.TryGetValue(grammarBlock.unit.word, out var cluster);
				if (cluster == null) {
					cluster = new ClusterBehaverEventListener();
					eventListenerDict.Add(grammarBlock.unit.word, cluster);
				}
				return cluster;
			}
			return null;
		}
	}
}