﻿namespace shared
{
    /// <summary>
    /// https://www.geeksforgeeks.org/priority-queue-using-binary-heap/
    /// https://www.dotnetlovers.com/article/231/priority-queue
    /// https://leetcode.com/problems/cheapest-flights-within-k-stops/solutions/164765/C-using-Dijkstra-+-PriorityQueue-(Beats-58.82)
    /// https://en.wikipedia.org/wiki/Shortest_path_problem#Single-source_shortest_paths
    /// 
    /// Priority Queue is an extension of the queue with the following properties:  
    ///   1. Every item has a priority associated with it.
    ///   2. An element with high priority is dequeued before an element with low priority.
    ///   3. If two elements have the same priority, they are served according to their order in the queue.
    /// 
    /// A Binary Heap is a Binary Tree with the following properties:  
    ///   1. It is a Complete Tree.This property of Binary Heap makes them suitable to be stored in an array.
    ///      A complete tree is a binary tree whose all levels except the last level are completely filled and all the leaves in the last level are all to the left side.
    ///   2. A Binary Heap is either Min Heap or Max Heap.
    ///   3. In a Min Binary Heap, the key at the root must be minimum among all keys present in Binary Heap. The same property must be recursively true for all nodes in Binary Tree.
    ///   4. Similarly, in a Max Binary Heap, the key at the root must be maximum among all keys present in Binary Heap. The same property must be recursively true for all nodes in Binary Tree.
    ///   
    /// Heap height is H = log2 N
    /// Leaf nodes are at height H or H-1
    /// Heap of height H has between h+1 and 2^(h+1)+1 nodes
    /// Arr[(i-1)/2]	Returns the parentIndex node
    /// Arr[(2 * i) + 1] Returns the left childIndex node
    /// Arr[(2 * i) + 2] Returns the right childIndex node
    /// leafs start from floor(n/2) index
    /// </summary>
    /// <typeparam name="T">element type, must implement <see cref="IComparable"/></typeparam>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public PriorityQueue(bool isMinPriorityQueue = false, IEnumerable<T>? initialInput = null) {
            this.isMinPriorityQueue = isMinPriorityQueue;
            queue = new List<T>();
            if (initialInput != null) {
                foreach (T item in initialInput) Enqueue(item);
            }
        }

        public List<T> queue;
        private readonly bool isMinPriorityQueue;

        public void Enqueue(T item) {
            queue.Add(item);
            if (isMinPriorityQueue)
                HeapifyUpMin(queue);
            else
                HeapifyUpMax(queue);
        }
        public T Dequeue() {
            if (queue.Count == 0)
                throw new Exception("Queue is empty");

            var returnVal = queue[0];
            var last = queue.Count - 1;
            queue[0] = queue[last];
            queue.RemoveAt(last);
            if (isMinPriorityQueue)
                HeapifyDownMin(queue, 0);
            else
                HeapifyDownMax(queue, 0);

            return returnVal;
        }

        public IEnumerable<T> DequeueAll() {
            while (!IsEmpty()) yield return Dequeue();
        }

        public static int GetLeftChildIndex(int parentIndex) => 2 * parentIndex + 1;
        public static int GetRightChildIndex(int parentIndex) => 2 * parentIndex + 2;
        public static int GetParentIndex(int childIndex) => (childIndex - 1) / 2;
        public static bool HasLeftChild(IList<T> arr, int parentIndex) => GetLeftChildIndex(parentIndex) <= arr.Count - 1;
        public static bool HasRightChild(IList<T> arr, int parentIndex) => GetRightChildIndex(parentIndex) <= arr.Count - 1;
        public static bool HasParent(int childIndex) => GetParentIndex(childIndex) >= 0;

        public T Peek() => queue[0];
        public int Count() => queue.Count;
        public int Height() => Height(Count());
        public int Height(int n) => (int)Math.Floor(Math.Log2(n));
        public bool IsEmpty() => queue.Count == 0;
        public T? GetParent(int childIndex) => HasParent(childIndex) ? queue[GetParentIndex(childIndex)] : default;
        public T? GetLeftChild(int parentIndex) => HasLeftChild(queue, parentIndex) ? queue[GetLeftChildIndex(parentIndex)] : default;
        public T? GetRightChild(int parentIndex) => HasRightChild(queue, parentIndex) ? queue[GetRightChildIndex(parentIndex)] : default;

        public List<T> Leafs() {
            // last leaf is Nth element (index N-1). it's parentIndex is N/2-nd element (index (N-1)/2).
            // there is no element such that its parentIndex is after N/2-nd elemnent (index (N-1)/2)
            // so, N/2+1 element and onwards (n/2 index and onwards) are all leafs
            var n = Count();
            var leafs = new List<T>();
            for (int i = (int)Math.Floor(n / 2.0); i < n; i++) {
                leafs.Add(queue[i]);
            }
            return leafs;
        }

        // https://www.hackerearth.com/practice/notes/heaps-and-priority-queues/
        /// <summary>
        /// called for heap deletion
        /// </summary>
        private static void HeapifyDownMin(IList<T> arr, int i) {
            var (left, right) = (GetLeftChildIndex(i), GetRightChildIndex(i));
            var smallest = HasLeftChild(arr, i) && arr[left].CompareTo(arr[i]) < 0 ? left : i;

            if (HasRightChild(arr, i) && arr[right].CompareTo(arr[smallest]) < 0)
                smallest = right;
            if (smallest != i) {
                (arr[i], arr[smallest]) = (arr[smallest], arr[i]);
                HeapifyDownMin(arr, smallest);
            }
        }

        /// <summary>
        /// called for heap deletion
        /// </summary>
        private static void HeapifyDownMax(IList<T> arr, int i) {
            var (left, right) = (GetLeftChildIndex(i), GetRightChildIndex(i));
            var largest = HasLeftChild(arr, i) && arr[left].CompareTo(arr[i]) > 0 ? left : i;

            if (HasRightChild(arr, i) && arr[right].CompareTo(arr[largest]) > 0)
                largest = right;
            if (largest != i) {
                (arr[i], arr[largest]) = (arr[largest], arr[i]);
                HeapifyDownMax(arr, largest);
            }
        }

        /// <summary>
        /// called for heap insertion
        /// </summary>
        private static void HeapifyUpMax(IList<T> arr) {
            var i = arr.Count - 1;
            while (HasParent(i) && arr[GetParentIndex(i)].CompareTo(arr[i]) < 0) {
                (arr[i], arr[GetParentIndex(i)]) = (arr[GetParentIndex(i)], arr[i]);
                i = GetParentIndex(i);
            }
        }
        /// <summary>
        /// called for heap insertion
        /// </summary>
        private static void HeapifyUpMin(IList<T> arr) {
            var i = arr.Count - 1;
            while (HasParent(i) && arr[GetParentIndex(i)].CompareTo(arr[i]) > 0) {
                (arr[i], arr[GetParentIndex(i)]) = (arr[GetParentIndex(i)], arr[i]);
                i = GetParentIndex(i);
            }
        }

        /// <summary>
        /// Sort input using Heap Sort
        /// </summary>
        public static IEnumerable<T> Sort(IEnumerable<T> input, bool isAscending) => new PriorityQueue<T>(isAscending, input).DequeueAll();
    }
}
