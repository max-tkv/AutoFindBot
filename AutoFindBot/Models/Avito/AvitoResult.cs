﻿using AutoFindBot.Lookups;

namespace AutoFindBot.Models.Avito;

public class AvitoResult
{
    public SourceType SourceType { get; set; }
    
    public string OriginId { get; set; }
    
    public string Title { get; set; }
    
    public string Сity { get; set; }
    
    public int Price { get; set; }
    
    public DateTime PublishedAt { get; set; }
    
    public string Url { get; set; }
    
    public int Year { get; set; }
    
    public string Vin { get; set; }
    
    public List<AvitoImage> Images { get; set; }
}