using System;
using System.Collections.Generic;
using System.Linq;

namespace Kuhn_Algorithm
{
    
    internal class KuhnAlgorithm
    {
        private int[] _mt;
        private bool[] _used;

        public KuhnAlgorithm()
        {
            _used = new bool[0];
            _mt = new int[0];
        }

        private bool try_kuhn(int v, List<int>[] g)
        {
            if (_used[v]) return false;
            _used[v] = true;
            for (var i = 0; i < g[v].Count; ++i)
            {
                var to = g[v][i];
                if (_mt[to] != -1 && !try_kuhn(_mt[to], g)) continue;
                _mt[to] = v;
                return true;
            }

            return false;
        }
        
        public List<(int, int)> findMaxMatchingInGraph(int n, int k, List<int>[] reb)
        {
            _mt = Enumerable.Repeat(-1, k).ToArray();
            for (var i = 0; i < n; ++i)
            {
                _used = Enumerable.Repeat(false, n).ToArray();
                try_kuhn(i, reb);
            }
            
            var result = new List<(int,int)>();
            for (var i = 0; i < k; ++i)
                if (_mt[i] != -1)
                {
                    result.Add((_mt[i] + 1, i + 1));
                }
            return result;
        }
        
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello world!");
        }
    }
}