# Угадай число
## Возможности
- три уровня сложности
- настройки игрока
- статистика
- сохранение настроек и статистики в JSON-файлы
- событийная модель: `GuessMade` и `GameFinished`

## Быстрый старт
Требуется .NET SDK 10.0 или новее.

```powershell
dotnet run --project src/GuessTheNumber
```

Запуск тестов:

```powershell
dotnet test
```


## Где хранятся данные

```text
src/GuessTheNumber/settings.json
src/GuessTheNumber/statistics.json
```
## Структура
```text
src/GuessTheNumber
├── Domain          — правила игры, сложности, события, настройки, статистика
├── Application     — сценарий игры и интерфейсы
├── Infrastructure  — JSON-хранилища и генератор случайных чисел
├── Presentation    — консольный интерфейс
└── Program.cs      — точка входа и сборка зависимостей
```

### Принципы SOLID

| Принцип | Где реализован |
|---|---|
| **S** — единственная ответственность | `GameSession` отвечает за правила партии, `ConsoleGameView` — за ввод/вывод, хранилища — за JSON-файлы. |
| **O** — открытость/закрытость | Новый уровень сложности добавляется в `DifficultyLevel`, не меняя `GameSession`; новые подписчики подключаются к событиям без изменения издателя. |
| **L** — подстановка Барбары Лисков | Любая реализация `INumberGenerator`, `ISettingsStore` и `IStatisticsStore` взаимозаменяема. |
| **I** — разделение интерфейсов | Используются отдельные `IGameView`, `IInputReader`, `ISettingsStore` и `IStatisticsStore`. |
| **D** — инверсия зависимостей | `GameController` зависит от абстракций, конкретные классы создаются в `Program.cs`. |

### События

`GameSession` публикует:

- `GuessMade` — после каждой попытки: вариант, результат сравнения, остаток попыток и суженный диапазон;
- `GameFinished` — один раз при победе или исчерпании попыток.

`ConsoleGameView` показывает результат попытки и итог партии, а `GameController` обновляет статистику по событию `GameFinished`.

## Структура репозитория

```text
src/GuessTheNumber/          основной проект
tests/GuessTheNumber.Tests/  модульные тесты
```
