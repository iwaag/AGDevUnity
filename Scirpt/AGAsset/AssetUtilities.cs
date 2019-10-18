using UnityEngine;
using AGBLang;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System;
using System.Linq;


using System.Net;
using System.Text;
using System.Collections.Specialized;
using AGDevUnity;
using AGAsset.StdUtil;
using AGDev.StdUtil;
using AGDev;
using System.Runtime.Serialization.Json;
using UnityEngine.Networking;

namespace AGAsset.StdUtil {
	/*public class ChainTaker<Item, NextElement> : Taker<AssetUnitInterface> {
		public Taker<Item> nextTaker;
		void AsyncProcess.OnFinish() {
			throw new NotImplementedException();
		}

		public void Take(AssetUnitInterface item) {
			item.
		}
	}*/
	public class RequiredFuncs {
		[Serializable]
		private class ArrayInside<Type> {
			public Type[] array = null;
		}
		public static void Log(string message) {
			Debug.Log(message);
		}
		public static Type FromJson<Type>(string json) {
			return JsonUtility.FromJson<Type>(json);
		}
		public static Type FromJson<Type>(byte[] json)  {
			var ser = new DataContractJsonSerializer(typeof(Type));
			return (Type)ser.ReadObject(new System.IO.MemoryStream(json));
		}
		public static Type[] FromJsonToArray<Type>(string json) {
			return JsonUtility.FromJson<ArrayInside<Type>>("{ \"array\": " + json + "}").array;
		}
		public static Type FromJsonAtPath<Type>(string path) {
			var jsonText = File.ReadAllText(path);
			return JsonUtility.FromJson<Type>(jsonText);
		}
		public static string ToJsonString<Type>(Type srcObj) {
			return Encoding.UTF8.GetString(ToJson(srcObj));
		}
		public static byte[] ToJson<Type>(Type srcObj) {
			var ser = new DataContractJsonSerializer(typeof(Type));
			var result = new MemoryStream();
			ser.WriteObject(result, srcObj);
			return result.ToArray();
		}
		public static void ProcessHTTP(string url, Action<string> onGetResponse, IEnumerable<KeyValuePair<string, string>> headers, byte[] data = null) {
			var headerDict = new Dictionary<string, string>();
			var webReq = new UnityWebRequest(url);
			foreach (var header in headers) {
				webReq.SetRequestHeader(header.Key, header.Value);
			}
			if(data != null)
				webReq.uploadHandler = new UploadHandlerRaw(data);
			ProcessWWW(webReq, (www) => onGetResponse(webReq.downloadHandler.text));
		}
		public static void ProcessWWW(UnityWebRequest www, System.Action<UnityWebRequest> callback, System.Action<string> onFail = null) {
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) {
				UnityEditor.EditorApplication.CallbackFunction routine = null;
				routine = () => {
					while (!www.isDone)
						return;
					if (www.error != null) {
						if(onFail != null) {
							onFail(www.error);
						}
						Debug.Log(www.error);
					}
					UnityEditor.EditorApplication.update -= routine;
					callback(www);
				};
				UnityEditor.EditorApplication.update += routine;
			}
#else
			//ToDO
			//DownloaderCoroutineHolderGObj = new GameObject.
#endif
		}
	}
	public class AssetUtils {
		public static string StdAssetIDName(AssetUnitInfo sourceAsset) {
			return sourceAsset.distributor + "_" + sourceAsset.shortname;
		}
		public static void PickAssetRef<AssetRefType>(GeneralGiver<string> contentGiver, Taker<AssetRefType> refTaker) {
			contentGiver.Give("agas.json", refTaker);
		}
		public static bool DoesContainType(AssetUnitInfo assetUnitInfo, string type) {
			return Array.Find(assetUnitInfo.assettype.Split(';'), (str) => CaseInsensitiveComparer.Equals(assetUnitInfo, type)) != null;
		}
		public static string GetPrimalType(AssetUnitInfo assetUnitInfo) {
			return assetUnitInfo.assettype.Split(';')[0];
		}
		public static bool IsRequestedType(AssetRequestUnit assetRequest, string type) {
			return CaseInsensitiveComparer.Equals(assetRequest.assettype, type);
		}
		public static string[] ExtractPackedAssetAttribute(string attribute) {
			return attribute.Split(';');
		}
		public static AssetUnitInfo GenerateNewAssetTemplate(string name, string type) {
			return new AssetUnitInfo {
				assettype = type,
				attributes = name,
				distributor = "GENERATOR",
				shortname = name + type,
				id = id++,
				lastupdate = "",
				reference = ""
			};
		}
		public static AssetUnitInfo LocalizeAssetRef(AssetUnitInfo sourceAsset, string newAssetLocalRef) {
			return new AssetUnitInfo {
				assettype = sourceAsset.assettype,
				attributes = sourceAsset.attributes,
				distributor = sourceAsset.distributor,
				shortname = sourceAsset.shortname,
				id = sourceAsset.id,
				lastupdate = sourceAsset.lastupdate,
				reference = newAssetLocalRef
			};
		}
		public static AssetUnitInfo CloneAssetRef(AssetUnitInfo sourceAsset) {
			return new AssetUnitInfo {
				assettype = sourceAsset.assettype,
				attributes = sourceAsset.attributes,
				distributor = sourceAsset.distributor,
				shortname = sourceAsset.shortname,
				id = sourceAsset.id,
				lastupdate = sourceAsset.lastupdate,
				reference = sourceAsset.reference
			};
		}
		public static AssetUnitInfo CombineAssetUnitInfo(AssetUnitInfo left, AssetUnitInfo right) {
			/*var combined = new AssetUnitInfo {
				assettype = left.assettype,
				attributes = left.attributes + ";" + right.attributes,
				creatorref = left.creatorref + ";"
			};
			if(left.assettype != right.assettype)
				combined.assettype = left.assettype + ";" + right.assettype;
			if (left.attributes != right.attributes)
				combined.attributes = left.attributes + ";" + right.attributes;
			left.assettype*/
			//todo
			return left;
		}
		public static AssetRequestUnit ChangeAssetRequestType(AssetRequestUnit baseReq, string assetType) {
			return new AssetRequestUnit { assettype = assetType, attributes = baseReq.attributes, creatorref = baseReq.creatorref };
		}
		public static AssetRequestUnit FindEquivelent(AssetRequest targetRq, AssetRequestUnit refReq) {
			var result = targetRq.units.Find((request) => {
				foreach (var newAttribute in refReq.attributes) {
					if (!request.attributes.Contains(newAttribute))
						return false;
				}
				if (!AGDev.StdUtil.Utilities.CompareNullableString( refReq.assettype, request.assettype ))
					return false;
				if (!AGDev.StdUtil.Utilities.CompareNullableString(refReq.sname, request.sname))
					return false;
				return true;
			});
			return result;
		}
		public static void AddReqUnitIfNotDuplicated(AssetRequest targetRq, AssetRequestUnit refReq) {
			if(FindEquivelent(targetRq, refReq) == null) {
				targetRq.units.Add(refReq);
			}
		}
		static int id = 10000;
	}
	#region simple asset interface
	class StdBridgeBasicColl<ContentType> : AssetBasicTaker<ContentType> {
		public Taker<ContentType> collector;
		void AssetBasicTaker<ContentType>.CollectAsset(ContentType assetType) {
			collector.Take(assetType);
		}

		void AssetBasicTaker<ContentType>.CollectRawAsset(byte[] assetType) {
			//stub
		}

		void AssetBasicTaker<ContentType>.OnFinish() {
			collector.None();
		}
	}
	public class AssetToolAUICustomizer : ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> {
		AssetUnitInterface ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO>.PickBestElement(AssetUnitBasicIO assetIO) {
			if( CaseInsensitiveComparer.Equals( assetIO.baseAssetUnitInfo.assettype, "AssetTool" ) )
				return new PrvtAssetInterface { basicAssetIO = assetIO };
			return null;
		}
		class PrvtAssetInterface : AssetUnitInterface, AssetReferInterface, AssetModifyInterface {
			public AssetUnitBasicIO basicAssetIO;
			AssetReferInterface AssetUnitInterface.referer => this;
			AssetModifyInterface AssetUnitInterface.modifier => this;
			AssetUnitInfo AssetUnitInterface.baseAssetInfo => basicAssetIO.baseAssetUnitInfo;

			void AssetReferInterface.PickContent<ContentType>(string path, Taker<ContentType> collector) {
				if (string.IsNullOrEmpty(path)) {
					if (typeof(ContentType) == typeof(MonoBAUICustomizer)) {
						basicAssetIO.assetOut.PickAssetAtPath("Customizer.prefab", new StdBridgeBasicColl<ContentType> { collector = collector });
					}
					else if (typeof(ContentType) == typeof(MonoBAUIntegrator)) {
						basicAssetIO.assetOut.PickAssetAtPath("Integrator.prefab", new StdBridgeBasicColl<ContentType> { collector = collector });
					}
					else
						collector.None();
				} else {
					basicAssetIO.assetOut.PickAssetAtPath(path, new StdBridgeBasicColl<ContentType> { collector = collector });
				}
			}
			void AssetModifyInterface.SetContent<ContentType>(AssetContentSettingParam<ContentType> setParam, AssetInResultListener<ContentType> listener) {
				if (string.IsNullOrEmpty(setParam.contentPath)) {
					if ((typeof(MonoBAUICustomizer) == typeof(ContentType))) {
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<ContentType> {
							content = setParam.content,
							contentPath = "Customizer.prefab",
							doOverwrite = setParam.doOverwrite
						}, listener);
					}
					else if ((typeof(MonoBAUIntegrator) == typeof(ContentType))) {
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<ContentType> {
							content = setParam.content,
							contentPath = "Integrator.prefab",
							doOverwrite = setParam.doOverwrite
						}, listener);
					}
					else {
						listener.OnFail();
					}
				} else {
					basicAssetIO.assetIn.SetContent(new BasicAssetInParam<ContentType> {
						content = setParam.content,
						contentPath = setParam.contentPath,
						doOverwrite = setParam.doOverwrite
					}, listener);
				}
			}
		}
	}
	[System.Serializable]
	public class StdAUICustomizer<MainContentType, MainContentBaseType> : ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> {
		public List<string> supportedAssetTypes = new List<string>();
		public string mainAssetName = "Main.prefab";
		AssetUnitInterface ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO>.PickBestElement(AssetUnitBasicIO assetIO) {
			if (supportedAssetTypes.Contains(assetIO.baseAssetUnitInfo.assettype))
				return new StdAssetInterface { basicAssetIO = assetIO, parent = this };
			return null;
		}
		class StdAssetInterface : AssetUnitInterface, AssetReferInterface, AssetModifyInterface {
			public StdAUICustomizer<MainContentType, MainContentBaseType> parent;
			public AssetUnitBasicIO basicAssetIO;
			AssetReferInterface AssetUnitInterface.referer { get { return this; } }
			AssetModifyInterface AssetUnitInterface.modifier { get { return this; } }

			AssetUnitInfo AssetUnitInterface.baseAssetInfo => basicAssetIO.baseAssetUnitInfo;

			void AssetModifyInterface.SetContent<AssetContentType>(
				AssetContentSettingParam<AssetContentType> settingParam,
				AssetInResultListener<AssetContentType> listener)
			{
				if (string.IsNullOrEmpty(settingParam.contentPath)) {
					if ((typeof(MainContentType) == typeof(AssetContentType))) {
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<AssetContentType> {
							content = settingParam.content,
							contentPath = parent.mainAssetName,
							doOverwrite = settingParam.doOverwrite
						}, listener);
					}
					else {
						listener.OnFail();
					}
				} else {
					basicAssetIO.assetIn.SetContent(new BasicAssetInParam<AssetContentType> {
						content = settingParam.content,
						contentPath = settingParam.contentPath,
						doOverwrite = settingParam.doOverwrite
					}, listener);
				}
				
			}
			/*string FindMainContent() {
				foreach (var mainContentPath in basicAssetIO.assetOut.contentPaths) {
					foreach (var postFix in parent.mainAssetPostfixs) {
						if (mainContentPath.EndsWith(postFix, StringComparison.CurrentCultureIgnoreCase))
							return mainContentPath;
					}
				}
				return null;
			}*/
			void AssetReferInterface.PickContent<ElementType>(string path, Taker<ElementType> collector) {
				if (string.IsNullOrEmpty(path)) {
					if (typeof(ElementType) == typeof(MainContentType)) {
						basicAssetIO.assetOut.PickAssetAtPath(parent.mainAssetName, new StdBridgeBasicColl<ElementType> { collector = collector });
					}
					else if (typeof(ElementType) == typeof(MainContentBaseType)) {
						var toBaseTaker = new StdBridgeBasicColl<MainContentType> { collector = new ToBaseTaker <MainContentType, ElementType> { baseTaker = collector } };
						basicAssetIO.assetOut.PickAssetAtPath(parent.mainAssetName, toBaseTaker);
					}
					else if (typeof(ElementType) == typeof(AssetRequest)) {
						basicAssetIO.assetOut.PickAssetAtPath("Dependencies.json", new StdBridgeBasicColl<ElementType> { collector = collector });
					}
					else {
						collector.None();
					}
				} else {
					basicAssetIO.assetOut.PickAssetAtPath(path, new StdBridgeBasicColl<ElementType> { collector = collector });
				}
			}
		}
	}
	public class PrefabAUICustomizer<BaseMonoBType> : ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> where BaseMonoBType : MonoBehaviour {
		public List<string> supportedAssetTypes = new List<string>();
		public string mainAssetName = "Main.prefab";
		AssetUnitInterface ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO>.PickBestElement(AssetUnitBasicIO assetIO) {
			if (supportedAssetTypes.Contains(assetIO.baseAssetUnitInfo.assettype))
				return new PrefabAssetInterface { basicAssetIO = assetIO, parent = this };
			return null;
		}
		class PrefabAssetInterface : AssetUnitInterface, AssetReferInterface, AssetModifyInterface {
			public PrefabAUICustomizer<BaseMonoBType> parent;
			public AssetUnitBasicIO basicAssetIO;
			AssetReferInterface AssetUnitInterface.referer { get { return this; } }
			AssetModifyInterface AssetUnitInterface.modifier { get { return this; } }

			AssetUnitInfo AssetUnitInterface.baseAssetInfo => basicAssetIO.baseAssetUnitInfo;
			#if false
			class PrvtInLis<MyType, ClientType> : AssetInResultListener<MyType> {
				public AssetInResultListener<ClientType> clientLis;
				void AssetInResultListener<MyType>.OnCopyContent(MyType coppied) {
					clientLis.OnCopyContent((ClientType)(object)coppied);
				}
				void AssetInResultListener<MyType>.OnFail() {
					clientLis.OnFail();
				}
				void AssetInResultListener<MyType>.OnOverwrite() {
					clientLis.OnOverwrite();
				}
				string AssetInResultListener<MyType>.OnRequestPathChange(string suggetion) {
					return clientLis.OnRequestPathChange(suggetion);
				}
				void AssetInResultListener<MyType>.OnSuccess() {
					clientLis.OnSuccess();
				}
			}
			#endif
			void AssetModifyInterface.SetContent<AssetContentType>(
				AssetContentSettingParam<AssetContentType> settingParam,
				AssetInResultListener<AssetContentType> listener) {
				if (string.IsNullOrEmpty(settingParam.contentPath)) {
					if (typeof(BaseMonoBType).IsAssignableFrom(settingParam.content.GetType())) {
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<GameObject> {
							content = (settingParam.content as MonoBehaviour).gameObject,
							contentPath = parent.mainAssetName,
							doOverwrite = settingParam.doOverwrite
						}, new StubAssetInResultListener<GameObject>());
					}
					else if (typeof(GameObject).IsAssignableFrom(settingParam.content.GetType())) {
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<GameObject> {
							content = settingParam.content as GameObject,
							contentPath = parent.mainAssetName,
							doOverwrite = settingParam.doOverwrite
						}, new StubAssetInResultListener<GameObject>());
					}
					else if (typeof(AssetContentType) == typeof(GameObject)) {
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<AssetContentType> {
							content = settingParam.content,
							contentPath = parent.mainAssetName,
							doOverwrite = settingParam.doOverwrite
						}, listener);
					} else {
						listener.OnFail();
					}
				} else {
					basicAssetIO.assetIn.SetContent(new BasicAssetInParam<AssetContentType> {
						content = settingParam.content,
						contentPath = settingParam.contentPath,
						doOverwrite = settingParam.doOverwrite
					}, listener);
				}

			}
			/*string FindMainContent() {
				foreach (var mainContentPath in basicAssetIO.assetOut.contentPaths) {
					foreach (var postFix in parent.mainAssetPostfixs) {
						if (mainContentPath.EndsWith(postFix, StringComparison.CurrentCultureIgnoreCase))
							return mainContentPath;
					}
				}
				return null;
			}*/
			public class PrvtMainColl : Taker<GameObject> {
				public Taker<BaseMonoBType> coll;
				void Taker<GameObject>.Take(GameObject item) {
					coll.Take(item.GetComponent<BaseMonoBType>());
				}
				void Taker<GameObject>.None() {
					coll.None();
				}
			}
			void AssetReferInterface.PickContent<ElementType>(string path, Taker<ElementType> collector) {
				if (string.IsNullOrEmpty(path)) {
					if (typeof(ElementType) == typeof(BaseMonoBType)) {
						basicAssetIO.assetOut.PickAssetAtPath(parent.mainAssetName, new StdBridgeBasicColl<GameObject> { collector = new PrvtMainColl { coll = new ToBaseTaker<BaseMonoBType, ElementType> { baseTaker = collector } } });
					}else if(typeof(MonoBehaviour) == typeof(GameObject)) {
						basicAssetIO.assetOut.PickAssetAtPath(parent.mainAssetName, new StdBridgeBasicColl<ElementType> { collector = collector});
					}
					else if (typeof(MonoBehaviour) == typeof(ElementType)) {
						basicAssetIO.assetOut.PickAssetAtPath("Dependencies.json", new StdBridgeBasicColl<ElementType> { collector = collector });
					} else {
						collector.None();
					}
				}
				else {
					basicAssetIO.assetOut.PickAssetAtPath(path, new StdBridgeBasicColl<ElementType> { collector = collector });
				}
			}
		}
	}
	public class JsonAUICustomizer<MainContentType, MainContentBaseType> : ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> {
		AssetUnitInterface ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO>.PickBestElement(AssetUnitBasicIO key) {
			return new PrvtAUInterface { basicAssetIO = key };
		}
		public class PrvtAUInterface : AssetUnitInterface, AssetReferInterface, AssetModifyInterface {
			public AssetUnitBasicIO basicAssetIO;

			AssetReferInterface AssetUnitInterface.referer => this;

			AssetModifyInterface AssetUnitInterface.modifier => this;

			AssetUnitInfo AssetUnitInterface.baseAssetInfo => basicAssetIO.baseAssetUnitInfo;

			void AssetReferInterface.PickContent<ContentType>(string path, Taker<ContentType> collector) {
				var tempPath = string.IsNullOrEmpty(path) ? "Main.json" : path;
				if (typeof(ContentType) == typeof(MainContentType)) {
					basicAssetIO.assetOut.PickAssetAtPath(tempPath, new StdBridgeBasicColl<ContentType> { collector = collector });
				}
				else if (typeof(ContentType) == typeof(MainContentBaseType)) {
					basicAssetIO.assetOut.PickAssetAtPath(tempPath, new StdBridgeBasicColl<MainContentType> { collector = new ToBaseTaker<MainContentType, ContentType> { baseTaker = collector } });
				}
			}

			void AssetModifyInterface.SetContent<ContentType>(AssetContentSettingParam<ContentType> setParam, AssetInResultListener<ContentType> listener) {
				if (string.IsNullOrEmpty(setParam.contentPath)) {
					if ((typeof(MainContentBaseType) == typeof(ContentType))) {
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<ContentType> {
							content = setParam.content,
							contentPath = "Main.json",
							doOverwrite = setParam.doOverwrite
						}, listener);
					}
					else if(typeof(MainContentType) == typeof(ContentType)) {
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<ContentType> {
							content = setParam.content,
							contentPath = "Main.json",
							doOverwrite = setParam.doOverwrite
						}, listener);
					}
					else if(typeof(byte[]) == typeof(ContentType)){
						basicAssetIO.assetIn.SetContent(new BasicAssetInParam<MainContentType> {
							content = RequiredFuncs.FromJson<MainContentType>(Encoding.UTF8.GetString((byte[])(object)setParam.content )),
							contentPath = "Main.json",
							doOverwrite = setParam.doOverwrite
						}, new StubAssetInResultListener<MainContentType>());
					}
				}
				else {
					basicAssetIO.assetIn.SetContent(new BasicAssetInParam<ContentType> {
						content = setParam.content,
						contentPath = setParam.contentPath,
						doOverwrite = setParam.doOverwrite
					}, listener);
				}
			}
		}
	}
	#endregion
	#region asset implementers
	public interface AssetImplementCustomizer<ImplementType> {
		List<ImplementType> implementedAssets { get; }
		bool MatchAsset(ImplementType implementedAsset, GrammarBlock gBlock);
		void EditAssetRequestFromGBlock(GrammarBlock gBlock, AssetRequestUnit assetReqUnit);
		ImplementType IntegrateImplementType<AssetType>(AssetType asset, AssetUnitInfo gBlock, AssetRequestUnit assetReqUnit = null);
		AssetType ExtractAsset<AssetType>(ImplementType implementedAsset);
	}
	[System.Serializable]
	public class NamedAsset<AssetType> {
		public string name;
		public AssetType asset;
	}
	[System.Serializable]
	public abstract class SimpleAssetImplementCustomizer<AssetType, ImplType> : AssetImplementCustomizer<ImplType> where ImplType : NamedAsset<AssetType> {
		public string assetType;
		public List<ImplType> _namedAssets;

		List<ImplType> AssetImplementCustomizer<ImplType>.implementedAssets => _namedAssets;
		void AssetImplementCustomizer<ImplType>.EditAssetRequestFromGBlock(GrammarBlock gBlock, AssetRequestUnit assetReqUnit) {
			assetReqUnit.assettype = assetType;
			if (gBlock.unit.word == null) {
				assetReqUnit.attributes.Add(gBlock.unit.word);
			}
		}

		ParamAssetType AssetImplementCustomizer<ImplType>.ExtractAsset<ParamAssetType>(ImplType implementedAsset) {
			if (typeof(ParamAssetType) == typeof(AssetType)) {
				return (ParamAssetType)(object)implementedAsset.asset;
			}
			return default(ParamAssetType);
		}

		public abstract ImplType IntegrateImplementType<ParamAssetType>(ParamAssetType asset, AssetUnitInfo auInfo, AssetRequestUnit assetReqUnit);

		bool AssetImplementCustomizer<ImplType>.MatchAsset(ImplType implementedAsset, GrammarBlock gBlock) {
			if (gBlock.unit != null) {
				return implementedAsset.name.Equals(gBlock.unit.word, StringComparison.CurrentCultureIgnoreCase);
			}
			return false;
		}
	}
	public class StdAssetImplementer<AssetType, ImplementType> : AssetImplementer<AssetType> {
		public AssetImplementCustomizer<ImplementType> customizer;
		IEnumerable<AssetType> AssetImplementer<AssetType>.implementedAssets {
			get {
				if (convertingEnum == null) {
					convertingEnum = new ConvertingEnumarable<AssetType, ImplementType> { convertFunc = (elem) => customizer.ExtractAsset<AssetType>(elem), sourceEnumerable = customizer.implementedAssets };
				}
				return convertingEnum;
			}
		}
		ConvertingEnumarable<AssetType, ImplementType> convertingEnum;
		public class PrvtinterfaceColl : Taker<AssetUnitInterface> {
			public AssetRequestUnit sourceRequest;
			public Taker<AssetType> externalTaker;
			public AssetImplementCustomizer<ImplementType> customizer;
			public List<ImplementType> assetList;
			void Taker<AssetUnitInterface>.Take(AssetUnitInterface newElement) {
				newElement.referer.PickContent("", new PrvtContentColl { customzier = customizer, externalTaker = externalTaker, sourceRequest = sourceRequest });
			}
			void Taker<AssetUnitInterface>.None() {
				externalTaker.None();
			}
		}
		public class PrvtContentColl : Taker<AssetType> {
			public AssetUnitInfo sourceAUInfo;
			public AssetRequestUnit sourceRequest;
			public Taker<AssetType> externalTaker;
			public AssetImplementCustomizer<ImplementType> customzier;
			void Taker<AssetType>.Take(AssetType newElement) {
				customzier.implementedAssets.Add(customzier.IntegrateImplementType(newElement, sourceAUInfo, sourceRequest));
				externalTaker.Take(newElement);
			}
			void Taker<AssetType>.None() {
				externalTaker.None();
			}
		}
		void AssetImplementer<AssetType>.SeekAsset(GrammarBlock gBlock, AssetSeekSupportListener<AssetType> listener) {
			var found = (this as AssetImplementer<AssetType>).PickImplementedAsset(gBlock);
			if (found != null) {
				listener.collectorOnImplement.Take(found);
                return;
			}
			var newAssetReqUnit = new AssetRequestUnit { };
			customizer.EditAssetRequestFromGBlock(gBlock, newAssetReqUnit);
			listener.auInterfaceGiver.Give(
				newAssetReqUnit,
				new PrvtinterfaceColl { externalTaker = listener.collectorOnImplement, customizer = customizer }
			);
		}
		void AssetImplementer<AssetType>.ImplementAsset(AssetImplementParam param, SimpleProcessListener listener) {
			param.auInterface.referer.PickContent("", new PrvtContentColl { customzier = customizer, externalTaker = null, sourceRequest = param.assetRrequest, sourceAUInfo = param.auInterface.baseAssetInfo });
			listener.OnFinish(true);
		}

		AssetType AssetImplementer<AssetType>.PickImplementedAsset(GrammarBlock gBlock) {
			var found = customizer.implementedAssets.Find((elem) => customizer.MatchAsset(elem, gBlock));
			if (found != null) {
				return customizer.ExtractAsset<AssetType>(found);
			}
			return default(AssetType);
		}
	}
	#endregion
	/*public abstract class MonoBSingleAssetGiver<AssetType, SerializedAssetType, AssetRefType> : MonoBAssetInerfaceGiver where AssetRefType: CommonAssetRef {
		public class PrvtAssetGiver : GeneralGiver<GeneralGiver<string>> {
			class PrimitiveAssetProcessor : FormalAssetProcessor<AssetRefType, AssetType> {
				public override void CreateAssetUsingAssetRef(AssetRefType assetRef, Taker<AssetType> collector) {
					var productTaker = new ToBaseTaker<SerializedAssetType, AssetType> { originalListener = collector };
					contentGiver.PickBestElement(assetRef.mainAssetPath, productTaker);
				}
			}
			void GeneralGiver<GeneralGiver<string>>.PickBestElement<ElementType>(GeneralGiver<string> contentGiver, Taker<ElementType> collector) {
				if (typeof(ElementType) != typeof(AssetType)) {
					//not supported
					collector.OnFinish();
					return;
				}
				var productTaker = new ToBaseTaker<AssetType, ElementType> { originalListener = collector };
				AssetUtils.PickAssetRef(contentGiver, new PrimitiveAssetProcessor { collector = productTaker, contentGiver = contentGiver });
			}
		}
	}*/
}
