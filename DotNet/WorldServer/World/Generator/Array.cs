using System;
namespace Sean.World
{
    public class ArrayLine<T>
    {
        public ArrayLine (int minX, int maxX, int scale)
        {
            _minX = minX;
            _maxX = maxX;
            _scale = scale;

            _data = new T[ToArrayCoord(_maxX)];
        }

        public T this[int x]
        {
            get { return _data[x]; }
            set { _data[x] = value; }
        } 

        private int ToArrayCoord(int x)
        {
            return (x - _minX) / _scale;
        }

        private T[] _data;
        private int _minX;
        private int _maxX;
        private int _scale;
    }

    public class Array<T>
    {
        public Array (int minX, int maxX, int minZ, int maxZ, int scale)
        {
            _minZ = minZ;
            _maxZ = maxZ;
            _minX = minX;
            _maxX = maxX;
            _scale = scale;

            _data = new ArrayLine<T>[ToArrayCoord(_maxZ)];
            for (int z = 0; z < ToArrayCoord(_maxZ); z++)
            {
                _data[z] = new ArrayLine<T>(_minX, _maxX, _scale);
            }
        }

        public object this[int z]
        {
            get { return _data[z]; }
        } 

        private int ToArrayCoord(int z)
        {
            return (z - _minZ) / _scale;
        }

        private ArrayLine<T>[] _data;
        private int _minZ;
        private int _maxZ;
        private int _minX;
        private int _maxX;
        private int _scale;
    }
}

