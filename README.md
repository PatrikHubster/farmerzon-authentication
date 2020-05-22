# farmerzon-authentication

The .NET Core Identtiy extension is used as it is. It gets extended with the address of a customer. We do this because the user 
interface requires the address to be stored with the user.

## Development documentation

The first time that I've heard about `SessionScoped`, `RequestScoped` and `ApplicationScoped` was in the Jakarta EE. The same mechanism
also exists in the .NET Core Framework for ASP.NET Core Web Applications. You can register services in the `Startup.cs` file.
A service can be added in three different ways:

```csharp
services.AddSingleton<IService, ServiceImpl>();
services.AddTrancient<IService, ServiceImpl>();
services.AddScoped<IService, ServiceImpl>();
```

`AddSingleton` is the same as `ApplicationScoped`, this means it exists as long as the application is up and running. `AddScoped` is
the same as `RequestScoped` this means that for every HTTP request a new instance is craeted. The next question was how is a `DbContext` 
added to the application. I found the answer under 
[this url](https://neelbhatt.com/2018/02/27/use-dbcontextpooling-to-improve-the-performance-net-core-2-1-feature/). This website explains 
that for every request a new `DbContext` gets created. This means `DbContext` is `RequestScoped` or added with `AddScoped`. The perfomrance
can be increased rapidly with adding the `DbContext` with `AddDbContextPool` instead of `AddDbContext`. 

I made a mistake by thinking that complex objects has to be created manually with the following syntax:

```csharp
services.AddScoped<IService, ServiceImpl>(resolver => 
{
    var firstArgument = services.GetService<IAnotherService>();
    return new ServiceImpl(firstArgument);
});
```

The truth is that this can be done even more elegant with telling the dependency injection container how to add instances. This means tell
the dependency injection container which dependencies you have and how to add them (for example per HTTP request). Doing this allows you
to add as well services which need parameters during instantiation and the dependency injection container will inject the correct instances.

This lead to adding as well the repositories to the services because doing this makes the whole solution independent.