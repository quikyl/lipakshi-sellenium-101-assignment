# Selenium 101 - C# (NUnit) LambdaTest Assignment

## Overview
This repository contains an automated solution for the Selenium 101 assignment (LambdaTest Certifications). It runs 3 scenarios on LambdaTest Cloud Selenium Grid and is configured for parallel execution.

## Folder structure
- `Selenium101.csproj` - project file
- `Tests/` - contains test code:
  - `LambdaTestConfig.cs` - reads LT credentials
  - `BaseTest.cs` - RemoteWebDriver setup and teardown
  - `Selenium101Tests.cs` - contains 3 test scenarios

## Pre-requisites
- .NET 8.0 SDK (or change target to net7.0)
- Environment variables:
  - `LT_USERNAME` — your LambdaTest username (email)
  - `LT_ACCESS_KEY` — your LambdaTest access key

Set these variables locally or in Gitpod / CI.

## How to run locally
1. Clone the repo
2. Set env vars:
   - On Windows (Powershell):
     ```powershell
     $env:LT_USERNAME="your_username"
     $env:LT_ACCESS_KEY="your_access_key"
     ```
   - On macOS/Linux:
     ```bash
     export LT_USERNAME="your_username"
     export LT_ACCESS_KEY="your_access_key"
     ```
3. Run:
```bash
dotnet restore
dotnet test
