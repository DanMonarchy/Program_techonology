using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FarmApp
{
    
    public class CsvRepository
    {
        private string _basePath;

        public CsvRepository(string basePath)
        {
            _basePath = basePath;
        }

        
        public List<Farmer> GetFarmers()
        {
            List<Farmer> result = new List<Farmer>();
            string path = Path.Combine(_basePath, "farmers.csv");
            string[] lines = File.ReadAllLines(path);

            if (lines.Length < 2)
            {
                Console.WriteLine($"Файл {path} пуст или содержит только заголовок.");
                return result;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] parts = lines[i].Split(',');
                if (parts.Length < 3) continue;

                Farmer farmer = new Farmer
                {
                    Id = int.Parse(parts[0], CultureInfo.InvariantCulture),
                    FullName = parts[1],
                    Experience = int.Parse(parts[2], CultureInfo.InvariantCulture)
                };
                result.Add(farmer);
            }

            return result;
        }

        
        public List<Pen> GetPens()
        {
            List<Pen> result = new List<Pen>();
            string path = Path.Combine(_basePath, "pens.csv");
            string[] lines = File.ReadAllLines(path);

            if (lines.Length < 2)
            {
                Console.WriteLine($"Файл {path} пуст или содержит только заголовок.");
                return result;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] parts = lines[i].Split(',');
                if (parts.Length < 4) continue;

                Pen pen = new Pen
                {
                    Id = int.Parse(parts[0], CultureInfo.InvariantCulture),
                    Number = int.Parse(parts[1], CultureInfo.InvariantCulture),
                    Area = int.Parse(parts[2], CultureInfo.InvariantCulture),
                    Type = parts[3]
                };
                result.Add(pen);
            }

            return result;
        }

        
        public List<Animal> GetAnimals()
        {
            List<Animal> result = new List<Animal>();
            string path = Path.Combine(_basePath, "animals.csv");
            string[] lines = File.ReadAllLines(path);

            if (lines.Length < 2)
            {
                Console.WriteLine($"Файл {path} пуст или содержит только заголовок.");
                return result;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] parts = lines[i].Split(',');
                if (parts.Length < 6) continue;

                Animal animal = new Animal
                {
                    Id = int.Parse(parts[0], CultureInfo.InvariantCulture),
                    Name = parts[1],
                    FarmerId = int.Parse(parts[2], CultureInfo.InvariantCulture),
                    PenId = int.Parse(parts[3], CultureInfo.InvariantCulture),
                    Age = int.Parse(parts[4], CultureInfo.InvariantCulture),
                    Weight = int.Parse(parts[5], CultureInfo.InvariantCulture)
                };
                result.Add(animal);
            }

            return result;
        }
    }
}                }

                string[] parts = lines[i].Split(',');
                if (parts.Length < 3)
                {
                    continue;
                }

                Farmer farmer = new Farmer();
                farmer.Id = int.Parse(parts[0], CultureInfo.InvariantCulture);
                farmer.FullName = parts[1];
                farmer.Experience = int.Parse(parts[2], CultureInfo.InvariantCulture);
                result.Add(farmer);
            }

            return result;
        }

        
        public List<Pen> GetPens()
        {
            List<Pen> result = new List<Pen>();
            string path = Path.Combine(_basePath, "pens.csv");
            string[] lines = File.ReadAllLines(path);

            if (lines.Length < 2)
            {
                Console.WriteLine($"Файл {path} пуст или содержит только заголовок.");
                return result;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                string[] parts = lines[i].Split(',');
                if (parts.Length < 4)
                {
                    continue;
                }

                Pen pen = new Pen();
                pen.Id = int.Parse(parts[0], CultureInfo.InvariantCulture);
                pen.Number = int.Parse(parts[1], CultureInfo.InvariantCulture);
                pen.Area = int.Parse(parts[2], CultureInfo.InvariantCulture);
                pen.Type = parts[3];
                result.Add(pen);
            }

            return result;
        }

        
        public List<Animal> GetAnimals()
        {
            List<Animal> result = new List<Animal>();
            string path = Path.Combine(_basePath, "animals.csv");
            string[] lines = File.ReadAllLines(path);

            if (lines.Length < 2)
            {
                Console.WriteLine($"Файл {path} пуст или содержит только заголовок.");
                return result;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                string[] parts = lines[i].Split(',');
                if (parts.Length < 6)
                {
                    continue;
                }

                Animal animal = new Animal();
                animal.Id = int.Parse(parts[0], CultureInfo.InvariantCulture);
                animal.Name = parts[1];
                animal.FarmerId = int.Parse(parts[2], CultureInfo.InvariantCulture);
                animal.PenId = int.Parse(parts[3], CultureInfo.InvariantCulture);
                animal.Age = int.Parse(parts[4], CultureInfo.InvariantCulture);
                animal.Weight = int.Parse(parts[5], CultureInfo.InvariantCulture);
                result.Add(animal);
            }

            return result;
        }
    }
}
