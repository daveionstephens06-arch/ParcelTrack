# ParcelTrack

CWEB2302 Web Development II, Common Coursework (Individual).

## Run it
1. Open `ParcelTrack.csproj` in Visual Studio.
2. Package Manager Console: `Update-Database`.
3. F5 (the **http** profile, http://localhost:5420), then open `/parcels`. Five parcels show.

## Demo accounts (made when the app starts)

| Email | Password | Role |
|---|---|---|
| admin@parceltrack.test | Pass123! | Admin |
| staff@parceltrack.test | Pass123! | Staff |

## What the starter already does
- Parcels and Branches tables (a foreign key), seeded, one migration
- Pages: `/parcels`, `/parcels/{id}`, add, edit, delete, `/branches/{id}`
- Identity with Admin and Staff roles: Staff add and edit, only the Admin deletes
- A JSON API at `/api/parcels` (GET, POST, PUT, DELETE) with a DTO. **Not locked yet.**

## My work
- **Part A (JWT Authentication & Authorization):** Added JWT Bearer authentication in `Program.cs` and `appsettings.json`, built `POST /api/auth/login` in `AuthApiController` returning 60-minute tokens with role claims, and locked down `POST`, `PUT`, and `DELETE` on `ParcelsApiController` (`CanManageParcels` policy for `POST`/`PUT`, `Admin` role for `DELETE`).
- **Part B (Request Logging Middleware):** Added `RequestLogMiddleware` in `Middleware/` to attach an 8-character `X-Request-Id` response header and log the HTTP method, path, status code, and elapsed milliseconds to the console.
- **Part C (Action Filter):** Created `[RequireClient]` (`RequireClientAttribute` in `Filters/`) requiring a non-empty `X-Client` header on parcel write endpoints (`POST`, `PUT`, `DELETE`) and logging action execution.
- **Part D (Delivery Fee Service):** Implemented `IDeliveryFeeCalculator` and `DeliveryFeeCalculator` in `Services/` (J$500 + J$150 per rounded-up kg + J$300 outside Kingston or St Andrew), injected it into `ParcelsController` and `ParcelsApiController`, and displayed the fee on `/parcels/{id}` and in `ParcelDto`.
- **Part E (Custom Validation):** Created `[TrackingCode]` (`TrackingCodeAttribute` in `Validation/`) enforcing `PT-` followed by 6 digits (`^PT-\d{6}$`) on both `Parcel` and `ParcelInput`.
- **Part F (HTTP Test Suite):** Updated `ParcelTrack.http` with all 10 sequential requests covering public reads, JWT login, authorization rules, header/validation checks, and expected status codes.
- **Part G (Build & Documentation):** Verified clean build on `net8.0`, tested on a fresh database with `Update-Database`, and documented all completed work here.

**Service lifetime (Part D):** Registered `IDeliveryFeeCalculator` as **Singleton** (`AddSingleton`) because `DeliveryFeeCalculator` is completely stateless and performs pure calculation on its inputs without using `AppDbContext` or per-request data, so one shared instance safely serves every request.

**What doesn't work:** Everything works as expected; there are no known issues.
