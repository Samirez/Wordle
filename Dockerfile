FROM nginx:alpine
COPY WEBGL_Builds /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]