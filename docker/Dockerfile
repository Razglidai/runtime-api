# Используем образ Debian
FROM debian:latest

# Устанавливаем основные утилиты
RUN apt-get update && apt-get upgrade && apt-get install -y \
    supervisor \
    git \
    python3 \
    gcc \
    python3-pip \
    wget \
    curl \
    nodejs \
    npm \
    systemd

# Устанавливаем .NET SDK
RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update && apt-get install -y dotnet-sdk-8.0


# Настраиваем рабочую директорию
WORKDIR /workspace

# Клонируем репозиторий с GitHub
RUN git clone https://github.com/Razglidai/runtime-api.git -b build .

# Копируем файл службы в систему
RUN cp ./docker/runtime-api.conf /etc/supervisor/conf.d/runtime-api.conf
# Устанавливаем начальную команду
ENTRYPOINT supervisord -c /etc/supervisor/supervisord.conf
