using System.Collections.Generic;
using System.Linq;

namespace System
{
    static class Extension
    {
        public static string AsString(this object obj)
        {
            string str;
            if (obj == null) str = "null";
            else if (obj is string) str = "\"" + obj + "\"";
            else if (obj is char) str = "\'" + obj + "\'";
            else if (obj is Array array) str = string.Join("", array.Select(t => AsString(t)));
            else str = obj.ToString();
            return str;
        }

        private static IEnumerable<string> Select(this Array array, Func<object, string> func)
        {
            if (array.Rank == 1 && array.GetLowerBound(0) == 0)
            {
                yield return "{ ";
                foreach (var item in array)
                {
                    yield return func(item);
                    yield return ", ";
                }
                yield return "}";
            }
            else
            {
                var enumerator = new ArrayEnumerator(array);
                int[] flag = Enumerable.Repeat(-1, array.Rank - 1).ToArray();
                yield return "{ ";
                while (enumerator.MoveNext())
                {
                    for (int i = 0; i < flag.Length; i++)
                    {
                        if (flag[i] != -1 && flag[i] != enumerator.Indices[i]) yield return "}, ";
                    }

                    for (int i = 0; i < flag.Length; i++)
                    {
                        if (flag[i] != enumerator.Indices[i])
                        {
                            flag[i] = enumerator.Indices[i];
                            yield return "{ ";
                        }
                    }

                    yield return func(enumerator.Current);
                    yield return ", ";
                }
                for (int i = 1; i < array.Rank; i++)
                {
                    yield return "}, ";
                }
                yield return "}";
            }
        }

        private sealed class ArrayEnumerator
        {
            private Array array;

            private int[] _indices;

            private bool _complete;

            public object Current
            {
                get
                {
                    return array.GetValue(_indices);
                }
            }

            public int[] Indices
            {
                get
                {
                    return _indices;
                }
            }

            internal ArrayEnumerator(Array array)
            {
                this.array = array;
                _indices = new int[array.Rank];
                int num = 1;
                for (int i = 0; i < array.Rank; i++)
                {
                    _indices[i] = array.GetLowerBound(i);
                    num *= array.GetLength(i);
                }

                _indices[_indices.Length - 1]--;
                _complete = (num == 0);
            }

            private void IncArray()
            {
                int rank = array.Rank;
                _indices[rank - 1]++;
                for (int num = rank - 1; num >= 0; num--)
                {
                    if (_indices[num] > array.GetUpperBound(num))
                    {
                        if (num == 0)
                        {
                            _complete = true;
                            break;
                        }

                        for (int i = num; i < rank; i++)
                        {
                            _indices[i] = array.GetLowerBound(i);
                        }

                        _indices[num - 1]++;
                    }
                }
            }

            public bool MoveNext()
            {
                if (_complete)
                {
                    return false;
                }

                IncArray();
                return !_complete;
            }
        }
    }
}
