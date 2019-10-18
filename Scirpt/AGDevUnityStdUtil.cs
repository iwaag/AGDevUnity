using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using AGBLang;
using AGDevUnity;
using AGDev;
using UnityEngine.Networking;

namespace AGDevUnity {
	public class NetworkUtil {
		public static void DefaultWWWEror(string error) {
			Debug.Log(error);
		}
		public static WWWProcessHolder wwwProcessHolder {
			get {
				if (_wwwProcessHolder == null) {
					var gObj = new GameObject();
					gObj.name = "WWWProcessHolder";
					_wwwProcessHolder = gObj.AddComponent<WWWProcessHolder>();
				}
				return _wwwProcessHolder;
			}
		}
		static WWWProcessHolder _wwwProcessHolder;
		public static void ProcessWebRequest(UnityWebRequest www, System.Action<UnityWebRequest> onComplete, System.Action<string> onError = null) {
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) {
				UnityEditor.EditorApplication.CallbackFunction routine = null;
				routine = () => {
					while (!www.isDone)
						return;
					UnityEditor.EditorApplication.update -= routine;
					if (www.error != null) {
						if (onError != null)
							onError(www.error);
						else
							Debug.Log(www.error);
						return;
					}
					onComplete(www);
				};
				UnityEditor.EditorApplication.update += routine;
			}
			else {
				wwwProcessHolder.BeginWWWProcess(www, onComplete);
			}
#else
			wwwProcessHolder.BeginWWWProcess(www, onComplete);
#endif
		}
		public static void DownloadAudio(string url, Taker<AudioClip> taker) {

			var downloadHandler = new DownloadHandlerAudioClip(string.Empty, AudioType.WAV);
			downloadHandler.streamAudio = true;
			var www = new UnityWebRequest(url, "GET", downloadHandler, null);
			ProcessWebRequest(
				www,
				(www2) => {
					var wwwe = www2;
					var content = DownloadHandlerAudioClip.GetContent(www);
					if (content != null) {
						taker.Take(content);
						return;
					}
					taker.None();
				}
			);
		}
	}
	public class UnityBTriggerBuilder {
		public List<UnityBehaviorTrigger> triggers;
		public void AddTrigger(UnityBehaviorTrigger trigger) {
			if (trigger != null) {
				if (triggers == null)
					triggers = new List<UnityBehaviorTrigger>();
				triggers.Add(trigger);
			}
		}
		public UnityBehaviorTrigger GetResult() {
			if (triggers != null) {
				if (triggers.Count == 1)
					return triggers[0];
				return new ClusterUnityBTrigger { triggers = triggers };
			}
			return null;
		}
	}
	public class StdUnityBehaviorReadySupport : UnityBehaviorReadySupport {
		public BehaviorReadySupport basic;
		public ImmediateGiver<UnityBehaver, GrammarBlock> giver;
		BehaviorReadySupport UnityBehaviorReadySupport.basicSupport => basic;

		ImmediateGiver<UnityBehaver, GrammarBlock> UnityBehaviorReadySupport.behaverGiver => giver;
	}
	public class UnityBCheckTriggerBuilder {
		public List<UnityBehaviorCheckTrigger> checkTriggers;
		public void AddTrigger(UnityBehaviorCheckTrigger trigger) {
			if (trigger != null) {
				if (checkTriggers == null)
					checkTriggers = new List<UnityBehaviorCheckTrigger>();
				checkTriggers.Add(trigger);
			}
		}
		public UnityBehaviorCheckTrigger GetResult() {
			if (checkTriggers != null) {
				if (checkTriggers.Count == 1)
					return checkTriggers[0];
				return new UnityOR_BehaviorCheckTrigger { checkTriggers = checkTriggers };
			}
			return null;
		}
	}
	public class UnityOR_BehaviorCheckTrigger : UnityBehaviorCheckTrigger {
		public IEnumerable<UnityBehaviorCheckTrigger> checkTriggers;
		void UnityBehaviorCheckTrigger.BeginBehavior(UnityBehaviorCheckSupportListener listener) {
			var outerListener = new JustOnceBCheckListener { listener = listener };
			foreach (var trigger in checkTriggers) {
				trigger.BeginBehavior(outerListener);
			}
		}
		public class JustOnceBCheckListener : UnityBehaviorCheckSupportListener {
			public UnityBehaviorCheckSupportListener listener;
			public bool didDecidedResult = false;

			BAgentSpace UnityBehaviorSupport.givenSpaceToBehave => listener.givenSpaceToBehave;

			void BehaviorCheckListener.OnResultInNegative() {
				//stub
			}

			void BehaviorCheckListener.OnResultInPositive() {
				if (!didDecidedResult) {
					didDecidedResult = true;
					listener.OnResultInPositive();
				}
			}
		}
	}
	public class ClusterUnityBTrigger : UnityBehaviorTrigger {
		public List<UnityBehaviorTrigger> triggers;
		void UnityBehaviorTrigger.BeginBehavior(UnityBehaviorSupportListener behaviorListener) {
			var outerListener = new BehaviorListenerForCluster { clientListener = behaviorListener, goalCount = triggers.Count };
			foreach (var trigger in triggers) {
				trigger.BeginBehavior(outerListener);
			}
		}
		class BehaviorListenerForCluster : UnityBehaviorSupportListener {
			public int goalCount;
			public int currentCount = 0;
			public UnityBehaviorSupportListener clientListener;
			BAgentSpace UnityBehaviorSupport.givenSpaceToBehave => clientListener.givenSpaceToBehave;
			void BehaviorListener.OnFinish() {
				currentCount++;
				if (currentCount == goalCount) {
					clientListener.OnFinish();
				}
			}
		}
	}
	public class Utilities{
		public static Type ConsistentInstantiate<Type>(Type prefab, Transform parent = null) where Type : MonoBehaviour {
			var instance = GameObject.Instantiate(prefab, parent);
			instance.name = prefab.name;
			return instance;
		}
	}
	public class StubBehaverAvatar : BehaverAvatar {
		public static BehaverAvatar instance => _instance == null ? _instance = new StubBehaverAvatar() : _instance;
		public static StubBehaverAvatar _instance;
		static StubBAgentSpace space;
		public class StubBAgentSpace : BAgentSpace {
			Transform BAgentSpace.origin => dummyTransform == null ? dummyTransform = new GameObject().transform : dummyTransform;
			static Transform dummyTransform;
			Vector3 BAgentSpace.lowerBoundary => -Vector3.one;

			Vector3 BAgentSpace.upperBoundary => Vector3.one;

			void BAgentSpace.AcceptBehaverAgent(BehaverAgent behaverr) { }

			void BAgentSpace.Clear() { }
		}
		AvatarPhysicalizeInterface BehaverAvatar.Physicalize(BAgentSpace spaceInfo) {
			return new StbAPInterface{ };
		}
		public class StbAPInterface : AvatarPhysicalizeInterface {
			void AvatarPhysicalizeInterface.Add(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> asyncTaker){
				asyncTaker.None();
			}
			void AvatarPhysicalizeInterface.Clear() {}
			void AvatarPhysicalizeInterface.Ensure(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> asyncTaker) {
				asyncTaker.None();
			}
			void AvatarPhysicalizeInterface.Search(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> asyncTaker){
				asyncTaker.None();
			}
		}
	}
	public class StdBehaverAvatar : BehaverAvatar {
		public Giver<IEnumerable<BehaverAgent>, GrammarBlock> agentFactory;
		public List<StdAPInterface> apInterfaces = new List<StdAPInterface>();
		AvatarPhysicalizeInterface BehaverAvatar.Physicalize(BAgentSpace spaceInfo) {
			var apInterface = apInterfaces.Find((item) => item.spaceInfo == spaceInfo);
			if(apInterface == null) {
				apInterface = new StdAPInterface { spaceInfo = spaceInfo, agentFactory = agentFactory };
				apInterfaces.Add(apInterface);
			}
			return apInterface;
		}
		public class StdAPInterface : AvatarPhysicalizeInterface {
			public Giver<IEnumerable<BehaverAgent>, GrammarBlock> agentFactory;
			public List<BehaverAgent> agents = new List<BehaverAgent>();
			public BAgentSpace spaceInfo;
			void AvatarPhysicalizeInterface.Add(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> asyncTaker) {
				agentFactory.Give(attribute, asyncTaker);
			}
			void AvatarPhysicalizeInterface.Clear() {
			
			}
			void AvatarPhysicalizeInterface.Ensure(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> taker) {
				bool didCollect = false;
				var pickedAgents = new List<BehaverAgent>();
				foreach (var agent in agents) {
					if (agent.MatchAttribue(attribute) == AttributeMatchResult.POSITIVE) {
						didCollect = true;
						pickedAgents.Add(agent);
					}
				}
				if(pickedAgents.Count > 0){
					taker.Take(pickedAgents);
				}
				if (!didCollect) {
					agentFactory.Give(attribute, new PrvColl { parent = this, taker = taker });
				}
			}
			public class PrvColl : Taker<IEnumerable<BehaverAgent>> {
				public StdAPInterface parent;
				public Taker<IEnumerable<BehaverAgent>> taker;
				void Taker<IEnumerable<BehaverAgent>>.Take(IEnumerable<BehaverAgent> item) {
					foreach (var element in item) {
						parent.agents.Add(element);
						parent.spaceInfo.AcceptBehaverAgent(element);
					}
					taker.Take(item);
				}

				void Taker<IEnumerable<BehaverAgent>>.None() {
					taker.None();
				}
			}
			void AvatarPhysicalizeInterface.Search(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> asyncTaker) {
				var pickedAgents = new List<BehaverAgent>();
				foreach (var agent in agents) {
					if( agent.MatchAttribue(attribute) == AttributeMatchResult.POSITIVE) {
						pickedAgents.Add(agent);
					}
				}
				asyncTaker.Take(pickedAgents);
			}
		}
	}
	public static class NetworkFuncs {
		
	}
}