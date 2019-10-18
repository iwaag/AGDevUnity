using AGDev.StdUtil;
using AGDevUnity.StdUtil;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
using AGDev;
using AGBLang.StdUtil;

namespace AGDevUnity {
	public class CustomizableUnityBehaver : MonoBUnityBehaver, UnityBehaver, Giver<IEnumerable<BehaverAgent>, GrammarBlock> {
		public StdBehaverAvatar stdAvatar => _stdAvatar = _stdAvatar ?? new StdBehaverAvatar { agentFactory = this };
		ItemType UnityBehaver.GetModule<ItemType>() {
			if (baseBehaver != null)
				return baseBehaver.behaver.GetModule<ItemType>();
			return default(ItemType);
		}
		StdBehaverAvatar _stdAvatar;
		public string nameOnCreate;
		public override UnityBehaver behaver => this;
		BehaverAvatar UnityBehaver.avatar {
			get {
				if(baseBehaver != null)
					return baseBehaver.behaver.avatar;
				else if (seekingAgentPrefab != null)
					return stdAvatar;
				return null;
			}
		}
		public MonoBUnityBehaver baseBehaver;
		public SeekingBehaverAgent seekingAgentPrefab;
		public List<MonoBUnityBehaviorSetter> bSetters = new List<MonoBUnityBehaviorSetter>();
		public List<MonoBUnityBehaviorChecker> bCheckers = new List<MonoBUnityBehaviorChecker>();
		/*ConvertingEnumarable<BehaviorSetter, MonoBBehaviorSetter> setterEnumerable;
		ConvertingEnumarable<BehaviorChecker, MonoBBehaviorChecker> checkerEnumerable;
		public BehaviorSetter setter;
		public BehaviorChecker checker;
		public override IEnumerable<BehaviorSetter> behaviorSetters {
			get {
				if(setterEnumerable == null)
					setterEnumerable = new ConvertingEnumarable<BehaviorSetter, MonoBBehaviorSetter> { convertFunc = (elem) => elem.behaviorSetter, gObjEnumerable = bSetters };
				return setterEnumerable;
			}
		}
		public override IEnumerable<BehaviorChecker> behaviorCheckers {
			get {
				if (checkerEnumerable == null)
					checkerEnumerable = new ConvertingEnumarable<BehaviorChecker, MonoBBehaviorChecker> { convertFunc = (elem) => elem.behaviorChecker, gObjEnumerable = bCheckers };
				return checkerEnumerable;
			}
		}

		public override BehaviorSetter behaviorSetter {
			get {
				if (setter == null)
					setter = new ClusterBehaviorSetter { bSetters = behaviorSetters };
				return setter;
			}
		}
		
		public override BehaviorChecker behaviorChecker {
			get {
				if (checker == null)
					checker = new ClusterBehaviorChecker { bCheckers = behaviorCheckers };
				return checker;
			}
		}

		public override MonoBUnityBehaver baseUnityBehaver {
			get {
				throw new NotImplementedException();
			}

			set {
				throw new NotImplementedException();
			}
		}
		*/
		UnityBehaviorTrigger UnityBehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport support) {
			var builder = new UnityBTriggerBuilder();
			if (baseBehaver != null)
				builder.AddTrigger(baseBehaver.behaver.ReadyBehavior(bExpr, support));
			foreach (var bSetter in bSetters) {
				builder.AddTrigger(bSetter.behaviorSetter.ReadyBehavior(bExpr, support));
			}
			return builder.GetResult();
		}
		UnityBehaviorCheckTrigger UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport support) {
			var builder = new UnityBCheckTriggerBuilder();
			if (baseBehaver != null)
				baseBehaver.behaver.ReadyCheckBehavior(bExpr, support);
			foreach (var bChecker in bCheckers) {
				bChecker.behaviorChecker.ReadyCheckBehavior(bExpr, support);
			}
			return builder.GetResult();
		}
		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
			if (baseBehaver != null)
				return baseBehaver.behaver.MatchAttribue(attribute);
			if (attribute.unit != null)
				return nameOnCreate == attribute.unit.word ? AttributeMatchResult.POSITIVE : AttributeMatchResult.NEGATIVE;
			return AttributeMatchResult.NEGATIVE;
		}
		void Giver<IEnumerable<BehaverAgent>, GrammarBlock>.Give(GrammarBlock key, Taker<IEnumerable<BehaverAgent>> colletor) {
			if (GrammarBlockUtils.IsUnit(key, nameOnCreate)) {
				var inst = Utilities.ConsistentInstantiate(seekingAgentPrefab);
				inst.behaverAttribute = seekingAgentPrefab.behaverAttribute;
				inst.gameObject.SetActive(true);
				colletor.Take(new List<BehaverAgent> { inst.behaverAgent });
			}
		}
	}
}
