using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InstanceBase<T> where T : new() {

	private static T instance;
	public static T Instance {
		get {
			if (instance == null) instance = new T();
			return instance;
		}
	}

}

public class AStarMgr: InstanceBase<AStarMgr> {

	public Node[,] Map { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }



	private List<Node> openList = new List<Node>();
	private List<Node> closeList = new List<Node>();
	private List<Node> path = new List<Node>();


	public void Init(int width, int height) {
		if (width < 0 || height < 0) throw new Exception("Width or Height must be bigger than zero");
		this.Width = width;
		this.Height = height;
		this.Map = new Node[width, height];
		Random r = new Random();
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				this.Map[i, j] = new Node(i, j, r.Next(100) < 30 ? ENodeType.Obstacle : ENodeType.None);
			}
		}
	}

	public void RefreshMap() {
		Random r = new Random();
		for (int i = 0; i < this.Width; i++) {
			for (int j = 0; j < this.Height; j++) {
				this.Map[i, j].nodeType = (r.Next(100) < 30 ? ENodeType.Obstacle : ENodeType.None);
			}
		}
	}

	public List<Node> FindPath(Node start, Node end) {
		path.Clear();
		if (!NodeIsLegal(start.X, start.Y) && !NodeIsLegal(end.X, end.Y)) return null;
		openList.Clear();
		closeList.Clear();
		start.ClearData();
		closeList.Add(start);
		while (true) {
			JudgeAroundNode(start, end);
			if (openList.Count <= 0) return null;
			openList.Sort((a, b) => (int)Math.Ceiling(a.F - b.F));
			start = openList[0];
			openList.RemoveAt(0);
			if (!closeList.Exists(x => x == start)) closeList.Add(start);
			if (start == end) {
				while (end.Parent != null) {
					path.Add(end);
					end = end.Parent;
				}
				path.Add(end);
				break;
			}
		}
		path.Reverse();
		return path;
	}

	private bool NodeIsLegal(int x, int y) {
		if (x >= 0 && x < Width && y >= 0 && y < Height) {
			return this.Map[x, y].nodeType != ENodeType.Obstacle;
		}
		return false;
	}

	private void JudgeAroundNode(Node center, Node end) {
		for (int i = -1; i < 2; i++) {
			for (int j = -1; j < 2; j++) {
				if (i == 0 && j == 0) continue;
				if (NodeIsLegal(center.X + i, center.Y + j) &&
					!this.openList.Contains(this.Map[center.X + i, center.Y + j]) &&
					!this.closeList.Contains(this.Map[center.X + i, center.Y + j])) {
					this.AddNodeToOpenList(this.Map[center.X + i, center.Y + j], end, center);
				}
			}
		}
	}

	private void AddNodeToOpenList(Node node, Node end, Node parent) {
		node.Parent = parent;
		node.G = parent.G + (parent.X == node.X || parent.Y == node.Y ? 1 : 1.4f);
		node.H = Math.Abs(end.X - node.X) + Math.Abs(end.Y - node.Y);
		node.F = node.G + node.H;
		this.openList.Add(node);
	}


}
