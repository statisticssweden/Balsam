FROM node:20.11.1-slim as build
WORKDIR /app
COPY . ./
RUN npm install
RUN npm run build

# production environment
FROM nginxinc/nginx-unprivileged
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]