# Runtime API Documentation

## Navigation
- [English Version](#english-version)
- [Русская версия](#русская-версия)

## English Version

### Runtime API
The Runtime API is an example of a web API written in ASP.Net Core backend for compiling and running single-file programs, used for code testing and task verification.

### Endpoint
The main endpoints for the API are:

| Endpoint           | Function                          | HTTP Method |
|--------------------|-----------------------------------|-------------|
| /api/runtime       | Outputs a list of current runtimes | GET         |
| /api/runtime/[lang]| Executes code written in [lang] language, requiring two parameters in the request body: string Code for the source code and ICollention<string\> input for the run parameters, one string for one run | POST        |

### About runtimes and adding new languages
The module is designed to run code with stdin and obtain values of its stdout, stderr, exit code, execution time, and allocated process memory.

In short, the source folder has been enriched with support for the following languages, which are compatible with the endpoints:

| Endpoint   | Language         | Language Type     | Comment                               |
|------------|------------------|-------------------|---------------------------------------|
| clang      | C                | Compiled          | Requires gcc installation             |
| cplusplus  | C++              | Compiled          | Requires gcc installation             |
| dlang      | D                | Compiled          | Requires gdc installation             |
| rust       | Rust             | Compiled          | Requires rustc installation           |
| basic      | BASIC            | Interpreted       | Requires yabasic installation         |
| javascript | JavaScript Node  | Interpreted       | Requires Node.js installation         |
| lua        | Lua              | Interpreted       | Requires lua 5.4 installation         |
| perl       | Perl             | Interpreted       | Requires perl installation            |
| python     | Python           | Interpreted       | Requires python3 installation         |
| ruby       | Ruby             | Interpreted       | Requires ruby-core installation       |

Constants for temporary file pool paths and compiler paths are also used - LangRunnerConstants:

| Variable           | Purpose                                               |
|--------------------|-------------------------------------------------------|
| SHELL_PATH         | Path to shell                                         |
| POOL_DIR           | Path to the pool where runners create temporary files |
| C_COMPILER         | Path to the clang compiler                            |
| CPP_COMPILER       | Path to the C++ compiler                              |
| D_COMPILER         | Path to the dlang compiler                            |
| RUST_COMPILER      | Path to the blazing fast rust compiler                |
| PYTHON_INTERPRETER | Path to the python3 interpreter                       |
| LUA_INTERPRETER    | Path to the lua interpreter                           |
| PERL_INTERPRETER   | Path to the perl interpreter                          |
| BASIC_INTERPRETER  | Path to the basic interpreter                         |
| RUBY_INTERPRETER   | Path to the ruby interpreter                          |
| NODE_JS_INTERPRETER| Path to the Node.js interpreter                       |

For creating universal uber-runners, the `RunnerData` record is created:

| Argument        | Value                                                                                         |
|-----------------|-----------------------------------------------------------------------------------------------|
| execPath        | Path to the compiler                                                                          |
| buildPrompt     | Build command, parsed by the runner: "%s" - source file (compiler), "%b" - binary file (compiler), "%f" - source file (interpreter) |
| sourceExtension | Source file extension. Some compilers and interpreters require a correct extension            |

## Русская версия

### Runtime API
Runtime API представляет собой пример webapi написанный на ASP.Net core бекенда для компиляции и запуска однофайловых программ, используемый для тестирования кода и средством проверки задач.

### Endpoint
Основным Эндпоинтом для api являются:

| Endpoint        | Функционал                      | HTTP Метод |
|-----------------|---------------------------------|------------|
| /api/runtime    | Выводит списов актуальных сред для запуска | GET        |
| /api/runtime/[lang] | Выполняет код написанный на [lang] языке, на вход в запросе требует в body два параметра, string Code -- отвечающий за исходный код и ICollention<string\> input -- отвечает за параметры на прогон, одина строка -- параметры для одного запуска | POST       |

### Про runtimes и добавление новых языков
Модуль предназначенный для прогона кода с stdin и получения значений его stdout, stderr, exit code, времени выполнения и выделенной памяти процесса.

Короче, в папку исходников было потужно надристано поддержкой следующих языков, которые по совместимости эндпоинты:

| Endpoint   | Язык           | Тип языка        | Комментарий                           |
|------------|----------------|------------------|---------------------------------------|
| clang      | C              | Компилируемый    | Требуется установка gcc               |
| cplusplus  | C++            | Компилируемый    | Требуется установка gcc               |
| dlang      | D              | Компилируемый    | Требуется установка gdc               |
| rust       | Rust           | Компилируемый    | Требуется установка rustc             |
| basic      | BASIC          | Интерпретируемый | Требуется установка yabasic           |
| javascript | JavaScript Node| Интерпретируемый | Требуется установка Node.js           |
| lua        | Lua            | Интерпретируемый | Требуется установка lua 5.4           |
| perl       | Perl           | Интерпретируемый | Требуется установка perl              |
| python     | Python         | Интерпретируемый | Требуется установка python3           |
| ruby       | Ruby           | Интерпретируемый | Требуется установка ruby-core         |

Также используются переменные для пула с временными файлами и путями к компиляторам - LangRunnerConstants:

| Переменная           | Зачем                                               |
|----------------------|-----------------------------------------------------|
| SHELL_PATH           | Путь до shell                                       |
| POOL_DIR             | Путь до пула, где раннеры создают временные файлы   |
| C_COMPILER           | Путь до компилятора clang                           |
| CPP_COMPILER         | Путь до компилятора C++                             |
| D_COMPILER           | Путь до компилятора dlang                           |
| RUST_COMPILER        | Путь до компилятора rust                            |
| PYTHON_INTERPRETER   | Путь до интерпретатора python3                      |
| LUA_INTERPRETER      | Путь до интерпретатора lua                          |
| PERL_INTERPRETER     | Путь до интерпретатора perl                         |
| BASIC_INTERPRETER    | Путь до интерпретатора basic                        |
| RUBY_INTERPRETER     | Путь до интерпретатора ruby                         |
| NODE_JS_INTERPRETER  | Путь до интерпретатора Node.js                      |

Для создания универсальных уберраннеров создан record `RunnerData`:

| Аргумент        | Значение                                                                                                |
|-----------------|---------------------------------------------------------------------------------------------------------|
| execPath        | Путь до компилятора                                                                                     |
| buildPrompt     | Команда сборки, парсится раннером: "%s" - исходный файл (компилятор), "%b" - бинарный файл (компилятор), "%f" - исходный файл (интерпретатор) |
| sourceExtension | Расширение файла с кодом. Некоторые компиляторы и интерпретаторы требуют корректное расширение          |
