global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using System.ComponentModel.DataAnnotations;
global using System.Net.Http.Json;
global using System.Web;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;

global using NewShop.Web.Components;
global using NewShop.Web.Components.Layout;
global using NewShop.Web.Components.Pages;
global using NewShop.Web.Components.Pages.Cart;
global using NewShop.Web.Components.Pages.Catalog;
global using NewShop.Web.Components.Pages.Checkout;
global using NewShop.Web.Components.Pages.Item;
global using NewShop.Web.Components.Pages.User;
global using NewShop.Web.Utilities.Services.CatalogServe;

global using NewShop.Web.Utilities.Item;
global using NewShop.Web.Utilities.Services.BasketServe;
global using NewShop.Web.Utilities.Services;
global using NewShop.Web.Utilities.Services.OrderStatus;


global using NewShop.Web.Utilities.SmallUnits;

// BasketServices
global using NewBasket.Grpc;
global using GrpcBasketItem = NewBasket.Grpc.BasketItem;
global using GrpcBasketClient = NewBasket.Grpc.Basket.BasketClient;