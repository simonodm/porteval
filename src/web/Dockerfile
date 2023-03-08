# pull official base image
FROM node:16.19.1-alpine

# set working directory
WORKDIR /app

# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH

# install app dependencies
COPY package.json ./
COPY package-lock.json ./
RUN npm install --silent
RUN npm install react-scripts@4.0.3 -g --silent

# add app
COPY . ./

# build & run app
RUN npm run build

# https://github.com/facebook/create-react-app/issues/11860
# Due to this issue, `npm start` fails to properly start the dev server, which is why `serve` is used until this is fixed.
CMD ["npm", "start"]
