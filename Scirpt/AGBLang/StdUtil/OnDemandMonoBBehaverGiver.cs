using AGBLang;
using AGBLang.StdUtil;
using AGDev;
using System.Collections.Generic;

namespace AGDevUnity.StdUtil {
	public class OnDemandMonoBBehaverGiver :
		MonoBBahaverGiver,
		ImmediateGiver<Behaver, GrammarBlock>,
		ImmediateGiver<UnityBehaver, GrammarBlock>
	{
		public List<BridgeBehaver> createdBehavers;
		public MonoBSpaceDistributer defaultSpace;
		public MonoBCommonSenseGiver csGiver;
		public SeekingBehaverAgent generalAgentPrefab;
		public List<BridgeBehaver> behavers = new List<BridgeBehaver>();
		public MonoBAssetMediator assetMed;
		public CustomizableUnityBehaver bRootPrefab;

		public override ImmediateGiver<Behaver, GrammarBlock> behaverGiver => this;

		Behaver ImmediateGiver<Behaver, GrammarBlock>.PickBestElement(GrammarBlock param) {
			return EnsureUnityBehaver(param);
		}

		UnityBehaver ImmediateGiver<UnityBehaver, GrammarBlock>.PickBestElement(GrammarBlock param) {
			return EnsureUnityBehaver(param).unityBehaver;
		}
		public BridgeBehaver EnsureUnityBehaver(GrammarBlock param) {
			var behaver = behavers.Find((elem) => (elem as AttributeMatcher).MatchAttribue(param) == AttributeMatchResult.POSITIVE);
			if (behaver != null) {
				return behaver;
			} else if (param.unit != null) {
				var unityBehaverRoot = new BridgeBehaver { parent = this };
				var saved = assetMed.assetMed.GetImplementedAsset<CustomizableUnityBehaver>(param);
				//already customized
				if (saved != null) {
					unityBehaverRoot.myCustomizable = Utilities.ConsistentInstantiate(saved).GetComponent<CustomizableUnityBehaver>();
				}
				//customize on demand
				if (unityBehaverRoot.myCustomizable == null) {
					unityBehaverRoot.myCustomizable = Utilities.ConsistentInstantiate(bRootPrefab);
					unityBehaverRoot.myCustomizable.name = param.unit.word;
					unityBehaverRoot.myCustomizable.nameOnCreate = param.unit.word;
					List<BehaverEquipper> equippers = new List<BehaverEquipper>();
					var implementedEquippers = assetMed.assetMed.GetImplementedAssets<MonoBBehaverEquipper>();
					foreach (var equipper in implementedEquippers) {
						equippers.Add(equipper.behaverEquipper);
					}
					var clEquipper = new ClusterBehaverEquipper { equippers = equippers };
					(clEquipper as BehaverEquipper).EquipBehaverByAttribute(param, unityBehaverRoot);
					unityBehaverRoot.setupParam = param;
					if (unityBehaverRoot.myCustomizable.baseBehaver == null) {
						//check common sense
						csGiver.commonSenseGiver.nounCSGiver.Give(
							param.unit.word,
							unityBehaverRoot
						);
					}
				}
				behavers.Add(unityBehaverRoot);
				return unityBehaverRoot;
			}
			return null;
		}
	}
	public class DependentUnityBehaver : UnityBehaver, BehaverAvatar {
		ItemType UnityBehaver.GetModule<ItemType>() {
			return customBehaverInstance.behaver.GetModule<ItemType>();
		}
		BehaverAvatar UnityBehaver.avatar => this;
		public CustomizableUnityBehaver customBehaverInstance;
		public BridgeBehaver hostBehaver;
		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
			return customBehaverInstance.behaver.MatchAttribue(attribute);
		}
		UnityBehaviorTrigger UnityBehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport listener) {
			return customBehaverInstance.behaver.ReadyBehavior(bExpr, listener);
		}
		UnityBehaviorCheckTrigger UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport listener) {
			return customBehaverInstance.behaver.ReadyCheckBehavior(bExpr, listener);
		}
		AvatarPhysicalizeInterface BehaverAvatar.Physicalize(BAgentSpace spaceInfo) {
			var baseItfc = (hostBehaver as UnityBehaver).avatar.Physicalize(spaceInfo);
			var obtained = apInterfaces.Find((item) => item.baseItfc == baseItfc);
			if (obtained != null)
				return obtained;
			obtained = new PrvtAPInterface { baseItfc = baseItfc, parent = this };
			apInterfaces.Add(obtained);
			return obtained;
		}
		public List<PrvtAPInterface> apInterfaces = new List<PrvtAPInterface>();
		public class PrvtAPInterface : AvatarPhysicalizeInterface {
			public DependentUnityBehaver parent;
			public AvatarPhysicalizeInterface baseItfc;
			void AvatarPhysicalizeInterface.Add(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> listener) {
				baseItfc.Add(parent.hostBehaver.setupParam, listener);
			}
			void AvatarPhysicalizeInterface.Clear() {
				baseItfc.Clear();
			}
			void AvatarPhysicalizeInterface.Ensure(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> listener) {
				baseItfc.Ensure(parent.hostBehaver.setupParam, listener);
			}
			void AvatarPhysicalizeInterface.Search(GrammarBlock attribute, Taker<IEnumerable<BehaverAgent>> listener) {
				baseItfc.Search(parent.hostBehaver.setupParam, listener);
			}
		}
	}
	public class BridgeBehaver : Behaver, BehaverEquipListener, Taker<NounCommonSenseUnit>, UnityBehaver {
		public GrammarBlock setupParam;
		public OnDemandMonoBBehaverGiver parent;
		public CustomizableUnityBehaver myCustomizable;
		public UnityBehaver unityBehaver => this;
		UnityBehaver unityBehaverInternal { get { return _unityBehaver != null ? _unityBehaver : myCustomizable.behaver; } set { _unityBehaver = value; } }

		BehaverAvatar UnityBehaver.avatar => unityBehaverInternal.avatar;

		UnityBehaver _unityBehaver;
		#region behaver
		BehaviorTrigger BehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, BehaviorReadySupport support) {
			var unityBRSupport = new BridgeUnityBReadySupport { parent = this, clientListener = support, bExpr = bExpr };
			var uniTrigger = unityBehaverInternal.ReadyBehavior(bExpr, unityBRSupport);
			
			if (uniTrigger != null) {
				var bridgeTrigger = new BridgeBTriggerFromUnity {
					parent = this,
					bExpr = bExpr,
					spaceDistributer = this.parent.defaultSpace.spaceDistributer,
					unityBTrigger = uniTrigger
				};
				return bridgeTrigger;
			}
			var eqLis = new UseOnEquip_EquipListener { canEquip = true, customBehaverInstance = myCustomizable, bExpr = bExpr, unityBRSupport = unityBRSupport };
			foreach (var equipper in parent.assetMed.assetMed.GetImplementedAssets<MonoBBehaverEquipper>()) {
				equipper.behaverEquipper.EquipBehaverByBehavior(bExpr, eqLis);
			}
			if (!eqLis.canEquip) {
				var seekEqLis = new SeekEQLis { parent = this, bExpr = bExpr };
				parent.assetMed.assetMed.SeekAsset(bExpr.asGBlock, seekEqLis);
			}
			return null;
		}
		BehaviorCheckTrigger BehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, BehaviorReadySupport support) {
			var unityBRSupport = new BridgeUnityBReadySupport { parent = this, clientListener = support, bExpr = bExpr };
			var unityTrigger = unityBehaverInternal.ReadyCheckBehavior(bExpr, unityBRSupport);
			var bridgeTrigger = new BridgeBTriggerFromUnity {
				parent = this,
				bExpr = bExpr,
				spaceDistributer = this.parent.defaultSpace.spaceDistributer,
				unityBCheckTrigger = unityTrigger
			};
			if (unityTrigger != null) {
				return bridgeTrigger;
			}
			var eqLis = new UseOnEquip_EquipListener { canEquip = true, customBehaverInstance = myCustomizable, bExpr = bExpr, unityBRSupport = unityBRSupport };
			foreach (var equipper in parent.assetMed.assetMed.GetImplementedAssets<MonoBBehaverEquipper>()) {
				equipper.behaverEquipper.EquipBehaverByBehavior(bExpr, eqLis);
			}
			if (!eqLis.canEquip) {
				var seekEqLis = new SeekEQLis { parent = this, bExpr = bExpr };
				parent.assetMed.assetMed.SeekAsset(bExpr.asGBlock, seekEqLis);
			}
			return null;
		}
		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
			return unityBehaverInternal.MatchAttribue(attribute);
		}
		#endregion
		#region equip self
		void BehaverEquipListener.OnEquipBehaviorChecker(MonoBUnityBehaviorChecker checker) {
			myCustomizable.bCheckers.Add(Utilities.ConsistentInstantiate(checker, myCustomizable.transform));
		}
		void BehaverEquipListener.OnEquipBehaviorSetter(MonoBUnityBehaviorSetter setter) {
			myCustomizable.bSetters.Add(Utilities.ConsistentInstantiate(setter, myCustomizable.transform));
		}
		void BehaverEquipListener.OnEquipSubBehaver(MonoBUnityBehaver behaver) {
			myCustomizable.baseBehaver = Utilities.ConsistentInstantiate(behaver, myCustomizable.transform);
			myCustomizable.name = myCustomizable.baseBehaver.name;
		}
		void BehaverEquipListener.OnEquipAvatar(MonoBBehaverAvatar avatar) {
			//stub
		}
		//only equip
		class SeekEQLis : Taker<MonoBBehaverEquipper> {
			public BridgeBehaver parent;
			public BehaviorExpression bExpr;
			void Taker<MonoBBehaverEquipper>.Take(MonoBBehaverEquipper newElement) {
				newElement.behaverEquipper.EquipBehaverByBehavior(bExpr, parent);
			}
			void Taker<MonoBBehaverEquipper>.None() { }
		}
		//equip and ready behavior
		public class UseOnEquip_EquipListener : BehaverEquipListener {
			public BehaviorExpression bExpr;
			public UnityBehaviorReadySupport unityBRSupport;
			public CustomizableUnityBehaver customBehaverInstance;
			public UnityBehaviorTrigger trigger = null;
			public UnityBehaviorCheckTrigger cTrigger = null;
			public bool canEquip = false;
			void BehaverEquipListener.OnEquipBehaviorSetter(MonoBUnityBehaviorSetter setter) {
				canEquip = true;
				var instance = Utilities.ConsistentInstantiate(setter, customBehaverInstance.transform);
				customBehaverInstance.bSetters.Add(instance);
				trigger = instance.behaviorSetter.ReadyBehavior(bExpr, unityBRSupport);
			}
			void BehaverEquipListener.OnEquipBehaviorChecker(MonoBUnityBehaviorChecker checker) {
				canEquip = true;
				var instance = Utilities.ConsistentInstantiate(checker, customBehaverInstance.transform);
				customBehaverInstance.bCheckers.Add(instance);
				cTrigger = instance.behaviorChecker.ReadyCheckBehavior(bExpr, unityBRSupport);
			}
			void BehaverEquipListener.OnEquipSubBehaver(MonoBUnityBehaver baseBehaver) {
				canEquip = true;
				var instance = Utilities.ConsistentInstantiate(baseBehaver, customBehaverInstance.transform);
				customBehaverInstance.baseBehaver = instance;
				if(! GrammarBlockUtils.HasMetaInfo( bExpr.asGBlock, StdMetaInfos.conditionSV.word ))
					instance.behaver.ReadyBehavior(bExpr, unityBRSupport);
				else
					instance.behaver.ReadyCheckBehavior(bExpr, unityBRSupport);
			}
			void BehaverEquipListener.OnEquipAvatar(MonoBBehaverAvatar avatar) {
				canEquip = true;
				//customBehaverInstance.independentAvatar = avatar;
			}
		}
		#endregion
		#region extend based on common sense

		void Taker<NounCommonSenseUnit>.Take(NounCommonSenseUnit item) {
			if (unityBehaverInternal.avatar == null) {
				if (item.isPhysical) {
					myCustomizable.seekingAgentPrefab = Utilities.ConsistentInstantiate(parent.generalAgentPrefab, myCustomizable.transform);
					myCustomizable.seekingAgentPrefab.SetAttribute(setupParam.unit);
					myCustomizable.seekingAgentPrefab.gameObject.SetActive(false);
					unityBehaverInternal = myCustomizable;
				}
				else if (item.givenBy != null) {
					//non physical, still given by something else
					foreach (var givenBy in item.givenBy) {
						var hostBehaver = parent.EnsureUnityBehaver(new StdGrammarUnit(givenBy));
						unityBehaverInternal = new DependentUnityBehaver { hostBehaver = hostBehaver };
						break;
					}
				}
			}
		}
		void Taker<NounCommonSenseUnit>.None() { }
		void SetGeneralAvatar() {
			/*var generalAvatar = Utilities.ConsistentInstantiate(parent.generalAvatarPrefab, customBehaverInstance.transform);
			generalAvatar.SetAttribute(setupParam.unit.word);
			customBehaverInstance.independentAvatar = generalAvatar;*/

		}
		#endregion
		public class UnityStubTrigger : UnityBehaviorTrigger {
			void UnityBehaviorTrigger.BeginBehavior(UnityBehaviorSupportListener behaviorListener) {
				behaviorListener.OnFinish();
			}
		}
		public class BridgeBTriggerFromUnity : BehaviorTrigger, BehaviorCheckTrigger {
			public BridgeBehaver parent;
			public BehaviorExpression bExpr;
			public UnityBehaviorTrigger unityBTrigger;
			public UnityBehaviorCheckTrigger unityBCheckTrigger;
			public SpaceDistributer spaceDistributer;
			void BehaviorTrigger.BeginBehavior(BehaviorListener behaviorListener) {
				var supLis = new PrvtBSupLis { parent = this, clientListener = behaviorListener };
				//seek space on behave
				spaceDistributer.DistributeSpaceToBehave(parent.myCustomizable.gameObject, bExpr, supLis);
				unityBTrigger.BeginBehavior(supLis);
			}
			public class PrvtBSupLis : UnityBehaviorSupportListener, SpaceInfoListener {
				public BridgeBTriggerFromUnity parent;
				public BehaviorListener clientListener;
				public BAgentSpace givenSpace;
				//ImmediateGiver<MonoBUnityBehaver, GrammarBlock> BehaviorSupport.behaverGiver => parent.parent.parent;
				BAgentSpace UnityBehaviorSupport.givenSpaceToBehave => givenSpace;
				void BehaviorListener.OnFinish() {
					clientListener.OnFinish();
				}

				void SpaceInfoListener.OnSpaceUpdated(BAgentSpace spaceInfo) {
					givenSpace = spaceInfo;
					if (parent.parent.unityBehaverInternal.avatar != null) {
						var apInterface = parent.parent.unityBehaverInternal.avatar.Physicalize(spaceInfo);
						apInterface.Ensure(parent.bExpr.subject, new PrvtAgentTaker { parent = this });
					}
				}
				public class PrvtAgentTaker : Taker<IEnumerable<BehaverAgent>> {
					public PrvtBSupLis parent;
					void Taker<IEnumerable<BehaverAgent>>.Take(IEnumerable<BehaverAgent> item) {
						foreach (var element in item) {
							element.rootBObj.transform.localPosition = BehaviorUtilities.LocalPosition(parent.givenSpace, parent.parent.bExpr);
						}
					}
					void Taker<IEnumerable<BehaverAgent>>.None() { }
				}
				/*class ShowOnUpdate : SpaceInfoListener {
					public MonoBUnityBehaver behaver;
					public PrvtBSupLis parent;
					void SpaceInfoListener.OnSpaceUpdated(BAgentSpace spaceInfo) {
						if(behaver.behaver.avatar != null)
							behaver.behaver.avatar.Physicalize(spaceInfo);
					}
				}*/
				/*MonoBUnityBehaver ImmediateGiver<MonoBUnityBehaver, GrammarBlock>.PickBestElement(GrammarBlock key) {
					var pick = (parent.parent.parent as ImmediateGiver<MonoBUnityBehaver, GrammarBlock>).PickBestElement(key);
					//expretimental: show behaver on refered
					if (pick != null) {
						if(pick.behaver.avatar != null) {
							pick.behaver.avatar.Physicalize();
							var spaceLis = new ShowOnUpdate { behaver = pick, parent = this };
							parent.spaceDistributer.DistributeSpaceToBehave(pick.gameObject, parent.bExpr, spaceLis);
						}
					}
					return pick;
				}*/
			}
			public class PrvtBCheckSupLis : UnityBehaviorCheckSupportListener, SpaceInfoListener {
				public BridgeBTriggerFromUnity parent;
				public BehaviorCheckListener clientListener;
				public BAgentSpace givenSpace;
				//ImmediateGiver<MonoBUnityBehaver, GrammarBlock> BehaviorSupport.behaverGiver => parent.parent.parent;
				BAgentSpace UnityBehaviorSupport.givenSpaceToBehave => throw new System.NotImplementedException();
				void BehaviorCheckListener.OnResultInNegative() {
					clientListener.OnResultInNegative();
				}
				void BehaviorCheckListener.OnResultInPositive() {
					clientListener.OnResultInPositive();
				}
				void SpaceInfoListener.OnSpaceUpdated(BAgentSpace spaceInfo) {
					givenSpace = spaceInfo;
				}
			}
			void BehaviorCheckTrigger.BeginBehavior(BehaviorCheckListener behaviorListener) {
				var supLis = new PrvtBCheckSupLis { parent = this, clientListener = behaviorListener };
				unityBCheckTrigger.BeginBehavior(supLis);
			}
		}

		#region unity behaver
		ItemType UnityBehaver.GetModule<ItemType>() {
			return unityBehaverInternal.GetModule<ItemType>();
		}
		UnityBehaviorTrigger UnityBehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport support) {
			return unityBehaverInternal.ReadyBehavior(bExpr, support);
		}
		UnityBehaviorCheckTrigger UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport support) {
			return unityBehaverInternal.ReadyCheckBehavior(bExpr, support);
		}
		#endregion

	}
	public class BridgeUnityBReadySupport : UnityBehaviorReadySupport {
		public BridgeBehaver parent;
		public BehaviorExpression bExpr;
		public BehaviorReadySupport clientListener;
		ImmediateGiver<UnityBehaver, GrammarBlock> UnityBehaviorReadySupport.behaverGiver => parent.parent;

		BehaviorReadySupport UnityBehaviorReadySupport.basicSupport => clientListener;
		//var bridgeTrigger = new BridgeUnityBehaviorTrigger { parent = parent, bExpr = bExpr, spaceDistributer = parent.parent.defaultSpace.spaceDistributer, unityBTrigger = trigger };
		//clientListener.OnSucceed(bridgeTrigger);

		//didCollect = true;
		//clientBCListener.OnSucceed(new BridgeUnityBehaviorTrigger { parent = parent, bExpr = bExpr, spaceDistributer = parent.parent.defaultSpace.spaceDistributer, unityBCheckTrigger = trigger });
	}
}
