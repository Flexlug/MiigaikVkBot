# MiigaikVkBot

Бот, предназначенный для автоматического получения расписания с университетского сайта http://studydep.miigaik.ru/.

![demo](https://github.com/Flexlug/MiigaikVkBot/raw/master/docs/demo.png)

## Требования

Необходимо получить следующие вещи:
1. Создать группу в VK и узнать её ID
2. В настройках группы в разделе "Ключи доступа" получить ключ";
3. Узнать URL вашей группы. Его можно найти [здесь](https://study.miigaik.ru/api/v1/groups).

Для запуска требуется Docker.

## Запуск

1. Клонируем репозиторий
```bash
git clone https://github.com/Flexlug/MiigaikVkBot.git
cd MiigaikVkBot
```

2. Редактируем `docker-compose.yml`, где указываем ключ доступа, ID группы и URL группы
3. Запускаем бота
```bash
docker compose up -d
```

Если все было сделано правильно - бот заработает. Получить к нему доступ можно через ЛС группы.