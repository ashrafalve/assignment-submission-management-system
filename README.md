# Assignment & Submission Management System

A full-stack, enterprise-grade application for managing academic assignments, student submissions, teacher reviews, and school administration.

---

## ⚡ Quick Start Instructions

### 1. Start the Backend API (.NET 10 & EF Core)

```bash
cd backend/AssignmentManagement.Api
dotnet run
```
> 📍 **Backend API & Swagger Docs:** [http://localhost:5000](http://localhost:5000)

---

### 2. Start the Frontend App (Next.js 15 & Tailwind)

In a new terminal window:

```bash
cd frontend
npm run dev
```
> 📍 **Frontend Web Application:** [http://localhost:3000](http://localhost:3000)

---

## 🔑 Demo Login Credentials

The database is automatically seeded on backend startup with the following test accounts:

| Role | Email | Password | Description |
|---|---|---|---|
| **Admin** | `admin@assignmentmanagement.com` | `Admin@1234` | System admin with full CRUD over users, classes, & subjects |
| **Teacher** | `john.teacher@assignmentmanagement.com` | `Teacher@1234` | Instructor who creates assignments and grades submissions |
| **Teacher** | `sarah.teacher@assignmentmanagement.com` | `Teacher@1234` | Second instructor |
| **Student** | `alex.student@assignmentmanagement.com` | `Student@1234` | Enrolled student who views assignments and submits work |
| **Student** | `emma.student@assignmentmanagement.com` | `Student@1234` | Enrolled student |

> 💡 *Note: The login page includes **1-Click Demo Preset Buttons** to fill credentials instantly.*

---

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 10, Entity Framework Core 10, PostgreSQL / In-Memory Db, JWT Bearer Authentication, BCrypt Password Hashing, Serilog.
- **Frontend**: Next.js 15 (App Router), TypeScript, Tailwind CSS, TanStack Query v5, React Hook Form, Zod, Axios, Lucide Icons, Shadcn UI.
