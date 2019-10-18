using AGDevUnity;
using AGDev.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
using AGBLang.StdUtil;

namespace AGDevUnity {
	public class PlaceHolderBehaverAgent : MonoBBehaverAgent, BehaverAgent, BAgentSpace {
		public GrammarUnit behaverAttribute;
		public Transform frame;
		public Vector3 _lowerBoundary;
		public Vector3 _upperBoundary;
		public override BehaverAgent behaverAgent => this;
		BAgentSpace BehaverAgent.selfSpace => this;
		BAgentSpace BehaverAgent.rootSpace { get => _rootSpace; set => _rootSpace = value; }
		BAgentSpace _rootSpace;
		Transform BAgentSpace.origin { get { return transform; } }
		Vector3 BAgentSpace.lowerBoundary { get { return _lowerBoundary; } }
		Vector3 BAgentSpace.upperBoundary { get { return _upperBoundary; } }

		GameObject BehaverAgent.rootBObj => gameObject;

		void BAgentSpace.AcceptBehaverAgent(BehaverAgent agent) {
			agent.rootBObj.transform.SetParent(transform);
		}

		void BAgentSpace.Clear() {}

		void BehaverAgent.DispatchFromSpace() {}

		BAgentSpace BehaverAgent.GetSubSapce(GrammarUnit name) {
			return this;
		}

		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
			return GrammarBlockUtils.IsUnit(attribute, behaverAttribute.word) ? AttributeMatchResult.POSITIVE : AttributeMatchResult.NEUTRAL;
		}
		void BehaverAgent.TryFittingSpace(Vector3 size) {
			frame.localScale = size;
		}
	}
}