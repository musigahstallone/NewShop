var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis").WithRedisCommander().WithDataVolume();
var catalogDb = builder.AddPostgres("catalog").WithDataVolume().AddDatabase("catalogdb");

var orderingApi = builder.AddProject<Projects.NewOrdering>("newordering");

var paymentApi = builder.AddProject<Projects.NewPayment>("newpayment");

var catalogService = builder.AddProject<Projects.NewCatalog>("newcatalog").WithReference(catalogDb);

var basketService = builder.AddProject<Projects.NewBasket>("newbasket").WithReference(cache);

builder.AddProject<Projects.NewShop_Web>("webapp")
    .WithReference(basketService)
    .WithReference(catalogService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
