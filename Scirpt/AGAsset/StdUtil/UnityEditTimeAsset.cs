using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Data;
using System.Linq;
namespace AGAsset.StdUtil {
	public class EditTimeAssetUtils  {
		public const string stdAssetRefFileName = "agas.json";
		public const string stdAssetArchiveFileName = "agas.zip";
		public static string stdDeviceAssetFetchPath = System.Environment.GetEnvironmentVariable("AGDEV_PATH") + "Fetched/";
		public static string stdDeviceAssetPickPath
			= System.Environment.GetEnvironmentVariable("AGDEV_PATH") + "Integrated/pickedAssets.json";
		public static string rootDir = "Assets/";
		public static string assetFetcheDir = "Assets/Fetched/";
		public static string temporaryDir = "Assets/Temp/";
		public static string assetGenRootPath = rootDir + "Generated";
		public static string builtinAssetRootPath = "Assets/Externals/AGDevCSBridge/BuiltinAssets";
		public static string assetGenDir = rootDir + "Generated/";
		public static string builtinAssetDir = "Assets/Externals/AGDevCSBridge/BuiltinAssets/";
		public static string manualAssetDir = "Assets/Manual/";
		public static string manualAssetGenDir = manualAssetDir + "Generated/";
		public static string manualERWCFileName = "ManualEWRCs.Asset";
		public static string GetAssetNameFromPackedAttributes(string attribute) {
			return attribute.Split(';')[0].Split('=').Last();
		}
		public static string LocalAssetRefForGenerated(AssetUnitInfo generatedAssetUnitInfo) {
			return assetGenDir + GetAssetDirectoryName(generatedAssetUnitInfo) + "/agas.json";
		}
		public static string GetAssetDirectoryName(AssetUnitInfo remoteAssetUnitInfo) {
			return remoteAssetUnitInfo.distributor + "_" + remoteAssetUnitInfo.shortname;
		}
		public static string GetAssetDirectoryPath(AssetUnitInfo remoteAssetUnitInfo) {
			return assetFetcheDir + GetAssetDirectoryName(remoteAssetUnitInfo);
		}
		public static AssetUnitInfo AUInfoRemoteToLocal(AssetUnitInfo remoteAssetUnitInfo) {
			var localizedInfo = new AssetUnitInfo {
				id = remoteAssetUnitInfo.id,
				reference = GetAssetDirectoryPath(remoteAssetUnitInfo) + "/agas.json",
				distributor = remoteAssetUnitInfo.distributor,
				shortname = remoteAssetUnitInfo.shortname,
				lastupdate = remoteAssetUnitInfo.lastupdate,
				assettype = remoteAssetUnitInfo.assettype,
				attributes = remoteAssetUnitInfo.attributes
			};
			return localizedInfo;
		}
#if false
        public static AssetUnitInfo EntityToAssetUnitInfo(IDataReader reader) {
			AssetUnitInfo assetUnitInfo = new AssetUnitInfo();
			assetUnitInfo.id = reader.GetInt32(0);
			assetUnitInfo.reference = reader.GetString(1);
			assetUnitInfo.distributor = reader.GetString(2);
			assetUnitInfo.shortname = reader.GetString(3);
			assetUnitInfo.lastupdate = reader.GetString(4);
			assetUnitInfo.assettype = reader.GetString(7);
			assetUnitInfo.attributes = reader.GetString(8);
			return assetUnitInfo;
		}
		public static AssetInfo EntityToAssetInfo(IDataReader reader) {
			AssetUnitInfo assetUnitInfo = EditTimeAssetUtils.EntityToAssetUnitInfo(reader);
			AssetInfo newAssetInfo = new AssetInfo();
			newAssetInfo.units = new List<AssetUnitInfo>();
			newAssetInfo.units.Add(assetUnitInfo);
			return newAssetInfo;
		}
#endif
	}
#if false
	class StdEditTimeUnityAssetIO : BasicAssetIO, BasicAssetIn, BasicAssetOut {
		public CommonAssetRef assetRefs;
		public AssetUnitInfo assetUnitInfo;
		BasicAssetIn BasicAssetIO.assetIn => this;
		BasicAssetOut BasicAssetIO.assetOut => this;

		void BasicAssetOut.PickAssetAtPath<ElementType>(string path, Taker<ElementType> collector) {
			try {
				var contentFullPath = Path.GetDirectoryName(assetUnitInfo.localref) + "/" + path;
				if (typeof(ElementType) == (typeof(ModelImporter))) {
					var importer = (ElementType)(object)ModelImporter.GetAtPath(contentFullPath);
					collector.Take(importer);
				} else if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(ElementType))) {	
					try {
						foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(contentFullPath)) {
							try {
								var concreteAsset = (ElementType)(object)asset;
								if (concreteAsset != null) {
									collector.Take(concreteAsset);
									break;
								}
							} catch { }
						}
					} catch {
						collector.OnFail("StdEditTimeUnityAssetIO.PickAssetAtPath: ");
					}
				} else {
					if(File.Exists(contentFullPath))
						collector.Take(RequiredFuncs.FromJsonAtPath<ElementType>(contentFullPath));
				}
			} catch (Exception e) {
				Debug.LogError(e);
			}
			collector.OnFinish();
		}
		AssetAddResult<AssetContentType> BasicAssetIn.SetContent<AssetContentType>(BasicAssetInParam<AssetContentType> settingParam) {
			try {
				var assetRootPath = Path.GetDirectoryName(assetUnitInfo.localref);
				if (!Directory.Exists(assetRootPath)) {
					Directory.CreateDirectory(assetRootPath);
				}
				var assetPath = assetRootPath + "/" + settingParam.assetPath;
				if (typeof(AssetContentType) == typeof(GameObject)) {
					var gObj = (GameObject)Convert.ChangeType(settingParam.content, typeof(GameObject));
					GameObject oldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
					GameObject newContent = null;
					if (oldPrefab != null && settingParam.doOverwrite)
						newContent = PrefabUtility.ReplacePrefab(gObj, oldPrefab);
					else
						newContent = PrefabUtility.CreatePrefab(assetPath, gObj);
					EditorUtility.SetDirty(newContent);
					AssetDatabase.SaveAssets();
					return new AssetAddResult<AssetContentType> { permanentContent = (AssetContentType)Convert.ChangeType(newContent, typeof(AssetContentType)), didSuccess = true };
				} else if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(AssetContentType))) {
					UnityEngine.Object obj = (UnityEngine.Object)(object)settingParam.content;
					var oldAssetPath = AssetDatabase.GetAssetPath(obj);
					if (Utilities.IsStringEmpty(oldAssetPath)) {
						AssetDatabase.CreateAsset(obj, assetPath);
					} else {
						if (oldAssetPath.StartsWith("Assets/"))
							AssetDatabase.MoveAsset(oldAssetPath, assetPath);
						else {
							if(typeof(AssetContentType) == typeof(Material))
								AssetDatabase.CreateAsset(new Material((Material)(object)obj), assetPath);
						}
					}
					EditorUtility.SetDirty(obj);
					AssetDatabase.SaveAssets();
					return new AssetAddResult<AssetContentType> { permanentContent = settingParam.content, didSuccess = true };
				} else if (typeof(CommonAssetRef).IsAssignableFrom(typeof(AssetContentType))) {
					if (File.Exists(assetPath) && settingParam.doOverwrite) {
						try {
							return new AssetAddResult<AssetContentType> { didSuccess = false, permanentContent = RequiredFuncs.FromJsonAtPath<AssetContentType>(assetPath) };
						} catch { return new AssetAddResult<AssetContentType> { didSuccess = false }; }
					}
					File.WriteAllText(assetPath, RequiredFuncs.ToJson(settingParam.content));
					//AssetDatabase.Refresh();
					//AssetDatabase.SaveAssets();
					return new AssetAddResult<AssetContentType> { permanentContent = settingParam.content, didSuccess = true };
				}
			} catch (Exception e) { Debug.LogError(e); }
			return new AssetAddResult<AssetContentType> { didSuccess = false };
		}
	}
	class StdEditTimeAssetLocalizingItfcGiver : Giver<AssetInterface, AssetUnitInfo> {
		public Taker<AssetUnitInfo> localizedAssetInfoColl;
		public ImmediateGiver<AssetUnitInfo, AssetUnitInfo> localizedAssetInfoGiver;
		public Dictionary<string, ImmediateGiver<AssetInterface, BasicAssetIO>> customAssetInterfaceGiver;
		public AssetReferer remoteAssetReferer;
		public Action TriggerScriptRebuild;
		public AssetInstallListener installListener;
		public AssetUnitSupplier localizedAssetSupplier;
		public int requestCount = 0;
		public int finishCount = 0;
		public bool DidAllRequestFinish() {
			return requestCount <= finishCount;
		}
		bool CheckDependencyAndCollectInterface(AssetInterface localAssetInterface) {
			//check if all dependencies are clear here
			var lis = new EasyTaker<AssetRequest>();
			localAssetInterface.referer.PickAssetByType(lis);
			bool isDependencyCheckOK = true;
			if (lis.chosen != null) {
				var lisetner = new JustLookAUSListener();
				foreach (var assetReqUnit in lis.chosen.units) {
					localizedAssetSupplier.SupplyAssetUnit(assetReqUnit, lisetner);
					if (!lisetner.didSupply) {
						installListener.OnDependencyRequired(assetReqUnit);
						isDependencyCheckOK = false;
					}
				}
			}
			return isDependencyCheckOK;
		}
		void CheckDependencyAndCollectInterface(AssetUnitInfo assetUnitInfo, Taker<AssetInterface> collector, bool doAddToLocalItfcDbs = false) {
			var customItfc = customAssetInterfaceGiver[AssetUtils.GetPrimalType(assetUnitInfo)];
			if (customItfc != null) {
				var localAssetInterface = customItfc.PickBestElement(new StdEditTimeUnityAssetIO { assetUnitInfo = assetUnitInfo });
				if (doAddToLocalItfcDbs) {
					localizedAssetInfoColl.Take(assetUnitInfo);
				}
				if (CheckDependencyAndCollectInterface(localAssetInterface)) {
					collector.Take(localAssetInterface);
				}
			}
			finishCount++;
			collector.OnFinish();
			return;
		}
		void Giver<AssetInterface, AssetUnitInfo>.PickBestElement(AssetUnitInfo assetUnitInfo, Taker<AssetInterface> collector) {
			requestCount++;
			var localAUInfo = localizedAssetInfoGiver.PickBestElement(assetUnitInfo);
			//already localized
			if (localAUInfo != null) {
				CheckDependencyAndCollectInterface(localAUInfo, collector);
			}
			//localref is empty = it's newly generated asset
			else if (Utilities.IsStringEmpty(assetUnitInfo.localref)) {
				var generatedLocalAssetInfo = assetUnitInfo;
				assetUnitInfo.localref = EditTimeAssetUtils.LocalAssetRefForGenerated(assetUnitInfo);
				CheckDependencyAndCollectInterface(assetUnitInfo, collector, true);
			}
			//it's remote asset
			else {
				var refListener = new PrvtAssetReferenceListener {
					baseAssetUnitInfo = assetUnitInfo,
					interfaceTaker = collector,
					parent = this
				};
				//someone want to controll download timing
				if (installListener != null) {
					installListener.OnInstallRemoteAsset(assetUnitInfo, new PrvtDelayedAssetRefTrigger { client = this, assetUnitInfo = assetUnitInfo, refListener = refListener });
				} else {
					remoteAssetReferer.ReferAsset(assetUnitInfo, refListener);
				}
			}
		}
		class PrvtDelayedAssetRefTrigger : SimpleTrigger {
			public StdEditTimeAssetLocalizingItfcGiver client;
			public AssetUnitInfo assetUnitInfo;
			public PrvtAssetReferenceListener refListener;
			void SimpleTrigger.Trigger(SimpleProcessListener simpleListener) {
				refListener.interfaceTaker = new PrvtAssetCollListener {
					interfaceTaker = refListener.interfaceTaker,
					simpleListener = simpleListener
				};
				client.remoteAssetReferer.ReferAsset(assetUnitInfo, refListener);
			}
		}
		public class PrvtAssetCollListener : Taker<AssetInterface> {
			public Taker<AssetInterface> interfaceTaker;
			public SimpleProcessListener simpleListener;
			public bool success = false;
			void Taker<AssetInterface>.Take(AssetInterface newElement) {
				success = true;
				interfaceTaker.Take(newElement);
			}

			void Taker<AssetInterface>.OnFail(string reason) {
				interfaceTaker.OnFail(reason);
				simpleListener.OnFail(reason);
			}

			void AsyncProcess.OnFinish() {
				interfaceTaker.OnFinish();
				if (success)
					simpleListener.OnSuccess();
				else{
					simpleListener.OnFail("Unknown");
				}
			}
		}
		class PrvtAssetReferenceListener : AssetReferenceListener {
			public Taker<AssetInterface> interfaceTaker;
			public AssetUnitInfo baseAssetUnitInfo;
			public StdEditTimeAssetLocalizingItfcGiver parent;
			AssetInterface localAssetInterface;
			string assetRootPath = "";
			AssetUnitInfo localAUInfo;
			void EnsureLocalAssetDirectory() {
#region create asset directory
				localAUInfo = EditTimeAssetUtils.AUInfoRemoteToLocal(baseAssetUnitInfo);
				assetRootPath = Path.GetDirectoryName(localAUInfo.localref);
				if (!Directory.Exists(assetRootPath)) {
					Directory.CreateDirectory(assetRootPath);
				}
				if (localAssetInterface == null) {
					var customItfc = parent.customAssetInterfaceGiver[AssetUtils.GetPrimalType(localAUInfo)];
					localAssetInterface = customItfc.PickBestElement(new StdEditTimeUnityAssetIO { assetUnitInfo = localAUInfo });
				}
#endregion
			}
			void AssetReferenceListener.OnAssetInfoObtained(AssetUnitInfo obtained, AssetReferer referer) {
				//todo : proper info combination
				//var newAssetUnitInfo = AssetUtils.CombineAssetUnitInfo(baseAssetUnitInfo, obtained);
				referer.ReferAsset(obtained, this);
			}

			void AssetReferenceListener.OnAssetContentObtained<AssetType>(AssetType asset) {
				EnsureLocalAssetDirectory();
				if (localAssetInterface != null) {
					if (baseAssetUnitInfo.assettype == "Picture" && typeof(Texture2D) == typeof(AssetType)) {
						var tex = (Texture2D)(object)asset;
						var tempPath = assetRootPath + "/temp.png";
						File.WriteAllBytes(tempPath, tex.EncodeToPNG());
						AssetDatabase.ImportAsset(tempPath);
						var texImporter = (TextureImporter)TextureImporter.GetAtPath(tempPath);
						texImporter.textureType = TextureImporterType.Sprite;
						texImporter.spriteImportMode = SpriteImportMode.Single;
						texImporter.SaveAndReimport();
						var importedTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(tempPath);
						localAssetInterface.modifier.ImportAsset(new AssetImportParam<Sprite> { assetUnitInfo = baseAssetUnitInfo, content = importedTexture, doOverwrite = true });
					} else {
						localAssetInterface.modifier.ImportAsset(new AssetImportParam<AssetType> { assetUnitInfo = baseAssetUnitInfo, content = asset, doOverwrite = true });
					}
					parent.localizedAssetInfoColl.Take(localAUInfo);
				}
			}

			void AssetReferenceListener.OnBeginRefering() {
			}

			void AssetReferenceListener.OnFinish() {
				parent.finishCount++;
				if (localAssetInterface != null) {
					if (parent.CheckDependencyAndCollectInterface(localAssetInterface)) {
						interfaceTaker.Take(localAssetInterface);
					}
				}
				interfaceTaker.OnFinish();
			}
			void AssetReferenceListener.OnInterfaceObtained(AssetInterface assetInterface) {
				localAssetInterface = assetInterface;
			}
			void RefreshAssets() {
				if (Directory.GetFiles(assetRootPath, "*.cs", SearchOption.AllDirectories).Length > 0) {
					parent.TriggerScriptRebuild();
					return;
				} else {
					AssetDatabase.Refresh();
					AssetDatabase.SaveAssets();
				}
			}
			void AssetReferenceListener.OnStdArchiveObtained(byte[] obtained) {
				EnsureLocalAssetDirectory();
				string tempArchiveFileName = assetRootPath + "/" + EditTimeAssetUtils.stdAssetArchiveFileName;
				File.WriteAllBytes(tempArchiveFileName, obtained);
				ZipUtil.Unzip(tempArchiveFileName, assetRootPath);
				File.Delete(tempArchiveFileName);
				parent.localizedAssetInfoColl.Take(localAUInfo);
				RefreshAssets();
			}
			void AssetReferenceListener.OnVCSReferenceObtained(VersionControlSystemRef vcsRef) {
				EnsureLocalAssetDirectory();
				try {
					if (vcsRef.systemName == "git") {
						System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
						myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
						myProcess.StartInfo.CreateNoWindow = false;
						myProcess.StartInfo.UseShellExecute = true;
						myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
						string gitCommand = "";
						if ( Utilities.IsStringEmpty(vcsRef.targetPath) ) {
							//gitCommand = "git clone --shallow-submodules --depth 1 " + vcsRef.repository + " " + assetRootPath;
							gitCommand = "git clone --depth 1 " + vcsRef.repository + " " + assetRootPath;
							myProcess.StartInfo.Arguments = "/K \"\"C:\\Program Files\\Git\\bin\\sh.exe\" --login -i -c \"" + gitCommand + "\"\"";
							Debug.Log(myProcess.StartInfo.Arguments);
							myProcess.EnableRaisingEvents = true;
							myProcess.Start();
							myProcess.WaitForExit();
							int ExitCode = myProcess.ExitCode;
							//if (ExitCode >= 0) {
								parent.localizedAssetInfoColl.Take(localAUInfo);
							//}
							Debug.Log(ExitCode);
						}
						else {
							gitCommand = "git archive --format=tar --remote=" + vcsRef.repository + " HEAD:" + vcsRef.targetPath + " > " + assetRootPath + "/Temp.tar.gz | tar xf " + assetRootPath + "/Temp.tar.gz";
						}
					}
				} catch (Exception e) {
					Debug.Log(e);
				}
				RefreshAssets();
			}

			void AssetReferenceListener.OnRawAssetContentObtained(byte[] rawContent, string contentType) {
				EnsureLocalAssetDirectory();
				if (contentType == "mp3") {
					var tempPath = assetRootPath + "/temp.mp3";
					File.WriteAllBytes(tempPath, rawContent);
					UnityEditor.AssetDatabase.ImportAsset(tempPath);
					var audioClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(tempPath);
					localAssetInterface.modifier.ImportAsset(new AssetImportParam<AudioClip> { assetUnitInfo = baseAssetUnitInfo, content = audioClip, doOverwrite = true });
					parent.localizedAssetInfoColl.Take(localAUInfo);
				}
			}

			void AssetReferenceListener.EnsureDependencyLocalized(AssetRequestUnit assetRequest) {
				var lisetner = new JustLookAUSListener();
				parent.localizedAssetSupplier.SupplyAssetUnit(assetRequest, lisetner);
				if (!lisetner.didSupply) {
					parent.installListener.OnDependencyRequired(assetRequest);
				}
			}
		}
	}
#endif
}
