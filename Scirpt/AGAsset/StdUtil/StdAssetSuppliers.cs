using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using AGDevUnity;
using AGDev;
using UnityEngine.Networking;
using AGDevUnity.StdUtil;

namespace AGAsset.StdUtil {
	#region asset supplier
	//supply from web
	[System.Serializable]
	public class HTMLAssetSupplier : AssetSupplier {
		public string serverURL;
		public List<AssetSupplyListener> supLis = new List<AssetSupplyListener>();
		public InterceptAcceptHelper inteceptHelper = new InterceptAcceptHelper();
		void AssetSupplier.SupplyAsset(AssetRequest assetRequest, AssetSupplyListener listener) {
			Debug.Log("Begin fetching assets."); 
			inteceptHelper.OnBeginSession(listener);
			var assetRequestJson = JsonUtility.ToJson(assetRequest);
			Debug.Log("Forward requst to Web.... " + assetRequestJson);
			WWWForm wwwForm = new WWWForm();
			wwwForm.AddField("assetRequestInJson", assetRequestJson);
			UnityWebRequest www = UnityWebRequest.Post(serverURL, wwwForm);
			NetworkUtil.ProcessWebRequest(www, (givenWebReq) => {
				bool didSuccess = false;
				if (givenWebReq.isNetworkError || givenWebReq.isHttpError) {
					Debug.Log(givenWebReq.error);
				} else {
					Debug.Log(Encoding.UTF8.GetString(givenWebReq.downloadHandler.data));
					var assetPick = RequiredFuncs.FromJson<AssetPick>(givenWebReq.downloadHandler.data);
					if (assetPick != null) {
						if (assetPick.units.Count > 0) {
							didSuccess = true;
							listener.supplyTaker.Take(assetPick);
						}
					}
				}
				if(!didSuccess)
					listener.supplyTaker.None();
				inteceptHelper.OnEndSession(listener);
			}
			);
			
			
		}
	}
	//supply from saved asset pick
	public class SpecificAssetPickSupplier : AssetSupplier {
		public AssetPick assetPick;
		void AssetSupplier.SupplyAsset(AssetRequest assetRequest, AssetSupplyListener listener) {
			listener.supplyTaker.Take(assetPick);
		}
	}
	//supply by integating assets together
	public class AssetUnitIntegrateSupplier : AssetUnitSupplier {
		public AssetUnitIntegrator Integrator;
		public ImmediateGiver<AssetUnitInterface, AssetUnitInfo> generatedAUInterfaceGiver;
		public MonoBAssetBasicIO generatedIO;
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			var myListner = new PrvtAssetIntegrateListener {
				clientListener = listener,
				assetRequest = assetRequest,
				parent = this
			};
			Integrator.IntegrateAssetUnit(assetRequest, myListner);
            if (!myListner.didBegin) {
                listener.supplyTaker.None();
            }
		}
		public class PrvtAssetIntegrateListener : AssetUnitIntegrateListener {
			public AssetUnitSupplyListener clientListener;
			public AssetRequestUnit assetRequest;
			public AssetUnitIntegrateSupplier parent;
			public PrvtSupport support;
            public bool didBegin = false;
			public class PrvtSupport : AssetUnitIntegrateSupport {
				public AssetUnitInterface generatedAssetItfc;
				public PrvtAssetIntegrateListener parent;
				AssetUnitInterface AssetUnitIntegrateSupport.generatedAssetInterface => generatedAssetItfc;
				Giver<AssetUnitInterface, AssetRequestUnit> AssetUnitIntegrateSupport.integrantGiver => parent.clientListener.integrantGiver;
				void AssetUnitIntegrateSupport.OnSucceed() {
					parent.clientListener.supplyTaker.Take(generatedAssetItfc.baseAssetInfo);
				}

				void AssetUnitIntegrateSupport.OnFail() {
                    parent.clientListener.supplyTaker.None();
				}
			}
			AssetUnitIntegrateSupport AssetUnitIntegrateListener.OnBeginIntegrate() {
                didBegin = true;
                var generatedAssetInfo = AssetUtils.GenerateNewAssetTemplate(assetRequest.attributes[0], assetRequest.assettype);
				generatedAssetInfo.reference = parent.generatedIO.assetIO.LocalizedAssetRef(generatedAssetInfo);
				generatedAssetInfo.distributor = "Integrator";
				var generatedAssetItfc = parent.generatedAUInterfaceGiver.PickBestElement(generatedAssetInfo);
				support = new PrvtSupport { parent = this, generatedAssetItfc = generatedAssetItfc };
				return support;
			}

			
		}
	}
	//seek integrants inside self recursively
#if false
	public class RecursiveAssetUnitSupplier : AssetUnitSupplier {
		public AssetUnitSupplier integrantSupplier;
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			integrantSupplier.SupplyAssetUnit(assetRequest, new RecursiveUnitSupplyListener { integrantSupplier = integrantSupplier, clientListener = listener });
		}
	}
	public class RecursiveUnitSupplyListener : AssetUnitSupplyListener, Taker<RequestAndTaker<AssetUnitInfo>> {
		bool collected = false;
		public AssetUnitSupplyListener clientListener;
		public AssetUnitSupplier integrantSupplier;
		Taker<AssetUnitInfo> AssetUnitSupplyListener.supplyTaker {
			get { return clientListener.supplyTaker; }
		}

		void Taker<RequestAndTaker<AssetUnitInfo>>.Take(RequestAndTaker<AssetUnitInfo> reqColl) {
			integrantSupplier.SupplyAssetUnit(reqColl.requestUnit, new RecursiveIntegrantUnitSupplyListener { integrantReqColl = reqColl, integrantSupplier = integrantSupplier, rootClientListener = clientListener });
		}

		void AsyncProcess.OnFinish() {
		}
		Taker<RequestAndTaker<AssetUnitInfo>> AssetUnitSupplyListener.OnRequestIntegrants() {
			return this;
		}
		class RecursiveIntegrantUnitSupplyListener : AssetUnitSupplyListener, Taker<AssetUnitInfo>, Taker<RequestAndTaker<AssetUnitInfo>> {
			public bool integrantRequestConcluded = false;
			public bool assetSupplyConcluded = false;
			public bool collected = false;
			public RequestAndTaker<AssetUnitInfo> integrantReqColl;
			public AssetUnitSupplyListener rootClientListener;
			public AssetUnitSupplier integrantSupplier;
			Taker<AssetUnitInfo> AssetUnitSupplyListener.supplyTaker {
				get { return this; }
			}

			void Taker<RequestAndTaker<AssetUnitInfo>>.Take(RequestAndTaker<AssetUnitInfo> subReqColl) {
				integrantSupplier.SupplyAssetUnit(subReqColl.requestUnit, new RecursiveIntegrantUnitSupplyListener {
					integrantReqColl = subReqColl,
					integrantSupplier = integrantSupplier,
					rootClientListener = rootClientListener
				});
			}

			void Taker<RequestAndTaker<AssetUnitInfo>>.OnFail(string reason) {
				(this as Taker<RequestAndTaker<AssetUnitInfo>>).OnFinish();
			}

			void AsyncProcess.OnFinish() {
				integrantRequestConcluded = true;
			}

			void AsyncProcess.OnFinish() {
				assetSupplyConcluded = true;
				if (!collected) {
					var reqCollColl = rootClientListener.OnRequestIntegrants();
					reqCollColl.Take(integrantReqColl);
					reqCollColl.OnFinish();
				} else {
					integrantReqColl.collector.OnFinish();
				}
			}

			void Taker<AssetUnitInfo>.OnFail(string reason) {
				(this as Taker<AssetUnitInfo>).OnFinish();
			}

			void Taker<AssetUnitInfo>.Take(AssetUnitInfo newElement) {
				assetSupplyConcluded = true;
				collected = true;
				integrantReqColl.collector.Take(newElement);
			}
		}
	}
#endif
#if false
	public class AssetUnitImplementSupplier : AssetUnitSupplier {
		public AssetUnitSupplier unitSupplier;
		public Dictionary<string, AssetImplementer> implementerDict;
		public ImmediateGiver<AssetUnitInterface, AssetUnitInfo> assetItfcGiver;
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			unitSupplier.SupplyAssetUnit(assetRequest,
				new SupplyIntegratingListener {
					assetItfcGiver = assetItfcGiver,
					assetRequest = assetRequest,
					integratorDict = implementerDict,
					clientListener = listener
				}
			);
		}
		class SupplyIntegratingListener : AssetUnitSupplyListener, Taker<AssetUnitInfo>, assetimple {
			public AssetRequestUnit assetRequest;
			public ImmediateGiver<AssetUnitInterface, AssetUnitInfo> assetItfcGiver;
			public Dictionary<string, AssetImplementer> integratorDict;
			public AssetUnitSupplyListener clientListener;
			bool collected = false;
			bool finished = false;
			Taker<AssetUnitInfo> AssetUnitSupplyListener.supplyTaker {
				get { return this; }
			}
			Taker<RequestAndTaker<AssetUnitInfo>> AssetUnitSupplyListener.OnRequestIntegrants() {
				return clientListener.OnRequestIntegrants();
			}
			void Taker<AssetUnitInfo>.Take(AssetUnitInfo collectedInfo) {
				collected = true;
				foreach (var type in AssetUtils.ExtractPackedAssetAttribute(collectedInfo.assettype)) {
					AssetImplementer implementer = null;
					integratorDict.TryGetValue(type, out implementer);
					if (implementer != null) {
						implementer.ImplementAsset(new AssetImplementParam {
							assetUnitInfo = collectedInfo,
							assetInterfaceGiver = assetItfcGiver,
							request = assetRequest
						}, this);
					} else {
						Debug.LogError("No Implementer for: " + type);
					}
				}
			}

			void AsyncProcess.OnFinish() {
				if (!collected)
					clientListener.supplyTaker.OnFinish();
				//if collected, finish after integrated
			}

			void AssetImplementListener.OnSucceed() {
				//prevent multi Finish() call: finish on first integration
				if (!finished)
					clientListener.supplyTaker.OnFinish();
				finished = true;
			}

			void AssetImplementListener.OnFail(string reason) {
				clientListener.supplyTaker.OnFail("Integration failed: " + reason);
			}
		}
	}
#endif
	//teamed-up suppliers
	public class AssetUnitSupplierLineup : AssetUnitSupplier {
		public IEnumerable<AssetUnitSupplier> suppliers = new List<AssetUnitSupplier>();
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			var enumarator = suppliers.GetEnumerator();
			if (enumarator.MoveNext()) {
				enumarator.Current.SupplyAssetUnit(assetRequest, new PrvtLis { clientListener = listener, suppliers = enumarator, assetRequest = assetRequest });
			} else {
				listener.supplyTaker.None();
			}
		}
		class PrvtLis : AssetUnitSupplyListener, Taker<AssetUnitInfo> {
			public AssetUnitSupplyListener clientListener;
			public AssetRequestUnit assetRequest;
			public IEnumerator<AssetUnitSupplier> suppliers;

			Taker<AssetUnitInfo> AssetUnitSupplyListener.supplyTaker => this;

			Giver<AssetUnitInterface, AssetRequestUnit> AssetUnitSupplyListener.integrantGiver => clientListener.integrantGiver;

			void OnPass() {
				if(suppliers.MoveNext()){
					suppliers.Current.SupplyAssetUnit(assetRequest, this);
				}
				else{
					clientListener.supplyTaker.None();
				}
			}

			void Taker<AssetUnitInfo>.None() {
				OnPass();
			}

			void Taker<AssetUnitInfo>.Take(AssetUnitInfo newElement) {
				clientListener.supplyTaker.Take(newElement);
			}
		}
	}
	//record all requests
	public class ReqCollectingUnitAssetSupplier : AssetUnitSupplier {
		public AssetUnitSupplier clientSup;
		public AssetRequestHolder req;
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			req.AddOrMergeRequest_GetAdded(assetRequest);
			clientSup.SupplyAssetUnit(assetRequest, listener);
		}
	}
	[System.Serializable]
	public class BridgeUnitAssetSupplier : AssetUnitSupplier, Gate {
		#region GateProcess
		public bool isGateOpen = false;
		bool Gate.isOpen => isGateOpen;
		public BridgeUnitAssetSupplier _bridgeSupplier;
		void Gate.Close() {
			isGateOpen = false;
		}
		void Gate.Open(bool doCloseImmediate) {
			isGateOpen = !doCloseImmediate;
			BeginAssetSupply();
		}
		#endregion
		AssetRequest CopyAssetReq(AssetRequest source) {
			var result = new AssetRequest();
			result.units = new List<AssetRequestUnit>(source.units);
			return result;
		}
		public AssetSupplier supplier;
		public AssetRequest req = new AssetRequest();
		Dictionary<int, AssetUnitSupplyListener> listenerDict = new Dictionary<int, AssetUnitSupplyListener>();
		void AssetUnitSupplier.SupplyAssetUnit(AssetRequestUnit assetRequest, AssetUnitSupplyListener listener) {
			listenerDict[assetRequest.ID] = listener;
			req.units.Add(assetRequest);
			if (isGateOpen)
				BeginAssetSupply();
		}
		public void BeginAssetSupply() {
			if (req.units.Count == 0)
				return;
			var copiedReq = req;
			var copiedListenerDict = listenerDict;
			req = new AssetRequest();
			listenerDict = new Dictionary<int, AssetUnitSupplyListener>();
			supplier.SupplyAsset(copiedReq, new PrvtAssetSupplyListener { listenerDict = copiedListenerDict });
			
		}
		class PrvtAssetSupplyListener : AssetSupplyListener, Taker<AssetPick> {
			public Dictionary<int, AssetUnitSupplyListener> listenerDict;
			Taker<AssetPick> AssetSupplyListener.supplyTaker {
				get { return this; }
			}

			void Taker<AssetPick>.Take(AssetPick newElement) {
				foreach (var pickUnit in newElement.units) {
					listenerDict.TryGetValue(pickUnit.reqID, out var unitListener);
					if (unitListener != null)
						unitListener.supplyTaker.Take(pickUnit.assetInfo.units[0]);
				}
			}
			void Taker<AssetPick>.None() {
				foreach (var listener in listenerDict.Values) {
					listener.supplyTaker.None();
				}
			}
		}
	}
#endregion
}
