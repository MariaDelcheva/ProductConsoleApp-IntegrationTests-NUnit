# Product Console App - Integration Tests 🧪 (.NET & NUnit)

This repository contains integration tests for the `ProductConsoleAPI` – a console-based .NET application designed to manage a product database. The project uses **NUnit** for testing and follows standard practices for writing isolated, asynchronous integration tests.

---

## 📋 Project Description

The `ProductConsoleAPI` is a console application built on the **.NET Framework**, allowing users to:

- Add new products
- Update existing product information
- Delete products
- List all available products
- Search products by country of origin

The application demonstrates clean architecture practices and handles **asynchronous data access**, **validation**, and **user input parsing**.

---

## 🧪 Integration Testing Instructions

You are expected to implement integration tests in the provided project:

- 📂 `ProductConsoleAPI.IntegrationTests.NUnit`
- 📝 Edit the file `IntegrationTests.cs`

### ✅ Testing Requirements

- Tests must be **isolated** (they should not depend on shared state).
- Use **`await`** to handle asynchronous DB operations properly.
- Apply **realistic test data** aligned with the validation rules from `ValidationConstants.cs`.

---

## 🛠️ Technologies Used

- .NET Framework
- C#
- NUnit (Testing Framework)
- Visual Studio

---

## 🚀 How to Run the Tests

1. Open the solution in **Visual Studio**.
2. Navigate to the test project:  
   `ProductConsoleAPI.IntegrationTests.NUnit`
3. Use the **Test Explorer** to run all tests, or right-click on the test file and select `Run Tests`.

> ⚠️ The project is already configured with all necessary packages and references.

---

## 📌 Notes

- Test inputs should mimic actual user data (e.g., realistic names, codes, prices).
- Data validation is based on annotations and constants in the business logic layer.
- Example test actions include verifying CRUD operations and validating retrieval results.

---

📚 A valuable exercise for practicing integration testing, test isolation, and working with async database operations in C#!
