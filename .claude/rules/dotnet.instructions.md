---
description: "Where to look up a .NET API or a diagnostic code. Use when writing C# or meeting an IDE, CA or compiler diagnostic."
paths: ["**/*.cs", "**/*.csproj", "**/*.props", "**/.editorconfig"]
---

# .NET lookups

- **.NET / C# APIs and IDE rules**: resolve any API's shape or location from the Microsoft Learn documentation for that API - including confirming that a type or member actually exists before writing code that uses it. Do not dig through `.dll` files, package internals, or the NuGet cache to find an API.
- **Diagnostic codes**: look up the single named code on Microsoft Learn. Do not scan a range of codes (e.g. "IDE0001-IDE9999") to guess what a code means - that is slow, and it pulls back documentation for hundreds of unrelated rules.
