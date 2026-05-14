# Pharmacy Backend AI Instruction File

**Project:** ABC Pharmacy Assessment
**Technology Stack:** ASP.NET Core Web API (.NET 8)
**Architecture:** Controller → Service → Repository → JSON File Storage
**Design Principles:** SOLID + Repository Pattern
**Storage:** `medicines.json` and `sales.json`
**Framework:** ASP.NET Core Web API with Controllers
**API Documentation:** Swagger/OpenAPI
**Requirement Source:** Publicis Sapient Coding Assessment 

---

# 1. Objective

Build a backend API for a Single Page Application that manages pharmacy medicines and sales.

The API must support:

1. View all medicines
2. View a medicine by ID
3. Add a new medicine
4. Record a sale of medicine
5. Persist all data to JSON files
6. Reduce stock quantity when a sale is recorded

The application must follow:

* SOLID principles
* Repository Pattern
* Service Layer Pattern
* Clean Architecture separation

---

# 2. Functional Requirements

## Medicine Attributes

* Id (integer, auto-generated)
* FullName (string)
* Notes (string)
* ExpiryDate (DateTime)
* Quantity (integer)
* Price (decimal, 2 decimal places)
* Brand (string)

## Sale Attributes

* Id (integer, auto-generated)
* MedicineId (integer)
* QuantitySold (integer)
* SoldAt (DateTime)
* TotalAmount (decimal)

## API Features

* Retrieve all medicines
* Retrieve a single medicine
* Add medicine
* Record medicine sale
* Prevent sale when stock is insufficient
* Save sales history

---

# 3. Non-Functional Requirements

* Use .NET 8
* Use ASP.NET Core Web API
* Use Controllers
* Use Swagger
* Use JSON file storage (no database)
* Use dependency injection
* Use asynchronous programming (`async/await`)
* Validate inputs
* Return proper HTTP status codes
* Follow REST conventions

---

# 4. Architecture

```text
Controllers
    ↓
Services
    ↓
Repositories
    ↓
File Storage (JSON)
```

### Responsibilities

#### Controllers

* Receive HTTP requests
* Validate model state
* Return HTTP responses

#### Services

* Contain business logic
* Coordinate repositories
* Enforce rules

#### Repositories

* Read/write JSON files
* Data persistence only

#### Models

* Domain entities

---

# 5. SOLID Principles to Follow

## Single Responsibility Principle

Each class has one responsibility.

## Open/Closed Principle

Code should be extendable without modification.

## Liskov Substitution Principle

Implementations must correctly replace interfaces.

## Interface Segregation Principle

Interfaces should be focused and minimal.

## Dependency Inversion Principle

Depend on abstractions, not concrete classes.

---

# 6. Project Folder Structure

```text
Pharmacy.Api/
│
├── Controllers/
│   └── MedicinesController.cs
│
├── Models/
│   ├── Medicine.cs
│   ├── SaleRecord.cs
│   └── RecordSaleRequest.cs
│
├── Repositories/
│   ├── Interfaces/
│   │   ├── IMedicineRepository.cs
│   │   └── ISaleRepository.cs
│   │
│   └── Json/
│       ├── MedicineRepository.cs
│       └── SaleRepository.cs
│
├── Services/
│   ├── Interfaces/
│   │   └── IMedicineService.cs
│   │
│   └── MedicineService.cs
│
├── Data/
│   ├── medicines.json
│   └── sales.json
│
├── Program.cs
└── appsettings.json
```
