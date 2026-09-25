# Сценарий профилирования ECS baseline

## Цель

Этот сценарий даёт сопоставимую исходную точку перед изменением игрового цикла,
bridge-слоя и hot path систем. Он измеряет текущую игру, а не производительность
Unity Editor в целом.

Сценарий выполняется в `Assets/Project/Scenes/Main.unity` в Play Mode. Каждый
прогон начинается с новой игровой сессии и не меняет сцену или игровые данные.

## Условия прогона

Перед записью каждого варианта:

1. Открыть `Main.unity` и дождаться окончания импорта/компиляции.
2. Открыть **Window → Analysis → Profiler**; включить модули **CPU Usage** и
   **Memory**. В CPU Usage выбрать Timeline и колонку GC Alloc.
3. Отключить Deep Profile и Profile Editor. Не включать сторонние окна,
   которые постоянно собирают данные или создают лог.
4. Запустить Play Mode, подождать 5 секунд для завершения загрузки ресурсов и
   создать свежий захват Profiler.
5. Для каждой фазы записать не менее 300 кадров. Повторить весь прогон 3 раза
   на той же машине и с теми же Quality/Resolution/Target Platform.

Не сравнивать результаты Editor и Android/standalone build между собой. Если
появится development build, для него ведётся отдельная таблица baseline.

## Управление в текущей игре

| Действие | Ввод | Реализующий код |
| --- | --- | --- |
| Движение | `A` / `D` | `InputSystem`, `CheckInputMoveSystem` |
| Прыжок | `Space` | `CheckInputJumpSystem` |
| Атака | ЛКМ | `CheckInputAttackSystem` |
| Урон игроку | `Q` | `CheckInputHurtSystem`, по 25 здоровья за нажатие |
| Restart сессии | кнопка **Restart** / **Start new game** на Finish UI | `FinishViewInitSystem` → `ReactionSystem` |

Клавиша `E` попадает в `InputComponent.IsDead`, но в текущем игровом цикле не
обрабатывается. Она не входит в сценарий.

## Последовательность baseline

Каждая фаза выполняется отдельным Play Mode запуском. После фазы остановить
Play Mode, сохранить захват с именем `baseline-<variant>-<run>.data` и начать
следующую фазу с чистой сессии.

| Вариант | Действия | Что покрывает |
| --- | --- | --- |
| `idle` | После warm-up ничего не нажимать 10 секунд. | Начальный ECS-мир, camera/UI, постоянные системы. |
| `movement` | Удерживать `D` 5 секунд, затем `A` 5 секунд; нажать `Space` 10 раз с интервалом около 0,5 секунды. | Input, `RunSystem`, `JumpSystem`, sensors, `Rigidbody2D`, animator и camera bridge. |
| `combat` | Подойти к ближайшему ящику и атаковать ЛКМ до уничтожения объекта; повторить для второго ближайшего ящика, если он доступен. | Raycast в `CheckHitSystem`, здоровье, health view, destruction и effects. |
| `coins` | Пройти через все доступные монеты, дожидаясь окончания анимации каждой перед следующей. | Sensor, `CoinsViewCheckSystem`, DOTween, счётчик и удаление view/entity. |
| `restart-during-tween` | Подойти к ближайшей монете. В момент пересечения её trigger быстро нажать `Q` четыре раза, чтобы здоровье игрока стало нулевым; после появления Finish UI немедленно нажать **Restart**. Проверить, что restart отправлен до завершения 0,5-секундной анимации монеты. | Конкуренция coin DOTween callback, уничтожения ECS-мира, view и scoped DI-container. |

Для варианта `restart-during-tween` принять прогон только если одновременно
видны shrinking/fly-away монеты и Finish UI в момент нажатия Restart. Если это
не удаётся воспроизвести из-за раскладки уровня или задержки ручного ввода,
пометить попытку как `not reproduced`, не включать её в baseline и завести
отдельную задачу на editor-only restart trigger. Остановка Play Mode не является
заменой игровому restart: она не проходит через `ReactionSystem`.

## Что записывать

Для каждого валидного прогона записать в таблицу:

| Метрика | Источник | Правило записи |
| --- | --- | --- |
| CPU/frame, ms | Profiler → CPU Usage | Среднее, p95 и максимум выбранных 300 кадров; отдельно отметить `PlayerLoop`, Scripts и Physics2D. |
| GC Alloc/frame, B | Profiler → CPU Usage | Среднее и максимум выбранных 300 кадров. |
| Managed/Total Used Memory | Profiler → Memory | Значение в конце warm-up и после окончания фазы. |
| Количество ECS entities | ECS diagnostics/debugger | На стартe, в пике фазы и после restart. Пока диагностический счётчик не добавлен — `N/A`, не оценивать на глаз. |
| Активные DOTween | presentation diagnostics | На старте, в пике coin/restart и после restart. Пока счётчик отсутствует — `N/A`; не заменять его количеством видимых объектов. |
| Время restart, ms | Profiler marker или stopwatch | От нажатия Restart до первого кадра новой управляемой сессии. |
| Ошибки/предупреждения Console | Unity Console | Количество и текст новых сообщений во время фазы. Любая ошибка делает прогон невалидным. |

Шаблон строки результата:

```text
дата | commit | Unity | платформа | вариант | run | frames | CPU avg/p95/max | GC avg/max |
memory before/after | entities start/peak/after | tweens start/peak/after |
restart ms | Console errors | примечание
```

## Критерии валидности

- Во время записи нет Console error и нет исключений после restart.
- Использована свежая Play Mode сессия, а не продолжение предыдущего прогона.
- В захват не попал скриптовый reload, импорт assets или включённый Deep Profile.
- Для каждой метрики сравниваются медианы трёх одинаковых прогонов, а не один
  случайный пик.
- Значения `N/A` для entities и active tween остаются явно отмеченными до
  появления runtime diagnostics; их нельзя выдавать за измеренные данные.

## Результат и следующие действия

Этот файл описывает процедуру, но не содержит числовых baseline-значений.
Следующая задача — выполнить все валидные варианты, сохранить захваты и внести
измерения в отдельную таблицу baseline. После появления `GameSession` сценарий
`restart-during-tween` повторяется как регрессионная проверка teardown.
