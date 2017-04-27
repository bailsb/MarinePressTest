using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LineSegmentIntersection;

namespace MarinePressTest
{
    public class Program
    {
        const double INTERSECTWARNING = 1.0;

        public static void Main(string[] args)
        {
            Vessel[] vessels = new Vessel[3];

            //Create Vessels
            String vesselName = "Vessel 1";
            Position[] positions = new Position[8];
            positions[0] = new Position(5, 5, new DateTime(2020, 1, 1, 7, 40, 0, DateTimeKind.Utc));
            positions[1] = new Position(9, 9, new DateTime(2020, 1, 1, 7, 55, 0, DateTimeKind.Utc));
            positions[2] = new Position(15, 11, new DateTime(2020, 1, 1, 8, 25, 0, DateTimeKind.Utc));
            positions[3] = new Position(22, 14, new DateTime(2020, 1, 1, 8, 50, 0, DateTimeKind.Utc));
            positions[4] = new Position(29, 16, new DateTime(2020, 1, 1, 9, 6, 0, DateTimeKind.Utc));
            positions[5] = new Position(35, 17, new DateTime(2020, 1, 1, 9, 24, 0, DateTimeKind.Utc));
            positions[6] = new Position(41, 20, new DateTime(2020, 1, 1, 9, 45, 0, DateTimeKind.Utc));
            positions[7] = new Position(48, 23, new DateTime(2020, 1, 1, 10, 13, 0, DateTimeKind.Utc));
            vessels[0] = new Vessel(vesselName, positions);

            vesselName = "Vessel 2";
            positions = new Position[8];
            positions[0] = new Position(35, 35, new DateTime(2020, 1, 1, 8, 24, 0, DateTimeKind.Utc));
            positions[1] = new Position(41, 33, new DateTime(2020, 1, 1, 8, 48, 0, DateTimeKind.Utc));
            positions[2] = new Position(45, 31, new DateTime(2020, 1, 1, 9, 15, 0, DateTimeKind.Utc));
            positions[3] = new Position(47, 27, new DateTime(2020, 1, 1, 9, 43, 0, DateTimeKind.Utc));
            positions[4] = new Position(52, 25, new DateTime(2020, 1, 1, 10, 12, 0, DateTimeKind.Utc));
            positions[5] = new Position(56, 23, new DateTime(2020, 1, 1, 10, 37, 0, DateTimeKind.Utc));
            positions[6] = new Position(62, 19, new DateTime(2020, 1, 1, 10, 58, 0, DateTimeKind.Utc));
            positions[7] = new Position(67, 17, new DateTime(2020, 1, 1, 11, 24, 0, DateTimeKind.Utc));
            vessels[1] = new Vessel(vesselName, positions);

            vesselName = "Vessel 3";
            positions = new Position[8];
            positions[0] = new Position(30, 5, new DateTime(2020, 1, 1, 7, 55, 0, DateTimeKind.Utc));
            positions[1] = new Position(29, 9, new DateTime(2020, 1, 1, 8, 20, 0, DateTimeKind.Utc));
            positions[2] = new Position(26, 15, new DateTime(2020, 1, 1, 8, 49, 0, DateTimeKind.Utc));
            positions[3] = new Position(24, 18, new DateTime(2020, 1, 1, 9, 14, 0, DateTimeKind.Utc));
            positions[4] = new Position(21, 23, new DateTime(2020, 1, 1, 9, 40, 0, DateTimeKind.Utc));
            positions[5] = new Position(19, 27, new DateTime(2020, 1, 1, 10, 8, 0, DateTimeKind.Utc));
            positions[6] = new Position(16, 31, new DateTime(2020, 1, 1, 10, 24, 0, DateTimeKind.Utc));
            positions[7] = new Position(15, 37, new DateTime(2020, 1, 1, 10, 43, 0, DateTimeKind.Utc));
            vessels[2] = new Vessel(vesselName, positions);
            //Calculate Distance each Vessel traveled
            foreach (Vessel v in vessels)
            {
                if (v.positions.Length > 1)
                {
                    TimeSpan time = v.positions[v.positions.Length - 1].timestamp - v.positions[0].timestamp;
                    double distance = 0;
                    double speed = 0;
                    for (int i = 0; i < v.positions.Length - 1; i++)
                    {
                        distance += GetDistance(v.positions[i].vector, v.positions[i + 1].vector);
                    }

                    speed = distance / time.TotalHours;

                    Console.WriteLine("Vessel '{0}' has travelled {1:F}kms at an average of {2:F}km/h", v.name, distance, speed);
                }
                else
                {
                    Console.WriteLine("Vessel '" + v.name + "' has not yet moved");
                }
            }

            //Test Line Segments
            for (int i = 0; i < vessels.Length - 1; i++)
            {
                for (int j = i+1; j < vessels.Length; j++)
                {
                    CompareVesselPaths(vessels[i], vessels[j]);
                }
            }           

            Console.ReadLine();
        }

        public static void CompareVesselPaths(Vessel v1, Vessel v2)
        {
            Vector intersect = new Vector();
            DateTime v1Time;
            DateTime v2Time;

            for (int i = 0; i < v1.positions.Length - 1; i++)
            {
                // k = positions of vessel B = vessel[i+1]
                for (int j = 0; j < v2.positions.Length - 1; j++)
                {
                    if (LineSegment.LineSegementsIntersect( v1.positions[i].vector,
                                                            v1.positions[i + 1].vector,
                                                            v2.positions[j].vector,
                                                            v2.positions[j + 1].vector,
                                                            out intersect,
                                                            true))
                    {
                        //Output intersections
                        Console.WriteLine("Vessels '{0}' and {1} intersect at {2}", v1.name, v2.name, intersect);

                        //Test for times of intersect - we'll use ratio of delta distance and delta time
                        v1Time = GetTimeOfIntersect(v1.positions[i], v1.positions[i + 1], intersect);
                        v2Time = GetTimeOfIntersect(v2.positions[j], v2.positions[j + 1], intersect);
                        if ((v1Time - v2Time).TotalHours < INTERSECTWARNING)
                        {
                            Console.WriteLine("\nWarning! '{0}' and '{1}' will both be at point {2} at approximately {3} and {4} respectively",
                                                v1.name,
                                                v2.name,
                                                intersect,
                                                v1Time,
                                                v2Time);
                            }
                    }
                }
            }
        }

        // Returns estimated time of intersection
        public static DateTime GetTimeOfIntersect(Position start, Position end, Vector intersect)
        {
            double d2End = GetDistance(start.vector, end.vector);
            double d2Intersect = GetDistance(start.vector, intersect);
            double mins2End = (end.timestamp - start.timestamp).TotalMinutes; //Using Minutes as smallest value

            double mins2Intesect = mins2End * (d2Intersect / d2End);

            return start.timestamp.AddMinutes(mins2Intesect);
        }

        public static double GetDistance(Vector start, Vector end)
        {
            double dx = start.X - end.X;
            double dy = start.Y - end.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public class Vessel
    {
        public String name { get; set; }
        public Position[] positions { get; set; }

        public Vessel(string name, Position[] positions)
        {
            this.name = name;
            this.positions = positions;
        }
    }

    public class Position
    {
        public Vector vector { get; private set; }
        public DateTime timestamp { get; private set; }

        public Position (double X, double Y, DateTime timestamp)
        {
            vector = new Vector(X, Y);
            this.timestamp = timestamp;
        }
    }
}
