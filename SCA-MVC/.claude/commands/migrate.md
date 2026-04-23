---
description: >
  Guided, safe EF Core migration workflow. Reviews pending model changes, generates
  a migration with a descriptive name, reviews the generated SQL for safety, and
  applies with rollback readiness. Invoke when: "add migration", "update database",
  "create migration", "schema change", "new table", "rename column".
---

# /migrate

## What

Guided EF Core migration workflow that takes you from model change to applied
migration safely. It reviews what changed in your entity model, creates a
properly named migration, reviews the generated SQL for data loss and locking
risks, and applies it with a documented rollback path.

This command enforces the "one logical change per migration" principle and
prevents the most common migration mistakes: blind application, data loss,
and un-rollbackable batched changes.

## When

- After modifying entity classes, DbContext configuration, or relationships
- User says: "add migration", "update database", "create migration"
- After adding a new entity or table
- After renaming, removing, or changing column types
- After modifying indexes or constraints
- When the user needs to generate SQL scripts for DBA review

## How

### Step 1: Assess Current State

```bash
dotnet ef migrations list
```

Check for:
- Pending migrations not yet applied to the development database
- Model changes that have not been captured in a migration yet

### Step 2: Review Model Changes

Inspect the changed entity classes and DbContext configurations in:
- `Models/` — entity classes
- `Data/Configurations/` — Fluent API configurations
- `Data/ApplicationDbContext.cs` — DbSets and SeedData

Confirm the change is a single logical unit. If not, guide the user to split
into multiple migrations.

### Step 3: Generate Migration

Create with a descriptive name that explains the change, not the entity:

```bash
dotnet ef migrations add <DescriptiveName>
```

Naming convention: `Add|Remove|Rename|Modify` + `WhatChanged`
- `AddOrderShippingAddress`
- `RenameCustomerEmailToContactEmail`
- `AddIndexOnOrderCreatedAt`
- `RemoveDeprecatedProductSku`

### Step 4: Review Generated SQL

```bash
dotnet ef migrations script --idempotent
```

Flag and report:
- **DROP COLUMN / DROP TABLE** — Confirm data loss is intentional
- **ALTER COLUMN** type changes — Check for precision loss or truncation
- **Large table ALTER** — Warn about potential lock duration
- **Missing DEFAULT** — New non-nullable columns need defaults for existing rows

If data transformation is needed, guide the user through a multi-step migration
(add nullable column, copy data, set non-nullable, drop old column).

### Step 5: Apply Migration

```bash
dotnet ef database update
```

### Step 6: Verify & Document Rollback

```bash
dotnet build
```

Document the rollback command for this migration:

```bash
dotnet ef database update <PreviousMigrationName>
```

## Example

```
User: /migrate

Claude: Checking migration state...
  Last applied: AddNombreUsuario
  Model changes detected: new ShippingAddress property on Empleado entity.

  Creating migration: AddEmpleadoDireccion
  Reviewing generated SQL...

  SQL Review:
  - ALTER TABLE Empleados ADD Direccion nvarchar(200) NULL
  No data loss. No locking risk. Safe to apply.

  Applying migration... Done.
  Build: green.

  Rollback command (if needed):
  dotnet ef database update AddNombreUsuario
```

## Related

- `/build-fix` — Fix build errors that may arise after migration changes
- `/db-review` — Audit EF Core queries, relations, and index coverage

$ARGUMENTS
