# E-Commerce API Documentation

## Beskrivning
Detta API möjliggör hantering av kunder, produkter och beställningar i ett e-handelssystem. API:et stödjer CRUD-operationer och kommunicerar med JSON.

## Kontaktinformation
- **Namn:** Refet Karangja
- **Email:** refet.karangja@iths.se
- **GitHub:** [Refet88](https://github.com/Refet88)

---
## Auth

### 1. Inloggning
- **Metod:** POST
- **URL:** `api/auth/login`
- **Beskrivning:** Hanterar användarens inloggning genom att validera användarnamn och lösenord. Om autentiseringen lyckas genereras en JWT-token.
- **Svar:**
  - 200 OK: Returnerar JWT-token vid lyckad autentisering.
  
- **Exempel på svar:**
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjdXN0b21lcklkIjoiMSIsInN1YiI6IkFkbWluIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJqdGkiOiIyNDIwMjAyNy1iYzMyLTRiMzItYmJkMy1jMTdmZDc2ODg3ZGIiLCJleHAiOjE3NDM5NjM4NzIsImlzcyI6IllvdXJJc3N1ZXIiLCJhdWQiOiJZb3VyQXVkaWVuY2UifQ.dxY1RVDt2ui4j5H2NsWuXHMBFEv1bdlE40jLtssbMy8"
  }
  ```
  - 401 Unauthorized: Returnerar att användaren inte hittades eller att inloggningsuppgifterna är felaktiga.
  - 400 Bad Request: Returnerar att inloggningsuppgifterna saknas.
  ---
<br>

## Customer

### 1. Hämta alla kunder
- **Metod:** GET
- **URL:** `/api/Customers`
- **Beskrivning:** Hämtar en lista med alla kunder.
- **Svar:**
  - **200 OK:** Returnerar alla kunder.
- **Exempel på svar:**
  ```json
  {
    "id": 1,
    "firstName": "Anna",
    "lastName": "Svensson",
    "email": "anna@example.com",
    "phoneNumber": "0701234567",
    "street": "Storgatan 10",
    "zipCode": "12345",
    "city": "Göteborg"
  }
  ```
  ---
<br>

### 2. Hämta kund via ID
- **Metod:** GET
- **URL:** `/api/Customers/{id}`
- **Beskrivning:** Hämtar en specifik kund baserat på deras unika ID.
- **Parametrar:**
  - **id** *(int)*: Kundens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar kunden om den hittas. 
- **Exempel på svar:**
  ```json
  {
    "customerId": 2,
    "firstName": "Anders",
    "lastName": "Svensson",
    "email": "anders@example.com",
    "phoneNumber": "0712345678",
    "street": "Kungsgatan 10",
    "zipCode": "12345",
    "city": "Stockholm"
  }
  ```
  - **404 Not Found:** Returnerar att kunden med angivet id inte hittades.
  ---
  <br>

  ### 3. Uppdatera kund via ID
- **Metod:** PUT
- **URL:** `/api/Customers/{id}`
- **Beskrivning:** Uppdaterar en specifik kund baserat på deras unika ID.
- **Parametrar:**
  - **id** *(int)*: Kundens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar att kundens uppgifter har uppdaterats.
  - **400 Bad Request:** Returnerar att kunduppgifter är null eller saknas.
  - **404 Not Found:** Returnerar att kunden med angivet id inte hittades.
  ---
  <br>

  ### 4. Radera kund via ID
- **Metod:** DELETE
- **URL:** `/api/Customers/{id}`
- **Beskrivning:** Raderar en specifik kund baserat på deras unika ID.
- **Parametrar:**
  - **id** *(int)*: Kundens unika identifierare.
- **Svar:**
  - **204 No Content:** Returnerar att kunden raderades framgångsrikt.
  - **404 Not Found:** Returnerar att kunden med angivet id inte hittades.
  ---
  <br>

  ### 5. Hämta kund via Email
- **Metod:** GET
- **URL:** `/api/Customers/email/{email}`
- **Beskrivning:** Hämtar en specifik kund baserat på deras mailadress.
- **Parametrar:**
  - **email** *(string)*: Kundens mailadress.
- **Svar:**
  - **200 OK:** Returnerar kunden med den angivna mailadressen.  
- **Exempel på svar:**
  ```json
  {
    "customerId": 8,
    "firstName": "Bertil",
    "lastName": "Nillsson",
    "email": "berra@example.com",
    "phoneNumber": "0701234212",
    "street": "Svanstigen 14",
    "zipCode": "12535",
    "city": "Malmö"
  }
  ```
  - **404 Not Found:** Returnerar att kunden med den agivna mailadressen inte hittades.
  ---
  <br>

  ### 6. Registrera ny kund
- **Metod:** POST
- **URL:** `/api/Customers/register`
- **Beskrivning:** Lägger till en ny kund i systemet.
- **Svar:**
  - **201 OK:** Returnerar att kunden har registrerats framgångsrikt.  
- **Exempel på svar:**
  ```json
  {
    "customerId": 12,
    "firstName": "Stig",
    "lastName": "Ekdal",
    "email": "stig@example.com",
    "phoneNumber": "0711214567",
    "street": "Björnstigen 4",
    "zipCode": "12455",
    "city": "Borås"
  }
  ```
  - **400 Bad Request:** Returnerar att indatan är ogiltligt.
  ---
## Orders

### 1. Hämta alla ordrar
- **Metod:** GET
- **URL:** `/api/Orders`
- **Beskrivning:** Hämtar en lista med alla ordrar från alla kunder.
- **Svar:**
  - **200 OK:** Returnerar alla ordrar från alla kunder.
- **Exempel på svar:**
  ```json
  {
    "orderId": 49,
    "customerId": 1,
    "orderDate": "2025-03-20T00:00:00",
    "customer": {
      "customerId": 1,
      "firstName": "Pelle",
      "lastName": "Jönsson",
      "email": "Pelle@fakemail.com",
      "phoneNumber": "0123458747",
      "street": "Fakevägen 3",
      "zipCode": "12365",
      "city": "Ankeborg"
    },
    "orderItems": [
      {
        "orderDetailId": 58,
        "orderId": 49,
        "productNumber": 20201,
        "quantity": 1,
        "price": 3990,
        "product": {
          "productNumber": 20201,
          "name": "ASUS VG27AQM1A",
          "description": "TUF Gaming VG27AQM1A Gaming Monitor – 27-tums",
          "price": 3990,
          "stockQuantity": 4,
          "category": "Gaming",
          "isDiscontinued": false
        }
      },
      {
        "orderDetailId": 59,
        "orderId": 49,
        "productNumber": 20202,
        "quantity": 1,
        "price": 14990,
        "product": {
          "productNumber": 20202,
          "name": "ASUS XG32QH3A",
          "description": "TUF Gaming XG32QH3A Gaming Monitor – 32-tums",
          "price": 14990,
          "stockQuantity": 4,
          "category": "Gaming",
          "isDiscontinued": false
        }
      }
    ]
  }
  ```
  ---
<br>

### 2. Hämta order via ID
- **Metod:** GET
- **URL:** `/api/Orders/{id}`
- **Beskrivning:** Hämtar en specifik order baserat på ett unikt ID.
- **Parametrar:**
  - **id** *(int)*: Orderns unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar ordern om den hittas. 
- **Exempel på svar:**
  ```json
  {
    "orderId": 49,
    "customerId": 1,
    "orderDate": "2025-03-20T00:00:00",
    "customer": {
      "customerId": 1,
      "firstName": "Pelle",
      "lastName": "Jönsson",
      "email": "Pelle@fakemail.com",
      "phoneNumber": "0123458747",
      "street": "Fakevägen 3",
      "zipCode": "12365",
      "city": "Ankeborg"
    },
    "orderItems": [
      {
        "orderDetailId": 58,
        "orderId": 49,
        "productNumber": 20201,
        "quantity": 1,
        "price": 3990,
        "product": {
          "productNumber": 20201,
          "name": "ASUS VG27AQM1A",
          "description": "TUF Gaming VG27AQM1A Gaming Monitor – 27-tums",
          "price": 3990,
          "stockQuantity": 4,
          "category": "Gaming",
          "isDiscontinued": false
        }
      },
      {
        "orderDetailId": 59,
        "orderId": 49,
        "productNumber": 20202,
        "quantity": 1,
        "price": 14990,
        "product": {
          "productNumber": 20202,
          "name": "ASUS XG32QH3A",
          "description": "TUF Gaming XG32QH3A Gaming Monitor – 32-tums",
          "price": 14990,
          "stockQuantity": 4,
          "category": "Gaming",
          "isDiscontinued": false
        }
      }
    ]
  }
  ```
  - **404 Not Found:** Returnerar att ordern med angivet id inte hittades.
  ---
  <br>

  ### 3. Radera order via ID
- **Metod:** DELETE
- **URL:** `/api/Orders/{id}`
- **Beskrivning:** Raderar en specifik order baserat på ett unikt ID.
- **Parametrar:**
  - **id** *(int)*: Orderns unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar att ordern har raderats framgångsrikt.
  - **404 Not Found:** Returnerar att ordern med angivet id inte hittades.
  ---
  <br>

  ### 4. Hämta order baserat på kund via kund-ID
- **Metod:** GET
- **URL:** `/api/Orders/customer/{customerId}`
- **Beskrivning:** Hämtar en specifik order baserat på en kunds unika ID.
- **Parametrar:**
  - **customerId** *(int)*: Kundens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar en lista med kundens ordrar.
  - **404 Not Found:** Returnerar att kunden med angivet id inte hittades eller att kunden inte har några ordrar.
  ---
  <br>

  ### 5. Sök efter ordrar
- **Metod:** GET
- **URL:** `/api/Orders/search/{query}`
- **Beskrivning:** Söker efter ordrar i systemet baserat på en söksträng.
- **Parametrar:**
  - **query** *(string)*: Söksträngen som används för att matcha ordrar.
- **Svar:**
  - **200 OK:** Returnerar en lista med ordrar som matchar söksträngen.  
- **Exempel på svar:**
  ```json
  {
    "customerId": 8,
    "firstName": "Bertil",
    "lastName": "Nillsson",
    "email": "berra@example.com",
    "phoneNumber": "0701234212",
    "street": "Svanstigen 14",
    "zipCode": "12535",
    "city": "Malmö"
  }
  ```
  - **404 Not Found:** Returnerar att kunden med den agivna mailadressen inte hittades.
  ---
  <br>

  ### 6. Skapa ny order
- **Metod:** POST
- **URL:** `/api/Orders/create`
- **Beskrivning:** Lägger till en ny order i systemet.
- **Svar:**
  - **201 OK:** Returnerar att ordern har lagts till framgångsrikt.  
- **Exempel på svar:**
  ```json
  {
    "orderId": 12,
    "customerId": 1,
    "orderDate": "2025-04-06T00:00:00+02:00",
    "customer": {
      "customerId": 1,
      "firstName": "Pelle",
      "lastName": "Jönsson",
      "email": "pelle@example.com",
      "phoneNumber": "0712345698",
      "street": "Bergsvägen 2",
      "zipCode": "12311",
      "city": "Ankeborg",
      "password": "AQAAAAIAAYagAAAAEHS/CFmov6pV8ws7dohdKtt27PtU4rrquKXAB8b1Feq8S06db+JlIA5AGHLeT9yhfg==",
      "role": "Customer"
    },
    "orderItems": [
      {
        "orderDetailId": 13,
        "orderId": 12,
        "productNumber": 1000,
        "quantity": 1,
        "price": 39990,
        "product": {
          "productNumber": 1000,
          "name": "ASUS ROG G700TF - Ultra 7 | 32GB | 2TB | RTX 5080",
          "description": "ASUS ROG G700 är byggd från grunden med högkvalitativa komponenter och drivs av en Intel Core Ultra 7 265KF-processor och ett NVIDIA GeForce RTX 5080 16GB Prime grafikkort vilket gör den pefekt för högpresterande spel.",
          "price": 39990,
          "stockQuantity": 1,
          "category": "Stationär dator",
          "isDiscontinued": false
        }
      }
    ]
  }
  ```
  - **400 Bad Request:** Returnerar att indatan är ogiltligt.
  - **404** Om den angivna kunden inte hittades.
  ---
  <br>

  ## Products

  ### 1. Hämta alla produkter
- **Metod:** GET
- **URL:** `/api/Products`
- **Beskrivning:** Hämtar en lista med alla produkter.
- **Svar:**
  - **200 OK:** Returnerar alla produkter.  
- **Exempel på svar:**
  ```json
  {
    "productNumber": 1000,
    "name": "ASUS ROG G700TF - Ultra 7 | 32GB | 2TB | RTX 5080",
    "description": "ASUS ROG G700 är byggd från grunden med högkvalitativa komponenter och drivs av en Intel Core Ultra 7 265KF-processor och ett NVIDIA GeForce RTX 5080 16GB Prime grafikkort vilket gör den pefekt för högpresterande spel.",
    "price": 39990,
    "stockQuantity": 1,
    "category": "Stationär dator",
    "isDiscontinued": false
  }
  ```
  ---
<br>

### 2. Lägg till en produkt
- **Metod:** POST
- **URL:** `/api/Products`
- **Beskrivning:** Lägger till en produkt i systemet.
- **Svar:**
  - **200 OK:** Returnerar att produkten har lagts till framgångsrikt. 
  - **400 Bad Request:** Returnerar att indatan är null eller ogiltlig.
---

### 3. Hämtar produkt via ID
- **Metod** GET
- **URL:** `/api/Products/{id}`
- **Beskrivning:** Hämtar en specifik produkt baserat på dens unika ID.
- **Parametrar:**
  - **id** *(int)*: Produktens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar att produkten om den hittas.
- **Exempel på svar:**
  ```json
  {
    "productNumber": 1000,
    "name": "ASUS ROG G700TF - Ultra 7 | 32GB | 2TB | RTX 5080",
    "description": "ASUS ROG G700 är byggd från grunden med högkvalitativa komponenter och drivs av en Intel Core Ultra 7 265KF-processor och ett NVIDIA GeForce RTX 5080 16GB Prime grafikkort vilket gör den pefekt för högpresterande spel.",
    "price": 39990,
    "stockQuantity": 1,
    "category": "Stationär dator",
    "isDiscontinued": false
  }
  ```
 
  - **400 Bad Request:** Returnerar att produkten inte finns.
---
<br>

### 4. Uppdaterar produkt via ID
- **Metod** PUT
- **URL:** `/api/Products/{id}`
- **Beskrivning:** Uppdaterar en specifik produkt baserat på dens unika ID.
- **Parametrar:**
  - **id** *(int)*: Produktens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar att produkten har uppdaterats framgångsrikt. 
  - **400 Bad Request:** Returnerar att produktuppgifter är null eller saknas.
  - **404 Not found:** Returnerar att produkten inte finns.
---

### 5. Radera produkt via ID
- **Metod** DELETE
- **URL:** `/api/Products/{id}`
- **Beskrivning:** Raderar en specifik produkt baserat på dens unika ID.
- **Parametrar:**
  - **id** *(int)*: Produktens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar att produkten har raderats framgångsrikt. 
  - **404 Not found:** Returnerar att produkten inte finns.
---

### 6. Sök efter en specifik produkt via ID
- **Metod** GET
- **URL:** `/api/Products/search/{query}`
- **Beskrivning:** Söker efter en specifik produkt baserat på en söksträng.
- **Parametrar:**
  - **id** *(int)*: Produktens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar en lista av produkter som matchar sökningen.
- **Exempel på svar:**
  ```json
  {
    "productNumber": 1001,
    "name": "Acer Nitro N50-655 - i5 | 16GB | 1TB | RTX 4060",
    "description": "Acer Nitro N50-655 har ett helt svart chassi med röd LED-belysning vilket ger en ordentlig känsla av modern gaming.",
    "price": 10990,
    "stockQuantity": 4,
    "category": "Stationär dator",
    "isDiscontinued": false
  }
  ``` 
  - **404 Not found:** Returnerar att produkten inte finns med den angivna sökningen.
---

### 7. Markera en specifik produkt att den inte finns i lager.
- **Metod** PUT
- **URL:** `/api/Products/{id}`
- **Beskrivning:** Markerar att en specifik produkt inte finns i lager baserat på dens unika ID.
- **Parametrar:**
  - **id** *(int)*: Produktens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar att produkten har markerats som ej i lager framgångsrikt.
  - **400 Bad Request:** Returnerar att produkten redan har markerats som ej i lager. 
  - **404 Not found:** Returnerar att produkten inte finns.
---

### 8. Markera en specifik produkt som åter är i lager.
- **Metod** PUT
- **URL:** `/api/Products/{id}`
- **Beskrivning:** Markerar att en specifik produkt åter finns i lager baserat på dens unika ID.
- **Parametrar:**
  - **id** *(int)*: Produktens unika identifierare.
- **Svar:**
  - **200 OK:** Returnerar att produkten har markerats som åter i lager framgångsrikt.
  - **400 Bad Request:** Returnerar att produkten redan har markerats som åter i lager. 
  - **404 Not found:** Returnerar att produkten inte finns.
---

### 9. Uppdaterar lagersaldo för en specifik produkt.
- **Metod** PUT
- **URL:** `/api/Products/{id}/update-stock`
- **Beskrivning:** Uppdatera lagersaldo för en specifik produkt baserat på dens unika ID.
- **Parametrar:**
  - **id** *(int)*: Produktens unika identifierare.
  - **antal** *(int)*: Antal att lägga till.
- **Svar:**
  - **204 OK:** Returnerar att lagersaldo för produkten har uppdaterats framgångsrikt.
  - **400 Bad Request:** Returnerar att saldot för produkten inte kan vara negativt. 
  - **404 Not found:** Returnerar att produkten inte finns.
---
