using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
namespace AGDevUnity {
	public abstract class MonoBSpaceDistributer : MonoBehaviour {
		public abstract SpaceDistributer spaceDistributer { get; }
	}
	public interface SpaceInfoListener {
		void OnSpaceUpdated(BAgentSpace spaceInfo);
	}
	public interface BAgentSpace {
		Transform origin { get; }
		Vector3 lowerBoundary { get; }
		Vector3 upperBoundary { get; }
		void AcceptBehaverAgent(BehaverAgent behaver);
		void Clear();
	}
	public struct SpaceFillInfo {
		public float contentScaling;
		public Vector3Int division;
	}
	public struct SpaceFillSetting {
		public Vector3 contentSize;
		public Vector3 spaceSize;
		public int contentCount;
	}
	public static class SpaceUtilities {
		public static Vector3 GetFillerPosition(BAgentSpace space, SpaceFillInfo fillInfo, Vector3 contenstSize, int index) {
			Vector3Int matrixIndex = Vector3Int.one;
			matrixIndex.z = index / (fillInfo.division[0] * fillInfo.division[1]);
			matrixIndex.y = (index - matrixIndex.z) / fillInfo.division[0];
			matrixIndex.x = index % fillInfo.division[0];
			var positionIndex = (matrixIndex + Vector3.one * 0.5f);
			for(int i = 0; i<3; i++) {
				positionIndex[i] = space.upperBoundary[i] - space.lowerBoundary[i] == 0 ? 0 : positionIndex[i];
			}
			/*var result =  space.origin.localToWorldMatrix * Vector3.Scale(
				Vector3.Scale(space.lowerBoundary, Vector3.Scale(contenstSize, fillInfo.contentScaling * (Vector3.one - positionIndex))	)
				+ Vector3.Scale(space.upperBoundary, Vector3.Scale(contenstSize, fillInfo.contentScaling * positionIndex))
				, new Vector3(-1, 1, 1));*/
			var result = space.origin.TransformPoint( Vector3.Scale(
				space.lowerBoundary + Vector3.Scale(contenstSize, fillInfo.contentScaling * positionIndex )
				, new Vector3(-1, 1, 1))
			);
			return result;
		}
		public static float GetLooseFittingScaling(Vector3 contentSize, Vector3 containerSize, out int fitAxisIndex) {
			var newScale = new Vector3(containerSize.x / contentSize.x, containerSize.y / contentSize.x, containerSize.z / contentSize.x);
			fitAxisIndex = 0;
			if (newScale.x > newScale.y && newScale.z > newScale.y && newScale.y != 0) {
				fitAxisIndex = 1;
				return newScale.y;
			} else if(newScale.y > newScale.x && newScale.z > newScale.x && newScale.x != 0) {
				fitAxisIndex = 0;
				return newScale.x;
			} else if (newScale.x > newScale.z && newScale.y > newScale.z && newScale.z != 0) {
				fitAxisIndex = 2;
				return newScale.z;
			}
			return 1;
		}
		public static Vector3 FaceUpOnSurface(Vector3 source) {
			return Quaternion.AngleAxis(90, Vector3.right) * source;
		}
		public static SpaceFillInfo ScaleToFill(SpaceFillSetting setting) {
			var result = new SpaceFillInfo();
			var division = Vector3Int.one;
			result.division = division;
			// max size case
			var looseFitScale = GetLooseFittingScaling(setting.contentSize, setting.spaceSize, out int fitAxisIndex);
			int looseFillCount = 1;
			for (int i = 0; i<3; i++) {
				int cellCount = Mathf.FloorToInt((setting.spaceSize[i] / (setting.contentSize[i] * looseFitScale)));
				if (cellCount <= 0)
					cellCount = 1;
				division[i] = cellCount;
				looseFillCount *= cellCount;
			}
			result.contentScaling = looseFitScale;
			int actualFillCount = looseFillCount;
			while (actualFillCount < setting.contentCount) {
				float newContentScale = -1;
				int increaseIndex = -1;
				for (int i = 0; i < 3; i++) {
					float decreasedContentSizeInAxis = setting.spaceSize[i] / (division[i] + 1);
					float decreasedFitScale = decreasedContentSizeInAxis / setting.contentSize[i];
					if (newContentScale < decreasedFitScale) {
						newContentScale = decreasedFitScale;
						increaseIndex = i;
					} 
				}
				result.contentScaling = newContentScale;
				division[increaseIndex]++;
				actualFillCount = division[0] * division[1] * division[2];
				result.division = division;
			}
			return result;
		}
	}
	public interface SpaceDistributer {
		void DistributeSpaceToBehave(GameObject behaver, BehaviorExpression behavior, SpaceInfoListener listener);
		IEnumerable<BAgentSpace> spaces { get; }
	}
}