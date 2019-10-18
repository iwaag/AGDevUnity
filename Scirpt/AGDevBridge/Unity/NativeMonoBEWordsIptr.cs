using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AGDev;
using AGDevUnity;
using AGDev.Native;
namespace AGDev.Native.Unity {
#if false
    public class AGDevBridgeFunctions {
		[DllImport("AGDevStdBridge")]
		public extern static void OnBehaviorRequestSucceed(System.IntPtr behaviorRequestListener, System.IntPtr behaviorTrigger);
		[DllImport("AGDevStdBridge")]
		public extern static void OnBehaviorCheckRequestSucceed(System.IntPtr behaviorCheckRequestListener, System.IntPtr behaviorCheckTrigger);
	}
	[RequireComponent(typeof(MonoBBaseFactory))]
	public class NativeMonoBEWordsIptr : MonoBEWordsIptr, ERWordsInterpreter {
		public BridgeHelperFactoryBridge helperFactory;
		public TextAsset[] formatConfigurations;
		public List<TextAsset> grammarConfigurations;
		public MonoBBaseFactory baseFactory;
		public MonoBBahaverFactory behaverGiver;
		InterpreterPackage iptrPackage;
		//public MonoBBehaviorContextListener contextListener;
		public bool initOnAwake = false;
		public override ERWordsInterpreter ewIptr {
			get {
				if (iptrPackage == null) {
					Init();
				}
				return this;
			}
		}

		List<NativeBehaviorTrigger> behaviorTriggers = new List<NativeBehaviorTrigger>();
#if false
        StdExprBehaverFactory stdNativeBehaverFactory;
		NativeTimeManager timeManager;
#endif
		void Init() {
#if false
			stdNativeBehaverFactory = new StdExprBehaverFactory();
			timeManager = new NativeTimeManager(stdNativeBehaverFactory);
			iptrPackage = new StdInterpreterPackage(baseFactory.iptrFactory, behaverGiver.behaverGiver);
			foreach (var formatConfiguration in formatConfigurations)
				iptrPackage.fAnalyser.ConfigureFormat(formatConfiguration.bytes);
			foreach (var grammarConfiguration in grammarConfigurations)
				iptrPackage.gAnalyser.ConfigureGrammar(grammarConfiguration.bytes);
#endif
        }

        BehaviorTrigger ERWordsInterpreter.InterpretERWordsAsBehavior(byte[] erWords) {
			return iptrPackage.erWordsIptr.InterpretERWordsAsBehavior(erWords);
			//return iptrPackage.contextIptr.InterpretWithContext(erWords, FindObjectOfType<MonoBBehaviorContextListener>().behaviorContextListener);
		}
		void Awake() {
			//if(initOnAwake)
			//	Init();
		}
		public List<NativeBehaviorTrigger> playAudioTriggers = new List<NativeBehaviorTrigger>();
	}
#endif
}