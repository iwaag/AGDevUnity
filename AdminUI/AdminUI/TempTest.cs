using AGBLang;
using AGDev;
using AGDevUnity.StdUtil;
using UnityEngine;

public class TempTest : MonoBehaviour, AGDev.Taker<GrammarBlock> {
    public StdMonoBGeneralPanel pannel;
    void AGDev.Taker<GrammarBlock>.Take(GrammarBlock item) {
        pannel.panel.ViewGBLock(item);
    }
	void Taker<GrammarBlock>.None() {
		Debug.Log("TempTest:None");
	}
	// Start is called before the first frame update
	void Start()  {
		//FindObjectOfType<HTML_MonoBLProcessor>().uri = "http://localhost:56721/Home/Process";
		FindObjectOfType<HTML_MonoBLProcessor>().uri = "http://localhost:7071/api/ProcessNLang";
		FindObjectOfType<HTML_MonoBLProcessor>().LProcessor.PerformSyntacticProcess("If the cards do not match, they are both turned back face down.", this);
    }

    // Update is called once per frame
    void Update()  {
        
    }

	
}
