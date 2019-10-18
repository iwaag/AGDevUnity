using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AGDev.StdUtil;
using System;

namespace AGDev.Native {
#if false
    #region behaver
    public class BehaverBridge {
		public SinglePtrReturnIntCallback matchAttributeCallback;
		public TwoPtrCallback setBehaviorCallback;
		public TwoPtrCallback checkBehaviorCallback;
		BridgeHelperFactoryBridge helperFactory;
		public Behaver behaverFront;
		public BehaverBridge(Behaver _behaverFront, BridgeHelperFactoryBridge _helperFactory) {
			helperFactory = _helperFactory;
			behaverFront = _behaverFront;
			matchAttributeCallback = (newAttributeGBlockPtr) => {
				try {
					var gBlock = NativeGrammarBlockUtils.GBlockPtrToGBlock(newAttributeGBlockPtr);
					var bf = this.behaverFront;
					return (int)behaverFront.MatchAttribue(gBlock);
				} catch (System.Exception e) {
					Debug.LogError(e);
					return (int)AttributeMatchResult.NEUTRAL;
				}
			};
			setBehaviorCallback = (behaviorExpression, listener) => {
				try {
					var gBlock = NativeGrammarBlockUtils.GBlockPtrToGBlock(behaviorExpression);
					behaverFront.ReadyBehavior(GrammarBlockUtils.GBlockToBExpression(gBlock), new NativeBehaviorRequestListener(listener, helperFactory));
					Debug.Log("READY Set Behaver Front behavior: " + gBlock.cluster.blocks[1].unit.word);
				} catch (Exception e) {
					Debug.LogError(e);
				}
			};
			checkBehaviorCallback = (behaviorExpression, listener) => {
				try {
					var gBlock = NativeGrammarBlockUtils.GBlockPtrToGBlock(behaviorExpression);
					behaverFront.ReadyCheckBehavior(GrammarBlockUtils.GBlockToBExpression(gBlock), new NativeBehaviorCheckRequestListener(listener, helperFactory));
					Debug.Log("READY Check Behaver Front behavior: " + gBlock.cluster.blocks[1].unit.word);
				} catch (Exception e) { Debug.LogError(e); }
			};
		}
	}
	#endregion
	#region behavior
	public class NativeBehaviorListener : BehaviorListener {
		public System.IntPtr implPtr;
		public NativeBehaviorListener(System.IntPtr _implPtr) {
			implPtr = _implPtr;
		}
		void BehaviorListener.OnFinish() {
			OnBehaviorFinish(implPtr);
		}
		[DllImport("AGDevStdBridge")]
		public extern static void OnBehaviorFinish(System.IntPtr behaviorListener);
	};
	public class NativeBehaviorRequestListener : BehaviorRequestListener {
		public System.IntPtr implPtr;
		public BridgeHelperFactoryBridge helperFacytory;
		public NativeBehaviorRequestListener(System.IntPtr _implPtr, BridgeHelperFactoryBridge _helperFacytory) {
			helperFacytory = _helperFacytory;
			implPtr = _implPtr;
		}
		void BehaviorRequestListener.OnSucceed(BehaviorTrigger controller) {
			OnBehaviorRequestSucceed(implPtr, helperFacytory.NewBehaviorTriggerBridge(controller));
		}
		[DllImport("AGDevStdBridge")]
		public extern static void OnBehaviorRequestSucceed(System.IntPtr behaviorRequestListener, System.IntPtr behaviorTrigger);
	}
	public class NativeBehaviorTrigger : BehaviorTrigger {
		public System.IntPtr implPtr;
		public BridgeHelperFactoryBridge helperFacytory;
		public NativeBehaviorTrigger(System.IntPtr _implPtr, BridgeHelperFactoryBridge _helperFacytory) {
			helperFacytory = _helperFacytory;
			implPtr = _implPtr;
		}
		void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
			try {
				BeginBehavior(implPtr, helperFacytory.NewBehaviorListenerBridge(behaviorListener));
			} catch (System.Exception e) {
				UnityEngine.Debug.Log(e);
			}
		}
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr BeginBehavior(System.IntPtr factory, System.IntPtr signalCallback);

	}
	public class BehaviorListenerBridge {
		public System.IntPtr ImplPtr;
		BehaviorListener listener;
		System.IntPtr helperFactory;
		SignalCallback onFinishCallback;
		public BehaviorListenerBridge(System.IntPtr _helperFactory, BehaviorListener _listener) {
			listener = _listener;
			helperFactory = _helperFactory;
			onFinishCallback = () => {
				listener.OnFinish();
			};
			ImplPtr = NewBehaviorListener(helperFactory, onFinishCallback);
		}
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewBehaviorListener(
			System.IntPtr behaviorFactory,
			SignalCallback onFinishCallback
		);
	}
	public class BehaviorTriggerBridge {
		public System.IntPtr ImplPtr;
		BehaviorTrigger controller;
		System.IntPtr helperFactory;
		SinglePtrCallback beginBehaviorCallback;
		SignalCallback stopBehaviorCallback;
		SignalCallback continueBehaviorCallback;
		public BehaviorTriggerBridge(System.IntPtr _helperFactory, BehaviorTrigger _controller) {
			controller = _controller;
			helperFactory = _helperFactory;
			beginBehaviorCallback = (bhvrListener) => {
				controller.BeginBehavior(new NativeBehaviorListener(bhvrListener));
			};
			ImplPtr = NewBehaviorTrigger(helperFactory,
				beginBehaviorCallback,
				stopBehaviorCallback = () => { },
				continueBehaviorCallback = () => { }
			);
		}
		[DllImport("AGDevStdBridge")]
		public extern static System.IntPtr NewBehaviorTrigger(
			System.IntPtr behaviorFactory,
			SinglePtrCallback beginBehaviorCallback,
			SignalCallback stopCallback,
			SignalCallback continueCallback
		);
	}
	#endregion
	#region behavior check
	public class NativeBehaviorCheckRequestListener : BehaviorCheckRequestListener {
		public System.IntPtr implPtr;
		public BridgeHelperFactoryBridge helperFacytory;
		public NativeBehaviorCheckRequestListener(System.IntPtr _implPtr, BridgeHelperFactoryBridge _helperFacytory) {
			helperFacytory = _helperFacytory;
			implPtr = _implPtr;
		}
		void BehaviorCheckRequestListener.OnSucceed(BehaviorCheckTrigger controller) {
			OnBehaviorCheckRequestSucceed(implPtr, helperFacytory.NewBehaviorCheckTriggerBridge(controller));
		}
		[DllImport("AGDevStdBridge")]
		public extern static void OnBehaviorCheckRequestSucceed(System.IntPtr behaviorRequestListener, System.IntPtr behaviorTrigger);
	}
	public class NativeBehaviorCheckListener : BehaviorCheckListener {
		public System.IntPtr implPtr;
		public NativeBehaviorCheckListener(System.IntPtr _implPtr) {
			implPtr = _implPtr;
		}
		void BehaviorCheckListener.OnResultInPositive() {
			OnResultInPositive(implPtr);
		}
		void BehaviorCheckListener.OnResultInNegative() {
			OnResultInNegative(implPtr);
		}
		[DllImport("AGDevStdBridge")]
		public extern static void OnResultInPositive(System.IntPtr behaviorCheckListener);
		[DllImport("AGDevStdBridge")]
		public extern static void OnResultInNegative(System.IntPtr behaviorCheckListener);


	};
	public class NativeBehaviorCheckTrigger : BehaviorCheckTrigger {
		public System.IntPtr implPtr;
		public BridgeHelperFactoryBridge helperFacytory;
		public NativeBehaviorCheckTrigger(System.IntPtr _implPtr, BridgeHelperFactoryBridge _helperFacytory) {
			helperFacytory = _helperFacytory;
			implPtr = _implPtr;
		}
		void BehaviorCheckTrigger.BeginBehavior(BehaviorCheckListener behaviorListener) {
			BeginBehavior(implPtr, helperFacytory.NewBehaviorCheckListenerBridge(behaviorListener));
		}
		public void BeginBehavior(NativeBehaviorListener behaviorListener) {

		}
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr BeginBehavior(System.IntPtr factory, System.IntPtr bhvrChkListener);

	}
	public class BehaviorCheckListenerBridge {
		public System.IntPtr ImplPtr;
		BehaviorCheckListener listener;
		System.IntPtr helperFactory;
		SignalCallback onResultInPositiveCallback;
		public BehaviorCheckListenerBridge(System.IntPtr _helperFactory, BehaviorCheckListener _listener) {
			listener = _listener;
			helperFactory = _helperFactory;
			onResultInPositiveCallback = () => {
				listener.OnResultInPositive();
			};
			ImplPtr = NewBehaviorCheckListener(helperFactory, onResultInPositiveCallback);
		}
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewBehaviorCheckListener(
			System.IntPtr behaviorFactory,
			SignalCallback onResultInPositiveCallback
		);
	}
	public class BehaviorCheckTriggerBridge {
		public System.IntPtr ImplPtr;
		BehaviorCheckTrigger controller;
		System.IntPtr helperFactory;
		SinglePtrCallback beginBehaviorCheckCallback;
		SignalCallback stopBehaviorCallback;
		SignalCallback continueBehaviorCallback;
		public BehaviorCheckTriggerBridge(System.IntPtr _helperFactory, BehaviorCheckTrigger _controller) {
			controller = _controller;
			helperFactory = _helperFactory;
			beginBehaviorCheckCallback = (bhvrChkListener) => {
				controller.BeginBehavior(new NativeBehaviorCheckListener(bhvrChkListener));
			};
			ImplPtr = NewBehaviorCheckTrigger(helperFactory,
				beginBehaviorCheckCallback,
				stopBehaviorCallback = () => { },
				continueBehaviorCallback = () => { }
			);
		}
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewBehaviorCheckTrigger(
			System.IntPtr behaviorFactory,
			SinglePtrCallback beginBehaviorCallback,
			SignalCallback stopCallback,
			SignalCallback continueCallback
		);
	}
	#endregion
	#region interpreter
	public interface InterpreterPackage {
		ConfigurableFAnalyserBridge fAnalyser { get; }
		ConfigurableGAnalyserBridge gAnalyser { get; }
		ConfigurableBAnalyserBridge bAnalyser { get; }
		ERWordsInterpreter erWordsIptr { get; }
	}
	public class NativeConfigurableLProcessor : ConfigurableLProcessor {
		public ConfigurableFAnalyserBridge fAnalyser;
		public ConfigurableGAnalyserBridge gAnalyser;
		void ConfigurableLProcessor.AddDictoinary(byte[] dictoinary) {
			gAnalyser.ConfigureGrammar(dictoinary);
		}
		void SyntacticProcessor.PerformSyntacticProcess(byte[] behaviorExpression, Taker<GrammarBlock> listener) {
			try {
				System.IntPtr marshalArray = Marshal.AllocHGlobal(behaviorExpression.Length);
				Marshal.Copy(behaviorExpression, 0, marshalArray, behaviorExpression.Length);
				var gBlockPtr = PerformSyntacticProcess(fAnalyser.implPtr, gAnalyser.implPtr, marshalArray, behaviorExpression.Length);
				Marshal.FreeHGlobal(marshalArray);
				listener.Take(NativeGrammarBlockUtils.GBlockPtrToGBlock(gBlockPtr));
				listener.OnFinish();
			} catch (Exception e) {
				Debug.Log(e);
				listener.OnFinish();
			}
		}

		void ConfigurableLProcessor.SetFormat(byte[] formatConfig) {
			fAnalyser.ConfigureFormat(formatConfig);
		}
		[DllImport("AGDevStdBridge")]
		extern static System.IntPtr PerformSyntacticProcess(System.IntPtr fAnalyzer, System.IntPtr bAnalyzer, System.IntPtr erWords, int length);
	}
	public class StdInterpreterPackage : InterpreterPackage {
		ConfigurableFAnalyserBridge InterpreterPackage.fAnalyser { get { return _fAnalyser; } }
		ConfigurableGAnalyserBridge InterpreterPackage.gAnalyser { get { return _gAnalyser; } }
		ConfigurableBAnalyserBridge InterpreterPackage.bAnalyser { get { return _bAnalyser; } }
		ERWordsInterpreter InterpreterPackage.erWordsIptr { get { return _configurableIptr; } }
		ConfigurableFAnalyserBridge _fAnalyser;
		ConfigurableGAnalyserBridge _gAnalyser;
		ConfigurableBAnalyserBridge _bAnalyser;
		NativeConfigurableERWInterpreter _configurableIptr;
		public StdInterpreterPackage(ConfigurableEWRIptrFactory factory, ImmediateGiver<Behaver, GrammarBlock> behaverGiver) {
			_fAnalyser = factory.NewConfigurableFAnalyser();
			_gAnalyser = factory.NewConfigurableGAnalyser();
			_bAnalyser = factory.NewConfigurableBAnalyser(behaverGiver);
			_configurableIptr = factory.NewNativeConfigurableERWInterpreter();
			_configurableIptr.SetFAnalyser(_fAnalyser);
			_configurableIptr.SetGAnalyser(_gAnalyser);
			_configurableIptr.SetBAnalyser(_bAnalyser);
		}
	}
	public class NativeConfigurableERWInterpreter : ERWordsInterpreter {
		public BridgeHelperFactoryBridge helperFactory;
		public NativeConfigurableERWInterpreter(System.IntPtr _implPtr, BridgeHelperFactoryBridge _helperFactory) {
			helperFactory = _helperFactory;
			implPtr = _implPtr;
		}
		BehaviorTrigger ERWordsInterpreter.InterpretERWordsAsBehavior(byte[] configuration) {
			//CHECK: faster way?
			System.IntPtr marshalArray = Marshal.AllocHGlobal(configuration.Length);
			Marshal.Copy(configuration, 0, marshalArray, configuration.Length);
			System.IntPtr behaviorTriggerPtr = InterpretERWordsAsBehavior(implPtr, marshalArray, configuration.Length);
			Marshal.FreeHGlobal(marshalArray);
			if (behaviorTriggerPtr == System.IntPtr.Zero)
				return null;
			return new NativeBehaviorTrigger(behaviorTriggerPtr, helperFactory);
		}
		public void SetFAnalyser(ConfigurableFAnalyserBridge fAnalyser) {
			currentFAnlys = fAnalyser;
			SetFAnalyser(implPtr, currentFAnlys.implPtr);
		}
		public void SetGAnalyser(ConfigurableGAnalyserBridge gAnalyser) {
			currentGAnlys = gAnalyser;
			SetGAnalyser(implPtr, currentGAnlys.implPtr);
		}
		public void SetBAnalyser(ConfigurableBAnalyserBridge bAnalyser) {
			currentBAnlys = bAnalyser;
			SetBAnalyser(implPtr, currentBAnlys.implPtr);
		}
		public ConfigurableFAnalyserBridge currentFAnlys;
		public ConfigurableGAnalyserBridge currentGAnlys;
		public ConfigurableBAnalyserBridge currentBAnlys;
		readonly public System.IntPtr implPtr;
		[DllImport("AGDevStdBridge")]
		extern static System.IntPtr InterpretERWordsAsBehavior(System.IntPtr iptr, System.IntPtr erWords, int length);
		[DllImport("AGDevStdBridge")]
		extern static void SetFAnalyser(System.IntPtr iptr, System.IntPtr analyser);
		[DllImport("AGDevStdBridge")]
		extern static void SetGAnalyser(System.IntPtr iptr, System.IntPtr analyser);
		[DllImport("AGDevStdBridge")]
		extern static void SetBAnalyser(System.IntPtr iptr, System.IntPtr analyser);
	}
	public class ConfigurableBAnalyserBridge {
		public void AddNativeBehaver(System.IntPtr behaver) {
			AddBehaver(behaver, implPtr);
		}
		public bool AskForFloatAnswer(GrammarBlock question, AnswerListener<float> listener) {
			var lisBridge = new FloatAnswerListenerBridge(listener);
			floatAnswerListeners.Add(lisBridge);
			return AskForNumericAnswer(helperFactory.implPtr, implPtr, helperFactory.GBlockToGBlockPtr(question), lisBridge.callback);
		}
		public List<FloatAnswerListenerBridge> floatAnswerListeners = new List<FloatAnswerListenerBridge>();
		public BehaverBridge bridge;
		public BridgeHelperFactoryBridge helperFactory;
		public System.IntPtr implPtr;
		[DllImport("AGDevStdBridge")]
		private extern static void AddBehaver(System.IntPtr behaver, System.IntPtr bAnlys);
		[DllImport("AGDevStdBridge")]
		private extern static bool AskForNumericAnswer(System.IntPtr helperFactory, System.IntPtr bAnlys, System.IntPtr grammarBlock, FloatCallback floatCallback);
	}
	public class ConfigurableEWRIptrFactory {
		public BridgeHelperFactoryBridge helperFactory;
		public List<BehaverFactoryBridge> factory = new List<BehaverFactoryBridge>();
		public ImmediateMultiGetter<System.IntPtr> nativeBehaverGetter;
		public System.IntPtr implPtr;
		public ConfigurableEWRIptrFactory(BridgeHelperFactoryBridge _helperFactory) {
			helperFactory = _helperFactory;
			implPtr = NewConfigurableTEIptrFactory();
		}
		~ConfigurableEWRIptrFactory() {
			DeleteConfigurableTEIptrFactory(implPtr);
		}
		public NativeConfigurableERWInterpreter NewNativeConfigurableERWInterpreter() {
			return new NativeConfigurableERWInterpreter(NewConfigurableTEInterpreter(implPtr), helperFactory);
		}
		public ConfigurableFAnalyserBridge NewConfigurableFAnalyser() {
			return new ConfigurableFAnalyserBridge(NewConfigurableFAnalyser(implPtr));
		}
		public ConfigurableGAnalyserBridge NewConfigurableGAnalyser() {
			return new ConfigurableGAnalyserBridge(NewConfigurableGAnalyser(implPtr));
		}
		public ConfigurableBAnalyserBridge NewConfigurableBAnalyser(ImmediateGiver<Behaver, GrammarBlock> behaverFactory) {
			var factoryBridge = new BehaverFactoryBridge(behaverFactory, helperFactory);
			factory.Add(factoryBridge);
			var anlys = new ConfigurableBAnalyserBridge { helperFactory = helperFactory, implPtr = NewConfigurableBAnalyser(implPtr, factoryBridge.nativeFactoryPtr) };
			if (nativeBehaverGetter != null) {
				nativeBehaverGetter.GetElement(new NativeBehaverGetter { bAnlys = anlys });
			}
			return anlys;
		}
		public class NativeBehaverGetter : Taker<System.IntPtr> {
			public ConfigurableBAnalyserBridge bAnlys;
			void Taker<System.IntPtr>.Take(System.IntPtr newElement) {
				bAnlys.AddNativeBehaver(newElement);
			}
		}
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewConfigurableTEIptrFactory();
		[DllImport("AGDevStdBridge")]
		private extern static void DeleteConfigurableTEIptrFactory(System.IntPtr factory);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewConfigurableTEInterpreter(System.IntPtr factory);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewConfigurableFAnalyser(System.IntPtr factory);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewConfigurableGAnalyser(System.IntPtr factory);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewConfigurableBAnalyser(System.IntPtr iptrFactory, System.IntPtr behaverFactory);
	}
	public class ConfigurableFAnalyserBridge {
		public ConfigurableFAnalyserBridge(System.IntPtr _implPtr) {
			implPtr = _implPtr;
		}
		public void ConfigureFormat(byte[] configuration) {
			//CHECK: faster way?
			System.IntPtr marshalArray = Marshal.AllocHGlobal(configuration.Length);
			Marshal.Copy(configuration, 0, marshalArray, configuration.Length);
			ConfigureFormat(implPtr, marshalArray, configuration.Length);
			Marshal.FreeHGlobal(marshalArray);
		}
		readonly public System.IntPtr implPtr;
		[DllImport("AGDevStdBridge")]
		extern static void ConfigureFormat(System.IntPtr analyser, System.IntPtr erWords, int length);
	}
	public class ConfigurableGAnalyserBridge {
		public ConfigurableGAnalyserBridge(System.IntPtr _implPtr) {
			implPtr = _implPtr;
		}
		public void ConfigureGrammar(byte[] configuration) {
			//CHECK: faster way?
			System.IntPtr marshalArray = Marshal.AllocHGlobal(configuration.Length);
			Marshal.Copy(configuration, 0, marshalArray, configuration.Length);
			ConfigureGrammar(implPtr, marshalArray, configuration.Length);
			Marshal.FreeHGlobal(marshalArray);
		}
		readonly public System.IntPtr implPtr;
		[DllImport("AGDevStdBridge")]
		extern static void ConfigureGrammar(System.IntPtr analyser, System.IntPtr erWords, int length);
	}
	public class BehaverFactoryBridge {
		public TwoPtrCallback behaverFrontFactoryCallback;
		public ImmediateGiver<Behaver, GrammarBlock> behaverGiver;
		public List<BehaverBridge> behavers = new List<BehaverBridge>();
		public BehaverFactoryBridge(ImmediateGiver<Behaver, GrammarBlock> _behaverGiver, BridgeHelperFactoryBridge _helperFactory) {
			behaverGiver = _behaverGiver;
			behaverFrontFactoryCallback = (factoryAgent, baseAttributeGBlock) => {
				try {
					var picked = behaverGiver.PickBestElement(NativeGrammarBlockUtils.GBlockPtrToGBlock(baseAttributeGBlock));
					if (picked != null) {
						behavers.Add(new BehaverBridge(picked, _helperFactory));
						ReadyBehaver(factoryAgent, behavers.Last().matchAttributeCallback, behavers.Last().setBehaviorCallback, behavers.Last().checkBehaviorCallback);
					}
				} catch (System.Exception e) {
					Debug.LogError(e);
				}
			};
			nativeFactoryPtr = _helperFactory.NewBehaverFactory(behaverFrontFactoryCallback);
		}
		public System.IntPtr nativeFactoryPtr;
		[DllImport("AGDevStdBridge")]
		private extern static void ReadyBehaver(
			System.IntPtr agent, SinglePtrReturnIntCallback onAddAttributeCallback,
			TwoPtrCallback setBehaviorCallback, TwoPtrCallback checkBehaviorCallback
		);
	}
	public class FloatAnswerListenerBridge {
		public AnswerListener<float> listener;
		public FloatCallback callback;
		public FloatAnswerListenerBridge(AnswerListener<float> _listener) {
			listener = _listener;
			callback = (float answer) => {
				_listener.OnAnswerUpdate(answer);
			};
		}
	}
	#endregion
	#region utilities
	public class BridgeHelperFactoryBridge {
		public readonly System.IntPtr implPtr;
		public BridgeHelperFactoryBridge() {
			implPtr = NewBridgeHelperFactory();
		}
		~BridgeHelperFactoryBridge() {
			DeleteBridgeHelperFactory(implPtr);
		}
		public System.IntPtr NewBehaverFactory(TwoPtrCallback behaverFactoryCallback) {
			return NewBehaverFactory(implPtr, behaverFactoryCallback);
		}
		public System.IntPtr NewBehaviorTriggerBridge(BehaviorTrigger controller) {
			var bridge = new BehaviorTriggerBridge(implPtr, controller);
			bhvrCtrlBridges.Add(bridge);
			return bridge.ImplPtr;
		}
		public System.IntPtr NewBehaviorCheckTriggerBridge(BehaviorCheckTrigger controller) {
			var bridge = new BehaviorCheckTriggerBridge(implPtr, controller);
			bhvrChkCtrlBridges.Add(bridge);
			return bridge.ImplPtr;
		}
		public System.IntPtr NewBehaviorListenerBridge(BehaviorListener listener) {
			var bridge = new BehaviorListenerBridge(implPtr, listener);
			bhvrCtrlListeners.Add(bridge);
			return bridge.ImplPtr;
		}
		public System.IntPtr NewBehaviorCheckListenerBridge(BehaviorCheckListener listener) {
			var bridge = new BehaviorCheckListenerBridge(implPtr, listener);
			bhvrChkListeners.Add(bridge);
			return bridge.ImplPtr;
		}
		public System.IntPtr GetMGUnitPtr(string words) {
			System.IntPtr val;
			if (minimumGBLockDict.TryGetValue(words, out val))
				return val;
			var byteArray = System.Text.Encoding.UTF8.GetBytes(words);
			System.IntPtr marshalArray = Marshal.AllocHGlobal(byteArray.Length);
			Marshal.Copy(byteArray, 0, marshalArray, byteArray.Length);
			var newGBLock = NewMutableGUnit(implPtr, marshalArray, byteArray.Length);
			minimumGBLockDict.Add(words, newGBLock);
			Marshal.FreeHGlobal(marshalArray);
			return newGBLock;
		}
		public System.IntPtr ClusterGBlockToMGBlockPtr(ClusterGrammarBlock gCluster) {
			var mgClusterPtr = NewMutableClusterGBlock(implPtr);
			foreach (var gBlock in gCluster.blocks) {
				var subMGBlockPtr = GBlockToMGBlockPtr(gBlock);
				var subGBlockPtr = ReferMGBlockAsGBlock(subMGBlockPtr);
				AddGBlockToCluster(mgClusterPtr, subGBlockPtr);
			}
			return ReferMGClusterAsMutableGBlock(mgClusterPtr);
		}
		public System.IntPtr GBlockToMGBlockPtr(GrammarBlock gBlock) {
			var mgBlockPtr = System.IntPtr.Zero;
			if (gBlock.unit != null) {
				mgBlockPtr = GetMGUnitPtr(gBlock.unit.word);
			} else if (gBlock.cluster != null) {
				mgBlockPtr = ClusterGBlockToMGBlockPtr(gBlock.cluster);
			}
			if (gBlock.metaInfo != null) {
				var metaBlockPtr = GBlockToMGBlockPtr(gBlock.metaInfo);
				AddMetaInfo(mgBlockPtr, metaBlockPtr);
			}
			if (gBlock.modifier != null) {
				var modifierPtr = GBlockToMGBlockPtr(gBlock.modifier);
				AddModifier(mgBlockPtr, modifierPtr);
			}
			return mgBlockPtr;
		}
		public System.IntPtr GBlockToGBlockPtr(GrammarBlock gBlock) {
			return ReferMGBlockAsGBlock(GBlockToMGBlockPtr(gBlock));
		}
		List<BehaviorListenerBridge> bhvrCtrlListeners = new List<BehaviorListenerBridge>();
		List<BehaviorCheckListenerBridge> bhvrChkListeners = new List<BehaviorCheckListenerBridge>();
		List<BehaviorTriggerBridge> bhvrCtrlBridges = new List<BehaviorTriggerBridge>();
		List<BehaviorCheckTriggerBridge> bhvrChkCtrlBridges = new List<BehaviorCheckTriggerBridge>();
		Dictionary<string, System.IntPtr> minimumGBLockDict = new Dictionary<string, System.IntPtr>();
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewBridgeHelperFactory();
		[DllImport("AGDevStdBridge")]
		private extern static void DeleteBridgeHelperFactory(System.IntPtr factory);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewBehaverFactory(System.IntPtr BridgeHelperFactoryBridgePtr, TwoPtrCallback behaverFrontFactoryCallback);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewMinimumGBlock(System.IntPtr factory, System.IntPtr formatConfiguration, int length);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewMutableGUnit(System.IntPtr factory, System.IntPtr formatConfiguration, int length);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr NewMutableClusterGBlock(System.IntPtr factory);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr ReferMGClusterAsMutableGBlock(System.IntPtr mgCluster);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr ReferMGBlockAsGBlock(System.IntPtr mgBlock);
		[DllImport("AGDevStdBridge")]
		private extern static void AddMetaInfo(System.IntPtr mgBlock, System.IntPtr gBlockToAdd);
		[DllImport("AGDevStdBridge")]
		private extern static void AddModifier(System.IntPtr mgBlock, System.IntPtr gBlockToAdd);
		[DllImport("AGDevStdBridge")]
		private extern static void AddGBlockToCluster(System.IntPtr mgCluster, System.IntPtr gBlockToAdd);
	}
	public class PtrFetcher {
		public System.IntPtr fetchedPtr;
		public SinglePtrCallback ptrFetchCallback;
		public PtrFetcher() {
			ptrFetchCallback = (ptr) => {
				fetchedPtr = ptr;
			};
		}
	}
	public class StrPtrFetcher {
		public string fetchedStr;
		public SingleWordCallback strFetchCallback;
		public StrPtrFetcher() {
			strFetchCallback = (str, length) => {
				fetchedStr = Marshal.PtrToStringAnsi(str, length);
			};
		}
	}
	public class PtrClusterFetcher {
		public List<System.IntPtr> fetchedPtrs = new List<System.IntPtr>();
		public SinglePtrCallback ptrFetchCallback;
		public PtrClusterFetcher() {
			ptrFetchCallback = (ptr) => {
				fetchedPtrs.Add(ptr);
			};
		}
	}
	#endregion
	public class NativeGrammarBlockUtils {
		[DllImport("AGDevStdBridge")]
		private extern static void ExtractGUnit(
			System.IntPtr gUnit,
			SingleWordCallback getWords,
			SinglePtrCallback ifHasModifier,
			SinglePtrCallback ifMetaInfo
		);
		[DllImport("AGDevStdBridge")]
		private extern static void ExtractClusterGBlock(
			System.IntPtr gCluster,
			SinglePtrCallback forEachElement,
			SinglePtrCallback ifHasModifier,
			SinglePtrCallback ifMetaInfo
		);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr IfGrammarUnit(System.IntPtr gBlockPtr);
		[DllImport("AGDevStdBridge")]
		private extern static System.IntPtr IfClusterGrammarBlock(System.IntPtr gBlockPtr);
		public static GrammarBlock GBlockPtrToGBlock(System.IntPtr gBlockPtr) {
			var gUnitPtr = IfGrammarUnit(gBlockPtr);
			if (gUnitPtr != System.IntPtr.Zero) {
				var wordFetcher = new StrPtrFetcher();
				var modFetcher = new PtrFetcher();
				var metaFetcher = new PtrFetcher();
				ExtractGUnit(gUnitPtr, wordFetcher.strFetchCallback, modFetcher.ptrFetchCallback, metaFetcher.ptrFetchCallback);
				var newGUnit = new StdGrammarUnit(wordFetcher.fetchedStr);
				if (modFetcher.fetchedPtr != System.IntPtr.Zero) {
					newGUnit.mod = GBlockPtrToGBlock(modFetcher.fetchedPtr);
				}
				if (metaFetcher.fetchedPtr != System.IntPtr.Zero) {
					newGUnit.meta = GBlockPtrToGBlock(metaFetcher.fetchedPtr);
				}
				return newGUnit;
			} else {
				var gClusterPtr = IfClusterGrammarBlock(gBlockPtr);
				if (gClusterPtr != System.IntPtr.Zero) {
					var elementsFetcher = new PtrClusterFetcher();
					var modFetcher = new PtrFetcher();
					var metaFetcher = new PtrFetcher();
					ExtractClusterGBlock(gClusterPtr, elementsFetcher.ptrFetchCallback, modFetcher.ptrFetchCallback, metaFetcher.ptrFetchCallback);
					var newGCluster = new StdClusterGrammarBlock();
					foreach (var element in elementsFetcher.fetchedPtrs) {
						newGCluster.blocks.Add(GBlockPtrToGBlock(element));
					}
					if (modFetcher.fetchedPtr != System.IntPtr.Zero) {
						newGCluster.mod = GBlockPtrToGBlock(modFetcher.fetchedPtr);
					}
					if (metaFetcher.fetchedPtr != System.IntPtr.Zero) {
						newGCluster.meta = GBlockPtrToGBlock(metaFetcher.fetchedPtr);
					}
					return newGCluster;
				}
			}
			return null;
		}
	}
#endif
}