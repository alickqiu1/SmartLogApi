# SmartLogApi: AI-Driven Centralized Logging Gateway

An enterprise-grade, asynchronous backend RESTful Web API built with **.NET 10.0**, **ASP.NET Core**, and **Entity Framework Core**. This application serves as a centralized aggregator for system error tracking, automatically augmenting incoming crash reports with instant AI diagnostics and code fixes.

## 🚀 Key Features
* **Centralized Ingestion:** High-throughput HTTP POST endpoints to record application errors, severities, and full stack traces.
* **Persistent SQLite Storage:** Built-in data permanence handling database migration states completely separated from core runtime assets.
* **Asynchronous AI Orchestration:** Integrates with high-performance LLM engines using secure environment secret parsing and strict native JSON output streaming.
* **Production-Grade Testing Suite:** Comprehensive automated test coverage spanning unit, data layer integration, concurrency boundary, and cloud network integration testing.

---

## 📁 Architectural Directory Layout

The workspace follows a strict industry-standard separation of source code and automation test assets:

```text
SmartLogApi/
 ├── src/                  # Production Source Code
 │    ├── Controllers/     # API Route Routing Handlers (LogsController)
 │    ├── Models/          # Strongly-Typed C# Entities & Schemas
 │    ├── Data/            # DB Context Maps (EF Core Context Layout)
 │    └── Services/        # Asynchronous Cloud AI Client Services
 ├── test/                 # Automated Verification Layers
 │    └── SmartLogApi.Tests/ # xUnit Assertion and Mock Testing Modules
 ├── test.http             # Visual API Iteration Script (REST Client Workflow)
 ├── smartlogs.db          # Local SQLite Database (Git Ignored File Asset)
 └── SmartLogApi.sln       # Core Master Compilation Solution Map
```

---

## ⚙️ Local Development & Environment Setup

### Prerequisites
* [.NET 10.0 SDK](https://microsoft.com) or newer installed on your computer.

### 1. Configure Secure AI Environment Variables
To enable automated error diagnostics, you will need a free developer API key from Groq:
1. Go to the **[Groq Console](https://groq.com)** and sign up for a free developer account (no credit card required).
2. Navigate to the **API Keys** tab in the sidebar and click **Create API Key**.
3. Copy your unique `gsk_...` key string immediately (you will only see it once).

Next, to prevent hardcoding sensitive tokens into your public source code, use the native .NET Secret Manager to save your key securely in your local environment:

```bash
# Initialize secret manager support in the src folder
cd src
dotnet user-secrets init

# Save your secure authorization token 
dotnet user-secrets set "Groq:ApiKey" "your_actual_api_key_here"
```

### 2. Initialize the Database Schema
Generate your physical SQLite tables by running Entity Framework migrations from the command line:

```bash
# Ensure global EF core tools are active
dotnet tool install --global dotnet-ef

# Execute code blueprints and create database file
dotnet ef database update
```

### 3. Compile and Run the Server
Launch your local application backend container environment:

```bash
dotnet run
```
The server will boot and dynamically host OpenAPI engine documentation routing traces on **`http://localhost:5245`**.

---

## 🧪 Automated Testing Strategy
This application is robustly guarded against regression failures by a suite of **8 automated xUnit test cases** evaluating 4 distinct architectural vectors.

To run the full verification battery, execute this tracking command from the repository root:
```bash
dotnet test
```

### Coverage Matrices Include:
1. **Model Property Validations:** Verifying entity fallback structures.
2. **Database Integration Assertions:** Ensuring records safely write and read back using isolated in-memory testing contexts.
3. **Concurrency & Boundary Edge Cases:** Stress-testing parallel database interactions and massive payload strings.
4. **Cloud Integration Smoke Tests:** Live client communication safety assertions utilizing sandbox secret key injection environments.

---

## 🔍 Interactive API Endpoint Testing via `test.http`

This project includes a built-in **`test.http`** automation script at the root level, allowing developers to immediately validate and execute live runtime tracking tests directly inside VS Code without needing external software like Postman.

### Prerequisites
* Install the **REST Client** extension (by Huachao Mao) inside Visual Studio Code.

### How to Execute the Integration Workflows

1. **Turn on the backend application container:**
   ```bash
   dotnet run
   ```
   *(Ensure your server is actively listening on `http://localhost:5245`)*

2. **Open the `test.http` file** located at the root of your workspace. 
   * *Note on Port Configuration:* If your terminal shows that your application booted up on a different local port than `5245`, simply update the variable value string at the very top of the file (`@baseUrl = http://localhost:YOUR_PORT`) to instantly map all request pathways.
   
3. **Run Test Scenario 1 (The POST AI Extraction Trigger):**
   Click **`Send Request`** right above `POST http://localhost:5245/api/logs`. 
   * *Expected Success Output:* Returns an `HTTP/1.1 201 Created` payload. The local server communicates securely with the cloud LLM, automatically populating the `aiDiagnosis` and `aiSuggestedFix` text blocks before caching the values.
   * *Expected Failure Outage Behavior:* If your local `Groq:ApiKey` user secret is missing, expired, or invalid, the database pipeline safely catches the exception. The raw log will still save securely with an `HTTP 201` status, but the AI-specific data fields will return a safe fallback value of `null` without crashing the thread execution flow.

4. **Run Test Scenario 2 (The SQLite GET Persistence Verification):**
   Click **`Send Request`** right above `GET http://localhost:5245/api/logs`.
   * *Expected Output:* Returns an `HTTP/1.1 200 OK` tracking array displaying all historic error records extracted directly from your local permanent `smartlogs.db` file, validating persistent database integrity.