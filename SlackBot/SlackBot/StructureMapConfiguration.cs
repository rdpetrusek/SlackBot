using SlackBot.SlackBotCommands;
using StructureMap;
using System;

namespace SlackBot
{
    public static class StructureMapConfiguration
    {
        public static Container Initialize()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AddAllTypesOf(typeof(IBotCommand<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IBotCommand<>));

                    scanner.AddAllTypesOf(typeof(IBotCommand<,>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IBotCommand<,>));
                });
            });

            Console.WriteLine(container.WhatDoIHave());
            return container;
        }
    }
}
