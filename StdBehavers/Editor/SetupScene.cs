using AGDev.StdExprBhvr.Unity;
using AGAsset;
using UnityEngine;
using UnityEditor;
using AGDevUnity;
using AGAsset.StdUtil;
using System;

namespace AGDev.StdExprBhvr.Unity {
#if false
	public class MenuItems {
		static GameObject interpreterPrefab;
		[MenuItem("AGDev/ReadyScene")]
		public static void SetupScene() {
			var scenePrepList = AssetDatabase.LoadAssetAtPath<PrefabList>(EditTimeAssetUtils.assetGenDir + "ScenePreps.asset");
			foreach (var scenePrep in scenePrepList.assets) {
				scenePrep.GetComponent<MonoBScenePrep>().scenePrep.SetupBase(new StubSimpleProcessListener());
			}
			//basic prefabs
			var basicPrefabDatabase = AssetDatabase.LoadAssetAtPath<PrefabDatabaseSource>(EditTimeAssetUtils.assetGenDir + "Prefabs.asset");
			//interpreter
			var interpterer = GameObject.FindObjectOfType<StdMonoBERWordsIptr>();
			if (interpterer == null) {
				interpterer = GameObject.Instantiate(
					basicPrefabDatabase.implementedAssetDatabase.PickBestElement(new StdGrammarUnit("Interpreter"))
				).GetComponent<StdMonoBERWordsIptr>();
			}
			//interpterer.grammarConfigurations = AssetDatabase.LoadAssetAtPath<TextAssetList>(EditTimeAssetUtils.assetGenDir + "Dictionaries.asset");
			UnityEditor.EditorUtility.SetDirty(interpterer);
			//erw holder
			var holder = GameObject.FindObjectOfType<MonoBERWordsHolder>();
			holder.assetRef = AssetDatabase.LoadAssetAtPath<TextAssetList>(EditTimeAssetUtils.assetGenDir + "ERWCs.asset");
			holder.interpreter = interpterer;
			UnityEditor.EditorUtility.SetDirty(holder);
			//Basic Prefabs
			Action<string> func = (name) => {
				var newViewFrame = GameObject.Instantiate(basicPrefabDatabase.implementedAssetDatabase.PickBestElement(new StdGrammarUnit(name)));
				UnityEditor.EditorUtility.SetDirty(newViewFrame);
			};
			if (GameObject.FindObjectOfType<MonoBBasicGObjField>() == null)
				func("ViewFrame");
			if (GameObject.FindObjectOfType<MonoBBFrontViewer>() == null)
				func("BFViewer");
			if (GameObject.FindObjectOfType<MonoBStdInputFactory>() == null)
				func("StdInput");
			if (GameObject.FindObjectOfType<MonoBPlayerInterface>() == null)
				func("PInterface");
			if (GameObject.FindObjectOfType<MonoBSpaceDistributer>() == null)
				func("SDistributer");
			//BFront
			{
#if false
				var BFrontFactory = GameObject.FindObjectOfType<StdMonoBBahaverFrontFactory>();
				if (BFrontFactory != null) {
					BFrontFactory.bFrontPrefabDatabase = AssetDatabase.LoadAssetAtPath<PrefabDatabaseSource>(EditTimeAssetUtils.assetGenDir + "BFronts.asset");
					BFrontFactory.bFrontViewer = GameObject.FindObjectOfType<MonoBBFrontViewer>();
					BFrontFactory.spaceDistributer = GameObject.FindObjectOfType<MonoBSpaceDistributer>();
				}
				UnityEditor.EditorUtility.SetDirty(BFrontFactory);
#endif
			}
			//behavers
			/*var bhvr = GameObject.FindObjectOfType<MonoBStdExprBehaver>();
			if (bhvr != null) {
				if (bhvr.monoBChoosingUIPrefab == null) {
					bhvr.monoBChoosingUIPrefab = basicPrefabDatabase.implementedAssetDatabase.PickBestElement(new StdGrammarUnit("ChoosingUI")).GetComponent<MonoBChoosingUI>();
					UnityEditor.EditorUtility.SetDirty(bhvr.monoBChoosingUIPrefab);
				}
				bhvr.monoGObjField = GameObject.FindObjectOfType<MonoBBasicGObjField>();
				bhvr.bFrontViewer = GameObject.FindObjectOfType<MonoBBFrontViewer>();
				foreach (var scenePrep in scenePrepList.assets) {
					scenePrep.GetComponent<MonoBScenePrep>().scenePrep.PrepareScene(new StdSimpleProcessListener());
				}
				UnityEditor.EditorUtility.SetDirty(bhvr);
			}*/
			var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
		}
	}
#endif
}