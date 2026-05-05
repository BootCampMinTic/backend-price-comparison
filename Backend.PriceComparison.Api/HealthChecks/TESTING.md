# Health Check Test Examples

## Testing Health Endpoints

### Using curl

1. **Complete Health Check**
```bash
curl -i http://localhost:5062/health
```

2. **Ready Check**
```bash
curl -i http://localhost:5062/health/ready
```

3. **Live Check**
```bash
curl -i http://localhost:5062/health/live
```

4. **API Controller Health Check**
```bash
curl -i http://localhost:5062/api/v1/health
```

### Using PowerShell

1. **Complete Health Check**
```powershell
Invoke-RestMethod -Uri "http://localhost:5062/health" -Method Get
```

2. **Ready Check**
```powershell
Invoke-RestMethod -Uri "http://localhost:5062/health/ready" -Method Get
```

3. **Live Check**
```powershell
Invoke-RestMethod -Uri "http://localhost:5062/health/live" -Method Get
```

### Expected Responses

#### Healthy Application
- Status Code: 200 OK
- Response includes all health check results
- Database connectivity confirmed

#### Unhealthy Application  
- Status Code: 503 Service Unavailable
- Response shows failed health checks
- Details about failures included

## Load Testing Health Endpoints

### Using PowerShell (Simple Load Test)
```powershell
1..100 | ForEach-Object -Parallel { 
    Invoke-RestMethod -Uri "http://localhost:5062/health/live" -Method Get 
} -ThrottleLimit 10
```

### Using curl (Multiple Requests)
```bash
for i in {1..10}; do curl -s http://localhost:5062/health/ready; done
```