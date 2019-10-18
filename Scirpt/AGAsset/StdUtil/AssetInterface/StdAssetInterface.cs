using AGAsset;
using AGDevUnity;
using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset.StdUtil;
namespace AGDevUnity.StdUtil {
	public class StdAssetInterface : MonoBAssetInterface, ImmediateGiver<AssetUnitInterface, AssetUnitInfo> {
		public MonoBAssetReferer assetReferer;
		public MonoBAssetBasicIO basicIO;
		public MonoBAUICustomizer customizer;
		public bool doCache = false;
		public override ImmediateGiver<AssetUnitInterface, AssetUnitInfo> assetInterface => this;
		public List<AddressAndAssetInterface> archive = new List<AddressAndAssetInterface>();
		AssetUnitInterface ImmediateGiver<AssetUnitInterface, AssetUnitInfo>.PickBestElement(AssetUnitInfo key) {
			var assetIDName = AssetUtils.StdAssetIDName(key);
			//check if already created
			archive.Find((item) => item.name == assetIDName);
			var customInterface = TryGetCustomInterface(key);
			if (customInterface != null) {
				archive.Add(new AddressAndAssetInterface { auInterface = customInterface, name = assetIDName });
				return customInterface;
			}
			var delayInterface = new DelayingAssetUnitInterface { parent = this, assetUnit = key };
			archive.Add(new AddressAndAssetInterface { auInterface = delayInterface, name = assetIDName });
			assetReferer.assetReferer.ReferAsset(key, delayInterface);
			return delayInterface;
		}
		public AssetUnitInterface TryGetCustomInterface(AssetUnitInfo key) {
			if (basicIO.assetIO.LocalizedAssetRef(key) == key.reference) {
				var io = basicIO.assetIO.assetGiver.PickBestElement(key);
				if (io != null) {
					return customizer.assetCustomizer.PickBestElement(io);
				}
			}
			return null;
		}
		[System.Serializable]
		public class AddressAndAssetInterface {
			public string name;
			public string referece;
			public AssetUnitInterface auInterface;
		}
		public class DelayingAssetUnitInterface : AssetReferenceListener, AssetUnitInterface, AssetReferInterface, AssetModifyInterface {
			public StdAssetInterface parent;
			public AssetUnitInterface obtainedInterface;
			public AssetUnitInfo assetUnit;
			AssetReferInterface AssetUnitInterface.referer => this;
			AssetModifyInterface AssetUnitInterface.modifier => this;

			AssetUnitInfo AssetUnitInterface.baseAssetInfo => assetUnit;

			static List<string> empty = new List<string>();
			public List<System.Action> tasks = new List<System.Action>();
			void ConsumeTasks() {
				foreach (var task in tasks ) {
					task();
				}
				tasks.Clear();
			}
			void AssetReferInterface.PickContent<ContentType>(string assetName, Taker<ContentType> collector) {
				if (obtainedInterface != null)
					obtainedInterface.referer.PickContent(assetName, collector);
				else {
					tasks.Add(
						() => { obtainedInterface.referer.PickContent(assetName, collector); }
					);
				}
			}
			void AssetModifyInterface.SetContent<ContentType>(
				AssetContentSettingParam<ContentType> setParam,
				AssetInResultListener<ContentType> listener
			) {
				if (obtainedInterface != null)
					obtainedInterface.modifier.SetContent(setParam, listener);
				else {
					tasks.Add(() => {
						obtainedInterface.modifier.SetContent(setParam, listener);
					}
					);
				}
			}
			void AssetReferenceListener.EnsureDependencyLocalized(AssetRequestUnit assetRequest) {
				//stub
			}
			AssetUnitInterface CreateLocalizedAUInterface() {
				var localizedAsset = AssetUtils.LocalizeAssetRef(assetUnit, parent.basicIO.assetIO.LocalizedAssetRef(assetUnit));
				return parent.TryGetCustomInterface(localizedAsset);
			}
			AssetUnitInterface CreateLocalizedAUInterface(AssetUnitInfo auinfo) {
				var localizedAsset = AssetUtils.LocalizeAssetRef(assetUnit, parent.basicIO.assetIO.LocalizedAssetRef(auinfo));
				return parent.TryGetCustomInterface(localizedAsset);
			}
			void AssetReferenceListener.OnAssetContentObtained<AssetContentType>(AssetContentType asset, string contentName) {
				obtainedInterface = obtainedInterface ?? CreateLocalizedAUInterface();
				if (obtainedInterface != null) {
					obtainedInterface.modifier.SetContent(
						new AssetContentSettingParam<AssetContentType> {
							content = asset,
							contentPath = contentName,
							doOverwrite = true
						},
						new StubAssetInResultListener<AssetContentType>()
					);
				}
				ConsumeTasks();
			}

			void AssetReferenceListener.OnAssetInfoObtained(AssetUnitInfo obtained, AssetReferer referer) {
				//stub
			}

			void AssetReferenceListener.OnBeginRefering() {
				//stub
			}

			void AssetReferenceListener.OnFinish() {
				//stub
			}

			void AssetReferenceListener.OnInterfaceObtained(AssetUnitInterface assetInterface) {
				if (parent.doCache) {
					obtainedInterface = new CachingAUInterface { local = CreateLocalizedAUInterface(), remote = assetInterface };
				} else
					obtainedInterface = assetInterface;
				ConsumeTasks();
			}

			void AssetReferenceListener.OnRawAssetContentObtained(byte[] rawData, string contentType) {
				obtainedInterface = obtainedInterface ?? CreateLocalizedAUInterface(); 
				obtainedInterface.modifier.SetContent(
					new AssetContentSettingParam<byte[]> {
						content = rawData,
						contentPath = "",
						doOverwrite = true
					},
					new StubAssetInResultListener<byte[]>());
				ConsumeTasks();
			}

			void AssetReferenceListener.OnStdArchiveObtained(byte[] archiveData) {
			}

			void AssetReferenceListener.OnVCSReferenceObtained(VersionControlSystemRef vcsRef) {
				obtainedInterface = obtainedInterface ?? CreateLocalizedAUInterface();
				if (obtainedInterface != null) {
					obtainedInterface.modifier.SetContent(
						new AssetContentSettingParam<VersionControlSystemRef> {
							content = vcsRef,
							contentPath = "",
							doOverwrite = true
						},
						new StubAssetInResultListener<VersionControlSystemRef>()
					);
				}
				ConsumeTasks();
			}

			void AssetReferenceListener.OnBasicIOObtained(AssetUnitBasicIO referInterface) {
				if (parent.doCache) {
					obtainedInterface = new CachingAUInterface { local = CreateLocalizedAUInterface(), remote = parent.customizer.assetCustomizer.PickBestElement(referInterface) };
				} else
					obtainedInterface = parent.customizer.assetCustomizer.PickBestElement(referInterface);
				//obtainedInterface = parent.customizer.assetCustomizer.PickBestElement(referInterface);
				ConsumeTasks();
			}

			void AssetReferenceListener.OnObjectObtained(object asset, string contentPath) {
				obtainedInterface = CreateLocalizedAUInterface();
				if (obtainedInterface != null) {
					obtainedInterface.modifier.SetContent(
						new AssetContentSettingParam<object> {
							content = asset,
							contentPath = contentPath,
							doOverwrite = true
						},
						new StubAssetInResultListener<object>()
					);
				}
				ConsumeTasks();
			}
		}
	}
}