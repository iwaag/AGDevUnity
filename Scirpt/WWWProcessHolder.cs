using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WWWProcessHolder : MonoBehaviour {
	public void BeginWWWProcess(UnityWebRequest www, System.Action<UnityWebRequest> onComplete) {
		StartCoroutine(ProcessWebRequestCo(www, onComplete));
	}
    public void BeginWebRequestProcess(UnityWebRequest webReq, System.Action<UnityWebRequest> onComplete) {
        StartCoroutine(ProcessWebRequestCo(webReq, onComplete));
    }
    public IEnumerator ProcessWebRequestCo(UnityWebRequest www, System.Action<UnityWebRequest> onComplete) {
        yield return www.SendWebRequest();
		if (www.error != null)
			Debug.Log(www.error);
		onComplete(www);
    }
}
