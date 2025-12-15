# Architecture Modifications & Team Notes

This document explains recent architectural decisions to avoid confusion and ensure strict adherence to Clean / Onion Architecture principles.

---

## ✅ What Changed

- Infrastructure registration moved into a single `AddInfrastructure()` extension method
- API references Infrastructure **only at startup** as the **composition root**
- API no longer directly references repositories or database classes
- **Direct API → Domain project reference was intentionally removed**

---

## ❌ What We Do NOT Do

- API does not instantiate repositories
- API does not depend on database-related classes
- Application does not know database or SQL details
- Repositories are **not** moved out of Infrastructure
- Domain is never referenced by Infrastructure consumers directly

---

## 🧠 Why This Is Correct

Clean Architecture allows the **outermost layer (API)** to wire dependencies.

This does **not** violate dependency rules because:
- Runtime business logic depends only on abstractions
- Infrastructure implements interfaces defined in Domain
- Application orchestrates use cases without knowing implementations
- Infrastructure remains replaceable and isolated

---

## 📌 Rule of Thumb

> If it touches the database, filesystem, or network → **Infrastructure**

---

## ✔ Team Guidance

- Keep repository interfaces in **Domain**
- Keep repository implementations in **Infrastructure**
- API references Infrastructure **only** for dependency wiring
- API does **not** need to reference Domain directly
- Do not add Infrastructure references inside Application or Domain
- All dependency wiring belongs in `Program.cs`
