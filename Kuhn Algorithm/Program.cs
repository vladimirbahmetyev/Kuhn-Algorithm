using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        public (int, int, List<int>[]) GetNextAlt()
        {
            var rebsList = new List<int>[_countOfFirstPart];
            for (var i = 0; i < _countOfFirstPart; i++)
            {
                rebsList[i] = new List<int> {i % _countOfSecondPart, (i + 1) % _countOfSecondPart};
            }

            if (_countOfFirstPart < _countOfSecondPart - 1)
            {
                for (var i = _countOfFirstPart + 1; i < _countOfSecondPart; i++)
                {
                    rebsList[i % _countOfFirstPart].Add(i);
                }
            }

            var numberOfAdditionalRebs = _random.Next(0, _countOfFirstPart + _countOfSecondPart);
            
            for (var i = 0; i < numberOfAdditionalRebs; i++)
            {
                var leftNode = _random.Next(0, _countOfFirstPart - 1);
                var rightNode = _random.Next(0, _countOfSecondPart - 1);
                if(!rebsList[leftNode].Contains(rightNode))
                    rebsList[leftNode].Add(rightNode);
            }

            return (_countOfFirstPart, _countOfSecondPart, rebsList);
        }
    }
    
    internal static class Program
    {
        public static void Main()
        {
            //Проверка для оценки O(n1m)
            //Прогрев CPU
            for (var i = 0; i < 10; i++)
            {
                TestDependenceFromRebs(1000, 1000, 7);
            }
            
            //Проверка зависимости от m
            var testResult = TestDependenceFromRebs(1000, 1000, 7);
            var resultString= "";
            
            foreach (var VARIABLE in testResult)
            {
                double tempDouble = VARIABLE.Item1;
                var tempString = $"{tempDouble.ToString(CultureInfo.InvariantCulture).Replace(",",".")}," +
                                 $"{VARIABLE.Item2.TotalMilliseconds.ToString(CultureInfo.InvariantCulture).Replace(",",".")}";
                resultString += "{"+ tempString +"},";
            }
            Console.WriteLine(resultString);
            
            //Проверка зависимости от кол-ва вершин в левой доле
            var firstPartTime = "";
            for (var n1 = 100; n1 < 1600; n1 += 300)
            {
                double nTemp = n1;
                var tempTime = TestDependenceFromNodes(n1, 500).TotalMilliseconds.ToString(CultureInfo.InvariantCulture).Replace(",",".");
                firstPartTime += "{" + nTemp.ToString(CultureInfo.InvariantCulture).Replace(",", ".") + "," + tempTime+ "},";
            }

            Console.WriteLine(firstPartTime);
        }

        private static (int, TimeSpan)[] TestDependenceFromRebs(int firstPartCount, int secondPartCount, int graphCount)
        {
            var kuhnObg = new KuhnAlgorithm();
            IEnumerable<(int, TimeSpan)> dependingFromRebs = new (int, TimeSpan)[0];
            var testGraphGen = new GraphGenerator(firstPartCount,secondPartCount);
            var timer = new Stopwatch();
            for (var i = 0; i < graphCount; i++)
            {
                var testGraph = testGraphGen.GetNextAlt();
                timer.Start();
                kuhnObg.FindMaxMatchingInGraph(testGraph.Item1, testGraph.Item2, testGraph.Item3);
                timer.Stop();
                dependingFromRebs = dependingFromRebs.Append((CountGraphRebs(testGraph.Item3), timer.Elapsed));
                timer.Reset();
            }

            /*dependingFromRebs = dependingFromRebs.OrderBy(result => result.Item1);*/
            
            return dependingFromRebs.ToArray();
        }

        private static TimeSpan TestDependenceFromNodes(int firstPartCount, int secondPartCount)
        {
            var timer = new Stopwatch();
            var tempGraphGen = new GraphGenerator(firstPartCount, secondPartCount);
            var (item1, item2, lists) = tempGraphGen.GetNextAlt();
            var kuhnObj = new KuhnAlgorithm();
            timer.Start();
            kuhnObj.FindMaxMatchingInGraph(item1, item2, lists);
            timer.Stop();
            return timer.Elapsed;
        }

        private static int CountGraphRebs(IEnumerable<List<int>> arrayRebs) => arrayRebs.Aggregate(0, (acc, rebsList) => acc + rebsList.Count);
        
    }
}