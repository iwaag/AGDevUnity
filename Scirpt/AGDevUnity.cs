using AGBLang;
using AGDev;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	#region behaver equipper
	public interface BehaverEquipper {
		void EquipBehaverByBehavior(BehaviorExpression bevrExpr, BehaverEquipListener listener);
		void EquipBehaverByAttribute(GrammarBlock attribute, BehaverEquipListener listener);
	}
	public interface BehaverEquipListener {
		void OnEquipSubBehaver(MonoBUnityBehaver baseBehaver);
		void OnEquipAvatar(MonoBBehaverAvatar avatar);
		void OnEquipBehaviorSetter(MonoBUnityBehaviorSetter setterPreafab);
		void OnEquipBehaviorChecker(MonoBUnityBehaviorChecker getterPrefab);
		//void OnEquipAvatar(GameObject target, MonoBBehaverAvatar avatar);
		//void OnEquipSubBehaver(GameObject target, MonoBBFrontRoot avatar);
	}
	#endregion
	#region unity behaver
	public interface UnityBehaviorSupport {
		BAgentSpace givenSpaceToBehave { get; }
		//ImmediateGiver<MonoBUnityBehaver, GrammarBlock> behaverGiver { get; }
	}
	public interface UnityBehaviorReadySupport {
		BehaviorReadySupport basicSupport { get; }
		ImmediateGiver<UnityBehaver, GrammarBlock> behaverGiver { get; }
	}
	public interface UnityBehaviorSupportListener : UnityBehaviorSupport, BehaviorListener { }
	public interface UnityBehaviorCheckSupportListener : UnityBehaviorSupport, BehaviorCheckListener { }
	public interface UnityBehaviorTrigger {
		void BeginBehavior(UnityBehaviorSupportListener behaviorListener);
	}
	public interface UnityBehaviorCheckTrigger {
		void BeginBehavior(UnityBehaviorCheckSupportListener behaviorListener);
	}
	public interface PermissionAsker {
		bool AskPermissionOnCreateTrigger();
	}
	public interface UnityBehaviorReqListener : UnityBehaviorReadySupport { }
	public interface UnityBehaviorCheckReqListener : UnityBehaviorReadySupport { }
	public interface UnityBehaviorSetter {
		UnityBehaviorTrigger ReadyBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport listener);
	}
	public interface UnityBehaviorChecker {
		UnityBehaviorCheckTrigger ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport listener);
	}
	public interface UnityBehaver : UnityBehaviorSetter, UnityBehaviorChecker, AttributeMatcher {
		BehaverAvatar avatar { get; }
		ItemType GetModule<ItemType>();
		//void GiveSubBehaver(GrammarBlock name, Taker<UnityBehaver> subBehaver);
	}
	#endregion
	#region behaver agent
	public interface BehaverAgent : AttributeMatcher {
		GameObject rootBObj { get; }
		BAgentSpace selfSpace { get; }
		BAgentSpace rootSpace { set;  get; }
		BAgentSpace GetSubSapce(GrammarUnit name);
		void DispatchFromSpace();
		void TryFittingSpace(Vector3 size);
	}
	#endregion
	#region behaver avatar
	public interface AvatarPhysicalizeInterface {
		void Add(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> listener);
		void Search(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> listener);
		void Ensure(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> listener);
		void Clear();
	}
	public interface BehaverAvatar {
		AvatarPhysicalizeInterface Physicalize(BAgentSpace spaceInfo);
	}
	#endregion
	#region interaction recorder
	public interface BFrontInteractionRecorder {
		void RecordBFrontInteraction(GameObject bFrontGObj);
	}
	#endregion
	#region player interface
	public interface GObjPickupInterface {
		void AddCandidate(MonoBBehaverAgent candidateGObj);
	}
	public interface GObjPickupListener {
		void OnCandidatePicked(MonoBBehaverAgent candidateGObj, System.Action discardTrigger);
	}
	public interface UnityPlayerInterface {
		GObjPickupInterface NewGObjChoiceField(GObjPickupListener listener);
	}
	#endregion
}
