using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public int width;
	public int height;
	public int xGap;
	public int YGap;

	public Material startMat;
	public Material pathMat;
	public Material endMat;
	public Material obstacleMat;
	public Material normalMat;

	private GameObject[,] grids;

	private Node startNode;
	private Node endNode;


	void Awake() {
		AStarMgr.Instance.Init(width, height);
	}

	void Start() {
		grids = new GameObject[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				grids[i, j] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				grids[i, j].layer = LayerMask.NameToLayer("Grid");
				grids[i, j].transform.SetParent(transform);
				grids[i, j].transform.localPosition = new Vector3(i * xGap, j * YGap, 0);
				Material mat = null;
				switch (AStarMgr.Instance.Map[i, j].nodeType) {
					case ENodeType.Obstacle:
						mat = obstacleMat;
						break;
					case ENodeType.None:
						mat = normalMat;
						break;
				}
				grids[i, j].GetComponent<MeshRenderer>().material = mat;
			}
		}
	}

	void Update() {
		if (Input.GetMouseButtonDown(0) && startNode == null) {
			startNode = MouseClickGetNode(startMat);
		}
		if (Input.GetMouseButtonDown(1) && endNode == null) {
			endNode = MouseClickGetNode(endMat);
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			Debug.Log("Press R");
			if (startNode != null && endNode != null) {
				Debug.Log("开始寻路");
				List<Node> path = AStarMgr.Instance.FindPath(startNode, endNode);
				Debug.Log("寻路完毕");
				if (path != null) {
					Debug.Log(path.Count);
					for (int i = 0; i < path.Count; i++) {
						if (path[i] == startNode || path[i] == endNode) continue;
						this.grids[path[i].X, path[i].Y].GetComponent<MeshRenderer>().material = pathMat;
					}
				} else Debug.Log("路径为null");
			} else {
				Debug.Log("startNode or endNode may be null");
			}
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			AStarMgr.Instance.RefreshMap();
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					Material mat = null;
					switch (AStarMgr.Instance.Map[i, j].nodeType) {
						case ENodeType.Obstacle:
							mat = obstacleMat;
							break;
						case ENodeType.None:
							mat = normalMat;
							break;
					}
					grids[i, j].GetComponent<MeshRenderer>().material = mat;
				}
			}
			startNode = null;
			endNode = null;
		}
	}

	private Node MouseClickGetNode(Material mat) {
		RaycastHit hitInfo;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, LayerMask.GetMask("Grid"))) {
			for (int i = 0; i < grids.GetLength(0); i++) {
				for (int j = 0; j < grids.GetLength(1); j++) {
					if (grids[i, j] == hitInfo.collider.gameObject) {
						hitInfo.collider.GetComponent<MeshRenderer>().material = mat;
						return AStarMgr.Instance.Map[i, j];
					}
				}
			}
		}
		return null;
	}
}
