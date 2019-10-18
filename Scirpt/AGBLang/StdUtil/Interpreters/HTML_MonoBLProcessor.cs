using AGAsset.StdUtil;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using AGBLang;
using AGDev;
using AGBLang.StdUtil;

namespace AGDevUnity.StdUtil {
	public class ClusterModuleInterceptListener : InterceptListener {
		public List<InterceptListener> subListeners = new List<InterceptListener>();
		void InterceptListener.OnAllProcessDone() {
			foreach (var listener in subListeners) {
				listener.OnAllProcessDone();
			}
		}

		void InterceptListener.OnBeginWorking() {
			foreach (var listener in subListeners) {
				listener.OnBeginWorking();
			}
		}
	}
	public class InterceptAcceptHelper {
		public List<object> sessionObjects = new List<object>();
		public ClusterModuleInterceptListener interceptListeners = new ClusterModuleInterceptListener();
		public void OnBeginSession(object sessionObj) {
			if (sessionObjects.Count == 1)
				(interceptListeners as InterceptListener).OnBeginWorking();
			sessionObjects.Add(sessionObj);
		}
		public void OnEndSession(object sessionObj) {
			sessionObjects.Remove(sessionObj);
			if (sessionObjects.Count == 0)
				(interceptListeners as InterceptListener).OnAllProcessDone();
			
		}
	}
	public class HTML_MonoBLProcessor : MonoBSyntacticProcessor, NaturalLanguageProcessor {
		InterceptAcceptHelper inteceptHelper = new InterceptAcceptHelper();
		public string uri;
		public override NaturalLanguageProcessor LProcessor => this;
        
        public void ListenerState(InterceptListener listener) {
			inteceptHelper.interceptListeners.subListeners.Add(listener);
        }
        
        void NaturalLanguageProcessor.PerformSyntacticProcess(string naturalLanuage, Taker<GrammarBlock> listener) {
			StartCoroutine(RemoteSPCo(naturalLanuage, listener));
		}
		IEnumerator RemoteSPCo(string eWords, Taker<GrammarBlock> listener) {
			inteceptHelper.OnBeginSession(listener);
			var form = new WWWForm();
            bool didCollect = false;
			form.AddBinaryData("ewords", System.Text.Encoding.UTF8.GetBytes(eWords));
			UnityWebRequest www = UnityWebRequest.Post(uri, form);
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError) {
                Debug.Log("HTML_MonoBLProcessor: " + www.error);
			} else {
				Debug.Log( Encoding.UTF8.GetString( www.downloadHandler.data ) );
				var gblock = RequiredFuncs.FromJson<DeserializedGBlock>(www.downloadHandler.data);
				listener.Take(gblock);
                didCollect = true;
            }
            inteceptHelper.OnEndSession(listener);
            if (!didCollect) {
                listener.None();
            }
            
		}
	}
}
