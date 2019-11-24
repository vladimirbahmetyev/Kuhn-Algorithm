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
                if (_currentMatching[checkingNode] == -1 || DepthFirstSearch(_currentMatching[checkingNode], reb))
                {
                    _currentMatching[checkingNode] = currentNode;
                    return true;
                }
            }
            return false;
        }
        
        public IEnumerable<(int, int)> FindMaxMatchingInGraph(int n, int k, List<int>[] reb)
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

    internal class GraphGenerator
    {
        private readonly int _countOfFirstPart;
        private readonly int _countOfSecondPart;
        private readonly Random _random;

        public GraphGenerator(int countOfFirstPart, int countOfSecondPart)
        {
            _countOfFirstPart = countOfFirstPart;
            _countOfSecondPart = countOfSecondPart;
            _random = new Random();
        }
        public (int, int, List<int>[]) GetNext()
        {
            var rebList = new List<int>[_countOfFirstPart];
            for (var firstPartNodeIndex = 0; firstPartNodeIndex < _countOfFirstPart; firstPartNodeIndex++)
            {
                var countOfReb = _random.Next(1, _countOfSecondPart);
                var nodeRebs = new List<int>();
                for (var newRebIndex = 0; newRebIndex < countOfReb; newRebIndex++)
                {
                    var secondNode = _random.Next(0, _countOfSecondPart);
                    if (!nodeRebs.Contains(secondNode))
                        nodeRebs.Add(secondNode);
                }
                rebList[firstPartNodeIndex] = nodeRebs;
            }
    
            var checkSecondNodes = new List<int>();
            for (var checkFisrtNode = 0; checkFisrtNode < _countOfFirstPart; checkFisrtNode++)
            {
                for (var rebFromFirstNode = 0; rebFromFirstNode < rebList[checkFisrtNode].Count; rebFromFirstNode++)
                {
                    if (!checkSecondNodes.Contains(rebList[checkFisrtNode][rebFromFirstNode]))
                    {
                        checkSecondNodes.Add(rebList[checkFisrtNode][rebFromFirstNode]);
                    }
                }
            }

            if (checkSecondNodes.Count < _countOfSecondPart)
            {
                for (var checkingSecondNode = 0; checkingSecondNode < _countOfSecondPart; checkingSecondNode++)
                {
                    if(!checkSecondNodes.Contains(checkingSecondNode))
                        rebList[_random.Next(0, _countOfFirstPart)].Add(checkingSecondNode);
                }
            }

            var checkFirstNodes = new List<int> {0};
            for (var checkingReb = 0; checkingReb < _countOfFirstPart; checkingReb++)
            {
                for (var checkingSecondNode = 0; checkingSecondNode < rebList[checkingReb].Count; checkingSecondNode++)
                {
                    for (var i = checkingReb + 1; i < _countOfFirstPart; i++)
                    {
                        if(rebList[i].Contains(checkingSecondNode))
                            checkFirstNodes.Add(i);
                    }
                }
            }

            if (checkFirstNodes.Count < _countOfFirstPart)
            {
                for (var i = 0; i < _countOfFirstPart; i++)
                {
                    if (!checkFirstNodes.Contains(i))
                    {
                        var checkedNode = _random.Next(0, checkFirstNodes.Count - 1);
                        var randomCheckedNodeReb = _random.Next(0, rebList[checkedNode].Count - 1);
                        rebList[i].Add(randomCheckedNodeReb);
                    }
                }
            }
            
            
            return (_countOfFirstPart, _countOfSecondPart, rebList);
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var testGraphGen = new GraphGenerator(10,10);
            var testGraph = testGraphGen.GetNext();
            var kuhnObg = new KuhnAlgorithm();
            var result = kuhnObg.FindMaxMatchingInGraph(testGraph.Item1, testGraph.Item2, testGraph.Item3);
            for (var i = 0; i < result.Count(); i++)
            {
                Console.Write(result.ElementAt(i).Item1);
                Console.Write("-");
                Console.WriteLine(result.ElementAt(i).Item2);
                
            }
        }
    }
}