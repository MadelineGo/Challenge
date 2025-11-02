#
# Multi-stage Dockerfile for the Vite React application
#

# -----------------------------
# Build stage
# -----------------------------
FROM node:22-alpine AS builder

WORKDIR /app

# Install dependencies based on lockfile to ensure reproducible builds
COPY package.json package-lock.json ./
RUN npm ci

# Copy the rest of the source and build the production bundle
COPY . .
RUN npm run build

# -----------------------------
# Production stage
# -----------------------------
FROM nginx:1.27-alpine

# Copy custom nginx configuration to support client-side routing
COPY docker/nginx.conf /etc/nginx/conf.d/default.conf

# Copy entrypoint script that injects runtime config before nginx starts
COPY docker/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Copy build artifacts from the previous stage
COPY --from=builder /app/dist /usr/share/nginx/html

EXPOSE 80

ENTRYPOINT ["/entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]
