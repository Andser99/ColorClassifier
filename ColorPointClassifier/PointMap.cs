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
        private int scale;

        private int[] sizeList = new int[100];
        private int sizeIndex = 0;

        /// <summary>
        /// Initializes a map with <paramref name="mapSize"/> in each direction and [0,0] and precomputes squares up to and including <paramref name="mapSize"/>
        /// </summary>
        /// <param name="mapSize">Defines the maximum map size in each direction</param>
        public PointMap(int mapSize, int seed)
        {
            rnd = new Random(seed);
            _mapCenterX = mapSize;
            _mapCenterY = mapSize;
            _mapSize = mapSize * 2 + 2;
            _mapPointsTotal = _mapSize * _mapSize;
            scale = 10000 / (_mapSize - 2);
            Map = new byte[_mapSize, _mapSize];
            FilledMap = new byte[_mapSize, _mapSize];

            PointsList = new List<(int x, int y)>();

            PrecomputedSquares = new int[_mapSize];
            PrecomputeSquares(mapSize);
            AddDefaultPoints();
        }

        public void SetSeed(int seed)
        {
            rnd = new Random(seed);
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
            for (int i = -_mapCenterY; i <= _mapCenterY; i += 1)
            {
                for (int j = -_mapCenterX; j <= _mapCenterX; j += 1)
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
                progressBar.Value = progress / (_mapPointsTotal / 100) % 100;
            }
            progress = 0;
            progressBar.Value = 0;
            int n = listToAdd.Count;
            while (n > 1)
            {
                progress++;
                progressBar.Value = progress / (listToAdd.Count / 100) % 100;
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
                if (color == 0x0)
                {
                    var x = "pizdec";
                }
                Map[point.y + _mapCenterY, point.x + _mapCenterX] = color;
                FilledMap[point.y + _mapCenterY, point.x + _mapCenterX] = color;
                progressBar.Value = progress / (listToAdd.Count / 100) % 100;
                if (progress % 10000 == 0) Console.WriteLine($"Progress: {((double)progress)/1000000:0.###}%");
            }
            progressBar.Value = 0;
        }

        public void Reset()
        {
            Map = new byte[_mapSize, _mapSize];
            FilledMap = new byte[_mapSize, _mapSize];
            PointsList.Clear();
            AddDefaultPoints();
        }
        

        /// <summary>
        /// Finds a point with the closest type using the kNN algorithm where k is <paramref name="countClosest"/>
        /// If there are more than 5000 points already, switches to a more effective algorithm.
        /// </summary>
        /// <param name="countClosest"></param>
        /// <param name="sourcePoint"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public byte FindClosestType(int countClosest, (int x, int y) sourcePoint, bool add = false)
        {
            //
            if (PointsList.Count > 5000 * countClosest || true)
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
                    closestPointsList.Add(Map[closestPoint.y+_mapCenterY, closestPoint.x+_mapCenterX]);
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
            List<(int Distance, int Color)> closestPointsList = new List<(int Distance, int Color)>();
            closestPointsList.Add((int.MaxValue, 0));
            int targetSize = int.MaxValue;
            while (!expanded)
            {
                int AbsSquare = PrecomputedSquares[Math.Abs(size)];
                //Top line
                if (coord.y + size <= _mapCenterY)
                {
                    for (int i = -size; i <= size; i++)
                    {
                        if (coord.x + i >= -_mapCenterX && coord.x + i <= _mapCenterX)
                        {
                            var color = Map[coord.y + size + _mapCenterY, coord.x + i + _mapCenterX];
                            if (color == 0x0) continue;
                            int xAbs = Math.Abs(i);
                            int distance = PrecomputedSquares[xAbs] + AbsSquare;
                            if (distance < closestPointsList.Last().Distance)
                            {
                                closestPointsList.Add((distance, color));
                                closestPointsList.Sort((a,b) => a.Distance - b.Distance);
                                if (closestPointsList.Count > countClosest)
                                {
                                    closestPointsList.RemoveAt(closestPointsList.Count-1);
                                }
                            }
                        }
                    }
                }
                //Bottom line
                if (coord.y - size >= -_mapCenterY) //Check out of bounds for whole line at the bottom
                {
                    for (int i = -size; i <= size; i++)
                    {
                        if (coord.x + i >= -_mapCenterX && coord.x + i <= _mapCenterX) // check side out of bounds for point on line
                        {
                            var color = Map[coord.y - size + _mapCenterY, coord.x + i + _mapCenterX];
                            if (color == 0x0) continue;
                            int xAbs = Math.Abs(i);
                            int distance = PrecomputedSquares[xAbs] + AbsSquare;
                            if (distance < closestPointsList.Last().Distance)
                            {
                                closestPointsList.Add((distance, color));
                                closestPointsList.Sort((a, b) => a.Distance - b.Distance);
                                if (closestPointsList.Count > countClosest)
                                {
                                    closestPointsList.RemoveAt(closestPointsList.Count - 1);
                                }
                            }
                        }
                    }
                }
                //Right line
                if (coord.x + size <= _mapCenterX)
                {
                    for (int i = -size + 1; i < size; i++)
                    {
                        if (coord.y + i >= -_mapCenterY && coord.y + i <= _mapCenterY)
                        {
                            var color = Map[coord.y + i + _mapCenterY, coord.x + size + _mapCenterX];
                            if (color == 0x0) continue;
                            int yAbs = Math.Abs(i);
                            int distance = AbsSquare + PrecomputedSquares[yAbs];
                            if (distance < closestPointsList.Last().Distance)
                            {
                                closestPointsList.Add((distance, color));
                                closestPointsList.Sort((a, b) => a.Distance - b.Distance);
                                if (closestPointsList.Count > countClosest)
                                {
                                    closestPointsList.RemoveAt(closestPointsList.Count - 1);
                                }
                            }
                        }
                    }
                }
                //Left line
                if (coord.x - size >= -_mapCenterX)
                {
                    for (int i = -size + 1; i < size; i++)
                    {
                        if (coord.y + i >= -_mapCenterY && coord.y + i <= _mapCenterY)
                        {
                            var color = Map[coord.y + i + _mapCenterY, coord.x - size + _mapCenterX];
                            if (color == 0x0) continue;
                            int yAbs = Math.Abs(i);
                            int distance = AbsSquare + PrecomputedSquares[yAbs];
                            if (distance < closestPointsList.Last().Distance)
                            {
                                closestPointsList.Add((distance, color));
                                closestPointsList.Sort((a, b) => a.Distance - b.Distance);
                                if (closestPointsList.Count > countClosest)
                                {
                                    closestPointsList.RemoveAt(closestPointsList.Count - 1);
                                }
                            }
                        }
                    }
                }

                if (closestPointsList.Count == countClosest && closestPointsList.First().Distance != int.MaxValue || size >= targetSize || size > 10000)
                {
                    if (targetSize == int.MaxValue) targetSize = (size * 1415) / 1000;

                    if (size > _mapSize / 2 || size >= targetSize)
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
                        int[] colorArr = new int[5];
                        foreach (var p in closestPointsList)
                        {
                            colorArr[p.Color]++;
                        }
                        colorArr[0] = 0;
                        var maxColor = 0;
                        var maxColorCount = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            if (colorArr[i] > maxColorCount)
                            {
                                maxColorCount = colorArr[i];
                                maxColor = i;
                            }
                        }
                        if (maxColor == 0)
                        {
                            Console.WriteLine("Fugged");
                        }
                        //AddPoint(coord.x, coord.y, mostCommonType);
                        return (byte) maxColor;
                    }
                }


                size += 1;
            }
            return 0x0;
        }


        private int ClassifyRandomRed(int kClosest)
        {
            int x = rnd.Next(-5000 / scale, 500 / scale);
            int y = rnd.Next(-5000 / scale, 500 / scale);
            int correct = 0;
            if (rnd.Next(100) == 1)
            {
                x = rnd.Next(-5000 / scale, 5000 / scale + 1);
                y = rnd.Next(-5000 / scale, 5000 / scale + 1);
                byte color = FindClosestType(kClosest, (x, y), true);
                AddPoint(x, y, color);
                if (color == 1) correct = 1;
            }
            else
            {
                byte color = FindClosestType(kClosest, (x, y), true);
                AddPoint(x, y, color);
                if (color == 1) correct = 1;
            }
            return correct;
        }

        private int ClassifyRandomGreen(int kClosest)
        {
            int x = rnd.Next(-500 / scale + 1, 5000 / scale + 1);
            int y = rnd.Next(-5000 / scale, 500 / scale);
            int correct = 0;
            if (rnd.Next(100) == 1)
            {
                x = rnd.Next(-5000 / scale, 5000 / scale + 1);
                y = rnd.Next(-5000 / scale, 5000 / scale + 1);
                byte color = FindClosestType(kClosest, (x, y), true);
                AddPoint(x, y, color);
                if (color == 2) correct = 1;
            }
            else
            {
                byte color = FindClosestType(kClosest, (x, y), true);
                AddPoint(x, y, color);
                if (color == 2) correct = 1;
            }
            return correct;
        }

        private int ClassifyRandomBlue(int kClosest)
        {
            int x = rnd.Next(-5000 / scale, 500 / scale);
            int y = rnd.Next(-500 / scale + 1, 5000 / scale + 1);
            int correct = 0;
            if (rnd.Next(100) == 1)
            {
                x = rnd.Next(-5000 / scale, 5000 / scale + 1);
                y = rnd.Next(-5000 / scale, 5000 / scale + 1);
                byte color = FindClosestType(kClosest, (x, y), true);
                AddPoint(x, y, color);
                if (color == 3) correct = 1;
            }
            else
            {
                byte color = FindClosestType(kClosest, (x, y), true);
                AddPoint(x, y, color);
                if (color == 3) correct = 1;
            }
            return correct;
        }

        private int ClassifyRandomPurple(int kClosest)
        {
            int x = rnd.Next(-500 / scale + 1, 5000 / scale + 1);
            int y = rnd.Next(-500 / scale + 1, 5000 / scale + 1);
            int correct = 0;
            if (rnd.Next(100) == 1)
            {
                x = rnd.Next(-5000 / scale, 5000 / scale + 1);
                y = rnd.Next(-5000 / scale, 5000 / scale + 1);
                byte color = FindClosestType(kClosest, (x, y), true);
                AddPoint(x, y, color);
                if (color == 4) correct = 1;
            }
            else
            {
                byte color = FindClosestType(kClosest, (x, y), true);
                AddPoint(x, y, color);
                if (color == 4) correct = 1;
            }
            return correct;
        }

        private void AddDefaultPoints()
        {
            //Red
            AddPoint(-4500 / scale, -4400 / scale, 1);
            AddPoint(-4100 / scale, -3000 / scale, 1);
            AddPoint(-1800 / scale, -2400 / scale, 1);
            AddPoint(-2500 / scale, -3400 / scale, 1);
            AddPoint(-2000 / scale, -1400 / scale, 1);


            //Green
            AddPoint(4500 / scale, -4400 / scale, 2);
            AddPoint(4100 / scale, -3000 / scale, 2);
            AddPoint(1800 / scale, -2400 / scale, 2);
            AddPoint(2500 / scale, -3400 / scale, 2);
            AddPoint(2000 / scale, -1400 / scale, 2);

            //Blue
            AddPoint(-4500 / scale, 4400 / scale, 3);
            AddPoint(-4100 / scale, 3000 / scale, 3);
            AddPoint(-1800 / scale, 2400 / scale, 3);
            AddPoint(-2500 / scale, 3400 / scale, 3);
            AddPoint(-2000 / scale, 1400 / scale, 3);

            //Purple
            AddPoint(4500 / scale, 4400 / scale, 4);
            AddPoint(4100 / scale, 3000 / scale, 4);
            AddPoint(1800 / scale, 2400 / scale, 4);
            AddPoint(2500 / scale, 3400 / scale, 4);
            AddPoint(2000 / scale, 1400 / scale, 4);

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
