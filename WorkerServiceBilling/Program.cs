using Autofac.Core;
using Microsoft.EntityFrameworkCore;
using Backend.PriceComparison.Domain.Billing.Ports;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Infrastructure.External.Plemsi.Adapter.FE.Retail;
using Backend.PriceComparison.Infrastructure.External.Plemsi.Adapter.POS.EDS;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.ClientBillintElectronic.DomainService.Impl;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;
using WorkerServiceBilling;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseMySql("Server=51.81.90.175;Database=eduar_facturacion_dian;User Id=eduar;Password=PoliApi2024*;ConvertZeroDateTime=True",
                     new MySqlServerVersion(new Version(8, 0, 30))));
builder.Services.AddTransient<IClientRepository, ClientBillingDomainService>();
//builder.Services.AddTransient<IBillingService, BillingPosService>();
//builder.Services.AddTransient<IInvoiceRepository,InvoiceFEService>();
//builder.Services.AddTransient<IInvoiceLastRepository, InvoiceLastFERepository>();
//builder.Services.AddTransient<IGetItem,GetItem>();
//builder.Services.AddTransient<IGetItemsInvoicePos, GetItemsInvoicePos>();
//builder.Services.AddTransient<IInsertInvoice, InsertInvoice>();
//builder.Services.AddTransient<IDatabaseUtils, DatabaseUtils>();
//builder.Services.Scan(scan => scan
//    .FromAssembliesOf(typeof(IDatabaseUtils))
//    .AddClasses()
//    .AsImplementedInterfaces()
//    .WithTransientLifetime());

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();

