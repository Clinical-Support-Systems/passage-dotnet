# passage-dotnet
.NET SDK for Passage Identity

## How to get running

1. Create account at [passage.id](https://passage.id/)
2. Add `appsettings.Development.json` to PassageIdentity.Tests and PassageIdentity.ExampleApp that contains:

```json
"Passage": {
  "AppId": "an app id",
  "ApiKey": "an api key"
}
```

## Contribute

If you'd like to contribute, I'd love the help.

## Todo

- [X] Setup packages
- [X] Setup CI/CD
- [X] Setup example
- [ ] Middleware integration with AspNetCore Identity
