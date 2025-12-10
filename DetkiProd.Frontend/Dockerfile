# ---------------------------------------------------------
# 1) Базовый образ: Node + Corepack (для pnpm)
# ---------------------------------------------------------
FROM node:20 AS base
WORKDIR /app
RUN corepack enable


# ---------------------------------------------------------
# 2) Установка зависимостей
# ---------------------------------------------------------
FROM base AS deps
COPY package.json pnpm-lock.yaml* ./
RUN pnpm install --frozen-lockfile


# ---------------------------------------------------------
# 3) Сборка проекта
# ---------------------------------------------------------
FROM deps AS build
COPY . .
RUN pnpm build


# ---------------------------------------------------------
# 4) Production runtime
# ---------------------------------------------------------
FROM node:20 AS runner
WORKDIR /app

ENV NODE_ENV=production
ENV NITRO_PRESET=node

COPY --from=build /app/.output ./.output

EXPOSE 3000
CMD ["node", ".output/server/index.mjs"]
