using System.Collections.Generic;
using UnityEngine;
using AGDevUnity;
using System.Linq;
using AGDev.StdUtil;
using AGBLang;
using AGDev;

namespace AGDevUnity.StdUtil
{
	public class NamedProcessBehaver :
		MonoBBehaver,
		Behaver
	{
		//public UnityEngine.Events.UnityEvent OnGetBTrigger;
		[System.Serializable]
		public class ProcessGroup
		{
			public string groupName;
			public List<string> members;
			public List<string> preProcess;
			public List<string> postProcess;
			public bool isSingleton = false;
		}
		public MonoBBehaviorTrigger bTriggerGiver;
		public List<UnityBehaviorDefinition> behaviorDefs;
		public List<ProcessGroup> processGroups;
		public Stack<UnityBehaviorDefinition> bDefStack {
			get {
				if (_bDefStack == null)
				{
					_bDefStack = new Stack<UnityBehaviorDefinition>();
				}
				return _bDefStack;
			}
		}

		public override Behaver behaver => this;

		public UnityBehaviorDefinition FindDef(string processName) {
			return behaviorDefs.Find((elem) => string.Compare(elem.behaviorName, processName, true) == 0);
		}
		private Stack<UnityBehaviorDefinition> _bDefStack;
		public CustomizableUnityBehaver bRootPrefab;
		public BehaviorTrigger NewBeginTrigger(UnityBehaviorDefinition bDef)
		{
			if (bDef.currentRunningTrigger == null)
			{
				bDefStack.Push(bDef);
				var process = bTriggerGiver.bTriggerGiver.PickBestElement(bDef.behaviorName);
				bDefStack.Pop();
				bDef.currentRunningTrigger = new NamedProcessTrigger { bDef = bDef, parent = this, process = process };
			}
			return bDef.currentRunningTrigger;
		}
		BehaviorTrigger BehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, BehaviorReadySupport reqListener)
		{
			if (bExpr.subject.unit != null)
			{
				var bDef = FindDef(bExpr.subject.unit.word);
				if (bDef != null)
				{
					if (string.Compare(bExpr.verb.word, "begin", true) == 0)
					{
						return NewBeginTrigger(bDef);
					}
					if (string.Compare(bExpr.verb.word, "end", false) == 0)
					{
						bDef.doEndExplicit = true;
						return new NamedProcessEndTRigger { behaviorDef = bDef };
					}
				}
			}
			return null;
		}
		BehaviorCheckTrigger BehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, BehaviorReadySupport chkReqListener){
			return null;
		}
		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute)
		{
			if (attribute.unit != null)
			{
				return FindDef(attribute.unit.word) != null ? AttributeMatchResult.POSITIVE : AttributeMatchResult.NEGATIVE;
			}
			else
			{
			}
			return AttributeMatchResult.NEGATIVE;
		}
	}
#if false
	public class DependentBehaver : UnityBehaver, Taker<NounCommonSenseUnit> {
		public class DependenntBTrigger : UnityBehaviorTrigger, UnityBehaviorReqListener {
			public DependentBehaver parent;
			public List<BehaviorSupportListener> bListeners = new List<BehaviorSupportListener>();
			public List<BehaviorCheckSupportListener> bCheckListeners = new List<BehaviorCheckSupportListener>();
			public UnityBehaviorTrigger actualBTrigger;
			public UnityBehaviorTrigger actualBCTrigger;
			public BehaviorExpression bExpr;
			public void OnGetActualBehaver() {
				parent.actualBehaver.ReadyBehavior(bExpr, this);
			}

			void UnityBehaviorTrigger.BeginBehavior(BehaviorSupportListener behaviorListener) {
				bListeners.Add(behaviorListener);
			}

			void Taker<UnityBehaviorTrigger>.Take(UnityBehaviorTrigger item) {
				actualBTrigger = item;
				foreach (var bListener in bListeners) {
					actualBTrigger.BeginBehavior(bListener);
				}
				foreach (var bListener in bListeners) {
					actualBCTrigger.BeginBehavior(bListener);
				}
			}

			bool PermissionAsker.AskPermissionOnCreateTrigger() {
				return true;
			}

			
		}
		public class DependenntBCTrigger : UnityBehaviorCheckTrigger, UnityBehaviorCheckReqListener {
			public DependentBehaver parent;
			public List<BehaviorCheckSupportListener> bCheckListeners = new List<BehaviorCheckSupportListener>();
			public UnityBehaviorCheckTrigger actualBCTrigger;
			public BehaviorExpression bExpr;
			public void OnGetActualBehaver() {
				parent.actualBehaver.ReadyCheckBehavior(bExpr, this);
			}
			void Taker<UnityBehaviorCheckTrigger>.Take(UnityBehaviorCheckTrigger item) {
				actualBCTrigger = item;
				foreach (var bcListener in bCheckListeners) {
					actualBCTrigger.BeginBehavior(bcListener);
				}
			}
			void UnityBehaviorCheckTrigger.BeginBehavior(BehaviorCheckSupportListener behaviorListener) {
				bCheckListeners.Add(behaviorListener);
			}
			bool PermissionAsker.AskPermissionOnCreateTrigger() {
				return true;
			}
		}
		public List<DependenntBTrigger> depBTriggers = new List<DependenntBTrigger>();
		public List<DependenntBCTrigger> depBCTriggers = new List<DependenntBCTrigger>();
		public ImmediateGiver<MonoBUnityBehaver, GrammarBlock> parent;
		public UnityBehaver actualBehaver;
		public GrammarUnit baseGUnit;
		BehaverAvatar UnityBehaver.avatar {
			get {
				if (actualBehaver != null) {
					return actualBehaver.avatar;
				}
				return StubBehaverAvatar.instance;
			}
		}

		//public Taker dependencyBehavers;
		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
			if (actualBehaver != null)
				return actualBehaver.MatchAttribue(attribute);
			return GrammarBlockUtils.IsUnit(attribute, baseGUnit.word) ? AttributeMatchResult.POSITIVE : AttributeMatchResult.NEUTRAL;
			
		}

		void Taker<NounCommonSenseUnit>.Take(NounCommonSenseUnit item) {
			foreach (var giver in item.givenBy) {
				actualBehaver = parent.PickBestElement(new StdGrammarUnit(giver)).behaver;
				break;
			}
			foreach (var bTrigger in depBTriggers) {
				bTrigger.OnGetActualBehaver();
			}
		}

		void UnityBehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, UnityBehaviorReqListener listener) {
			if(actualBehaver != null) {
				actualBehaver.ReadyBehavior(bExpr, listener);
			}
			else {
				var newDepTrigger = new DependenntBTrigger { parent =this, bExpr = bExpr };
				depBTriggers.Add(newDepTrigger);
			}
		}

		void UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorCheckReqListener listener) {
			if (actualBehaver != null) {
				actualBehaver.ReadyCheckBehavior(bExpr, listener);
			}
			else {
				var newDepTrigger = new DependenntBCTrigger { parent = this, bExpr = bExpr };
				depBCTriggers.Add(newDepTrigger);
			}
		}
	}
#endif
	[System.Serializable]
	public class UnityBehaviorDefinition
	{
		public string behaviorName;
		public bool doEndExplicit = false;
		public MonoBSpaceDistributer spaceDistributer;
		public NamedProcessTrigger currentRunningTrigger;
		public BehaviorListener rawProcessListener;
	}
	public class NamedProcessTrigger : BehaviorTrigger
	{
		public NamedProcessBehaver parent;
		public UnityBehaviorDefinition bDef;
		public BehaviorTrigger process;
		void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener)
		{
			process.BeginBehavior(new StubBehaviorListener());
			behaviorListener.OnFinish();
		}
	}
	public class NamedProcessEndTRigger : BehaviorTrigger
	{
		public UnityBehaviorDefinition behaviorDef;
		void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener)
		{
			behaviorDef.rawProcessListener.OnFinish();
			behaviorListener.OnFinish();
		}
	}
	//[RequireComponent(typeof(StdMonoBBFrontInteractionRecorder))]
}