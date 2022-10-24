namespace Core.Models
{
    public class Node
    {
        public string FullName { get; }
        public string Name { get; }
        public long Lenght { get; set; }
        public bool IsDirectory { get; }
        public List<Node>? Childs { get; set; }

        public Node(string fullName, string name, bool isDirectory = false)
        {
            FullName = fullName;
            Name = name;
            IsDirectory = isDirectory;
        }

        public Node(string fullName, string name, long lenght) : this(fullName, name)
        {
           Lenght = lenght;
        }

        public Node(string fullName, string name, List<Node>? childs) : this(fullName, name, true)
        {
            Childs = childs;
        }
    }
}
