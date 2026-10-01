using System;

namespace FarmApp
{
    /// <summary>
    /// Представляет фермера, ухаживающего за животными.
    /// </summary>
    public class Farmer
    {
        private int _experience;

        /// <summary>
        /// Уникальный идентификатор фермера.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Полное имя фермера.
        /// </summary>
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// Стаж работы. Не может быть отрицательным.
        /// </summary>
        public int Experience
        {
            get => _experience;
            init => _experience = ValidateExperience(value);
        }

        /// <summary>
        /// Признак опытного фермера: стаж больше 5 лет.
        /// </summary>
        public bool IsExperienced => Experience > 5;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public Farmer()
        {
        }

        /// <summary>
        /// Конструктор с полным набором параметров и проверкой значений.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Если имя пустое, идентификатор отрицательный
        /// или стаж отрицательный.
        /// </exception>
        public Farmer(int id, string fullName, int experience)
        {
            Id = ValidateId(id);
            FullName = ValidateFullName(fullName);
            Experience = ValidateExperience(experience);
        }

        /// <summary>
        /// Конструктор без идентификатора (удобно для новых записей).
        /// </summary>
        public Farmer(string fullName, int experience)
            : this(0, fullName, experience)
        {
        }

        /// <summary>
        /// Возвращает краткую информацию о фермере.
        /// </summary>
        public string GetInfo()
        {
            return $"{FullName} ({Experience} {GetYearWord(Experience)} опыта)";
        }

        /// <summary>
        /// Проверяет идентификатор: не может быть отрицательным.
        /// </summary>
        private static int ValidateId(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException(
                    "Идентификатор не может быть отрицательным.", nameof(Id));
            }
            return value;
        }

        /// <summary>
        /// Проверяет имя: не может быть пустым или состоять из пробелов.
        /// </summary>
        private static string ValidateFullName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Имя не может быть пустым.", nameof(FullName));
            }
            return value;
        }

        /// <summary>
        /// Проверяет стаж: не может быть отрицательным.
        /// </summary>
        private static int ValidateExperience(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Стаж не может быть отрицательным.", nameof(Experience));
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
