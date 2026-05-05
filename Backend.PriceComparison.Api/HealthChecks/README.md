# Health Checks Documentation

## Overview
This application includes comprehensive health checks to monitor the application's status, database connectivity, and overall system health.

## Health Check Endpoints

### 1. Complete Health Status
- **URL:** `GET /health`
- **Description:** Returns detailed health status of all configured health checks
- **Response:** JSON with status, duration, and details for each health check
- **Use Case:** Comprehensive monitoring and debugging

### 2. Ready Status
- **URL:** `GET /health/ready` 
- **Description:** Indicates if the application is ready to serve traffic (includes database connectivity)
- **Response:** HTTP 200 (Ready) or HTTP 503 (Not Ready)
- **Use Case:** Kubernetes readiness probes, load balancer health checks

### 3. Live Status
- **URL:** `GET /health/live`
- **Description:** Indicates if the application process is running
- **Response:** HTTP 200 (Alive)
- **Use Case:** Kubernetes liveness probes

### 4. Health Controller Endpoints
- **URL:** `GET /api/v1/health`
- **Description:** Detailed health status via controller with Swagger documentation
- **URL:** `GET /api/v1/health/ready`
- **Description:** Ready status via controller
- **URL:** `GET /api/v1/health/live`  
- **Description:** Live status via controller

## Configured Health Checks

### 1. Application Health Check
- **Name:** `application`
- **Tags:** `ready`, `live`
- **Description:** Monitors basic application functionality
- **Data Included:**
  - Application version
  - Build date
- Environment name
  - Machine name
  - Processor count
  - Working set memory

### 2. Database Health Check
- **Name:** `database`
- **Tags:** `ready`
- **Description:** Custom database connectivity check
- **Functionality:** Tests database connection using ClientDbContext

### 3. Entity Framework Database Check
- **Name:** `ef-database`
- **Tags:** `ready`
- **Description:** Built-in EF Core health check
- **Functionality:** Validates Entity Framework database connectivity

## Response Format

### Healthy Response (HTTP 200)
```json
{
  "status": "Healthy",
  "totalDuration": 45.2,
  "timestamp": "2024-01-15T10:30:45.123Z",
  "entries": [
    {
      "name": "application",
   "status": "Healthy",
      "duration": 1.2,
      "description": "Application is running",
      "tags": ["ready", "live"],
      "data": {
        "version": "1.0.0.0",
        "buildDate": "2024-01-15 08:30:45 UTC",
        "environment": "Development",
        "machineName": "SERVER-01",
   "processorCount": 8,
      "workingSet": 67108864
      }
    }
  ]
}
```

### Unhealthy Response (HTTP 503)
```json
{
  "status": "Unhealthy",
  "totalDuration": 5000.0,
  "timestamp": "2024-01-15T10:30:45.123Z",
  "entries": [
    {
      "name": "database",
      "status": "Unhealthy",
      "duration": 5000.0,
      "description": "Cannot connect to database",
      "tags": ["ready"],
      "exception": "Connection timeout occurred"
    }
  ]
}
```

## Kubernetes Integration

### Liveness Probe
```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 30
  periodSeconds: 10
```

### Readiness Probe  
```yaml
readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 5
```

## Monitoring Integration

### Prometheus
The health check endpoints can be monitored by Prometheus for alerting and metrics collection.

### Application Insights
Health check results are automatically logged and can be tracked in Application Insights.

## Configuration

Health checks are configured in `Program.cs` using the `AddCustomHealthChecks()` extension method. The configuration includes:

- Custom health check implementations
- Built-in Entity Framework health checks  
- Response formatting
- Endpoint routing and tagging

## Troubleshooting

### Common Issues
1. **Database Unhealthy:** Check connection string and database availability
2. **EF Context Issues:** Verify Entity Framework configuration
3. **Application Unhealthy:** Check application dependencies and configuration

### Debugging
Use the detailed `/health` endpoint to get comprehensive information about each health check component.