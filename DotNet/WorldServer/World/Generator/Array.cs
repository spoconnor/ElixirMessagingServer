using System;
using System.Collections;
using System.Collections.Generic;


namespace Sean.World
{
    public struct Vector2
    {
        public float x;
        public float y;
    }

    public class ArraySize
    {
        public int minZ;
        public int maxZ;
        public int minX;
        public int maxX;
        public int scale;
        public int width { get { return maxZ - minZ; } }
        public int height { get { return maxX - minX; } }
    }

    public class ArrayLine<T> 
    {
        public ArrayLine (ArraySize size)
        {
            _size = size;
            _data = new T[ToArrayCoord(_size.maxX)];
        }

        public T this[int x]
        {
            get { return _data[x]; }
            set { _data[x] = value; }
        } 
            
        public T Get(int x)
        {
            return _data [x];
        }
        public void Set(int x, int z, T value)
        {
            _data [x] = value;
        }

        public IEnumerator<T> GetCells ()
        {
            for (int x = 0; x < ToArrayCoord (_size.maxX); x++) 
            {
                yield return _data [x];
            }
        }

        private int ToArrayCoord(int x)
        {
            return (x - _size.minX) / _size.scale;
        }

        private T[] _data;
        private ArraySize _size;
    }

    public class Array<T>
    {
        public Array (ArraySize size)
        {
            _size = size;
            _data = new ArrayLine<T>[ToArrayCoord(_size.maxZ)];
            for (int z = 0; z < ToArrayCoord(_size.maxZ); z++)
            {
                _data[z] = new ArrayLine<T>(_size);
            }
        }

        public ArraySize Size { get { return _size; } }

        public object this[int z]
        {
            get { return _data[z]; }
        } 

        public T Get(int x, int z)
        {
            return _data [z].Get (x);
        }
        public void Set(int x, int z, T value)
        {
            _data [z].Set (x, value);
        }

        public IEnumerable<ArrayLine<T>> GetLines ()
        {
            for (int z = 0; z < ToArrayCoord (_size.maxZ); z++) 
            {
                yield return _data [z];
            }
        }
        public IEnumerable<T> GetCells ()
        {
            for (int z = 0; z < ToArrayCoord (_size.maxZ); z++) 
            {
                foreach (var cell in _data[z].GetCells()) {
                    yield return cell;
                }
            }
        }

        private int ToArrayCoord(int z)
        {
            return (z - _size.minZ) / _size.scale;
        }

        private ArrayLine<T>[] _data;
        private ArraySize _size;
    }
}

