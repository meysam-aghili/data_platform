using Api.Services.Repositories;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Extentions;
using System.ComponentModel;

namespace Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(
    ProductRepository productRepository,
    CategoryRepository categoryRepository,
    ImageRepository imageRepository) : ControllerBase
{
    private readonly ProductRepository _productRepository = productRepository;
    private readonly CategoryRepository _categoryRepository = categoryRepository;
    private readonly ImageRepository _imageRepository = imageRepository;

    [HttpGet]
    public async Task<IActionResult> GetAsync(
        int page_number = 1,
        int page_size = 30,
        string sort_by = "created_at",
        ListSortDirection sort_direction = ListSortDirection.Descending,
        string? search_slug = null,
        string? brand = null,
        string? color = null,
        bool? in_stock = null,
        int? min_price = null,
        int? max_price = null,
        string? category = null)
    {
        if (page_size > 50)
            return BadRequest("page_size can not be more than 50.");

        List<long> categoryIds = [];
        if (category is not null)
        {
            var categories = await _categoryRepository.GetAsync();
            var categoryModel = (await _categoryRepository.GetAsync(category))?.ToDto(categories);
            if(categoryModel is null)
                return NotFound();
            categoryIds.Add(categoryModel.Id);
            categoryIds.AddRange(Helper.Flatten(categoryModel.Children));
        }

        var models = await _productRepository
            .GetAsync(page_number, page_size, sort_by, sort_direction, search_slug, 
                brand, color, in_stock, min_price, max_price, categoryIds);

        var modelsDto = models.Data.Select(m => m.ToDto()).ToList();
        var imageTasks = modelsDto
            .SelectMany(m => m.ProductVariants ?? Enumerable.Empty<ProductVariantDto>())
            .SelectMany(v => v.Images ?? Enumerable.Empty<ImageDto>())
            .Select(async image =>
            {
                image.Url = await _imageRepository.GetUrl(image.Url);
            });
        await Task.WhenAll(imageTasks);

        return Ok(new
        {
            models.TotalCount,
            models.PageNumber,
            models.PageSize,
            Data = modelsDto,
        });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetAsync(string slug)
    {
        var model = await _productRepository.GetAsync(slug);
        if (model is null)
            return NotFound();

        var modelDto = model.ToDto();
        var imageTasks = (modelDto.ProductVariants ?? Enumerable.Empty<ProductVariantDto>())
            .SelectMany(m => m.Images ?? Enumerable.Empty<ImageDto>())
            .Select(async image =>
            {
                image.Url = await _imageRepository.GetUrl(image.Url);
            });
        await Task.WhenAll(imageTasks);

        return Ok(modelDto);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        var model = await _productRepository.GetAsync(id);
        if (model is null)
            return NotFound();

        var modelDto = model.ToDto();
        var imageTasks = (modelDto.ProductVariants ?? Enumerable.Empty<ProductVariantDto>())
            .SelectMany(m => m.Images ?? Enumerable.Empty<ImageDto>())
            .Select(async image =>
            {
                image.Url = await _imageRepository.GetUrl(image.Url);
            });
        await Task.WhenAll(imageTasks);

        return Ok(modelDto);
    }
}
