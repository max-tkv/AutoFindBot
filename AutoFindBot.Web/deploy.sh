#!/bin/bash

set -e # включение проверки ошибок

echo "Удаление существующего образа..."
docker rmi -f autofindbot >/dev/null 2>&1 || true # игнорирование ошибки, если образ уже удален

echo "Создание нового образа..."
docker build -f "/home/dev/AutoFindBot/AutoFindBot.Web/Dockerfile" --force-rm -t autofindbot "/home/dev/AutoFindBot" || { echo "Ошибка: не удалось создать образ" >&2; exit 1; }

echo "Удаление существующего контейнера, если он есть..."
docker rm -f autofindbot-container >/dev/null 2>&1 || true # игнорирование ошибки, если контейнер уже удален

echo "Запуск нового контейнера..."
docker run -d -p 5001:80 --name autofindbot-container autofindbot || { echo "Ошибка: не удалось запустить контейнер" >&2; exit 1; }

echo "Готово!"