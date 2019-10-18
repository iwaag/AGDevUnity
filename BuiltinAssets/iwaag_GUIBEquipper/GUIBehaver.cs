using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDev;
using AGBLang;
using AGDevUnity;
using System;
using AGDev.Native;
using AGDev.StdUtil;
using AGDevUnity.StdUtil;
using AGBLang.StdUtil;

namespace iwaag {
	public class GUIBehaver : MonoBUnityBehaver, UnityBehaver, GObjPickupListener, SpaceInfoListener, Giver<IEnumerable<BehaverAgent>, GrammarBlock> {
		public override UnityBehaver behaver => this;
		ItemType UnityBehaver.GetModule<ItemType>() {
			return default(ItemType);
		}
		BehaverAvatar UnityBehaver.avatar => stdAvatar = stdAvatar ?? new StdBehaverAvatar { agentFactory = this };
		StdBehaverAvatar stdAvatar;
		public BAgentSpace currentSpace;
		public MonoBBehaverAgent textButtonPrefab;
		public List<HitButtonTrigger> buttonHitCheckTriggerList = new List<HitButtonTrigger>();
		public Dictionary<string, MonoBBehaverAgent> nameToButtonDict = new Dictionary<string, MonoBBehaverAgent>();
		public class HitButtonTrigger : UnityBehaviorCheckTrigger {
			public string buttonName;
			public MonoBBehaverAgent button;
			public GUIBehaver parent;
			public BehaviorCheckListener behaviorListener;

			void UnityBehaviorCheckTrigger.BeginBehavior(UnityBehaviorCheckSupportListener _behaviorListener) {
				behaviorListener = _behaviorListener;
				parent.nameToButtonDict.TryGetValue(buttonName, out button);
				if (button != null)
					parent.pickupInterface.AddCandidate(button);
			}
		}
		/*public class AddButtonTrigger : UnityBehaviorTrigger {
			public GUIBehaver parent;
			public BehaviorExpression bExpr;
			void UnityBehaviorTrigger.BeginBehavior(UnityBehaviorSupportListener behaviorListener) {
				GrammarBlockUtils.ForEachUnits(
					bExpr.subject.modifier,
					(unit) => {
						if (!parent.nameToButtonDict.ContainsKey(unit.word)) {
							MonoBBehaverAgent newButton;
							if (parent.currentSpace != null)
								parent.nameToButtonDict[unit.word] = newButton = GameObject.Instantiate(parent.textButtonPrefab, parent.currentSpace.origin);
							else
								parent.nameToButtonDict[unit.word] = newButton = GameObject.Instantiate(parent.textButtonPrefab);
							newButton.GetComponent<TextViewer>().SetText(unit.word);
						}
					}
				);
				UpdateSpace();
				behaviorListener.OnFinish();
			}
			public void UpdateSpace() {
				if (parent.currentSpace == null)
					return;
				var localPos = BehaviorUtilities.LocalPosition(parent.currentSpace, bExpr);
				GrammarBlockUtils.ForEachUnits(
					bExpr.subject.modifier,
					(unit) => {
						if (parent.nameToButtonDict.ContainsKey(unit.word)) {
							var button = parent.nameToButtonDict[unit.word];
							button.transform.SetParent(parent.currentSpace.origin);
							button.transform.localPosition = localPos;
							button.GetComponent<BehaverAgent>().TryFittingSpace(new Vector3(0.4f, 1f, 1));
							localPos.y -= 0.1f;
						}
					}
				);
			}
		}*/
		GrammarBlock lastMatchedCardAttribute;
		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
			lastMatchedCardAttribute = attribute;
			if (attribute.unit.word == "button" || attribute.unit.word == "title" || attribute.unit.word == "text") {
				return AttributeMatchResult.POSITIVE;
			}
			return AttributeMatchResult.NEUTRAL;
		}
		GObjPickupInterface pickupInterface {
			get {
				if (_pickupInterface == null)
					_pickupInterface = FindObjectOfType<MonoBPlayerInterface>().playerInterface.NewGObjChoiceField(this);
				return _pickupInterface;
			}
		}
		GObjPickupInterface _pickupInterface;
		UnityBehaviorTrigger UnityBehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport reqListener) { return null; }
		public HitButtonTrigger NewHitButtonCheckTrigger(string buttonName) {
			buttonHitCheckTriggerList.Add(new HitButtonTrigger { parent = this, buttonName = buttonName });
			return buttonHitCheckTriggerList[buttonHitCheckTriggerList.Count - 1];
		}
		UnityBehaviorCheckTrigger UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport chkReqListener) {
			return null;
		}
		void GObjPickupListener.OnCandidatePicked(MonoBBehaverAgent candidateGObj, Action discardTrigger) {
			foreach (var triggers in buttonHitCheckTriggerList) {
				if (triggers.button == candidateGObj) {
					triggers.behaviorListener.OnResultInPositive();
					break;
				}
			}
		}
		void SpaceInfoListener.OnSpaceUpdated(BAgentSpace spaceInfo) {
			currentSpace = spaceInfo;
		}


		void Giver<IEnumerable<BehaverAgent>, GrammarBlock>.Give(GrammarBlock key, Taker<IEnumerable<BehaverAgent>> colletor) {
			if (GrammarBlockUtils.IsUnit(key, "button")) {
				GrammarBlockUtils.ForEachUnits(
					key.modifier,
					(unit) => {
						MonoBBehaverAgent newButton;
						nameToButtonDict[unit.word] = newButton = GameObject.Instantiate(textButtonPrefab);
						newButton.GetComponent<TextViewer>().SetText(unit.word);
						newButton.GetComponent<BehaverAgent>().TryFittingSpace(new Vector3(0.4f, 1f, 1));
						colletor.Take(new List<BehaverAgent> { newButton.behaverAgent });
					}
				);
			}
		}
	}
}