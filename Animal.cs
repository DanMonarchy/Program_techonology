using System;

namespace FarmApp
{
    /// <summary>
    /// Представляет животное на ферме.
    /// </summary>
    public class Animal
    {
        private int _age;
        private int _weight;

        /// <summary>
        /// Уникальный идентификатор животного.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Имя животного.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Идентификатор фермера, ухаживающего за животным.
        /// </summary>
        public int FarmerId { get; init; }

        /// <summary>
        /// Идентификатор загона, в котором содержится животное.
        /// </summary>
        public int PenId { get; init; }

        /// <summary>
        /// Возраст животного. Не может быть отрицательным.
        /// </summary>
        public int Age
        {
            get => _age;
            init => _age = ValidateAge(value);
        }

        /// <summary>
        /// Вес животного. Не может быть отрицательным.
        /// </summary>
        public int Weight
        {
            get => _weight;
            init => _weight = ValidateWeight(value);
        }

        /// <summary>
        /// Признак взрослого животного: возраст больше 2 лет.
        /// </summary>
        public bool IsAdult => Age > 2;

        /// <summary>
        /// Признак тяжёлого животного: вес больше 500 кг.
        /// </summary>
        public bool IsHeavy => Weight > 500;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public Animal()
        {
        }

        /// <summary>
        /// Конструктор с полным набором параметров и проверкой значений.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Если имя пустое, идентификаторы отрицательные,
        /// возраст или вес отрицательные.
        /// </exception>
        public Animal(int id, string name, int farmerId, int penId, int age, int weight)
        {
            Id = ValidateId(id, nameof(Id));
            Name = ValidateName(name);
            FarmerId = ValidateId(farmerId, nameof(FarmerId));
            PenId = ValidateId(penId, nameof(PenId));
            Age = ValidateAge(age);
            Weight = ValidateWeight(weight);
        }

        /// <summary>
        /// Конструктор без идентификатора (удобно для новых записей).
        /// </summary>
        public Animal(string name, int farmerId, int penId, int age, int weight)
            : this(0, name, farmerId, penId, age, weight)
        {
        }

        /// <summary>
        /// Возвращает краткую информацию о животном.
        /// </summary>
        public string GetInfo()
        {
            return $"{Name} ({Age} {GetYearWord(Age)}, {Weight} кг)";
        }

        /// <summary>
        /// Проверяет идентификатор: не может быть отрицательным.
        /// </summary>
        private static int ValidateId(int value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentException(
                    $"Идентификатор {paramName} не может быть отрицательным.", paramName);
            }
            return value;
        }

        /// <summary>
        /// Проверяет имя: не может быть пустым или состоять из пробелов.
        /// </summary>
        private static string ValidateName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Имя не может быть пустым.", nameof(Name));
            }
            return value;
        }

        /// <summary>
        /// Проверяет возраст: не может быть отрицательным.
        /// </summary>
        private static int ValidateAge(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Возраст не может быть отрицательным.", nameof(Age));
            }
            return value;
        }

        /// <summary>
        /// Проверяет вес: не может быть отрицательным.
        /// </summary>
        private static int ValidateWeight(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Вес не может быть отрицательным.", nameof(Weight));
            }
            return value;
        }

        /// <summary>
        /// Подбирает правильную форму слова "год" для числа лет.
        /// </summary>
        private static string GetYearWord(int years)
        {
            int rem100 = years % 100;
            int rem10 = years % 10;

            if (rem100 >= 11 && rem100 <= 14) return "лет";
            if (rem10 == 1) return "год";
            if (rem10 >= 2 && rem10 <= 4) return "года";
            return "лет";
        }
    }
}
