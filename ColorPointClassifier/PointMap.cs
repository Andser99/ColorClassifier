using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorPointClassifier
{
    class PointMap
    {
        public static readonly bool DEBUG = false;
        // Points are mapped as follows: Red - 1, Blue - 2, Green - 3, Purple - 4
        private readonly int _mapSize;
        private readonly int _mapCenterX;
        private readonly int _mapCenterY;
        private readonly int _mapPointsTotal;
        private Random rnd;


        private int[] PrecomputedSquares;
        private byte[,] Map;
        private byte[,] FilledMap;
        private List<(int x, int y)> PointsList;

        private int[] sizeList = new int[100];
        private int sizeIndex = 0;

        /// <summary>
        /// Initializes a map with <paramref name="mapSize"/> and precomputes squares up to and including <paramref name="mapSize"/>
        /// </summary>
        /// <param name="mapSize">Defines the maximum map size in each direction</param>
        public PointMap(int mapSize, int seed)
        {
            rnd = new Random(seed);
            _mapCenterX = mapSize;
            _mapCenterY = mapSize;
            _mapSize = mapSize * 2 + 2;
            _mapPointsTotal = _mapSize * _mapSize;
            Map = new byte[_mapSize, _mapSize];
            FilledMap = new byte[_mapSize, _mapSize];

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

        public byte GetByteAtFilled(int y, int x)
        {
            return FilledMap[y, x];
        }

        public int GetSize()
        {
            return _mapSize - 1;
        }

        public async void FillRest(int kClosest, ProgressBar progressBar)
        {
            int progress = 0;
            List<(short x, short y)> listToAdd = new List<(short x, short y)>();
            for (int i = -5000; i <= 5000; i += 1)
            {
                for (int j = -5000; j <= 5000; j += 1)
                {
                    if (Map[i + _mapCenterY, j + _mapCenterX] == 0x0)
                    {
                        //FilledMap[i + _mapCenterY, j + _mapCenterX] = FindClosestType(kClosest, (j, i));
                        listToAdd.Add(((short)j, (short)i));
                    }
                    else
                    {
                        FilledMap[i + _mapCenterY, j + _mapCenterX] = Map[i + _mapCenterY, j + _mapCenterX];
                    }
                    progress++;
                }
                progressBar.Value = progress / (_mapPointsTotal / 100);
            }
            progress = 0;
            progressBar.Value = 0;
            int n = listToAdd.Count;
            while (n > 1)
            {
                progress++;
                progressBar.Value = progress / (listToAdd.Count / 100);
                n--;
                int k = rnd.Next(n + 1);
                var value = listToAdd[k];
                listToAdd[k] = listToAdd[n];
                listToAdd[n] = value;
            }

            progress = 0;
            foreach (var point in listToAdd)
            {
                progress++;
                var color = FindClosestType(kClosest, point, true);
                Map[point.y + _mapCenterY, point.x + _mapCenterX] = color;
                FilledMap[point.y + _mapCenterY, point.x + _mapCenterX] = color;
                progressBar.Value = progress / (listToAdd.Count / 100);
                if (progress % 100000 == 0) Console.WriteLine($"Progress: {progress}");
            }
            progressBar.Value = 0;
        }

        public void Reset()
        {
            Map = new byte[_mapSize, _mapSize];
            FilledMap = new byte[_mapSize, _mapSize];
            AddDefaultPoints();
        }

        public byte FindClosestType(int countClosest, (int x, int y) sourcePoint, bool add = false)
        {
            if (PointsList.Count > 5000)
            {
                return SnailSearch(sourcePoint, countClosest);
            }
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
            //Console.Write(mostCommonType);

            if (add) AddPoint(sourcePoint.x, sourcePoint.y, mostCommonType);
            return mostCommonType;
        }

        private byte SnailSearch((int x, int y) coord, int countClosest)
        {
            bool expanded = false;
            int size = 1;
            //List<(int Distance, int Color)> closestPointsList = new List<(int Distance, int Color)>();
            int closestDistance = int.MaxValue;
            int targetSize = int.MaxValue;
            (int x, int y) closestPoint = (int.MaxValue, int.MaxValue);
            while (!expanded)
            {
                int AbsSquare = PrecomputedSquares[Math.Abs(size)];
                //Top line
                if (coord.y + size <= 5000)
                {
                    for (int i = -size; i <= size; i++)
                    {
                        if (coord.x + i >= -5000 && coord.x + i <= 5000)
                        {
                            var color = Map[coord.y + size + _mapCenterY, coord.x + i + _mapCenterX];
                            if (color == 0x0) continue;
                            int xAbs = Math.Abs(i);
                            int distance = PrecomputedSquares[xAbs] + AbsSquare;
                            if (distance < closestDistance)
                            {
                                //closestPointsList.Add((closestDistance, color));
                                closestDistance = distance;
                                closestPoint = (coord.x + i, coord.y + size);
                            }
                        }
                    }
                }
                //Bottom line
                if (coord.y - size >= -5000) //Check out of bounds for whole line at the bottom
                {
                    for (int i = -size; i <= size; i++)
                    {
                        if (coord.x + i >= -5000 && coord.x + i <= 5000) // check side out of bounds for point on line
                        {
                            var color = Map[coord.y - size + _mapCenterY, coord.x + i + _mapCenterX];
                            if (color == 0x0) continue;
                            int xAbs = Math.Abs(i);
                            int distance = PrecomputedSquares[xAbs] + AbsSquare;
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestPoint = (coord.x + i, coord.y - size);
                            }
                        }
                    }
                }
                //Right line
                if (coord.x + size <= 5000)
                {
                    for (int i = -size + 1; i < size; i++)
                    {
                        if (coord.y + i >= -5000 && coord.y + i <= 5000)
                        {
                            var color = Map[coord.y + i + _mapCenterY, coord.x + size + _mapCenterX];
                            if (color == 0x0) continue;
                            int yAbs = Math.Abs(i);
                            int distance = AbsSquare + PrecomputedSquares[yAbs];
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestPoint = (coord.x + size, coord.y + i);
                            }
                        }
                    }
                }
                //Left line
                if (coord.x - size >= -5000)
                {
                    for (int i = -size + 1; i < size; i++)
                    {
                        if (coord.y + i >= -5000 && coord.y + i <= 5000)
                        {
                            var color = Map[coord.y + i + _mapCenterY, coord.x - size + _mapCenterX];
                            if (color == 0x0) continue;
                            int yAbs = Math.Abs(i);
                            int distance = AbsSquare + PrecomputedSquares[yAbs];
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestPoint = (coord.x - size, coord.y + i);
                            }
                        }
                    }
                }

                if (closestDistance != int.MaxValue || size >= targetSize || size > 10000)
                {
                    if (targetSize == int.MaxValue) targetSize = (size * 1415) / 1000;

                    if (size > 5001 || size >= targetSize)
                    {
                        //sizeList[sizeIndex] = size;
                        //sizeIndex++;
                        //if (sizeIndex > 99)
                        //{
                        //    int total = 0;
                        //    foreach (var x in sizeList)
                        //    {
                        //        total += x;
                        //    }
                        //    Console.WriteLine($"Avg size needed in last 100: {total / 100}");
                        //    sizeIndex = 0;
                        //}
                        byte t = Map[closestPoint.y + _mapCenterY, closestPoint.x + _mapCenterX];
                        //AddPoint(coord.x, coord.y, t);
                        return t;
                    }
                }


                size += 1;
            }
            return 0x0;
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
                if (FindClosestType(kClosest, (x, y), true) == 1) correct = 1;
            }
            else
            {
                if (FindClosestType(kClosest, (x, y), true) == 1) correct = 1;
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
                if (FindClosestType(kClosest, (x, y), true) == 2) correct = 1;
            }
            else
            {
                if (FindClosestType(kClosest, (x, y), true) == 2) correct = 1;
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
                if (FindClosestType(kClosest, (x, y), true) == 3) correct = 1;
            }
            else
            {
                if (FindClosestType(kClosest, (x, y), true) == 3) correct = 1;
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
                if (FindClosestType(kClosest, (x, y), true) == 4) correct = 1;
            }
            else
            {
                if (FindClosestType(kClosest, (x, y), true) == 4) correct = 1;
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
