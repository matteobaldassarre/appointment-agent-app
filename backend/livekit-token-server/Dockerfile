# Stage 1: Build the app (compile TypeScript to JavaScript)
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy source code and config
COPY tsconfig.json ./
COPY src ./src

# Build the TypeScript code
RUN npm run build

# Stage 2: Runtime image
FROM node:20-alpine AS final

# Set working directory
WORKDIR /app

# Copy built artifacts from build stage
COPY --from=build /app/dist ./dist
COPY --from=build /app/package*.json ./

# Install only production dependencies
RUN npm install --only=production

# Expose port 3000
ENV PORT 3000
EXPOSE 3000

# Entrypoint command to run the app
CMD ["node", "dist/index.js"]