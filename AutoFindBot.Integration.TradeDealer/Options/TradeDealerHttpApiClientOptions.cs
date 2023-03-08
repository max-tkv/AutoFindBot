﻿using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Options;

public class TradeDealerHttpApiClientOptions : HttpApiClientOptions
{
    public string Host { get; set; }
    public string GetAutoByFilterMethod { get; set; }
}