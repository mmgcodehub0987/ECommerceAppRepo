# Pharmacy API - Quick Testing Guide

## Using Swagger UI (Recommended for Testing)

### Step 1: Run the Application
```powershell
cd Pharmacy-api
dotnet run
```

### Step 2: Open Swagger UI
Navigate to: `https://localhost:7xxx/swagger` (check console output for exact port)

You'll see all available endpoints in the Swagger interface.

---

## Testing Sequence

### Test 1: Add a Medicine
**Endpoint:** POST `/api/medicines`

**Request Body:**
```json
{
  "fullName": "Amoxicillin 500mg",
  "notes": "Take with food",
  "expiryDate": "2025-12-31T00:00:00",
  "quantity": 100,
  "price": 50.00,
  "brand": "GlaxoSmithKline"
}
```

**Expected Response:** 201 Created
```json
{
  "id": 1,
  "fullName": "Amoxicillin 500mg",
  "notes": "Take with food",
  "expiryDate": "2025-12-31T00:00:00",
  "quantity": 100,
  "price": 50.00,
  "brand": "GlaxoSmithKline"
}
```

---

### Test 2: Add Another Medicine
**Endpoint:** POST `/api/medicines`

**Request Body:**
```json
{
  "fullName": "Paracetamol 500mg",
  "notes": "For fever and headache",
  "expiryDate": "2025-06-30T00:00:00",
  "quantity": 200,
  "price": 20.50,
  "brand": "Cipla"
}
```

**Expected Response:** 201 Created with ID = 2

---

### Test 3: Get All Medicines
**Endpoint:** GET `/api/medicines`

**Expected Response:** 200 OK
```json
[
  {
	"id": 1,
	"fullName": "Amoxicillin 500mg",
	"notes": "Take with food",
	"expiryDate": "2025-12-31T00:00:00",
	"quantity": 100,
	"price": 50.00,
	"brand": "GlaxoSmithKline"
  },
  {
	"id": 2,
	"fullName": "Paracetamol 500mg",
	"notes": "For fever and headache",
	"expiryDate": "2025-06-30T00:00:00",
	"quantity": 200,
	"price": 20.50,
	"brand": "Cipla"
  }
]
```

---

### Test 4: Get Specific Medicine
**Endpoint:** GET `/api/medicines/1`

**Expected Response:** 200 OK
```json
{
  "id": 1,
  "fullName": "Amoxicillin 500mg",
  "notes": "Take with food",
  "expiryDate": "2025-12-31T00:00:00",
  "quantity": 100,
  "price": 50.00,
  "brand": "GlaxoSmithKline"
}
```

---

### Test 5: Record a Sale (Successful)
**Endpoint:** POST `/api/medicines/record-sale`

**Request Body:**
```json
{
  "medicineId": 1,
  "quantitySold": 10
}
```

**Business Logic Executed:**
1. Medicine ID 1 verified (Amoxicillin exists)
2. Stock check: Available 100 >= Requested 10 ✓
3. Quantity reduced: 100 - 10 = 90
4. Sale record created:
   - TotalAmount = 50.00 × 10 = 500.00
5. Changes persisted to both JSON files

**Expected Response:** 200 OK
```json
{
  "id": 1,
  "medicineId": 1,
  "quantitySold": 10,
  "soldAt": "2024-01-15T10:30:45.1234567",
  "totalAmount": 500.00
}
```

---

### Test 6: Verify Stock Was Reduced
**Endpoint:** GET `/api/medicines/1`

**Expected Response:** 200 OK
```json
{
  "id": 1,
  "fullName": "Amoxicillin 500mg",
  "quantity": 90,  ← Reduced from 100 to 90
  "price": 50.00,
  ...
}
```

---

### Test 7: Record Another Sale
**Endpoint:** POST `/api/medicines/record-sale`

**Request Body:**
```json
{
  "medicineId": 2,
  "quantitySold": 5
}
```

**Expected Response:** 200 OK
```json
{
  "id": 2,
  "medicineId": 2,
  "quantitySold": 5,
  "soldAt": "2024-01-15T10:35:20.5678901",
  "totalAmount": 102.50  ← 20.50 × 5
}
```

---

## Error Testing

### Test 8: Insufficient Stock
**Endpoint:** POST `/api/medicines/record-sale`

**Request Body:**
```json
{
  "medicineId": 1,
  "quantitySold": 100  ← Only 90 available
}
```

**Expected Response:** 400 Bad Request
```json
{
  "message": "Insufficient stock. Available: 90, Requested: 100"
}
```

---

### Test 9: Medicine Not Found
**Endpoint:** POST `/api/medicines/record-sale`

**Request Body:**
```json
{
  "medicineId": 999,
  "quantitySold": 5
}
```

**Expected Response:** 404 Not Found
```json
{
  "message": "Medicine with ID 999 not found."
}
```

---

### Test 10: Invalid Medicine Data
**Endpoint:** POST `/api/medicines`

**Request Body:**
```json
{
  "fullName": "Invalid Medicine",
  "notes": "Missing price and brand",
  "quantity": -10,  ← Negative quantity
  "price": 0,       ← Zero price
  "expiryDate": "2020-01-01T00:00:00"  ← Past date
}
```

**Expected Response:** 400 Bad Request
```json
{
  "message": "Medicine price must be greater than zero."
}
```

---

## Verifying Data Persistence

### Check medicines.json
```powershell
Get-Content ".\Pharmacy-api\Data\medicines.json" | ConvertFrom-Json | ConvertTo-Json
```

You should see your added medicines with updated quantities after sales.

### Check sales.json
```powershell
Get-Content ".\Pharmacy-api\Data\sales.json" | ConvertFrom-Json | ConvertTo-Json
```

You should see all recorded sales with calculated totals.

---

## Key Testing Points

✅ **ID Auto-Generation**
- First medicine should have ID = 1
- Second medicine should have ID = 2
- First sale should have ID = 1
- Second sale should have ID = 2

✅ **Stock Management**
- Quantity decreases after each sale
- Cannot sell more than available stock
- Original quantity shown after each GET

✅ **Sale Amount Calculation**
- TotalAmount = Medicine.Price × QuantitySold
- Verify with: Amoxicillin (50.00) × 10 = 500.00
- Verify with: Paracetamol (20.50) × 5 = 102.50

✅ **Data Persistence**
- Stop the app and restart
- GET /api/medicines should return the same medicines
- Quantities should remain reduced
- Sales history should remain intact

✅ **Error Handling**
- Appropriate HTTP status codes
- Clear error messages
- No unhandled exceptions

---

## Using PowerShell for API Testing (Alternative)

If you prefer command-line testing:

```powershell
# Get all medicines
$response = Invoke-WebRequest -Uri "https://localhost:7000/api/medicines" -Method Get
$response.Content | ConvertFrom-Json | ConvertTo-Json

# Add medicine
$medicine = @{
	fullName = "Aspirin 100mg"
	notes = "Pain relief"
	expiryDate = "2025-12-31"
	quantity = 150
	price = 15.00
	brand = "Bayer"
} | ConvertTo-Json

Invoke-WebRequest -Uri "https://localhost:7000/api/medicines" -Method Post `
	-ContentType "application/json" -Body $medicine

# Record sale
$sale = @{
	medicineId = 1
	quantitySold = 5
} | ConvertTo-Json

Invoke-WebRequest -Uri "https://localhost:7000/api/medicines/record-sale" -Method Post `
	-ContentType "application/json" -Body $sale
```

---

## Notes

- Replace `7000` with the actual port shown in console output
- API is available over HTTPS
- All dates should be in ISO 8601 format (YYYY-MM-DDTHH:MM:SS)
- Prices should have 2 decimal places
- Quantities must be positive integers
