using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace BinarySearchTreeValidator
{
    /// <summary>
    /// Node class for the binary tree
    /// </summary>
    public class TreeNode
    {
        public int Value { get; set; }
        public TreeNode Left { get; set; }
        public TreeNode Right { get; set; }

        public TreeNode(int value)
        {
            Value = value;
            Left = null;
            Right = null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Binary Search Tree Validator");
            Console.WriteLine("----------------------------");

            // Test cases
            string[][] testCases = new string[][] {
                new string[] {"(2,1)", "(4,2)", "(4,7)"},
                new string[] {"(2,1)", "(4,2)", "(7,5)", "(4,7)", "(7,9)"},
                new string[] {"(2,1)", "(2,3)", "(12,2)", "(2,5)"},
                new string[] {"(4,2)", "(5,4)", "(9,5)", "(11,10)", "(9,11)", "(5,8)"}
            };

            // Process each test case
            for (int i = 0; i < testCases.Length; i++)
            {
                Console.WriteLine($"\nTest Case #{i + 1}:");
                Console.WriteLine($"Input: [{string.Join(", ", testCases[i])}]");

                var (isValid, reason) = IsValidBST(testCases[i]);
                Console.WriteLine($"Output: {isValid}");

                if (isValid == "false")
                {
                    Console.WriteLine($"Reason: {reason}");
                }
                else if (isValid == "true" && testCases[i].Length <= 10)
                {
                    // Visual representation for smaller trees
                    Console.WriteLine("\nTree structure:");
                    TreeNode root = BuildTreeForVisualization(testCases[i]);
                    PrintTree(root, 0);
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Determines if the given string array can form a valid Binary Search Tree
        /// </summary>
        static (string isValid, string reason) IsValidBST(string[] input)
        {
            // Extract parent-child pairs from input strings
            var pairs = new List<(int parent, int child)>();

            foreach (string pair in input)
            {
                // Extract integers using regex
                var match = Regex.Match(pair, @"\((\d+),(\d+)\)");
                if (match.Success && match.Groups.Count >= 3)
                {
                    int parent = int.Parse(match.Groups[1].Value);
                    int child = int.Parse(match.Groups[2].Value);
                    pairs.Add((parent, child));
                }
            }

            // Build the tree from pairs
            var nodes = new Dictionary<int, TreeNode>();
            var childNodes = new HashSet<int>(); // Track nodes that are children

            // First pass: Create all nodes
            foreach (var (parent, child) in pairs)
            {
                if (!nodes.ContainsKey(parent))
                {
                    nodes[parent] = new TreeNode(parent);
                }

                if (!nodes.ContainsKey(child))
                {
                    nodes[child] = new TreeNode(child);
                }

                childNodes.Add(child);
            }

            // Find root (node that is not a child of any other node)
            TreeNode root = null;
            foreach (var key in nodes.Keys)
            {
                if (!childNodes.Contains(key))
                {
                    // If there are multiple roots, it's not a valid tree
                    if (root != null)
                    {
                        return ("false", "Multiple roots detected. A valid tree must have exactly one root node.");
                    }
                    root = nodes[key];
                }
            }

            // If no root found, we might have a cycle
            if (root == null)
            {
                return ("false", "No root node found. This indicates a cycle in the tree structure.");
            }

            // Second pass: Connect nodes
            foreach (var (parent, child) in pairs)
            {
                TreeNode parentNode = nodes[parent];
                TreeNode childNode = nodes[child];

                // Determine if child should be left or right based on BST property
                if (child < parent)
                {
                    // Child should be left child
                    if (parentNode.Left != null)
                    {
                        return ("false", $"Node {parent} already has a left child ({parentNode.Left.Value}), but another left child ({child}) was found.");
                    }
                    parentNode.Left = childNode;
                }
                else if (child > parent)
                {
                    // Child should be right child
                    if (parentNode.Right != null)
                    {
                        return ("false", $"Node {parent} already has a right child ({parentNode.Right.Value}), but another right child ({child}) was found.");
                    }
                    parentNode.Right = childNode;
                }
                else
                {
                    return ("false", $"Child value ({child}) equals parent value ({parent}). BST nodes must have unique values.");
                }
            }

            // Verify the resulting tree is a valid BST
            (bool isValid, string invalidReason) = IsValidBSTStructure(root, int.MinValue, int.MaxValue);
            return isValid ? ("true", "") : ("false", invalidReason);
        }

        /// <summary>
        /// Recursive function to check if tree is a valid BST
        /// </summary>
        static (bool isValid, string reason) IsValidBSTStructure(TreeNode node, int min, int max)
        {
            // Empty tree is valid BST
            if (node == null)
                return (true, "");

            // Check current node's value against allowable range
            if (node.Value <= min)
                return (false, $"Node {node.Value} is less than or equal to the minimum allowed value ({min}) for its position in the tree.");

            if (node.Value >= max)
                return (false, $"Node {node.Value} is greater than or equal to the maximum allowed value ({max}) for its position in the tree.");

            // Check left subtree
            var (leftValid, leftReason) = IsValidBSTStructure(node.Left, min, node.Value);
            if (!leftValid)
                return (false, leftReason);

            // Check right subtree
            var (rightValid, rightReason) = IsValidBSTStructure(node.Right, node.Value, max);
            if (!rightValid)
                return (false, rightReason);

            // Both subtrees are valid
            return (true, "");
        }

        /// <summary>
        /// Helper method to build a tree for visualization purposes
        /// </summary>
        static TreeNode BuildTreeForVisualization(string[] input)
        {
            var pairs = new List<(int parent, int child)>();

            foreach (string pair in input)
            {
                var match = Regex.Match(pair, @"\((\d+),(\d+)\)");
                if (match.Success && match.Groups.Count >= 3)
                {
                    int parent = int.Parse(match.Groups[1].Value);
                    int child = int.Parse(match.Groups[2].Value);
                    pairs.Add((parent, child));
                }
            }

            var nodes = new Dictionary<int, TreeNode>();
            var childNodes = new HashSet<int>();

            foreach (var (parent, child) in pairs)
            {
                if (!nodes.ContainsKey(parent))
                    nodes[parent] = new TreeNode(parent);

                if (!nodes.ContainsKey(child))
                    nodes[child] = new TreeNode(child);

                childNodes.Add(child);
            }

            TreeNode root = null;
            foreach (var key in nodes.Keys)
                if (!childNodes.Contains(key))
                    root = nodes[key];

            foreach (var (parent, child) in pairs)
            {
                if (child < parent)
                    nodes[parent].Left = nodes[child];
                else
                    nodes[parent].Right = nodes[child];
            }

            return root;
        }

        /// <summary>
        /// Prints an ASCII representation of the tree structure
        /// </summary>
        static void PrintTree(TreeNode node, int level)
        {
            if (node == null) return;

            // Print right branch
            PrintTree(node.Right, level + 1);

            // Print current node
            Console.WriteLine(new string(' ', level * 4) + node.Value);

            // Print left branch
            PrintTree(node.Left, level + 1);
        }
    }
}