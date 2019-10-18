using AGDev;
using AGBLang;
using AGDev.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang.StdUtil;

public class GrammarBlockVisualizer : MonoBehaviour
{
	public GrammarBlockVisualizeUnit visualizerUnitPrefab;
	public Color modColor;
	public Color nounColor;
	public Color verbColor;
	public Color nominalClusterColor;
	public Color verbalClusterColor;
	public Color svSentenceColor;
	public Color sentenceClusterColor;
	public Color otherClusterColor;
	public Color modifiedBlockColor;
	public GrammarBlockVisualizeUnit CreateVisualizeUnit(GrammarBlock gBlock, GrammarBlock parent = null) {
		var visUnit = Instantiate(visualizerUnitPrefab);
		if(gBlock.unit != null) {
			Destroy(visUnit.horizontalClusterRoot.gameObject);
			Destroy(visUnit.verticalClusterRoot.gameObject);
			if (GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.quoteBlock.word) || GrammarBlockUtils.HasMetaInfo(parent, StdMetaInfos.quoteBlock.word))
				visUnit.word.GetComponentInChildren<UnityEngine.UI.Text>().text = " \"" + gBlock.unit.word + "\" ";
			else
				visUnit.word.GetComponentInChildren<UnityEngine.UI.Text>().text = gBlock.unit.word;
			if (GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.nominalBlock.word) || GrammarBlockUtils.HasMetaInfo(parent, StdMetaInfos.nominalBlock.word)) {
				visUnit.contentBG.color = Color.red;
			}
			if (GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.verbalBlock.word) || GrammarBlockUtils.HasMetaInfo(parent, StdMetaInfos.verbalBlock.word)) {
				visUnit.contentBG.color = Color.blue;
			}
			
		}
		else {
			Destroy(visUnit.word.gameObject);
			UnityEngine.RectTransform clusterRoot = null;
			if (GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.sentenceCluster.word)) {
				Destroy(visUnit.horizontalClusterRoot.gameObject);
				clusterRoot = visUnit.verticalClusterRoot;
				if (parent != null)
					visUnit.contentBG.color = sentenceClusterColor;
				else
					visUnit.contentBG.color = otherClusterColor;
			}
			else {
				Destroy(visUnit.verticalClusterRoot.gameObject);
				clusterRoot = visUnit.horizontalClusterRoot;
				if (GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.nominalBlock.word))
					visUnit.contentBG.color = nominalClusterColor;
				else if (GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.verbalBlock.word))
					visUnit.contentBG.color = verbalClusterColor;
				else if (GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.sv.word) || GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.conditionSV.word))
					visUnit.contentBG.color = svSentenceColor;
				else
					visUnit.contentBG.color = otherClusterColor;
			}
			foreach (var subBlock in gBlock.cluster.blocks) {
				var newSubVisUnit = CreateVisualizeUnit(subBlock, gBlock);
				newSubVisUnit.transform.SetParent(clusterRoot, false);
			}
		}
		if (gBlock.modifier == null) {
			Destroy(visUnit.modifierRoot.gameObject);
		}
		else {
			var modVisUnit = CreateVisualizeUnit(gBlock.modifier);
			modVisUnit.transform.SetParent(visUnit.modifierRoot, false);
			//modVisUnit.transform.localScale = Vector3.Scale(modVisUnit.transform.localScale, Vector3.one * 0.8f);
			//if (gBlock.cluster != null)
			modVisUnit.AddBackGround().color = modColor;
			visUnit.AddBackGround().color = modifiedBlockColor; 
		} 
		if (GrammarBlockUtils.HasMetaInfo(gBlock, StdMetaInfos.conditionSV.word)) {
			visUnit.metaRoot.GetComponentInChildren<UnityEngine.UI.Text>().text = " If ";
		} else {
			Destroy(visUnit.metaRoot.gameObject);
		}
		return visUnit;
	}
}
