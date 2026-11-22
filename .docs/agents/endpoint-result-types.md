# Endpoint result types (ASP.NET Core minimal APIs)

Canonical return types for HTTP responses in this repo. Use when **writing or reviewing endpoint signatures**, and when **writing specs or tickets** that name a response type - this is the authoritative list so that the correct name is chosen *before* any compiler is involved.

All types live in the `Microsoft.AspNetCore.Http.HttpResults` namespace.

## Accepted return types

| Response intent | Type | `TypedResults` factory |
| --- | --- | --- |
| 200 with a body | `Ok<TValue>` | `TypedResults.Ok(value)` |
| 200, no body | `Ok` | `TypedResults.Ok()`	|
| 400 validation error | `ValidationProblem` | `TypedResults.ValidationProblem(errors)` |
| 404, no body | `NotFound` | `TypedResults.NotFound()` |
| 404 with a body | `NotFound<TValue>` | `TypedResults.NotFound(value)` |
| 401 unauthorized | `UnauthorizedHttpResult` | `TypedResults.Challenge(...)` |
| 403 forbidden | `ForbidHttpResult` | `TypedResults.Forbid(...)` |
| 204 no content | `NoContent` | `TypedResults.NoContent()` |
| 500 / server error | `ProblemHttpResult` | `TypedResults.Problem(...)` |

## Rules

- **Pick by intent; a sibling's name does not imply yours.** The `*HttpResult` suffix applies to some of the 4xx/5xx shapes (`ProblemHttpResult`, `UnauthorizedHttpResult`); the 400, 404, and 200 shapes carry no suffix at all.
- **The table is exhaustive.** A name not in it does not exist here.
- **Body or no body decides generic vs. non-generic.** `Ok<T>` / `NotFound<T>` only when the response carries a payload; the bare `Ok` / `NotFound` otherwise.
- **Confirm before writing.** If a needed shape is not in the table, verify the exact type name on Microsoft Learn (`Microsoft.AspNetCore.Http.HttpResults` namespace, net-10.0) before using it - and compile as soon as the first unverified type is written. The compiler is th ereal gate for the factory-type pairing (e.g. that `TypedResults.NotFound()` returns `NotFound`). Report back the undocumented type and offer to add it to the table above, do not update this document.