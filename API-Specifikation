# The Loading Bean – API Specification

**Version:** 1.0  
**Author:** Tina Lagesson  
**Base URL:** `/api`

---

## 📦 Product Endpoints

| Method | Route                         | Description                     |
|--------|-------------------------------|---------------------------------|
| GET    | /api/product                  | Get all products                |
| GET    | /api/product/{id}            | Get product by ID               |
| POST   | /api/product                  | Create new product (Admin only) |
| PUT    | /api/product/{id}            | Update product (Admin only)     |
| DELETE | /api/product/{id}            | Delete product (Admin only)     |
| GET    | /api/product/search?query=   | Search by name or product number |

---

## 👤 Customer Endpoints

| Method | Route                              | Description                  |
|--------|------------------------------------|------------------------------|
| GET    | /api/customer                      | Get all customers (Admin)    |
| GET    | /api/customer/{id}                | Get customer by ID           |
| POST   | /api/customer                      | Register new customer        |
| PUT    | /api/customer/{id}                | Update customer information  |
| DELETE | /api/customer/{id}                | Delete customer              |
| GET    | /api/customer/search?email=       | Search customer by email     |

---

## 🧾 Order Endpoints

| Method | Route             | Description             |
|--------|-------------------|-------------------------|
| GET    | /api/order        | Get all orders (Admin)  |
| GET    | /api/order/{id}  | Get order by ID         |
| POST   | /api/order        | Place new order         |
| DELETE | /api/order/{id}  | Delete order (Admin)    |

---

## 🔐 Test Users

**Admin Login:**  
- Email: `admin@theloadingbean.com`  
- Password: `Admin123!`  

**Test Customer:**  
- Email: `customer@example.com`  
- Password: `Customer123!`

---

## 🧪 Example Data

```json

PRODUCT
{
  "ProductNumber": "KAFFE001",
  "Name": "Nerd Roast",
  "Description": "Dark roast blend for late-night coders",
  "Price": 89.9,
  "Category": "Coffee",
  "IsAvailable": true,
  "IsDiscontinued": false
}

CUSTOMER
{
  "FirstName": "Tina",
  "LastName": "Lagesson",
  "Email": "tina@loadingbean.dev",
  "Phone": "070-1234567",
  "Address": "Buggränd 1, Kodstad"
}

ORDER
{
  "CustomerId": "abc123",
  "Items": [
    { "ProductId": "xyz789", "Quantity": 2 }
  ],
  "OrderDate": "2025-06-17T12:34:56Z",
  "TotalAmount": 179.80
}
