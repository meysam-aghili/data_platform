FROM node:22-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm install
EXPOSE 5480 5481
ENV VITE_DOCKER_MODE=true
ENV PORT=5480
ENV DEV_API_SERVER_PORT=5481
COPY scripts/dev.sh /dev.sh
COPY scripts/prod.sh /prod.sh
RUN chmod +x /dev.sh /prod.sh
COPY . .
RUN npm run build
CMD ["/dev.sh"]
