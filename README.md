# Project Template for Dotnet API and Angular SPA

This project is designed to be the bases of a robust and modular application framework. It includes a well-defined project structure, comprehensive configuration management, and extensibility features to support future growth and customization.

## Project Structure

The project is organized as follows:

```plain
├── .cicd
│   ├── app - All SPA deployment configuration
│   └── service - All .NET service deployment configuration
├── .devops
│   ├── app.yaml - Azure DevOps build pipeline for SPA
│   └── service.yaml - Azure DevOps build pipeline for .NET service
├── app - Angular application
└── service - .NET application
```

## Changes to Make

To update the template from the defaults, make the following changes.

1. Change all `changeme` strings.
2. Update all `TODO`s.

### Angular Application Setup

1. Replace all instances of `changeme-appname` with your kebab-case project name, e.g. `project-ui`
1. Replace all instances of `changeme-appabbr` with your desired 2-3 character element prefix, e.g. `app`
1. Replace `changeme-title` with a good default page title to be embedded in the main `index.html`

### .NET Application Setup

1. Rename the solution and project to match your project name.
1. Key Vault - If you are using key vault for secrets, add AppSettings:AzureKeyVault:KeyVaultUrl to your appsettings.\<env\>.json files.
   1. Make sure your managed identity has the `Key Vault Secrets User` role on the key vault and the access policy grants it the Get and List permissions on secrets.
   1. You'll want the same for any user that needs to access the key vault as well.

## Patterns Used

Important patterns used in the applications are listed below.

### Angular Patterns

#### Code style and linting

This project template defaults to strict code style and linting rules.

- Prettier is used to enforce code style, with a PR check included
- ESLint is used to enforce linting
  - Recommended base ESLint and Angular configuration
  - Strict typing and stylistic checking for Typescript
  - A plugin is also included that autofixes import sorting
  - Many rules can be safely turned off in tests, using the `**/*.spec.ts` files selector in [karma.config.js](app/karma.conf.js)

#### Pipelines and release process

This project is configured by default to use [Semantic Release](https://github.com/semantic-release/semantic-release) for versioning. Commits to the main branch should follow a specific pattern described in their documentation, e.g. `fix: thing` for fixes will trigger a patch version release, and `feat: thing` will trigger a minor version release.

A PR check is included that verifies the PR title matches this convention. However, it's important to ensure that GitHub defaults the PR title as the commit title when merging, otherwise releases may not trigger.

#### Recommendations

- **I18N** - While `@ngx-translate/core` has been the standard runtime i18n library for Angular for years, it is abandoned as of 2024. To ensure version compatibility and ensure control over behavior in the long term, it is feasible to create a functional equivalent, using a service, pipe, and json files for the translations.
- **CSS Utilities** - It is suggested to use Tailwind for CSS utilities and styling. The Tailwind theme can be customized to match your project styles, and can be integrated with Prettier to auto-sort classes using `prettier-plugin-tailwindcss`. If using Tailwind with VS Code, the Tailwind CSS Intellisense (`bradlc.vscode-tailwindcss`) plugin is also recommended. With Tailwind, component-specific stylesheets should generally be unnecessary, since all the required styles should be included via classes in the HTML file.
- **Basic UI Components** - If your application is intended to match existing mockups, it is generally recommended to build components from the ground up using Tailwind and the Angular CDK rather than attempt to alter existing component libraries to match the mockup styles. However, if the application is simple or does not require extensive branding, the Angular Material component library is recommended.
- **Icons** - Use FontAwesome icons with the `@fortawesome/angular-fontawesome` library. Schneider has a license for pro icons; install the specific icon sets like `@fortawesome/pro-regular-svg-icons` and/or `@fortawesome/pro-solid-svg-icons`. Import specific icons and use them via properties on components, so that they can be tree-shaken. Loading the full set of FontAwesome icons via a full style sheet is not recommended as it will unnecessarily increase the application bundle size.
- **Environment Configuration** - If your UI application requires environment-specific configuration, it is recommended to use a JSON file loaded during application initialization, rather than using build-time `environment.ts` file replacements. This pattern involves placing an `app.config.json` in the [public](app/public) folder, then loading that file via the app initializer and storing the configuration in a service. Avoiding environment-specific application builds makes for easier deployment strategies, since the same build output can be deployed to multiple environments using post-build file replacements.
- **Angular 18 Features** - Prefer using modern Angular features over legacy features
  - Use `inject` to inject dependencies instead of constructor injection
  - Use `input` / `output` to set up component inputs and outputs instead of `Input` / `Output` decorators
  - Use Angular built-in control flow (e.g. `@if`, `@for`, `@let`) instead of structural directives, where possible
- **Barrel files** - Barrel files (`index.ts`) can often increase bundle size, as importing from a barrel file immediately includes all files linked via that barrel file by default. Either avoid using barrel files, or add the property `"sideEffects": false` to [package.json](app/package.json) to indicate barrel files can be tree-shaken.
- **State management** - Avoid heavy state management libraries (e.g. Redux-based libraries) unless absolutely necessary. Angular's built in features (services, RXJS) generally serve this purpose natively.
- **RXJS** - Use the `async` pipe and `takeUntilDestroyed` operator (from `@angular/core/rxjs-interop`) to ensure subscriptions are cleaned up. Do not perform async operations (`subscribe`, `await`, `then`) within a `subscribe` statement, instead use a `switchMap` or related operator. If possible, use [eslint-plugin-rxjs](https://github.com/cartant/eslint-plugin-rxjs) to enforce good habits (as of August 2024 it is not possible since it is incompatible with ESLint 9.x.x).

### .NET Application Patterns

#### Logging

[Contextual Logging](https://github.com/SE-Sustainability-Business/shared-nuget-contextual-logging) and [Open Telemetry](https://github.com/SE-Sustainability-Business/shared-nuget-otel-instrumentation) are already configured in the project.

Use the `ILogger` interface to log messages. The `ILogger` interface is available in the `Microsoft.Extensions.Logging` namespace. The `SE.Sustainability.Shared.ContextualLogging` namespace includes extension methods to make it easier to log messages with context.

```csharp
using SE.Sustainability.Shared.ContextualLogging;

logger.Info("User logged in", new { UserId = userId });
```

Use this instead of the built in `ILogger.LogInformation`, etc methods.

#### Application Configuration

The application uses json files for configuration management, allowing for environment-specific settings. The configuration files are located in the `API` directory and include:

- [appsettings.local.json](./service/SE.Sustainability.Template.API/appsettings.local.json]: Configuration for local development.
- [appsettings.nonprod.json](./service/SE.Sustainability.Template.API/appsettings.nonprod.json]: Configuration for non-production environments.
- [appsettings.preprod.json](./service/SE.Sustainability.Template.API/appsettings.preprod.json]: Configuration for pre-production environments.
- [appsettings.prod.json](./service/SE.Sustainability.Template.API/appsettings.prod.json]: Configuration for production environments.

These files are loaded at runtime to configure the application based on the current environment. If `APP_ENV` is set, its value will be used for the environment part of the file name. If not set, `local` will be assumed.

#### API

For the API, there is a base path of `/api` for all routes. This _does
not_ mean that all routes must start with `/api`, it means that .NET will ignore that part of the path when matching routes. This way, you can configure your Kubernetes service to listen on `/api` and the API will still work as expected. For local access, you do not need to use /api in the URL but it
_will_ work if you do.

#### Secret Configuration

Whenever possible, prefer to access services using Azure managed identity. This uses the identity of the process executing the service (the local developer or the pod in the cluster) to authenticate with Azure services. The identity can be assigned roles in Azure that give permissions to the service and do not require an access key or secret. This is the most secure way to access services.

If you must use a secret, the secret should be stored in Azure Key Vault. The service should be configured to use managed identity to access the secret in Key Vault. This is the second most secure way to access secrets.

The application is configured to use Azure Key Vault to load secrets into the app configuration but the functionality is commented out by default. See [Program.cs](service/SE.Sustainability.Template.API/Program.cs) for the commented out code. To enable this functionality, uncomment the code and add the `AppSettings:AzureKeyVault:KeyVaultUrl` setting to the appsettings.\<env\>.json files.
