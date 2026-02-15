# Project #7 — Distributed Caching Demo (ASP.NET Core + Redis)

## Overview

This project demonstrates a production-style distributed caching implementation using:

- ASP.NET Core Minimal API
- Cache-Aside Pattern
- Explicit Cache Invalidation
- Redis Distributed Cache
- Graceful Degradation on Cache Failure

---

# Architecture

```
Endpoint
   ↓
CacheAsideService
   ↓
ICacheStore (Abstraction)
   ↓
RedisCacheStore OR InMemoryCacheStore
```

Endpoints never interact with Redis directly.
Caching logic is centralized and infrastructure is swappable.

---

# Core Concepts Demonstrated

## 1. Cache-Aside Pattern

Flow:

1. Check cache
2. If hit → return value
3. If miss → query repository
4. Store in cache
5. Return value

Implemented in `CacheAsideService`.

---

## 2. Cache Invalidation (Explicit Strategy)

Write operations invalidate cache keys:

- PUT /products/{id}
- DELETE /products/{id}

This prevents stale reads.

Invalidation is deterministic and intentional.

---

## 3. Distributed Cache via Redis

Redis is used as the backing store.

Benefits demonstrated:

- Cache survives API restarts
- Multiple API instances can share cache
- TTL is respected at Redis level

---

## 4. Graceful Degradation

If Redis is unavailable:

- API does NOT crash
- Redis errors are logged
- Repository fallback continues

Caching improves performance but is not a hard dependency.

---

# Project Structure

```
Cache/
    ICacheStore.cs
    CacheEntry.cs
    CacheOptions.cs

Caching/
    CacheAsideService.cs
    CacheKeyBuilder.cs

Stores/
    InMemoryCacheStore.cs
    RedisCacheStore.cs

Data/
    FakeProductRepository.cs

Models/
    Product.cs
```

---

# Running the Project

## 1. Install Dependencies

```
dotnet restore
```

## 2. Start Redis (Docker)

```
docker run -d -p 6379:6379 --name redis-demo redis
```

## 3. Run API

```
dotnet run
```

Check console for listening port.

---

# Testing the Cache

## GET Product (Cache Miss First Time)

```
GET /products/11111111-1111-1111-1111-111111111111
```

Console:

```
Cache MISS
```

Repeat request:

```
Cache HIT
```

---

## Update Product (Invalidates Cache)

```
PUT /products/{id}?name=Updated&price=199.99
```

Console:

```
Cache INVALIDATE
```

Next GET:

```
Cache MISS
```

---

## Verify Distributed Behavior

1. GET product (populate cache)
2. Stop API
3. Start API
4. GET again

If Redis is active → Cache HIT

---

## Redis Failure Test

Stop Redis container:

```
docker stop redis-demo
```

Call GET endpoint.

Expected:

- API still works
- Redis warnings logged
- Repository fallback used

---

# Design Decisions

- Cache-aside instead of write-through
- Explicit invalidation instead of background refresh
- JSON serialization for store independence
- Store abstraction for infrastructure swapability
- TTL stored per entry

---

# Possible Extensions

- Sliding expiration
- Cache metrics middleware
- Response headers (X-Cache: HIT/MISS)
- Multi-instance load test
- Circuit breaker for Redis

---
