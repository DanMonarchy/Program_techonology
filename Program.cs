 using System;
using System.Collections.Generic;

namespace FarmApp
{
    /// <summary>
    /// Точка входа в программу. Отвечает за выбор источника данных
    /// и демонстрацию аналитических методов.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Уникальный ключ источника данных "в памяти".
        /// </summary>
        private static string InMemoryKey { get; init; } = "inmemory";

        /// <summary>
        /// Уникальный ключ источника данных "CSV".
        /// </summary>
        private static string CsvKey { get; init; } = "csv";

        /// <summary>
        /// Главный метод программы.
        /// </summary>
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Farmer> farmers;
            List<Pen> pens;
            List<Animal> animals;

            Console.WriteLine("Выберите источник данных:");
            Console.WriteLine($"{InMemoryKey} - InMemoryRepository");
            Console.WriteLine($"{CsvKey}      - CsvRepository");
            Console.Write("Ваш выбор: ");

            string key = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            try
            {
                // switch expression по уникальному ключу
                (farmers, pens, animals) = key switch
                {
                    var k when k == InMemoryKey => LoadFromInMemory(),
                    var k when k == CsvKey      => LoadFromCsv(),
                    _ => throw new ArgumentException("Неверный выбор")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении данных: {ex.Message}");
                return;
            }

            // Дальше вся логика работает только со списками
            // и не знает, откуда пришли данные.

            Console.WriteLine();

            Farmer? foundFarmer = FindFarmerByAnimalName(animals, farmers, "Бурёнка");
            Console.WriteLine("1. FindFarmer(\"Бурёнка\"): " +
                (foundFarmer != null ? foundFarmer.GetInfo() : "null"));

            Pen? foundPen = FindPenByAnimalName(animals, pens, "Бурёнка");
            Console.WriteLine("2. FindPen(animal \"Бурёнка\"): " +
                (foundPen != null ? foundPen.GetInfo() : "null"));

            int totalWeight = GetTotalWeight(animals);
            Console.WriteLine($"3. GetTotalWeight: {totalWeight} кг");

            Dictionary<string, Animal> heaviestPerPen = GetHeaviestAnimalPerPen(animals, pens);
            Console.Write("4. GetHeaviestAnimalPerPen: ");
            PrintHeaviestPerPen(heaviestPerPen, pens);

            Console.WriteLine("5. PrintAllAnimals:");
            PrintAllAnimals(animals, farmers, pens);

            Console.WriteLine();
            Farmer? notFound = FindFarmerByAnimalName(animals, farmers, "Неизвестное животное");
            Console.WriteLine("Не найдено: FindFarmer(\"Неизвестное животное\") -> " +
                (notFound != null ? notFound.GetInfo() : "null"));
        }

        /// <summary>
        /// Загружает данные из InMemoryRepository.
        /// </summary>
        private static (List<Farmer>, List<Pen>, List<Animal>) LoadFromInMemory()
        {
            InMemoryRepository repository = new InMemoryRepository();
            return (repository.GetFarmers(), repository.GetPens(), repository.GetAnimals());
        }

        /// <summary>
        /// Загружает данные из CsvRepository.
        /// </summary>
        private static (List<Farmer>, List<Pen>, List<Animal>) LoadFromCsv()
        {
            CsvRepository repository = new CsvRepository("data");
            return (repository.GetFarmers(), repository.GetPens(), repository.GetAnimals());
        }

        /// <summary>
        /// Находит фермера, ухаживающего за животным с заданным именем.
        /// </summary>
        public static Farmer? FindFarmerByAnimalName(List<Animal> animals, List<Farmer> farmers, string animalName)
        {
            if (animals == null || farmers == null) return null;

            Animal? foundAnimal = null;
            for (int i = 0; i < animals.Count; i++)
            {
                if (animals[i].Name == animalName)
                {
                    foundAnimal = animals[i];
                    break;
                }
            }

            if (foundAnimal == null) return null;

            for (int i = 0; i < farmers.Count; i++)
            {
                if (farmers[i].Id == foundAnimal.FarmerId)
                {
                    return farmers[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Находит загон, в котором содержится животное с заданным именем.
        /// </summary>
        public static Pen? FindPenByAnimalName(List<Animal> animals, List<Pen> pens, string animalName)
        {
            if (animals == null || pens == null) return null;

            Animal? foundAnimal = null;
            for (int i = 0; i < animals.Count; i++)
            {
                if (animals[i].Name == animalName)
                {
                    foundAnimal = animals[i];
                    break;
                }
            }

            if (foundAnimal == null) return null;

            for (int i = 0; i < pens.Count; i++)
            {
                if (pens[i].Id == foundAnimal.PenId)
                {
                    return pens[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Считает суммарный вес всех животных в списке.
        /// </summary>
        public static int GetTotalWeight(List<Animal> animals)
        {
            if (animals == null) return 0;

            int totalWeight = 0;
            for (int i = 0; i < animals.Count; i++)
            {
                totalWeight += animals[i].Weight;
            }
            return totalWeight;
        }

        /// <summary>
        /// Находит самое тяжёлое животное в каждом загоне.
        /// </summary>
        public static Dictionary<string, Animal> GetHeaviestAnimalPerPen(List<Animal> animals, List<Pen> pens)
        {
            Dictionary<string, Animal> result = new Dictionary<string, Animal>();

            if (animals == null || pens == null) return result;

            for (int i = 0; i < pens.Count; i++)
            {
                Pen pen = pens[i];
                Animal? heaviest = null;

                for (int j = 0; j < animals.Count; j++)
                {
                    if (animals[j].PenId == pen.Id)
                    {
                        if (heaviest == null || animals[j].Weight > heaviest.Weight)
                        {
                            heaviest = animals[j];
                        }
                    }
                }

                if (heaviest != null)
                {
                    string resultKey = $"Загон №{pen.Number}";
                    result[resultKey] = heaviest;
                }
            }

            return result;
        }

        /// <summary>
        /// Печатает результат GetHeaviestAnimalPerPen.
        /// </summary>
        private static void PrintHeaviestPerPen(Dictionary<string, Animal> heaviestPerPen, List<Pen> pens)
        {
            if (heaviestPerPen == null || pens == null) return;

            List<string> parts = new List<string>();

            for (int i = 0; i < pens.Count; i++)
            {
                string resultKey = $"Загон №{pens[i].Number}";
                if (heaviestPerPen.ContainsKey(resultKey))
                {
                    Animal animal = heaviestPerPen[resultKey];
                    parts.Add($"Загон {pens[i].Number} — {animal.Name} ({animal.Weight})");
                }
            }

            for (int i = 0; i < parts.Count; i++)
            {
                Console.Write(parts[i]);
                if (i < parts.Count - 1)
                {
                    Console.Write(", ");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Печатает информацию о каждом животном вместе с его фермером и загоном.
        /// </summary>
        public static void PrintAllAnimals(List<Animal> animals, List<Farmer> farmers, List<Pen> pens)
        {
            if (animals == null || farmers == null || pens == null) return;

            for (int i = 0; i < animals.Count; i++)
            {
                Animal animal = animals[i];

                Farmer? farmer = null;
                for (int j = 0; j < farmers.Count; j++)
                {
                    if (farmers[j].Id == animal.FarmerId)
                    {
                        farmer = farmers[j];
                        break;
                    }
                }

                Pen? pen = null;
                for (int j = 0; j < pens.Count; j++)
                {
                    if (pens[j].Id == animal.PenId)
                    {
                        pen = pens[j];
                        break;
                    }
                }

                string farmerName = farmer != null ? farmer.FullName : "—";
                string penNumber = pen != null ? pen.Number.ToString() : "—";

                Console.WriteLine($"\"{animal.GetInfo()}\" — фермер {farmerName}, загон №{penNumber}");
            }
        }
    }
}
