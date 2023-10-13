FROM node:18-alpine as build
WORKDIR /app
COPY . ./
RUN npm install
RUN npm run build

# production environment
FROM nginx:alpine3.18
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]