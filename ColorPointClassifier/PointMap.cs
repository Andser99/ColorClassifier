using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPointClassifier
{
    class PointMap
    {
        public static readonly bool DEBUG = false;
        // Points are mapped as follows: Red - 1, Blue - 2, Green - 3, Purple - 4
        private readonly int _mapSize;
        private readonly int _mapCenterX;
        private readonly int _mapCenterY;
        private Random rnd = new Random();


        private int[] PrecomputedSquares;
        private byte[,] Map;
        private List<(int x, int y)> PointsList;

        /// <summary>
        /// Initializes a map with <paramref name="mapSize"/> and precomputes squares up to and including <paramref name="mapSize"/>
        /// </summary>
        /// <param name="mapSize">Defines the maximum map size in each direction</param>
        public PointMap(int mapSize)
        {
            _mapCenterX = mapSize;
            _mapCenterY = mapSize;
            _mapSize = mapSize * 2 + 2;
            Map = new byte[_mapSize, _mapSize];

            PointsList = new List<(int x, int y)>();

            PrecomputedSquares = new int[_mapSize];
            PrecomputeSquares(mapSize);

            AddDefaultPoints();
        }

        public void AddPoint(int x, int y, byte type)
        {
            if (Map[_mapCenterY + y, _mapCenterX + x] == 0)
            {
                Map[_mapCenterY + y, _mapCenterX + x] = type;
                PointsList.Add((x, y));
            }
        }

        public double AddRandom(int kClosest, int quadCount = 10000)
        {
            double correct = 0;
            for (int i = 0; i < quadCount; i++)
            {
                correct += ClassifyRandomRed(kClosest);
                correct += ClassifyRandomGreen(kClosest);
                correct += ClassifyRandomBlue(kClosest);
                correct += ClassifyRandomPurple(kClosest);

            }

            return correct / (quadCount * 4);
        }

        public byte TestPoint()
        {
            throw new NotImplementedException();
        }

        public byte GetByteAt(int y, int x)
        {
            return Map[y, x];
        }

        public int GetSize()
        {
            return _mapSize - 1;
        }

        public byte FindClosestType(int countClosest, (int x, int y) sourcePoint)
        {
            List<int> closestPointsList = new List<int>();
            for (int i = 0; i < countClosest; i++)
            {
                int closestDistance = _mapSize * _mapSize * 2;
                (int x, int y) closestPoint = (int.MaxValue, int.MaxValue);
                foreach (var point in PointsList)
                {
                    int xAbs = Math.Abs(point.x - sourcePoint.x);
                    int yAbs = Math.Abs(point.y - sourcePoint.y);
                    int distance = PrecomputedSquares[xAbs] + PrecomputedSquares[yAbs];
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPoint = point;
                    }
                }
                if (closestPoint.x != int.MaxValue && closestPoint.y != int.MaxValue)
                {
                    closestPointsList.Add(Map[closestPoint.y+5000, closestPoint.x+5000]);
                }
            }
            byte mostCommonType = (byte)(closestPointsList.GroupBy(_ => _).OrderByDescending(_ => _).First().Key);
            Console.Write(mostCommonType);

            AddPoint(sourcePoint.x, sourcePoint.y, mostCommonType);
            return mostCommonType;
        }

        private int ClassifyRandomRed(int kClosest)
        {
            int x = rnd.Next(-5000, 500);
            int y = rnd.Next(-5000, 500);
            int correct = 0;
            if (rnd.Next(100) == 1)
            {
                x = rnd.Next(-5000, 5001);
                y = rnd.Next(-5000, 5001);
                if (FindClosestType(kClosest, (x, y)) == 1) correct = 1;
            }
            else
            {
                if (FindClosestType(kClosest, (x, y)) == 1) correct = 1;
            }
            return correct;
        }

        private int ClassifyRandomGreen(int kClosest)
        {
            int x = rnd.Next(-499, 5001);
            int y = rnd.Next(-5000, 500);
            int correct = 0;
            if (rnd.Next(100) == 1)
            {
                x = rnd.Next(-5000, 5001);
                y = rnd.Next(-5000, 5001);
                if (FindClosestType(kClosest, (x, y)) == 2) correct = 1;
            }
            else
            {
                if (FindClosestType(kClosest, (x, y)) == 2) correct = 1;
            }
            return correct;
        }

        private int ClassifyRandomBlue(int kClosest)
        {
            int x = rnd.Next(-5000, 500);
            int y = rnd.Next(-499, 5001);
            int correct = 0;
            if (rnd.Next(100) == 1)
            {
                x = rnd.Next(-5000, 5001);
                y = rnd.Next(-5000, 5001);
                if (FindClosestType(kClosest, (x, y)) == 3) correct = 1;
            }
            else
            {
                if (FindClosestType(kClosest, (x, y)) == 3) correct = 1;
            }
            return correct;
        }

        private int ClassifyRandomPurple(int kClosest)
        {
            int x = rnd.Next(-499, 5001);
            int y = rnd.Next(-499, 5001);
            int correct = 0;
            if (rnd.Next(100) == 1)
            {
                x = rnd.Next(-5000, 5001);
                y = rnd.Next(-5000, 5001);
                if (FindClosestType(kClosest, (x, y)) == 4) correct = 1;
            }
            else
            {
                if (FindClosestType(kClosest, (x, y)) == 4) correct = 1;
            }
            return correct;
        }

        private void AddDefaultPoints()
        {
            //Red
            AddPoint(-4500, -4400, 1);
            AddPoint(-4100, -3000, 1);
            AddPoint(-1800, -2400, 1);
            AddPoint(-2500, -3400, 1);
            AddPoint(-2000, -1400, 1);


            //Green
            AddPoint(4500, -4400, 2);
            AddPoint(4100, -3000, 2);
            AddPoint(1800, -2400, 2);
            AddPoint(2500, -3400, 2);
            AddPoint(2000, -1400, 2);

            //Blue
            AddPoint(-4500, 4400, 3);
            AddPoint(-4100, 3000, 3);
            AddPoint(-1800, 2400, 3);
            AddPoint(-2500, 3400, 3);
            AddPoint(-2000, 1400, 3);

            //Purple
            AddPoint(4500, 4400, 4);
            AddPoint(4100, 3000, 4);
            AddPoint(1800, 2400, 4);
            AddPoint(2500, 3400, 4);
            AddPoint(2000, 1400, 4);

            if (DEBUG)
            {
                int rnd1 = rnd.Next(5);
                int rnd2 = rnd.Next(5);
                for (int i = -4800; i < -4200; i+=1)
                {
                    for (int j = -4900; j < -4100; j+= rnd.Next(5))
                    {
                        AddPoint(i + rnd.Next(5), j+rnd.Next(5), (byte)(rnd.Next(4) + 1));
                    }
                }
            }
        }

        private void PrecomputeSquares(int mapSize)
        {
            for (int i = 1; i < mapSize + 1; i++)
            {
                PrecomputedSquares[i] = i * i;
            }
        }
    }
}
