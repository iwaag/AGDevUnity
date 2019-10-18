using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrammarBlockVisualizeUnit : MonoBehaviour {
	public UnityEngine.UI.Image contentBG;
	public UnityEngine.RectTransform metaRoot;
	public UnityEngine.RectTransform modifierRoot;
	public UnityEngine.RectTransform verticalClusterRoot;
	public UnityEngine.RectTransform horizontalClusterRoot;
	public UnityEngine.RectTransform word;
	public List<UnityEngine.UI.Image> backGrounds;
	public UnityEngine.UI.Image AddBackGround() {
		var newBackFround = Instantiate(contentBG, transform);
		newBackFround.transform.SetSiblingIndex(0);
		AddPaddding(10);
		backGrounds.Add(newBackFround);
		return newBackFround;
	}
	public void AddPaddding(int offset) {
		var oldPadding = transform.GetComponent<UnityEngine.UI.LayoutGroup>().padding;
		foreach (var backGround in backGrounds) {
			backGround.rectTransform.sizeDelta -= offset * 2 * Vector2.one;
		}
		transform.GetComponent<UnityEngine.UI.LayoutGroup>().padding = new RectOffset(oldPadding.left + offset, oldPadding.right + offset, oldPadding.top + offset, oldPadding.bottom + offset);
	}
}
