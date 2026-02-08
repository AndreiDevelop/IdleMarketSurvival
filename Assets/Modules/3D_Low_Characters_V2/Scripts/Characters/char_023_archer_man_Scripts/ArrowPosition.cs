
/*XXXXXXXXXX.XXXXXXXXXX.XXXXXXXXXX.XXXXXXXXXX
 ----------.----------.----------.----------
(c)	Puxxe Studio | 2022
	16/11/2022 (14:08:01)
 ----------.----------.----------.----------
XXXXXXXXXX.XXXXXXXXXX.XXXXXXXXXX.XXXXXXXXXX*/

namespace PuxxeStudio{	

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class ArrowPosition : MonoBehaviour{
		public Transform bowArmature;
		public Transform boneString;
		public Transform handDrag;
		Vector3 boneStringStartPosition;
		float distance;
		public bool dragingArrow = false;
		void Start(){
			if (boneString != null){
				boneStringStartPosition = boneString.position;
				distance  = Vector3.Distance(bowArmature.position, boneString.position);
				print("Distance to other: " + distance);
			}
		}
		void Update(){
			if (bowArmature != null && boneString != null && handDrag != null){
				if (dragingArrow == true){
					boneString.position = new Vector3(handDrag.position.x, handDrag.position.y, handDrag.position.z);
				}else{             
					boneString.position = (boneString.position - bowArmature.transform.position).normalized * distance + bowArmature.transform.position;               
				}
			}
		}
		public void DragArrow(bool value){
			dragingArrow = value;
		}
		public void ReleaseArrow(){
			boneString.position = (boneString.position - bowArmature.transform.position).normalized * distance + bowArmature.transform.position;
		}
	}	
}
