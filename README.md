# Smart Office Controller

## 📌 Overview
A C# smart office controller developed for **CO2401 – Software Development**.  
It manages **doors**, **lights**, and the **fire alarm** and was built using a **Test-Driven Development (TDD)** workflow with a dedicated test suite.

The solution contains:
- `SmartOffice` — main application code
- `SmartOfficeTests` — automated unit tests
(see solution file) ✅
  
## ✨ Features
- Lock/unlock doors
- Switch lights on/off
- Trigger/reset fire alarm
- Clear separation of concerns via interfaces for external services (e.g., web/email logging)
- TDD from the start: each feature backed by unit tests

## 🛠 Tech
- **Language:** C#
- **Testing:** NUnit (project `SmartOfficeTests`)
- **IDE:** Visual Studio
