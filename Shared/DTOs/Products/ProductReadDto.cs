﻿using System.ComponentModel.DataAnnotations.Schema;

namespace net_core_web_api_clean_ddd.Shared.DTOs.Products;

public class ProductReadDto
{
    public required Guid Id { get; set; }
    [ForeignKey("CategoryId")] public required Guid CategoryId { get; set; }
    public required string Title { get; set; }
    public required int StockCount { get; set; }
}