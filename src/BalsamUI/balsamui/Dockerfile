FROM node:18-alpine as build
WORKDIR /app
COPY . ./
ENV NODE_ENV=development
RUN npm install

# ENTRYPOINT ["tail", "-f", "/dev/null"]
RUN npm run build

# production environment
FROM nginx:alpine3.18
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]