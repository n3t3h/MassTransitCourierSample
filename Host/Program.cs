using MassTransit;
using MassTransit.Courier;
using MassTransit.MongoDbIntegration.Courier;
using MassTransit.MongoDbIntegration.Courier.Documents;
using MassTransit.RabbitMqTransport;
using MongoDB.Driver;
using System;

namespace Host
{
    class Program
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<RoutingSlipDocument> _routingSlips;
        private readonly IBusControl _busControl;
        private readonly IRoutingSlipEventPersister _persister;

        public Program()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("masstransitcouriersample");
            _routingSlips = _database.GetCollection<RoutingSlipDocument>("routing_slips");
            _persister = new RoutingSlipEventPersister(_routingSlips);

            _busControl = Bus.Factory.CreateUsingRabbitMq(cfg => {
                var host = cfg.Host(new Uri("rabbit://localhost"), hc => {
                    hc.Username("guest");
                    hc.Password("guest");
                });

                /*
                    We can now use these in endpoint configurators

                    ep.RoutingSlipActivityEventConsumers(_persister);
                    ep.RoutingSlipEventConsumers(_persister);
                 */

                cfg.ReceiveEndpoint(host, "generatefinalinvoice", ep => {
                    ep.ExecuteActivityHost<GenerateFinalProratedInvoice, GenerateFinalProratedInvoiceArguments>();
                });

                cfg.ReceiveEndpoint(host, "deletefinalinvoice", ep => {
                    ep.CompensateActivityHost<GenerateFinalProratedInvoice, GenerateFinalProratedInvoiceLog>();
                });

                cfg.ReceiveEndpoint(host, "chargeallopeninvoices", ep => {
                    ep.ExecuteActivityHost<ChargeAllOpenInvoices, ChargeAllOpenInvoicesArguments>();
                });

                cfg.ReceiveEndpoint(host, "dontcompensatecharginginvoices", ep => {
                    ep.CompensateActivityHost<ChargeAllOpenInvoices, ChargeAllOpenInvoicesLog>();
                });

                cfg.ReceiveEndpoint(host, "closeaccount", ep => {
                    ep.ExecuteActivityHost<CloseAccount, CloseAccountArguments>();
                });

                cfg.ReceiveEndpoint(host, "openaccount", ep => {
                    ep.CompensateActivityHost<CloseAccount, CloseAccountLog>();
                });
            });
        }

        public void Run()
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity(nameof(GenerateFinalProratedInvoice), new Uri("rabbitmq://localhost/generatefinalinvoice"));
            builder.AddActivity(nameof(ChargeAllOpenInvoices), new Uri("rabbitmq://localhost/chargeallopeninvoices"));
            builder.AddActivity(nameof(CloseAccount), new Uri("rabbitmq://localhost/closeaccount"));
            var routingSlip = builder.Build();
            _busControl.Execute(routingSlip);
        }

        static void Main(string[] args)
        {
            new Program().Run();
            Console.ReadLine();
        }
    }
}
