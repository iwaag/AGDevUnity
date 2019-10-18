using AGDev;
using AGBLang;
using AGDev.StdUtil;
using AGDevUnity;
using AGDevUnity.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBehaver : MonoBUnityBehaver, UnityBehaver {
	public override UnityBehaver behaver => this;

	BehaverAvatar UnityBehaver.avatar => StubBehaverAvatar.instance;

	AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
		return attribute.unit != null ?
			(attribute.unit.word.StartsWith("how ",	System.StringComparison.CurrentCultureIgnoreCase) ? AttributeMatchResult.POSITIVE : AttributeMatchResult.NEGATIVE)
			: AttributeMatchResult.NEGATIVE;
	}

	UnityBehaviorTrigger UnityBehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport support) {
		return null;
	}

	UnityBehaviorCheckTrigger UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport support) {
		return null;
	}
	ItemType UnityBehaver.GetModule<ItemType>() {
		return default(ItemType);
	}
}
