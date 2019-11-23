using System;
using System.Collections.Generic;
using System.Linq;

namespace Kuhn_Algorithm
{
    
    internal class KuhnAlgorithm
    {
        private int[] _currentMatching;
        private bool[] _used;

        public KuhnAlgorithm()
        {
            _used = new bool[0];
            _currentMatching = new int[0];
        }

        private bool DepthFirstSearch(int currentNode, IReadOnlyList<List<int>> reb)
        {
            if (_used[currentNode]) return false;
            _used[currentNode] = true;
            for (var checkingNodeIndex = 0; checkingNodeIndex < reb[currentNode].Count; ++checkingNodeIndex)
            {
                var checkingNode = reb[currentNode][checkingNodeIndex];
                if (_currentMatching[checkingNode] != -1 && !DepthFirstSearch(_currentMatching[checkingNode], reb)) continue;
                _currentMatching[checkingNode] = currentNode;
                return true;
            }
            return false;
        }
        
        public List<(int, int)> findMaxMatchingInGraph(int n, int k, List<int>[] reb)
        {
            _currentMatching = Enumerable.Repeat(-1, k).ToArray();
            for (var currentNode = 0; currentNode < n; ++currentNode)
            {
                _used = Enumerable.Repeat(false, n).ToArray();
                DepthFirstSearch(currentNode, reb);
            }
            
            var result = new List<(int,int)>();
            for (var checkingMatchIndex = 0; checkingMatchIndex < k; ++checkingMatchIndex)
                if (_currentMatching[checkingMatchIndex] != -1)
                {
                    result.Add((_currentMatching[checkingMatchIndex] + 1, checkingMatchIndex + 1));
                }
            return result;
        }
        
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var kuhnObg = new KuhnAlgorithm();
        }
    }
}