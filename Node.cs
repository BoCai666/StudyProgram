using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENodeType {
	None,
	Obstacle,
}

public class Node {
	public int X { get; set; }
	public int Y { get; set; }
	public float F { get; set; }
	public float G { get; set; }
	public float H { get; set; }
	public Node Parent { get; set; }
	public ENodeType nodeType;

	public Node(int x, int y, ENodeType type = ENodeType.None) {
		this.X = x;
		this.Y = y;
		this.nodeType = type;
	}

	public void ClearData() {
		F = 0;
		G = 0;
		H = 0;
		Parent = null;
	}
}

