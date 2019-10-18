using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeekPicture : MonoBehaviour
{
	public string imageName;
	private void Start() {
		if (!string.IsNullOrEmpty(imageName)) {
			FindObjectOfType<AGAssetModules>().systemAssetInterface.auInterface.referer.PickContent(imageName, new PrvtColl { image = GetComponent<Image>() });
		}
	}
	class PrvtColl : Taker<Texture2D> {
		public UnityEngine.UI.Image image;
		void Taker<Texture2D>.Take(Texture2D item) {
			var sprite = Sprite.Create(item, new Rect(0, 0, item.width, item.height), Vector2.one * 0.5f, item.width / 50.0f);
			image.sprite = sprite;
		}
		void Taker<Texture2D>.None() {}
	}
}
