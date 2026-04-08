namespace NetOpsConsole;

public class NodeDataFile
{
    public List<Node> Nodes { get; set; } = new();
}
public class Node
{
    public string Id { get; set; }
    public string Address { get; set; }
    public long Ping { get; set; }
    public bool isReachable { get; set; }
}