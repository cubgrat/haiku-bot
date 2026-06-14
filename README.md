# 🌸 Haiku Telegram Bot

Телеграм-бот на .NET 10, который сочиняет хайку по тексту пользователя.
Под капотом — две нейросети: одна **сочиняет** хайку, вторая **оценивает** его
качество. Если оценка низкая, бот пробует заново (до 3 раз), и только одобренный
вариант отправляется пользователю. Под хайку есть кнопки 👍 / 👎 — по 👎 бот
сочиняет новый вариант, по 👍 фиксирует результат.

Проект построен по **гексагональной архитектуре** (ports & adapters), а LLM можно
переключать между реальным провайдером и **моком** (для тестов без внешнего API).

## Архитектура (гексагон)

```
                        ┌─────────────────────────────┐
   driving-адаптер      │        Application          │     driven-адаптеры
                        │   (use-cases / ядро)        │
  ┌───────────┐  IHaikuService  ┌───────────────┐     IHaikuComposer  ┌──────────────┐
  │ Telegram  │ ───────────────▶│  HaikuService │ ───────────────────▶│ LLM (Mock /  │
  │ (polling) │                 │  (пайплайн)   │     IHaikuCritic    │ OpenAI-совм.)│
  └───────────┘                 └───────┬───────┘                     └──────────────┘
                                        │ IHaikuRepository
                                        ▼
                                 ┌──────────────┐
                                 │ PostgreSQL   │  (EF Core)
                                 └──────────────┘
```

Проекты (`src/`):

| Проект | Слой | Зависит от |
|---|---|---|
| `HaikuBot.Domain` | Доменные **модели** (POCO, без зависимостей) | — |
| `HaikuBot.Application` | **Порты** (входные/исходящие), **DTO**, use-cases | Domain |
| `HaikuBot.Infrastructure` | Driven-адаптеры: EF-**сущности** + БД, LLM (Mock/OpenAI) | Application |
| `HaikuBot.Telegram` | Driving-адаптер (бот, polling) | Application |
| `HaikuBot.Host` | Composition root (DI, конфиг, Dockerfile) | все |

Модели / DTO / сущности разнесены: доменные модели — в `Domain/Models`, DTO
приложения — в `Application/Dtos`, EF-сущности БД — в `Infrastructure/Persistence/Entities`,
маппинг между ними — в `Infrastructure/Persistence/Mapping`.

## Как работает пайплайн

```
текст → [Сочинитель] → хайку → [Критик] → оценка ≥ порога?
                          ▲                        │ нет (до 3 раз, с замечанием критика)
                          └────────────────────────┘
                                              │ да
                            пользователю + кнопки 👍 / 👎
                                              │
                              👎 → новая генерация
                              👍 → ✅ зафиксировано
```

Все запросы, попытки генерации, вердикты критика и реакции сохраняются в Postgres
(`requests`, `generations`, `feedbacks`).

## Тестовый этап: мок LLM

По умолчанию `Llm:Provider=Mock` — бот работает **без обращения к внешнему API**:

- `MockHaikuComposer` собирает псевдо-хайку из шаблонов по теме (каждый раз разное);
- `MockHaikuCritic` выставляет детерминированную по тексту оценку 0–10.

Так как варианты различаются, реально срабатывают и повторные попытки, и обе ветки
кнопок. Нужен только токен Telegram — ключ нейросети не требуется.

Переключение на реальный LLM — одной переменной: `LLM_PROVIDER=OpenAiCompatible`
(+ `LLM_API_KEY`). Подойдёт любой OpenAI-совместимый эндпоинт (OpenRouter, Groq,
OpenAI, локальная Ollama).

## Быстрый старт

1. Получи токен бота у [@BotFather](https://t.me/BotFather).
2. Скопируй шаблон и впиши токен:
   ```bash
   cp .env.example .env   # заполни BOT_TOKEN; LLM_PROVIDER=Mock уже стоит
   ```
3. Запусти:
   ```bash
   docker compose up --build
   ```
4. Напиши боту любую тему — например, «осенний дождь».

Чтобы подключить реальную нейросеть, в `.env` поставь `LLM_PROVIDER=OpenAiCompatible`
и заполни `LLM_API_KEY` (например, бесплатный ключ с [openrouter.ai/keys](https://openrouter.ai/keys)).

## Конфигурация (`.env`)

| Переменная | Назначение | По умолчанию |
|---|---|---|
| `BOT_TOKEN` | Токен Telegram-бота | — (обязательно) |
| `LLM_PROVIDER` | `Mock` или `OpenAiCompatible` | `Mock` |
| `LLM_API_KEY` | Ключ LLM (только для `OpenAiCompatible`) | — |
| `LLM_BASE_URL` | OpenAI-совместимый эндпоинт | `https://openrouter.ai/api/v1` |
| `LLM_GENERATOR_MODEL` | Модель-сочинитель | `meta-llama/llama-3.3-70b-instruct:free` |
| `LLM_JUDGE_MODEL` | Модель-критик | `google/gemini-2.0-flash-exp:free` |
| `MAX_GENERATION_ATTEMPTS` | Попыток до одобрения | `3` |
| `APPROVAL_THRESHOLD` | Порог одобрения (0–10) | `7` |
| `POSTGRES_USER` / `POSTGRES_PASSWORD` / `POSTGRES_DB` | Реквизиты БД | `haiku` |

## Локальная разработка без Docker

```bash
docker compose up -d postgres   # поднять только БД

cd src/HaikuBot.Host
export ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=haiku;Username=haiku;Password=haiku"
export Bot__Token="<токен>"
export Llm__Provider="Mock"
dotnet run
```

## Заметки

- Схема БД создаётся автоматически при старте (`EnsureCreated`) с повторами —
  Postgres в контейнере может стартовать дольше бота.
- Имена бесплатных моделей у OpenRouter со временем меняются; при ошибке
  «model not found» проверь актуальный список на openrouter.ai/models.
