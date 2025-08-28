using System;
using System.Collections;
using System.Collections.Generic;

namespace Public
{
    /// <summary>
    /// Generic list struct optimized for 0-3 items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct SmallList<T> : IEnumerable<T>, IEquatable<SmallList<T>>, IReadOnlyList<T>
        where T : IEquatable<T>
    {
        private readonly int _count;
        private readonly T _firstItem;
        private readonly T _secondtItem;
        private readonly T _thirdItem;
        public SmallList()
        {
        }

        public SmallList(T firstItem)
        {
            _firstItem = firstItem;
            _count = 1;
        }

        public SmallList(T firstItem, T secondItem)
        {
            _firstItem = firstItem;
            _secondtItem = secondItem;
            _count = 2;
        }

        public SmallList(T firstItem, T secondItem, T thirdItem)
        {
            _firstItem = firstItem;
            _secondtItem = secondItem;
            _thirdItem = thirdItem;
            _count = 3;
        }

        public SmallList<T> Add(T item)
        {
            return _count switch
            {
                0 => new SmallList<T>(item),
                1 => new SmallList<T>(_firstItem, item),
                2 => new SmallList<T>(_firstItem, _secondtItem, item),
                _ => throw new InvalidOperationException("SmallList can only hold up to 3 items.")
            };
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <inheritdoc />
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this); ;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this); ;
        }

        public int Count => _count;

        public struct Enumerator : IEnumerator<T>
        {
            private readonly SmallList<T> _list;
            private int _index;
            public Enumerator(SmallList<T> list)
            {
                _list = list;
                _index = -1;
            }
            public T Current
            {
                get
                {
                    return _index switch
                    {
                        0 => _list._firstItem,
                        1 => _list._secondtItem,
                        2 => _list._thirdItem,
                        _ => throw new InvalidOperationException()
                    };
                }
            }
            object IEnumerator.Current => Current;
            public bool MoveNext()
            {
                if (_index + 1 < _list._count)
                {
                    _index++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _index = -1;
            }
            public void Dispose()
            {
            }
        }

        public bool Equals(SmallList<T> other)
        {
            return _count == other._count
                && EqualityComparer<T>.Default.Equals(_firstItem, other._firstItem)
                && EqualityComparer<T>.Default.Equals(_secondtItem, other._secondtItem)
                && EqualityComparer<T>.Default.Equals(_thirdItem, other._thirdItem);
        }

        public override bool Equals(object obj)
        {
            return obj is SmallList<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _count;
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(_firstItem);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(_secondtItem);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(_thirdItem);
                return hashCode;
            }
        }

        public static bool operator ==(SmallList<T> left, SmallList<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SmallList<T> left, SmallList<T> right)
        {
            return !left.Equals(right);
        }

        public T this[int index] 
        {
            get 
            {
                return index switch
                {
                    0 when _count > 0 => _firstItem,
                    1 when _count > 1 => _secondtItem,
                    2 when _count > 2 => _thirdItem,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.")
                };
            }
        }
    }
}
