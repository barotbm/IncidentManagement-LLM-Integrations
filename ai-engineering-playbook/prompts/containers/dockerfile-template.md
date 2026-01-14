# Dockerfile Template

**Category:** Containers  
**Use Case:** Generate production-ready Dockerfiles  
**Maturity Level:** 3  
**Last Updated:** January 2026

---

## Purpose

Generate secure, optimized Dockerfiles for containerizing applications.

---

## Prompt Template

```markdown
Generate a production-ready Dockerfile for the following application:

## Application Details

**Type:** {ASP.NET Core API | Node.js API | Python FastAPI | React SPA | etc.}  
**Runtime:** {.NET 9 | Node.js 20 | Python 3.11 | etc.}  
**Project Structure:**
```
{PASTE_PROJECT_STRUCTURE}
```

## Requirements

### Base Image
- Use official images from Microsoft, Node, Python (not "latest" tag)
- Specific version tags for reproducibility
- For .NET: Use `mcr.microsoft.com/dotnet/aspnet:{version}` for runtime
- For Node: Use `node:{version}-alpine` for smaller size

### Multi-Stage Build
- **Stage 1 (Build):** Compile/build application
- **Stage 2 (Runtime):** Copy artifacts, run application
- Minimize final image size (exclude build tools, source code)

### Security
- Run as non-root user
- Use specific image tags (no `:latest`)
- Minimize attack surface (distroless or alpine base)
- No secrets in image (use runtime environment variables)

### Performance
- Leverage Docker layer caching
- Copy dependency files first (package.json, .csproj) before source code
- Install dependencies in separate layer
- Minimize layers

### Port Configuration
- Expose port {PORT} (specify application port)
- Use environment variable for port if configurable

### Health Check
- Add HEALTHCHECK instruction
- Check application readiness endpoint

### Environment
- Set ASPNETCORE_ENVIRONMENT=Production (or equivalent)
- Use ARG for build-time variables
- Use ENV for runtime variables

## Constraints

**Do NOT:**
- Use `:latest` tags
- Run as root user in final image
- Copy unnecessary files (.git, tests, docs)
- Install dev dependencies in production image
- Hardcode secrets or credentials

**DO:**
- Use .dockerignore to exclude unnecessary files
- Set working directory explicitly
- Expose only necessary ports
- Add labels for metadata (version, maintainer)
- Optimize for layer caching

## Context

**Application Type:** {API | SPA | Background Service}  
**Build Output:** {./bin/Release | ./build | ./dist}  
**Entry Point:** {dll | js file | python module}  
**Deployment Target:** {Kubernetes | Azure Container Instances | Docker Compose}

## Expected Output

Provide:
1. Complete Dockerfile
2. .dockerignore file
3. Build command
4. Run command (with example environment variables)
5. Image size estimate

## Example Structure (for reference)

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Project.csproj", "./"]
RUN dotnet restore "Project.csproj"
COPY . .
RUN dotnet publish "Project.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Security: Run as non-root
RUN addgroup --system --gid 1000 appgroup \
    && adduser --system --uid 1000 --ingroup appgroup appuser
USER appuser

EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Project.dll"]
```
```

---

## Usage Example

**Input:**

```markdown
Generate a production-ready Dockerfile for the following application:

## Application Details

**Type:** ASP.NET Core API  
**Runtime:** .NET 9  
**Project Structure:**
```
IncidentManagement.Api/
├── IncidentManagement.Api.csproj
├── Program.cs
├── Controllers/
└── Services/
```

Port: 8080
Health endpoint: /health
```

**AI Output:**

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (separate layer for caching)
COPY ["IncidentManagement.Api/IncidentManagement.Api.csproj", "IncidentManagement.Api/"]
RUN dotnet restore "IncidentManagement.Api/IncidentManagement.Api.csproj"

# Copy source and build
COPY IncidentManagement.Api/ IncidentManagement.Api/
WORKDIR "/src/IncidentManagement.Api"
RUN dotnet publish "IncidentManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Create non-root user for security
RUN addgroup --system --gid 1001 apigroup \
    && adduser --system --uid 1001 --ingroup apigroup apiuser

# Set ownership
RUN chown -R apiuser:apigroup /app

# Switch to non-root user
USER apiuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "IncidentManagement.Api.dll"]
```

**.dockerignore:**
```
**/.git
**/.vscode
**/bin
**/obj
**/node_modules
**/dist
**/*.md
**/tests
Dockerfile
.dockerignore
```

**Build:**
```bash
docker build -t incidentmanagement-api:1.0.0 .
```

**Run:**
```bash
docker run -d \
  -p 8080:8080 \
  -e DATABASE_CONNECTION="Server=..." \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --name incident-api \
  incidentmanagement-api:1.0.0
```

**Estimated Image Size:** ~220MB (ASP.NET runtime + app)

---

## Expected Output Quality

AI should generate:
- ✅ Multi-stage build (separate build and runtime)
- ✅ Specific version tags (not :latest)
- ✅ Non-root user configured
- ✅ Proper layer caching (dependencies before source)
- ✅ Health check included
- ✅ .dockerignore provided
- ✅ Build and run commands

Common issues to watch for:
- ❌ Running as root in final image
- ❌ Using :latest tags
- ❌ Copying unnecessary files
- ❌ Missing HEALTHCHECK
- ❌ Hardcoded configuration

---

## Security Scanning

After generating Dockerfile:

```bash
# Scan for vulnerabilities
docker scan incidentmanagement-api:1.0.0

# Or use Trivy
trivy image incidentmanagement-api:1.0.0
```

**Target:** Zero high/critical vulnerabilities

---

## Optimization Tips

### Reduce Image Size

- Use alpine-based images where possible
- Remove unnecessary dependencies
- Use distroless images for maximum security

### Improve Build Speed

- Order layers from least to most frequently changing
- Use BuildKit for parallel builds
- Leverage remote cache

### Security Hardening

- Run as non-root (✅ included in template)
- Use read-only filesystem where possible
- Drop unnecessary capabilities
- Use Docker Content Trust

---

## Platform-Specific Variations

### For Kubernetes Deployment

```markdown
Add labels for Kubernetes:

LABEL org.opencontainers.image.title="IncidentManagement API"
LABEL org.opencontainers.image.version="1.0.0"
LABEL org.opencontainers.image.vendor="CompanyName"
```

### For Azure Container Instances

```markdown
Ensure port is configurable via environment variable:

ENV PORT=8080
EXPOSE ${PORT}
```

### For Development (separate Dockerfile.dev)

```markdown
Generate a separate Dockerfile.dev with:
- Hot reload enabled
- Development tools included
- Debug symbols preserved
```

---

## Related Prompts

- [../k8s/deployment-manifest.md](../k8s/deployment-manifest.md) - Deploy container to K8s
- [docker-compose-template.md](docker-compose-template.md) - Multi-container setup
- [../cicd/build-container-pipeline.md](../cicd/build-container-pipeline.md) - CI/CD for containers

---

## Ownership

- **Created by:** Platform Team
- **Maintained by:** DevOps Engineers
- **Last reviewed:** January 2026
- **Usage count:** TBD

---

## Feedback

If generated Dockerfile has security issues or doesn't build correctly, document and improve prompt.
