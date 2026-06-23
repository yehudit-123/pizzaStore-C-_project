<img width="3888" height="5184" alt="P1200014" src="https://github.com/user-attachments/assets/f3ad07e8-a19f-4b86-8450-2e28411088cd" /># 🍕 מערכת ניהול חנות פיצה — אופקים

מערכת לניהול הזמנות, לקוחות ופיצות, בנויה בארכיטקטורת שכבות מלאה עם ממשק משתמש גרפי.

---

## 🏗️ מבנה הפרויקט

```
PizzaStore/
├── Entities/              — מחלקות הנתונים (Pizza, Customer, Order)
├── InfrastructureAPI/     — ממשקי גישה לבסיס הנתונים
├── Infrastructure/        — מימוש גישה ל-DB עם Entity Framework
├── ServiceInterface/      — ממשקי הלוגיקה העסקית
├── Service/               — מימוש הלוגיקה העסקית
├── WebAPI/                — שרת ASP.NET Core Web API + Swagger
└── PizzasDesktopApp/      — ממשק משתמש Windows Forms
```

---

## ✨ פונקציונליות

- **קליטת לקוח** — הוספת לקוח חדש עם בדיקת תקינות (שם, טלפון, מייל, גיל, מגדר)
- **חיפוש לקוח** — רשימת לקוחות עם פילטור בזמן אמת, ואפשרות להוסיף הזמנה ישירות מהרשימה
- **יצירת הזמנה** — בחירת לקוח ופיצה, הוספת משוב ותזכורת
- **לוח תזכורות** — הצגת כל ההזמנות עם צביעה לפי סטטוס:
  - 🔴 אדום — הזמנה פתוחה מעל 3 ימים
  - 🟡 צהוב — הזמנה פתוחה רגילה
  - ✅ ירוק — הזמנה סגורה

---

## 🗄️ בסיס הנתונים

**SQL Server** עם 3 טבלאות:

| טבלה | תיאור |
|---|---|
| `Customers` | לקוחות עם פרטי קשר |
| `Pizzas` | מאגר פיצות עם סוג ומחיר |
| `Orders` | הזמנות עם סטטוס, משוב ותזכורת |

---

## 🛠️ טכנולוגיות

| טכנולוגיה | שימוש |
|---|---|
| C# .NET 8 | שפת הפיתוח |
| Entity Framework Core 8 | גישה לבסיס הנתונים |
| SQL Server (LocalDB) | בסיס הנתונים |
| ASP.NET Core Web API | שרת ה-API |
| Swagger | תיעוד ובדיקת ה-API |
| Windows Forms | ממשק משתמש גרפי |

---

## 🚀 הרצת הפרויקט

### דרישות מקדימות
- Visual Studio 2022
- .NET 8 SDK
- SQL Server / LocalDB

### הגדרת בסיס הנתונים

פתחי SSMS והריצי את הסקריפט:

```sql
CREATE DATABASE OfakimPizzaDB;
GO
USE OfakimPizzaDB;
GO

CREATE TABLE Customers (
    custumerId INT PRIMARY KEY IDENTITY(1,1),
    gender NVARCHAR(10), [name] NVARCHAR(20) NOT NULL,
    phone NVARCHAR(10), age INT, email NVARCHAR(50)
);

CREATE TABLE Pizzas (
    pizzaId INT PRIMARY KEY IDENTITY(1,1),
    [type] NVARCHAR(10), [name] NVARCHAR(20) NOT NULL, price INT NOT NULL
);

CREATE TABLE Orders (
    id INT PRIMARY KEY IDENTITY(1,1),
    creatingDate DATETIME DEFAULT GETDATE(),
    custumerId INT REFERENCES Customers(custumerId),
    pizzaId INT REFERENCES Pizzas(pizzaId),
    feedback NVARCHAR(50), isClose BIT DEFAULT 0,
    ReminderDate DATETIME, reminderSent BIT DEFAULT 0
);
```

### הרצה

1. פתחי את `PizzaStore.sln` ב-Visual Studio
2. עדכני את ה-connection string ב-`WebAPI/appsettings.json`
3. בחרי `PizzasDesktopApp` כ-Startup Project
4. לחצי **F5**

---

## 📁 ארכיטקטורת השכבות

```
PizzasDesktopApp
      ↓
   WebAPI          ← Composition Root — מרכז הרישום
      ↓
   Service         ← לוגיקה עסקית
      ↓
Infrastructure     ← גישה ל-DB
      ↓
   Entities        ← מחלקות נתונים בלבד
```

כל שכבה מתקשרת עם השכבה שמתחתיה **דרך ממשק (Interface) בלבד** — לשמירה על עקרון ה-Dependency Inversion.
### צילומי מסך
# 🍕 מערכת ניהול חנות פיצה — אופקים

מערכת לניהול הזמנות, לקוחות ופיצות, בנויה בארכיטקטורת שכבות מלאה עם ממשק משתמש גרפי.

---

## 🏗️ מבנה הפרויקט

```
PizzaStore/
├── Entities/              — מחלקות הנתונים (Pizza, Customer, Order)
├── InfrastructureAPI/     — ממשקי גישה לבסיס הנתונים
├── Infrastructure/        — מימוש גישה ל-DB עם Entity Framework
├── ServiceInterface/      — ממשקי הלוגיקה העסקית
├── Service/               — מימוש הלוגיקה העסקית
├── WebAPI/                — שרת ASP.NET Core Web API + Swagger
└── PizzasDesktopApp/      — ממשק משתמש Windows Forms
```

---

## ✨ פונקציונליות

- **קליטת לקוח** — הוספת לקוח חדש עם בדיקת תקינות (שם, טלפון, מייל, גיל, מגדר)
- **חיפוש לקוח** — רשימת לקוחות עם פילטור בזמן אמת, ואפשרות להוסיף הזמנה ישירות מהרשימה
- **יצירת הזמנה** — בחירת לקוח ופיצה, הוספת משוב ותזכורת
- **לוח תזכורות** — הצגת כל ההזמנות עם צביעה לפי סטטוס:
  - 🔴 אדום — הזמנה פתוחה מעל 3 ימים
  - 🟡 צהוב — הזמנה פתוחה רגילה
  - ✅ ירוק — הזמנה סגורה

---

## 🗄️ בסיס הנתונים

**SQL Server** עם 3 טבלאות:

| טבלה | תיאור |
|---|---|
| `Customers` | לקוחות עם פרטי קשר |
| `Pizzas` | מאגר פיצות עם סוג ומחיר |
| `Orders` | הזמנות עם סטטוס, משוב ותזכורת |

---

## 🛠️ טכנולוגיות

| טכנולוגיה | שימוש |
|---|---|
| C# .NET 8 | שפת הפיתוח |
| Entity Framework Core 8 | גישה לבסיס הנתונים |
| SQL Server (LocalDB) | בסיס הנתונים |
| ASP.NET Core Web API | שרת ה-API |
| Swagger | תיעוד ובדיקת ה-API |
| Windows Forms | ממשק משתמש גרפי |

---

## 🚀 הרצת הפרויקט

### דרישות מקדימות
- Visual Studio 2022
- .NET 8 SDK
- SQL Server / LocalDB

### הגדרת בסיס הנתונים

פתחי SSMS והריצי את הסקריפט:

```sql
CREATE DATABASE OfakimPizzaDB;
GO
USE OfakimPizzaDB;
GO

CREATE TABLE Customers (
    custumerId INT PRIMARY KEY IDENTITY(1,1),
    gender NVARCHAR(10), [name] NVARCHAR(20) NOT NULL,
    phone NVARCHAR(10), age INT, email NVARCHAR(50)
);

CREATE TABLE Pizzas (
    pizzaId INT PRIMARY KEY IDENTITY(1,1),
    [type] NVARCHAR(10), [name] NVARCHAR(20) NOT NULL, price INT NOT NULL
);

CREATE TABLE Orders (
    id INT PRIMARY KEY IDENTITY(1,1),
    creatingDate DATETIME DEFAULT GETDATE(),
    custumerId INT REFERENCES Customers(custumerId),
    pizzaId INT REFERENCES Pizzas(pizzaId),
    feedback NVARCHAR(50), isClose BIT DEFAULT 0,
    ReminderDate DATETIME, reminderSent BIT DEFAULT 0
);
```

### הרצה

1. פתחי את `PizzaStore.sln` ב-Visual Studio
2. עדכני את ה-connection string ב-`WebAPI/appsettings.json`
3. בחרי `PizzasDesktopApp` כ-Startup Project
4. לחצי **F5**

---

## 📁 ארכיטקטורת השכבות

```
PizzasDesktopApp
      ↓
   WebAPI          ← Composition Root — מרכז הרישום
      ↓
   Service         ← לוגיקה עסקית
      ↓
Infrastructure     ← גישה ל-DB
      ↓
   Entities        ← מחלקות נתונים בלבד
```

כל שכבה מתקשרת עם השכבה שמתחתיה **דרך ממשק (Interface) בלבד** — לשמירה על עקרון ה-Dependency Inversion.

---

## 📸 צילומי מסך

### קליטת לקוח



<!-- גררי תמונה לכאן, או השתמשי בתחביר: -->
<!-- ![קליטת לקוח](screenshots/add-customer.png) -->

### חיפוש לקוח
<!-- ![חיפוש לקוח](screenshots/search-customer.png) -->

### יצירת הזמנה
<!-- ![יצירת הזמנה](screenshots/create-order.png) -->

### לוח תזכורות
<!-- ![תזכורות](screenshots/reminders.png) -->
