# ================== Build stage ======================

# --- NodeJS Image ---
FROM node:24-slim AS build

# Directory for deployed code on container
WORKDIR /app

# Copy package dependencies to /app
COPY package.json package-lock.json ./

# Always use npm ci (or npm clean install) over npm i (or npm install) in Docker builds since it's deterministic (i.e., installs exactly what's in package-lock.json) and faster
RUN npm ci

# Copy all content except .dockerignore references
COPY . .

# Build the project
RUN npm run build

# ================ Production stage ====================

# --- Nginx Image ---
FROM nginx:1.27-alpine AS production

# --- Copy nginx config from project to nginx image
COPY nginx.conf /etc/nginx/conf.d/default.conf

# --- Copy dist from node build image to nginx image
COPY --from=build /app/dist /usr/share/nginx/html

# --- Expose port to listen in final nginx container ---
EXPOSE 80

# --- Set the default container command ---
CMD ["nginx", "-g", "daemon off;"]