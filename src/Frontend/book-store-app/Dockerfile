FROM node:20-alpine

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy source code
COPY . .

# Expose Vite dev server port
EXPOSE 5174

# Run Vite dev server on 0.0.0.0 and explicit port 5174 so it matches docker-compose mapping
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
