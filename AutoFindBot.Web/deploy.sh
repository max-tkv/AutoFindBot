#!/bin/bash

set -e # включение проверки ошибок

# Обработка аргументов командной строки
OPTIONS=$(getopt -o t: --long token: -n 'parse-options' -- "$@")
eval set -- "$OPTIONS"

TOKEN=""

while true; do
  case "$1" in
    -t|--token) TOKEN="$2"; shift 2;;
    --) shift; break;;
    *) echo "Ошибка: некорректный аргумент $1" >&2; exit 1;;
  esac
done

if [ -z "$TOKEN" ]; then
  echo "Ошибка: токен не указан" >&2
  exit 1
fi

echo "Удаление существующего образа..."
docker rmi -f autofindbot >/dev/null 2>&1 || true # игнорирование ошибки, если образ уже удален

if [ ! -d "/home/dev/AutoFindBot" ]; then
  echo "Клонирование репозитория..."
  git clone https://github.com/max-tkv/AutoFindBot.git /home/dev/AutoFindBot
else
  echo "Получение последних изменений из репозитория..."
  cd /home/dev/AutoFindBot
  git pull --force
fi

echo "Создание нового образа..."
docker build -f "/home/dev/AutoFindBot/AutoFindBot.Web/Dockerfile" --force-rm -t autofindbot "/home/dev/AutoFindBot" || { echo "Ошибка: не удалось создать образ" >&2; exit 1; }

echo "Удаление существующего контейнера, если он есть..."
docker rm -f autofindbot-container >/dev/null 2>&1 || true # игнорирование ошибки, если контейнер уже удален

echo "Запуск нового контейнера..."
docker run --privileged -d -p 5001:80 --name autofindbot-container autofindbot || { echo "Ошибка: не удалось запустить контейнер" >&2; exit 1; }

echo "Готово!"
