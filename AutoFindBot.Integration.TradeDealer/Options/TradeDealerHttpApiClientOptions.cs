﻿using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Options;

public class TradeDealerHttpApiClientOptions : HttpApiClientOptions
{
    public string SiteUrl { get; set; }
    
    public string GetAutoByFilterQuery { get; set; }
}