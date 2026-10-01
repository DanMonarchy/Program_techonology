using System;
using System.Collections.Generic;

namespace FarmApp
{
    public class InMemoryRepository
    {
        private List<Farmer> _farmers;
        private List<Pen> _pens;
        private List<Animal> _animals;

        public InMemoryRepository()
        {
            _farmers = new List<Farmer>
            {
                new Farmer(1, "Иванов И.И.",   10),
                new Farmer(2, "Петров П.П.",   3),
                new Farmer(3, "Сидорова А.А.", 7),
                new Farmer(4, "Кузнецов К.К.", 1),
                new Farmer(5, "Орлова О.О.",   15)
            };

            _pens = new List<Pen>
            {
                new Pen(1, 1, 90,  "коровник"),
                new Pen(2, 2, 60,  "телятник"),
                new Pen(3, 3, 150, "коровник"),
                new Pen(4, 4, 40,  "свинарник"),
                new Pen(5, 5, 200, "конюшня")
            };

            _animals = new List<Animal>
            {
                new Animal(1, "Бурёнка", 1, 3, 3, 600),
                new Animal(2, "Бык",     1, 1, 5, 800),
                new Animal(3, "Корова",  2, 2, 4, 600),
                new Animal(4, "Хрюша",   3, 4, 2, 1500),
                new Animal(5, "Гнедой",  4, 5, 6, 1000)
            };
        }

        public List<Farmer> GetFarmers() => _farmers;
        public List<Pen> GetPens() => _pens;
        public List<Animal> GetAnimals() => _animals;
    }
}
