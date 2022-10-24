namespace Core.Models
{
    public class FileTree
    {
        public Node Root { get; }
        public FileTree(Node root)
        {
            Root = root;
            Root.Lenght = GetLength(root);
        }

        private long GetLength(Node node)
        {
            if (node.Childs == null)
            {
                return node.Lenght;
            }

            foreach (var child in node.Childs)
            {
                node.Lenght += GetLength(child);
            }
            return node.Lenght;
        }
    }
}
