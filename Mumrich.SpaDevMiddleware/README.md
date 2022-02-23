# Mumrich.SpaDevMiddleware

## Usage

1. Install NuGet `Mumrich.SpaDevMiddleware` into the desired **ASP.NET Core 6** project
2. Implement the `Mumrich.SpaDevMiddleware.Contracts.ISpaDevServerSettings` interface

3. Register `RegisterSinglePageAppDevMiddleware` for the background-node instances to run automatically:

   ```csharp
   builder.RegisterSinglePageAppDevMiddleware(appSettings.SinglePageApps);
   ```

4. Map the single page applications:
   1. As group: `app.MapSinglePageApps(appSettings.SinglePageApps);`
   2. One by one: `app.MapSinglePageApp("/", new SpaSettings {/* options... */ });`
