using System.Collections;
using System.ComponentModel;

namespace DynamicList
{
    internal class DynamicList : IList
    {
        private List<(Type type, IList list)> _data = [];
        public object? this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                int currentIndex = 0;
                foreach (var item in this)
                {
                    if (currentIndex == index)
                    {
                        return item;
                    }
                    currentIndex++;
                }

                return new Exception("Unexpected failure");
            }
            set
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                DynamicList newList = [];

                for (int i = 0; i < this.Count; i++)
                {
                    if (i == index)
                    {
                        newList.Add(value);
                    }
                    else
                    {
                        newList.Add(this[i]);
                    }
                }

                this._data = newList._data;
            }
        }

        public int Count => _data.Sum(entry => entry.list.Count);

        public bool IsSynchronized => false;

        public object SyncRoot => new();

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public int Add(object? value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var valueType = value.GetType();

            var entry = _data.LastOrDefault(e => e.type == valueType);
            if (entry.list == null || _data.IndexOf(entry) != _data.Count - 1)
            {
                var listType = typeof(List<>).MakeGenericType(valueType);
                var newList = (IList)Activator.CreateInstance(listType)!;
                newList.Add(value);
                _data.Add((valueType, newList));
            }
            else
            {
                entry.list.Add(value);
            }

            return this.Count - 1;
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(object? value)
        {
            bool result = false;

            foreach (var item in this)
            {
                result |= value?.Equals(item) ?? false;
            }

            return result;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var (type, list) in _data)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }

        public int IndexOf(object? value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i]?.Equals(value) ?? false)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, object? value)
        {
            if (index < 0 || index > this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            DynamicList newList = [];

            for (int i = 0; i <= this.Count; i++)
            {
                if (i < index)
                {
                    newList.Add(this[i]);
                }
                else if (i == index)
                {
                    newList.Add(value);
                }
                else
                {
                    newList.Add(this[i - 1]);
                }
            }

            this._data = newList._data;
        }

        public void Remove(object? value)
        {
            int index = this.IndexOf(value);

            if (index == -1) return;

            DynamicList newList = [];

            for (int i = 0; i < this.Count; i++)
            {
                if (i != index)
                {
                    newList.Add(this[i]);
                }
            }

            this._data = newList._data;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            DynamicList newList = [];

            for (int i = 0; i < this.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }

                newList.Add(this[i]);
            }

            this._data = newList._data;
        }

        /// <summary>
        /// This method is not supported for <c>DynamicList</c>.
        /// </summary>
        /// <remarks>
        /// Attempting to call this method will result in a <see cref="NotSupportedException"/>.
        /// </remarks>
        [Obsolete("This method is not supported for this class", false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException("CopyTo is not supported by DynamicList");
        }
    }
}
