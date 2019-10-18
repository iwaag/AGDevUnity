using AGAsset;
using AGBLang;
using AGDev.StdUtil;
using System.Collections.Generic;
using UnityEngine;
using AGAsset.StdUtil;
using AGBLang.StdUtil;

namespace AGDevUnity.StdUtil {
	public class BasicAssetImplementer : MonoBAssetImplementer, AssetImplementerGetter {
		public override AssetImplementerGetter assetImplGetter => this;
		public BAgentSimpleAssetImplementCustomizer behaverAgentAMed;
		public CustomizableUnityBehaverImplementCustomizer customizedBehaverAMed;
		public BehaverEquipperImplementCustomizer behaverEquipperAMed;
		public AudioClipImplCustomizer audioAMed;
		public BasicPrefabImplCustomizer basicPrefabAMed;
		AssetImplementer<AssetType> AssetImplementerGetter.GetAssetImplementer<AssetType>(GrammarBlock gBlock) {
			if (typeof(AssetType) == typeof(MonoBBehaverAgent)) {
				return new StdAssetImplementer<AssetType, MonoBBehaverAgent> { customizer = behaverAgentAMed };
			} else if (typeof(AssetType) == typeof(CustomizableUnityBehaver)) {
				return new StdAssetImplementer<AssetType, CustomizableUnityBehaver> { customizer = customizedBehaverAMed };
			} else if (typeof(AssetType) == typeof(MonoBBehaverEquipper)) {
				return new StdAssetImplementer<AssetType, MonoBBehaverEquipper> { customizer = behaverEquipperAMed };
			} else if (typeof(AssetType) == typeof(AudioClip)) {
				return new StdAssetImplementer<AssetType, NamedAudioClip> { customizer = audioAMed };
			} else if (typeof(AssetType) == typeof(GameObject)) {
				return new StdAssetImplementer<AssetType, NamedGObj> { customizer = basicPrefabAMed };
			}
			return null;
		}
		[System.Serializable]
		public class NamedAudioClip : NamedAsset<AudioClip> { }
		[System.Serializable]
		public class AudioClipImplCustomizer : SimpleAssetImplementCustomizer<AudioClip, NamedAudioClip> {
			public override NamedAudioClip IntegrateImplementType<ParamAssetType>(ParamAssetType asset, AssetUnitInfo auInfo, AssetRequestUnit assetReqUnit) {
				if (typeof(ParamAssetType) == typeof(Texture2D)) {
					return new NamedAudioClip { asset = (AudioClip)(object)asset, name = AssetUtils.ExtractPackedAssetAttribute(auInfo.attributes)[0] };
				}
				return null;
			}
		};
		[System.Serializable]
		public class NamedGObj : NamedAsset<GameObject> { }
		[System.Serializable]
		public class BasicPrefabImplCustomizer : SimpleAssetImplementCustomizer<GameObject, NamedGObj> {
			public override NamedGObj IntegrateImplementType<ParamAssetType>(ParamAssetType asset, AssetUnitInfo auInfo, AssetRequestUnit assetReqUnit) {
				if (typeof(ParamAssetType) == typeof(GameObject)) {
					return new NamedGObj { asset = (GameObject)(object)asset, name = AssetUtils.ExtractPackedAssetAttribute(auInfo.attributes)[0] };
				}
				return null;
			}
		};
		[System.Serializable]
		public class BAgentSimpleAssetImplementCustomizer : AssetImplementCustomizer<MonoBBehaverAgent> {
			public List<MonoBBehaverAgent> bAgents;
			List<MonoBBehaverAgent> AssetImplementCustomizer<MonoBBehaverAgent>.implementedAssets => bAgents;
			void AssetImplementCustomizer<MonoBBehaverAgent>.EditAssetRequestFromGBlock(GrammarBlock gBlock, AssetRequestUnit assetReqUnit) {
				assetReqUnit.assettype = "BAgent";
				if (gBlock.unit != null) {
					assetReqUnit.attributes.Add(gBlock.unit.word);
				}
			}

			AssetType AssetImplementCustomizer<MonoBBehaverAgent>.ExtractAsset<AssetType>(MonoBBehaverAgent implementedAsset) {
				if(typeof(AssetType) == typeof(MonoBBehaverAgent)) {
					return (AssetType)(object)implementedAsset;
				}
				return default(AssetType);
			}

			MonoBBehaverAgent AssetImplementCustomizer<MonoBBehaverAgent>.IntegrateImplementType<AssetType>(AssetType asset, AssetUnitInfo gBlock, AssetRequestUnit assetReqUnit) {
				if (typeof(AssetType) == typeof(MonoBBehaverAgent)) {
					return (MonoBBehaverAgent)(object)asset;
				}
				return null;
			}

			bool AssetImplementCustomizer<MonoBBehaverAgent>.MatchAsset(MonoBBehaverAgent implementedAsset, GrammarBlock gBlock) {
				return implementedAsset.behaverAgent.MatchAttribue(gBlock) == AttributeMatchResult.POSITIVE;
			}
		}
		[System.Serializable]
		public class CustomizableUnityBehaverImplementCustomizer : AssetImplementCustomizer<CustomizableUnityBehaver> {
			public List<CustomizableUnityBehaver> customBehavers;
			List<CustomizableUnityBehaver> AssetImplementCustomizer<CustomizableUnityBehaver>.implementedAssets => customBehavers;
			void AssetImplementCustomizer<CustomizableUnityBehaver>.EditAssetRequestFromGBlock(GrammarBlock gBlock, AssetRequestUnit assetReqUnit) {
				assetReqUnit.assettype = "CustomBehaver";
				if (gBlock.unit != null) {
					assetReqUnit.attributes.Add(gBlock.unit.word);
				}
			}

			AssetType AssetImplementCustomizer<CustomizableUnityBehaver>.ExtractAsset<AssetType>(CustomizableUnityBehaver implementedAsset) {
				if (typeof(AssetType) == typeof(CustomizableUnityBehaver)) {
					return (AssetType)(object)implementedAsset;
				}
				return default(AssetType);
			}

			CustomizableUnityBehaver AssetImplementCustomizer<CustomizableUnityBehaver>.IntegrateImplementType<AssetType>(AssetType asset, AssetUnitInfo gBlock, AssetRequestUnit assetReqUnit) {
				if (typeof(AssetType) == typeof(CustomizableUnityBehaver)) {
					return (CustomizableUnityBehaver)(object)asset;
				}
				return null;
			}

			bool AssetImplementCustomizer<CustomizableUnityBehaver>.MatchAsset(CustomizableUnityBehaver implementedAsset, GrammarBlock gBlock) {
				return implementedAsset.behaver.MatchAttribue(gBlock) == AttributeMatchResult.POSITIVE;
			}
		}
		[System.Serializable]
		public class BehaverEquipperImplementCustomizer : AssetImplementCustomizer<MonoBBehaverEquipper> {
			public List<MonoBBehaverEquipper> equippers;
			List<MonoBBehaverEquipper> AssetImplementCustomizer<MonoBBehaverEquipper>.implementedAssets => equippers;
			void AssetImplementCustomizer<MonoBBehaverEquipper>.EditAssetRequestFromGBlock(GrammarBlock gBlock, AssetRequestUnit assetReqUnit) {
				assetReqUnit.assettype = "CustomBehaver";
				if (gBlock.unit != null) {
					assetReqUnit.attributes.Add(gBlock.unit.word);
				}
			}

			AssetType AssetImplementCustomizer<MonoBBehaverEquipper>.ExtractAsset<AssetType>(MonoBBehaverEquipper implementedAsset) {
				if (typeof(AssetType) == typeof(MonoBBehaverEquipper)) {
					return (AssetType)(object)implementedAsset;
				}
				return default(AssetType);
			}

			MonoBBehaverEquipper AssetImplementCustomizer<MonoBBehaverEquipper>.IntegrateImplementType<AssetType>(AssetType asset, AssetUnitInfo gBlock, AssetRequestUnit assetReqUnit) {
				if (typeof(AssetType) == typeof(MonoBBehaverEquipper)) {
					return (MonoBBehaverEquipper)(object)asset;
				}
				return null;
			}

			bool AssetImplementCustomizer<MonoBBehaverEquipper>.MatchAsset(MonoBBehaverEquipper implementedAsset, GrammarBlock gBlock) {
				var checker = new JustCheckEquipListener();
				if (GrammarBlockUtils.ShallowSeek(gBlock.metaInfo, "SV") == null)
					implementedAsset.behaverEquipper.EquipBehaverByBehavior(new StdBehaviorExpression(gBlock.cluster.blocks[0], gBlock.cluster.blocks[1].unit), checker);
				else
					implementedAsset.behaverEquipper.EquipBehaverByAttribute(gBlock, checker);
				return checker.canEquip;
			}
		}
	}
}
