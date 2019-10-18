using AGBLang;
using AGBLang.StdUtil;
using AGDev;
using AGDevUnity;
using System.Collections;
using UnityEngine;

public class SystemBehaver : MonoBUnityBehaver, UnityBehaver {
	public TextViewer textViewerPrefab;
	public BAgentSpace currentSpace;
	public MeshRenderer faderPrefab;
	public MonoBSpaceDistributer spaceD => FindObjectOfType<MonoBSpaceDistributer>();
	BehaverAvatar UnityBehaver.avatar => _avatar = _avatar ?? new StubBehaverAvatar { };
	public BehaverAvatar _avatar;
	public override UnityBehaver behaver {
		get { return this; }
	}

	public class ShowNumberTrigger : UnityBehaviorTrigger, AnswerListener<float> {
		public SystemBehaver parent;
		public GrammarBlock modifier;
		public TextViewer textViewerPrefab;
		public TextViewer textViewerInstance;
		void UnityBehaviorTrigger.BeginBehavior(UnityBehaviorSupportListener behaviorListener) {
			var space = behaviorListener.givenSpaceToBehave;
			//FindObjectOfType<MonoBEWordsIptr>().anlysPackage.bAnalyser.AskForFloatAnswer(modifier, this);
			if (textViewerInstance == null)
				textViewerInstance = GameObject.Instantiate(textViewerPrefab, space.origin);
			else
				textViewerInstance.transform.SetParent(space.origin);
			textViewerInstance.gameObject.SetActive(true);
		}
		void AnswerListener<float>.OnAnswerUpdate(float answer) {
			textViewerInstance.SetText("Player's point: " + answer.ToString());
		}
	}
	public class QuitTrigger : UnityBehaviorTrigger {
		void UnityBehaviorTrigger.BeginBehavior(UnityBehaviorSupportListener behaviorListener) {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
			behaviorListener.OnFinish();
		}
	}
	public class ClearAllSpaces : UnityBehaviorTrigger {
		public MonoBSpaceDistributer spaceDistributer;
		void UnityBehaviorTrigger.BeginBehavior(UnityBehaviorSupportListener behaviorListener) {
			foreach (var space in spaceDistributer.spaceDistributer.spaces) {
				space.Clear();
			}
			behaviorListener.OnFinish();
		}
	}
	public class FadeProcess {
		public BehaviorListener currentListner;
		public MeshRenderer fadeEffector;
		public float currentAlpha;
		public float deltaAlpha;
		public bool didFinish = false;
		public void BeginFade(UnityBehaviorSupportListener _currentListner) {
			currentListner = new PrvtLis { clientListener = _currentListner, parent = this };
			didFinish = false;
		}
		public void Update() {
			if (currentListner != null && !didFinish) {
				currentAlpha += deltaAlpha;
				if (deltaAlpha > 0 && currentAlpha > 1) {
					currentAlpha = 1;
					currentListner.OnFinish();
				}
				if (deltaAlpha < 0 && currentAlpha < 0) {
					currentAlpha = 0;
					currentListner.OnFinish();
				}
				fadeEffector.material.color = new Color(0, 0, 0, currentAlpha);
			}
		}
		class PrvtLis : BehaviorListener {
			public FadeProcess parent;
			public BehaviorListener clientListener;
			void BehaviorListener.OnFinish() {
				parent.currentListner = null;
				parent.didFinish = true;
				clientListener.OnFinish();
			}
		}
	}
	public class FadeTrigger : UnityBehaviorTrigger {
		public FadeProcess fadeProcess;
		public float fadeDelta = 1;
		void UnityBehaviorTrigger.BeginBehavior(UnityBehaviorSupportListener behaviorListener) {
			if (fadeProcess.currentListner != null && !fadeProcess.didFinish) {
				fadeProcess.currentListner.OnFinish();
			}
			fadeProcess.deltaAlpha = fadeDelta;
			fadeProcess.BeginFade(behaviorListener);
		}
	}
	public FadeProcess fadeProcess {
		get {
			if (_fadeTrigger == null) {
				var fader = GameObject.Instantiate(faderPrefab, Camera.main.transform);
				_fadeTrigger = new FadeProcess {
					fadeEffector = fader,
				};
				fader.material.color = new Color(0, 0, 0, 0);
			}
			return _fadeTrigger;
		}
	}
	public FadeProcess _fadeTrigger;
	AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
		if (GrammarBlockUtils.IsUnit(attribute, "system"))
			return AttributeMatchResult.POSITIVE;
		return AttributeMatchResult.NEUTRAL;
	}
	void Update() {
		fadeProcess.Update();
	}
	UnityBehaviorTrigger UnityBehaviorSetter.ReadyBehavior(BehaviorExpression behavior, UnityBehaviorReadySupport support) {
		if (CaseInsensitiveComparer.Equals(behavior.verb.word, "show")) {
			var howMany = GrammarBlockUtils.ShallowSeekModifier(behavior.verb, "how many");
			if (howMany != null) {
				var trigger = new ShowNumberTrigger { modifier = behavior.verb.modifier, textViewerPrefab = textViewerPrefab };
				return trigger;
			}
			else {
				//stub : show something
			}
		}
		else if (CaseInsensitiveComparer.Equals(behavior.verb.word, "clear")) {
			if (GrammarBlockUtils.ShallowSeekModifier(behavior.verb, "space") != null) {
				var trigger = new ClearAllSpaces { spaceDistributer = spaceD };
				return trigger;
			}
		}
		else if (CaseInsensitiveComparer.Equals(behavior.verb.word, "quit")) {
			return new QuitTrigger();
		}
		else if (CaseInsensitiveComparer.Equals(behavior.verb.word, "fade in")) {
			return new FadeTrigger { fadeDelta = -0.01f, fadeProcess = fadeProcess };
		}
		else if (CaseInsensitiveComparer.Equals(behavior.verb.word, "fade out")) {
			return new FadeTrigger { fadeDelta = 0.01f, fadeProcess = fadeProcess };
		}
		return null;
	}

	UnityBehaviorCheckTrigger UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression behavior, UnityBehaviorReadySupport support) {
		return null;
	}

	ItemType UnityBehaver.GetModule<ItemType>() {
		return default(ItemType);
	}
}
