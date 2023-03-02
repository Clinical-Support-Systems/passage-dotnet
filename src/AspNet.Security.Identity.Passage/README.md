# PassageIdentity Authentication Middleware for .NET

## Usage

```c#
builder.Services.AddAuthentication().AddPassage(options =>
{
    options.AppId = builder.Configuration.GetValue<string>("Passage:AppId", string.Empty);
    options.ApiKey = builder.Configuration.GetValue<string>("Passage:ApiKey", string.Empty);
});
```
