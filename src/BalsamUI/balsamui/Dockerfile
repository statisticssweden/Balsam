FROM node:18-alpine as build
WORKDIR /app
COPY . ./
ENV NODE_ENV=development
RUN npm install

# ENTRYPOINT ["tail", "-f", "/dev/null"]
RUN npm run build

# production environment
FROM nginxinc/nginx-unprivileged 
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]