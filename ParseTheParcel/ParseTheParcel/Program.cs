using System;
using System.IO;
using System.Collections.Generic;

namespace ParseTheParcel
{

    /// <summary>
    /// Simple program that reads in file to create a stored list of parcel specifications.
    /// Reads in a user's item sizes and then finds the cheapest suitable parcel size and returns it back to the user.
    /// Inputs (both the file parcels and the user) are sorted smallest to largest to ensure different orientations can still be matched.
    /// 
    /// If I had more time to work on this solution I would have liked to have added functionality to sort by parcel volume.
    /// I would have then checked to see if I could find the same priced parcel with the smallest suitable volume.
    /// Another cool extention on this would have been to add signing, overnight delivery, and tracking costs.
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            List<Parcel> StoredParcels = new List<Parcel>();

            //some manual entires if you choose not to load a file. 
            /*
            StoredParcels.Add(new Parcel("Small", 5.00, 5, 200, 150, 300));
            StoredParcels.Add(new Parcel("Huge", 50.00, 50, 3000, 2000, 1000));
            StoredParcels.Add(new Parcel("Medium", 7.50, 10, 300, 200, 400));
            StoredParcels.Add(new Parcel("Large", 8.50, 25, 400, 250, 600));
            */

            //load file
            LoadFile(ref StoredParcels);

            //get user input from the console
            double ValueWeight = GetInput("weight (kg)");
            double ValueLength = GetInput("length (mm)");
            double ValueHeight = GetInput("height (mm)");
            double ValueBreadth = GetInput("breadth (mm)");

            // the users measurements sorted smallest to larges
            List<double> UserMeasurements = SortedMeasurements(ValueLength, ValueHeight, ValueBreadth);

            //Find a parcel to match the inputs and return to user - Note if I had more time I would find the smallest volume with the same cost
            Parcel UserParcel = FindParcel(StoredParcels, ValueWeight, UserMeasurements[0], UserMeasurements[1], UserMeasurements[2]);

            //Return data to user
            if (UserParcel == null)
            {
                Console.WriteLine("No suitable parcel found :(");
            }
            else
            {
                Console.WriteLine($"Parcel {UserParcel.Code} ({string.Format("${0:N2}", UserParcel.Cost)}) will be suitable for your needs! :)");
            }

        }

        /// <summary>
        /// Load file of parcel sizes
        /// </summary>
        /// <param name="StoredParcels">List to populate with parcels</param>
        private static void LoadFile(ref List<Parcel> StoredParcels)
        {
            const int ExpectedItems = 6;
            const String FilePath = "../parcelsizes.csv"; // Enter the file location here (ParseTheParcel/bin/debug folder)

            using var fileReader = new StreamReader(FilePath);

            while (!fileReader.EndOfStream)
            {
                String Fileline = fileReader.ReadLine();
                string[] FileValues = Fileline.Split('^'); // This is the file delimeter ^

                //do some basic file validation, check the number of items and that the values can be parsed to a double. 
                if (FileValues.Length != ExpectedItems)
                {
                    throw new Exception($"File row length {FileValues.Length} is not correct, expected {ExpectedItems}");
                }

                for (int i = 0; i < FileValues.Length; i++)
                {
                    //skip over parcel name as this is a string
                    if (i != 0)
                    {
                        double testVal;
                        if (!double.TryParse(FileValues[i], out testVal))
                        {
                            throw new Exception($"Could not parse value to double, value: {FileValues[i]}");
                        }
                    }
                }

                List<double> Measurements = SortedMeasurements(double.Parse(FileValues[3]), double.Parse(FileValues[4]), double.Parse(FileValues[5]));

                // Measurements 0 is the smallest value
                StoredParcels.Add(new Parcel(FileValues[0], double.Parse(FileValues[1]), double.Parse(FileValues[2]), Measurements[0], Measurements[1], Measurements[2]));
            }
        }

        /// <summary>
        /// Gets input from a user for a specified value
        /// </summary>
        /// <param name="ValueType">Describes the value to measure and its units e.g. length (mm)</param>
        /// <returns>The user input value (double)</returns>
        private static double GetInput(string ValueType)
        {
            double value;

            while (true)
            {
                Console.WriteLine($"Please enter item {ValueType}");
                if (!double.TryParse(Console.ReadLine(), out value))
                {
                    Console.WriteLine("Please only enter numbers, e.g. 0.5, 5, 500");
                }
                else
                {
                    if (value <= 0)
                    {
                        Console.WriteLine("Please enter a number greater than 0");
                    }
                    else
                    {
                        return value;
                    }
                }
            }
        }

        /// <summary>
        /// Takes in measurements and adds them to a list to be sorted and returns the sorted list. 
        /// </summary>
        /// <param name="length">Input length</param>
        /// <param name="height">Input height</param>
        /// <param name="breadth">Input breadth</param>
        /// <returns></returns>
        private static List<Double> SortedMeasurements(double length, double height, double breadth)
        {
            List<Double> Measurements = new List<double>();
            Measurements.Add(length);
            Measurements.Add(height);
            Measurements.Add(breadth);

            Measurements.Sort(); //compare the measurement values and sort them smallest to largest

            return Measurements;
        }

        /// <summary>
        /// Compare the parcels by cost
        /// </summary>
        /// <param name="parcelX">Parcel x to compare</param>
        /// <param name="parcelY">Parcel y to compare</param>
        /// <returns></returns>
        private static int CompareByCost(Parcel parcelX, Parcel parcelY)
        {
            return parcelX.Cost.CompareTo(parcelY.Cost);
        }

        /// <summary>
        /// Simple loop that searches for a parcel that matches the user inputs.
        /// </summary>
        /// <param name="StoredParcels">A list of parcels</param>
        /// <param name="weight">The user input weight</param>
        /// <param name="length">The user input length</param>
        /// <param name="height">The user input height</param>
        /// <param name="breadth">The user input breadth</param>
        /// <returns></returns>
        private static Parcel FindParcel(List<Parcel> StoredParcels, double weight, double length, double height, double breadth)
        {

            StoredParcels.Sort(CompareByCost); //Sort the file by cost

            foreach (Parcel itemParcel in StoredParcels)
            {
                if (itemParcel.Weight >= weight && itemParcel.Length >= length && itemParcel.Height >= height && itemParcel.breadth >= breadth)
                {
                    return itemParcel;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Parcel object. 
    /// </summary>
    class Parcel
    {
        public string Code { get; set; }
        public double Cost { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public double breadth { get; set; }
        public double Volume { get; set; }

        public Parcel(string inputCode, double inputCost, double inputWeight, double inputLength, double inputHeight, double inputbreadth)
        {
            Code = inputCode;
            Cost = inputCost;
            Weight = inputWeight;
            Length = inputLength;
            Height = inputHeight;
            breadth = inputbreadth;
            Volume = inputbreadth * inputHeight * inputHeight;
        }
    }
}