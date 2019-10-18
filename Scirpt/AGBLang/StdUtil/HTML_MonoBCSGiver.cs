using AGAsset;
using AGAsset.StdUtil;
using AGDev.StdUtil;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AGDev;
using AGBLang;
using System;

namespace AGDevUnity.StdUtil {
	public class HTML_MonoBCSGiver : MonoBCommonSenseGiver, CommonSenseGiver, Giver<NounCommonSenseUnit, string>, Gate {
		#region GateProcess
		bool Gate.isOpen => isGateOpen;
		public bool isGateOpen = false;
		public BridgeUnitAssetSupplier _bridgeSupplier;
		void Gate.Close() {
			isGateOpen = false;
		}
		void Gate.Open(bool doCloseImmediate) {
			isGateOpen = !doCloseImmediate;
			SendNCSRequest(new StubSimpleProcessListener());
		}
		#endregion
		public string serverURL;
		public NounCommonSenseRequest pendingNCSReq;
		public StoredNounCommonSenseUnit defaultNCSUnit;
		public Dictionary<string, List<Taker<NounCommonSenseUnit>>> pendingNCSTakers = new Dictionary<string, List<Taker<NounCommonSenseUnit>>>();
		public List<StoredNounCommonSenseUnit> cachedNCS;
		public override CommonSenseGiver commonSenseGiver => this;
		Giver<NounCommonSenseUnit, string> CommonSenseGiver.nounCSGiver {
			get {
				if (_nounCSGiver == null) {
					_nounCSGiver = new GiverLineup<NounCommonSenseUnit, string> { subGivers = new List<Giver<NounCommonSenseUnit, string>> { this, new DefaultValueGiver<NounCommonSenseUnit, string>{ defaultValue = defaultNCSUnit } } };
				}
				return _nounCSGiver;
			}
		}
			
        Giver<NounCommonSenseUnit, string> _nounCSGiver;
        void Giver<NounCommonSenseUnit, string>.Give(string key, Taker<NounCommonSenseUnit> collector) {
			pendingNCSReq.units.Add(new NounCommonSenseRequestUnit { word = key });
			if (!pendingNCSTakers.TryGetValue(key, out var collectorList)) {
				collectorList = new List<Taker<NounCommonSenseUnit>>();
				pendingNCSTakers[key] = collectorList;
			}
			collectorList.Add(collector);
			if (isGateOpen)
				SendNCSRequest(new StubSimpleProcessListener());
		}
		public void SendNCSRequest(SimpleProcessListener listener) {
			if (pendingNCSReq.units.Count == 0)
				return;
			var reqInJson = RequiredFuncs.ToJsonString(pendingNCSReq);
			pendingNCSReq = new NounCommonSenseRequest { units = new List<NounCommonSenseRequestUnit>() };
			var copiedTakers = pendingNCSTakers;
			pendingNCSTakers = new Dictionary<string, List<Taker<NounCommonSenseUnit>>>();
			var wwwForm = new WWWForm();
			wwwForm.AddField("ncsReqInJson", reqInJson);
			Debug.Log("CommonSenseReq: " + reqInJson);
			UnityWebRequest www = UnityWebRequest.Post(serverURL + "GetNounCommonSense", wwwForm);
			NetworkUtil.ProcessWebRequest(www, (givenWebReq) => {
				if (www.error != null) {
					listener.OnFinish(false);
					return;
				}
				var ncs = RequiredFuncs.FromJson<StoredNounCommonSense>(www.downloadHandler.text);
				Debug.Log(www.downloadHandler.text);
				foreach (var ncsUnit in ncs.units) {
                    foreach (var pair in copiedTakers) {
                        if (pair.Key.Equals(ncsUnit.noun, StringComparison.CurrentCultureIgnoreCase)) {
                            foreach (var collector in pair.Value)  {
                                cachedNCS.Add(ncsUnit);
                                collector.Take(ncsUnit);
                            }
                        }
                        else {
                            foreach (var collector in pair.Value) {
                                collector.None();
                            }
                        }
                    }
                }
			});
		}
	}
	
	[System.Serializable]
	public class NounCommonSenseRequest {
		public List<NounCommonSenseRequestUnit> units;
	}
	[System.Serializable]
	public class StoredNounCommonSenseUnit : NounCommonSenseUnit {
		public string noun;
		public string give;
		public string givenby;
		public bool isphysical = false;
		IEnumerable<string> NounCommonSenseUnit.give => string.IsNullOrEmpty(give) ? null : give.Split(';');
		IEnumerable<string> NounCommonSenseUnit.givenBy => string.IsNullOrEmpty(givenby) ? null : givenby.Split(';');
		bool NounCommonSenseUnit.isPhysical => isphysical;
	}
	[System.Serializable]
	public class StoredNounCommonSense {
		public List<StoredNounCommonSenseUnit> units;
	}
	[System.Serializable]
	public class NounCommonSenseRequestUnit {
		public string word;
	}
	
}