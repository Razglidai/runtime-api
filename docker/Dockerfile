# Используем образ Debian
FROM debian:latest

# Устанавливаем основные утилиты
RUN apt-get update && apt-get install -y \
    git \
    python3 \
    gcc \
    python3-pip \
    openjdk-11-jdk \
    apt-transport-https \
    wget \
    curl \
    nodejs \
    npm \
    systemd

# Устанавливаем .NET SDK
RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update && apt-get install -y dotnet-sdk-6.0

# Устанавливаем необходимые библиотеки для каждого языка
RUN pip3 install --upgrade pip
RUN npm install -g npm

# Настраиваем рабочую директорию
WORKDIR /workspace

# Клонируем репозиторий с GitHub
RUN git clone https://github.com/Razglidai/runtime-api.git -b build .

# Копируем файл службы в систему
COPY ./docker/app-daemon.service /etc/systemd/system/

# Включаем и запускаем службу
RUN systemctl enable app-daemon.service
RUN systemctl start app-daemon.service

# Устанавливаем начальную команду
CMD ["systemctl", "default"]
