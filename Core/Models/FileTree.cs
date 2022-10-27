namespace Core.Models
{
    public class FileTree
    {
        public Node Root { get; }
        public FileTree(Node root)
        {
            Root = root;
            Root.Length = GetLength(root);
        }

        private long GetLength(Node node)
        {
            if (node.Childs == null)
            {
                return node.Length;
            }

            foreach (var child in node.Childs)
            {
                node.Length += GetLength(child);
            }
            return node.Length;
        }
    }
}
