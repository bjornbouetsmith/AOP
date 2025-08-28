---
applyTo: "**/*.cs"
---

- All automated test generation must use the xUnit framework. Do not use other test frameworks such as NUnit or MSTest.
- Only xUnit should be used for test generation. Do not generate tests using any other frameworks.
- All mocks or stubs used in tests must utilize the Moq framework. Do not use other mocking libraries.