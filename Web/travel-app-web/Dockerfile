# ─────────────────────────────────────────────
# Build context: repo ROOT
# Run from root as:
#   docker build -f Web/travel-app-web/Dockerfile -t travelapp-web .
#
# Two stages:
#   Stage 1 (node) — installs dependencies and builds Angular
#   Stage 2 (nginx) — serves the built files (tiny, production-ready image)
#
# WHY nginx? The Angular build produces static HTML/JS/CSS files.
# You don't need Node.js to serve static files — nginx is a lightweight
# web server that serves them much faster and with less memory.
# ─────────────────────────────────────────────

# Stage 1: Build the Angular app
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files first so npm install only re-runs when
# dependencies change (not on every code change — faster builds)
COPY Web/travel-app-web/package*.json ./

# Install all dependencies
RUN npm ci

# Copy the rest of the frontend source code
COPY Web/travel-app-web/ ./

# Build for production
# --output-path controls where the built files land
RUN npm run build -- --configuration production --output-path=/app/dist

# Stage 2: Serve with nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Remove nginx's default placeholder page
RUN rm -rf ./*

# Copy the built Angular files from Stage 1
COPY --from=build /app/dist/browser ./

# Copy our custom nginx config (handles Angular routing)
COPY Web/travel-app-web/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]