## Интеграционные тесты: 
<!-- <details> -->

### UserData
<details>

**1.1. Authoriaztion.LoginController**

|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ✔ |

**1.2. Authoriaztion.LogoutController**

|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ❌ |


**2. CompleteMoveController**

|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `POST`  | Проверить увеличение хода | ▶ |
| `POST`  | Проверить при привышении лимита не завершается ход| ❌ |

**3. InfoController**

|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |

**4. TeamActionLogsController**

|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `Post`  | Проверка запсис действий | ▶ |
</details>

### LitholgicalData

<details>

**1. BoreholeLogController**
|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Проверка работоспособности методов | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |


**2. BoreholeZipFileController**
|Метод | Описание | Статус |
|:-----|:---------|:-----:|
| `All`  | Проверка пользователя на авторизацию | ❌ |


**3. LithologicalModelController**
|Метод | Описание | Статус |
|:-----|:---------|:-----:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Проверка работоспособности методов | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |

**4. LogNameController**
|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Проверка работоспособности методов | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |

**5. SeismicController**
|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Проверка работоспособности методов | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |

</details>


### Purchased

<details>

**1. BoreholeExplorationController**
|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Проверка работоспособности методов | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |
| `Post`  | Добавление новой сквахины без кустовой площадки | ▶ |
| `Delete` | Удаление сквахины другой команды | ▶ |
| `Delete` | Удаление после завершения хода | ▶ |

**2. BoreholeProductionController**
|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Проверка работоспособности методов | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |
| `Post`  | Добавление новой сквахины без кустовой площадки | ▶ |
| `Post`  | Добавление новой сквахины c неверным кодом статуса | ▶ |
| `Patch` | Обновление сквахины неверным кодом статуса | ▶ |
| `Delete` | Удаление сквахины другой команды | ▶ |
| `Delete` | Удаление после завершения хода | ▶ |

**3. ObjectOfArrngementController**
|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |
| `All`  | Проверка работоспособности методов | ▶ |
| `Post`  | В место где уже есть объект | ▶ |

**4. SeismicController**
|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |
| `All`  | Проверка работоспособности методов   | ▶ |

**5. WellTopController**
|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `All`  | Проверка пользователя на авторизацию | ▶ |
| `All`  | Соответвсие типов ответа схемам данных (смотреть summary) | ▶ |
| `All`  | Проверка работоспособности методов   | ▶ |

</details>
</details>



## Модульные тесты:


<details>

**Authoriaztion.JwtGenerator**

|Метод | Описание | Статус |
|:-----|:---------|:------:|
| `CreateToken` | Создание и валидность токена | ▶ |

</details>

##Инфо:
<details>
<summary>Статусы</summary>

 - ✔ - Готово
 - ▶ - Выполняется
 - ❌ - Невыполняется

 </details>