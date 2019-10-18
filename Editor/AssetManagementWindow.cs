using UnityEngine;
using UnityEditor;
using AGAsset;
using System.IO;
using System.Collections.Generic;
using System;
using AGAsset.StdUtil;
using AGDevUnity.StdUtil;
using AGDevUnity;
using AGDev.StdUtil;
using AGDev;
using AGBLang;

namespace AGAsset {
#if false
	public class SimpleInstallListener_DetroyGObj : AssetInstallListener {
		public AssetInstallListener clientListener;
		void AssetInstallListener.OnFail(string reason) {
			//GameObject.DestroyImmediate(gObj);
			Debug.Log("OnFail : deleting manager");
			AssetManagementWindow.DiscardAssetManager();
			clientListener.OnFail(reason);
		}

		void AssetInstallListener.OnSuccess(bool didGetNewERWCs) {
			Debug.Log("OnSuccess : deleting manager");
			AssetManagementWindow.DiscardAssetManager();
			clientListener.OnSuccess(didGetNewERWCs);
		}
		void AssetInstallListener.NoInstallRequired() {
			Debug.Log("NoInstallRequired : deleting manager");
			AssetManagementWindow.DiscardAssetManager();
			clientListener.NoInstallRequired();
		}
		void AssetInstallListener.OnScriptRebuildRequired() {
			Debug.Log("OnScriptRebuildRequired : deleting manager");
			AssetManagementWindow.DiscardAssetManager();
			clientListener.OnScriptRebuildRequired();
		}

		void AssetInstallListener.OnInstallRemoteAsset(AssetUnitInfo pickUnit, SimpleTrigger installTrigger) {
			clientListener.OnInstallRemoteAsset(pickUnit, installTrigger);
		}

		void AssetInstallListener.OnDependencyRequired(AssetRequestUnit pickUnit) {
			clientListener.OnDependencyRequired(pickUnit);
		}
	}
	public class SimpleProcessListener_DetroyGObj : SimpleProcessListener {
		public SimpleProcessListener clientListener;
		public void OnFinish(bool didSuccess) {
			GameObject.DestroyImmediate(AssetManagementWindow.assetManager.gameObject);
			clientListener.OnFinish();
		}
	}
#endif
	public class AssetState {
		public Enum assetInstallState;
		public AssetRequest pendingAssetRequest;
		public AssetPick pendingAssetPick;
	}
	public class AssetManagementWindow : EditorWindow {
		public static List<AssetPickDownloadParam> pickAndDownloadTriggers;
		public static AssetRequest dependencyAssetReq;
		public struct AssetInstallSetting {
			public bool recursive;
		}
		public static string continueDummyFilePath = EditTimeAssetUtils.temporaryDir + "InstallContinueTriggerDummy.text";
		public static string reinstallDummyFilePath = EditTimeAssetUtils.temporaryDir + "InstallTriggerDummy.text";
		public static string lastInstallFilePath = EditTimeAssetUtils.temporaryDir + "LastInstallSetting.text";
		public bool doAutoDonwload = false;
		string[] baseAssetRequests = new string[0];
		public int index = 0;
#if false
		private static AssetInstallListener GetListener(bool recursive, bool doAutoDownload, AssetManagementWindow clientWindow) {
			return doAutoDownload ?
				new SimpleInstallListener_DetroyGObj {
					clientListener = new RecursiveInstallListener()
				}
				: new SimpleInstallListener_DetroyGObj {
					clientListener = new OnStepInstallListener { clientWindow = clientWindow, autoDownload = doAutoDownload }
				};
		}
#endif
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded() {
#if false
			Debug.Log("Script Reloaded.");
			AssetManagementWindow window = (AssetManagementWindow)EditorWindow.GetWindow(typeof(AssetManagementWindow));
			DiscardAssetManager();
			bool recursive = true;
			try {
				var setting = RequiredFuncs.FromJsonAtPath<AssetInstallSetting>(lastInstallFilePath);
				recursive = setting.recursive;
			} catch{}
			if (File.Exists(AssetManagementWindow.continueDummyFilePath)) {
				File.Delete(AssetManagementWindow.continueDummyFilePath);
				if (EditorUtility.DisplayDialog("Confirm", "OnScriptsReloaded(): Will continue install.", "OK", "Cancel")) {
					(NewAssetManager() as AssetInstaller).ContinueUnfinishedInstall(GetListener(recursive, window.doAutoDonwload, window));
				}
			}
			if (File.Exists(AssetManagementWindow.reinstallDummyFilePath)) {
				File.Delete(AssetManagementWindow.reinstallDummyFilePath);
				if (EditorUtility.DisplayDialog("Confirm", "OnScriptsReloaded(): Will install assets by new ERWCs.", "OK", "Cancel")) {
					(NewAssetManager() as AssetInstaller).InstallAssetsFromERWCs( new AssetRequest(), GetListener(recursive, window.doAutoDonwload, window));
				}
			}
#endif
		}
		AssetManagementWindow() {
			Directory.CreateDirectory(EditTimeAssetUtils.temporaryDir);
		}
		[MenuItem("AGDev/Temp")]
		static void Temp() {
			Debug.Log(JsonUtility.ToJson(Vector3.zero));
		}
		// Add menu named "My Window" to the Window menu
		[MenuItem("AGDev/AssetManagementWindow")]
		static public void Init() {
			// Get existing open window or if none, make a new one:
			AssetManagementWindow window = (AssetManagementWindow)EditorWindow.GetWindow(typeof(AssetManagementWindow));
			window.Update();
			window.Show();
		}
		public void AddAssetPickUnit(AssetUnitInfo pickUnit, Trigger installTrigger) {
			pickAndDownloadTriggers.Add(new AssetPickDownloadParam { assetUnitToDownload = pickUnit, installTrigger = installTrigger });
		}
		public class AssetPickDownloadParam {
			public AssetUnitInfo assetUnitToDownload;
			public Trigger installTrigger;
			public bool didInstalled = false;
		}
		void Update() {
			var filePaths = Directory.GetFiles(EditTimeAssetUtils.builtinAssetDir + "BaseAssetRequests", "*.json");
			baseAssetRequests = new string[filePaths.Length];
			int i = 0;
			foreach (var filePath in filePaths) {
				baseAssetRequests[i++] = Path.GetFileName(filePath);
			}
		}
		class TestCSColl : Taker<NounCommonSenseUnit> {
			void Taker<NounCommonSenseUnit>.Take(NounCommonSenseUnit item) { }
			void Taker<NounCommonSenseUnit>.None() {}
		}
		void OnGUI() {
			EditorGUILayout.LabelField("BaseAssets");
			index = EditorGUILayout.Popup(index, baseAssetRequests);
			doAutoDonwload = EditorGUILayout.Toggle("Auto", doAutoDonwload);
			if (GUILayout.Button("AllowRemoteAssetSupply")) {
				FindObjectOfType<HTML_AUSupplier>().bridgeSupplier.BeginAssetSupply();
			}
			if (GUILayout.Button("TestCommonSense")) {
				FindObjectOfType<HTML_MonoBCSGiver>().SendNCSRequest(new StubSimpleProcessListener { });
			}
#if false
			if (GUILayout.Button("InstallBaseAssets")) {
				var currentReq = dependencyAssetReq;
				dependencyAssetReq = new AssetRequest();
				Directory.CreateDirectory(EditTimeAssetUtils.temporaryDir);
				File.WriteAllText(lastInstallFilePath, RequiredFuncs.ToJson(new AssetInstallSetting { recursive = false }));
				(NewAssetManager() as AssetInstaller).InstallAssetsFromRequest(
					RequiredFuncs.FromJsonAtPath<AssetRequest>(EditTimeAssetUtils.builtinAssetDir + "BaseAssetRequests/" + baseAssetRequests[index]),
					GetListener(false, doAutoDonwload, this)
				);
			}
			if (GUILayout.Button("InstallAllAssetsRecursive")) {
				var currentReq = dependencyAssetReq;
				dependencyAssetReq = new AssetRequest();
				Directory.CreateDirectory(EditTimeAssetUtils.temporaryDir);
				File.WriteAllText(lastInstallFilePath, RequiredFuncs.ToJson(new AssetInstallSetting { recursive = true }));
				(NewAssetManager() as AssetInstaller).InstallAssetsFromERWCs(
					RequiredFuncs.FromJsonAtPath<AssetRequest>(EditTimeAssetUtils.builtinAssetDir + "BaseAssetRequests/" + baseAssetRequests[index]),
					GetListener(true, doAutoDonwload, this));
			}
			if (GUILayout.Button("UninstallAllAssets")) {
				if (EditorUtility.DisplayDialog("Delete all assets.", "Really?", "OK", "Cancel")) {
					(NewAssetManager() as AssetInstaller).UninstallAllAsets(new SimpleProcessListener_DetroyGObj { clientListener = new StdSimpleProcessListener() });
				}
			}
			if (assetManager != null) {
				GUILayout.Label("Asset Manager is Active.");
				if (GUILayout.Button("ConcludeBehaverPrefabs")) {
					//GameObject.FindObjectOfType<PrepMonoBBehaverFrontFactory>().GenerateBehaverFrontPrefabs();
				}
				if (GUILayout.Button("Stop")) {
					DiscardAssetManager();
				}
				if (pickAndDownloadTriggers != null) {
					foreach (var pickAndTrigger in pickAndDownloadTriggers) {
						GUILayout.Label(
							"AssetPickUnit: " + pickAndTrigger.assetUnitToDownload.sname
						);

						if (!pickAndTrigger.didInstalled) {
							if (GUILayout.Button("Install")) {
								pickAndTrigger.installTrigger.Trigger(new PrvtAssetPickTriggerListener { param = pickAndTrigger });
							}
						} else {
							GUILayout.Label("Already Installed.");
						}
					}
				}
			}
			if (dependencyAssetReq != null) {
				foreach (var assetReqUnit in dependencyAssetReq.units) {
					if (!String.IsNullOrEmpty(assetReqUnit.sname)) {
						GUILayout.Label("DependencyAssetReqUnits: " + assetReqUnit.sname);
					}
				}
				if (GUILayout.Button("InstallDependencies")) {
					(NewAssetManager() as AssetInstaller).InstallAssetsFromRequest(
						dependencyAssetReq, GetListener(false, doAutoDonwload, this)
					);
				}
			}
#endif
		}
	}
}