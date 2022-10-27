﻿namespace Core.Models
{
    public class Node
    {
        public string FullName { get; }
        public string Name { get; }
        public long Length { get; set; }
        public bool IsDirectory { get; }
        public List<Node>? Children { get; set; }

        public Node(string fullName, string name, bool isDirectory = false)
        {
            FullName = fullName;
            Name = name;
            IsDirectory = isDirectory;
        }

        public Node(string fullName, string name, long length) : this(fullName, name)
        {
           Length = length;
        }

        public Node(string fullName, string name, List<Node>? children) : this(fullName, name, true)
        {
            Children = children;
        }
    }
}
