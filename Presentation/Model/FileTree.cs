

using System.Collections.ObjectModel;

namespace Presentation.Model
{
    public class FileTree
    {
        public Node Root { get; }

        public FileTree(Core.Models.FileTree tree)
        {
            var rootNode = tree.Root;
            Root = new Node(rootNode.Name, rootNode.Length, 0, rootNode.IsDirectory);
            if (tree.Root.Children != null)
            {
                SetChilds(rootNode, Root);
            }
        }

        private void SetChilds(Core.Models.Node node, Node dtoNode)
        {
            if (node.Children != null)
            {
                dtoNode.Children = new ObservableCollection<Node>();
                foreach (var child in node.Children)
                {
                    double sizeInPercent = node.Length == 0? 0 : (double)child.Length/ (double)node.Length * 100;

                    Node newNode = new Node(child.Name, child.Length, sizeInPercent, child.IsDirectory);
                    if (child.Children != null)
                    {
                        SetChilds(child, newNode);
                    }
                    dtoNode.Children.Add(newNode);

                }
            }            
        }
    }
}
