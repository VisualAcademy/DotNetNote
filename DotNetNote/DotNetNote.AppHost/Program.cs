var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DotNetNote_Web>("dotnetnote-web");

builder.Build().Run();
