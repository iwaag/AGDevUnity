using System.Collections.Generic;
using UnityEngine;
using AGBLang;
using AGDev;
using AGBLang.StdUtil;

namespace AGDevUnity {
	public class SeekingBehaverAgent : MonoBBehaverAgent, BehaverAgent, BAgentSpace, Taker<MonoBBehaverAgent> {
		public MonoBAssetMediator assetMed;
		//place holder until picked
		public MonoBBehaverAgent placeHolderPrefab;
		//loaded agent
		public List<SeekingBehaverAgent> subInstances;
		public MonoBBehaverAgent actualAgent;
		public GrammarUnit behaverAttribute;
		GameObject BehaverAgent.rootBObj => gameObject;
		public SeekingBehaverAgent NewInstance() {
			var inst = Instantiate(this);
			inst.gameObject.SetActive(true);
			subInstances.Add(inst);
			return inst;
		}
		void BehaverAgent.DispatchFromSpace() {
			gameObject.SetActive(false);
		}
		public void SetAttribute(GrammarUnit givenAttriibute) {
			behaverAttribute = givenAttriibute;
			actualAgent = GameObject.Instantiate(placeHolderPrefab, transform);
			var textViewer = actualAgent.GetComponent<TextViewer>();
			if (textViewer != null) {
				textViewer.SetText(givenAttriibute.unit.word);
			}
			assetMed.assetMed.SeekAsset(givenAttriibute, this);
		}
		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
			return GrammarBlockUtils.IsUnit(attribute, behaverAttribute.word) ? AttributeMatchResult.POSITIVE : AttributeMatchResult.NEUTRAL;
		}
		public override BehaverAgent behaverAgent => this;

		BAgentSpace BehaverAgent.selfSpace => this;
		BAgentSpace BehaverAgent.rootSpace { get => _rootSpace; set => _rootSpace = value; }
		BAgentSpace _rootSpace;

		Transform BAgentSpace.origin { get { return actualAgent.behaverAgent.selfSpace.origin; } }

		Vector3 BAgentSpace.lowerBoundary { get { return actualAgent.behaverAgent.selfSpace.lowerBoundary; } }

		Vector3 BAgentSpace.upperBoundary { get { return actualAgent.behaverAgent.selfSpace.upperBoundary; } }

		void BAgentSpace.AcceptBehaverAgent(BehaverAgent behaver) {
			behaver.rootBObj.transform.SetParent(transform);
		}

		void BehaverAgent.TryFittingSpace(Vector3 size) {
			actualAgent.behaverAgent.TryFittingSpace(size);
		}

		void Taker<MonoBBehaverAgent>.None() {}
		void Taker<MonoBBehaverAgent>.Take(MonoBBehaverAgent newElement) {
			if (actualAgent != null) {
				Destroy(actualAgent.gameObject);
			}
			actualAgent = GameObject.Instantiate(newElement, transform);
			foreach (var subInstance in subInstances) {
				(subInstance as Taker<MonoBBehaverAgent>).Take(newElement);
			}
		}

		void BAgentSpace.Clear() {

		}

		BAgentSpace BehaverAgent.GetSubSapce(GrammarUnit name) {
			return actualAgent.behaverAgent.GetSubSapce(name);
		}
	}
}