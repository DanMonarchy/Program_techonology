using System;

namespace FarmApp
{
    /// <summary>
    /// Представляет загон, в котором содержатся животные.
    /// </summary>
    public class Pen
    {
        /// <summary>
        /// Уникальный идентификатор загона.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Номер загона.
        /// </summary>
        public int Number { get; init; }

        /// <summary>
        /// Площадь загона в квадратных метрах.
        /// </summary>
        public int Area { get; init; }

        /// <summary>
        /// Тип загона (например, "коровник").
        /// </summary>
        public string Type { get; init; } = string.Empty;

        /// <summary>
        /// Признак большого загона: площадь больше 100 м².
        /// </summary>
        public bool IsBig => Area > 100;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public Pen()
        {
        }

        /// <summary>
        /// Конструктор с полным набором параметров и проверкой значений.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Если идентификатор отрицательный, номер меньше 1,
        /// площадь отрицательная или тип пустой.
        /// </exception>
        public Pen(int id, int number, int area, string type)
        {
            Id = ValidateId(id);
            Number = ValidateNumber(number);
            Area = ValidateArea(area);
            Type = ValidateType(type);
        }

        /// <summary>
        /// Конструктор без идентификатора (удобно для новых записей).
        /// </summary>
        public Pen(int number, int area, string type)
            : this(0, number, area, type)
        {
        }

        /// <summary>
        /// Возвращает краткую информацию о загоне.
        /// </summary>
        public string GetInfo()
        {
            return $"Загон №{Number} ({Area} м², {Type})";
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
        /// Проверяет номер загона: должен быть больше нуля.
        /// </summary>
        private static int ValidateNumber(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException(
                    "Номер загона должен быть больше нуля.", nameof(Number));
            }
            return value;
        }

        /// <summary>
        /// Проверяет площадь: не может быть отрицательной.
        /// </summary>
        private static int ValidateArea(int value)
        {
            if (value < 0)
            {
                throw new ArgumentException(
                    "Площадь не может быть отрицательной.", nameof(Area));
            }
            return value;
        }

        /// <summary>
        /// Проверяет тип: не может быть пустым или состоять из пробелов.
        /// </summary>
        private static string ValidateType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Тип не может быть пустым.", nameof(Type));
            }
            return value;
        }
    }
}
