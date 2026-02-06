using Api.Services.Repositories;
using Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Extentions;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

[ApiController]
[Route("api")]
public class GeneralController(
    ColorRepository colorRepository, 
    BrandRepository brandRepository, 
    CategoryRepository categoryRepository,
    StateRepository stateRepository,
    CityRepository cityRepository,
    UserRepository userRepository,
    UserAddressRepository userAddressRepository) : ControllerBase
{
    private readonly ColorRepository _colorRepository = colorRepository;
    private readonly BrandRepository _brandRepository = brandRepository;
    private readonly CategoryRepository _categoryRepository = categoryRepository;
    private readonly StateRepository _stateRepository = stateRepository;
    private readonly CityRepository _cityRepository = cityRepository;
    private readonly UserRepository _userRepository = userRepository;
    private readonly UserAddressRepository _userAddressRepository = userAddressRepository;

    [HttpGet("colors")]
    public async Task<IActionResult> GetColorsAsync()
    {
        var models = await _colorRepository.GetAsync();
        return Ok(models.Select(m => m.Title));
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrandsAsync()
    {
        var models = await _brandRepository.GetAsync();
        return Ok(models.Select(m => m.Title));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var models = await _categoryRepository.GetAsync();
        return Ok(models.Where(r => r.Level == 1).Select(m => m.ToDto(models)));
    }

    [HttpGet("categories/{slug}/parent")]
    public async Task<IActionResult> GetCategoriesAsync(string slug)
    {
        var models = await _categoryRepository.GetAsync();
        var categories = models.Where(r => r.Level == 1).Select(m => m.ToDto(models));
        var filtered = categories.FilterTreeBySlug(slug);

        return Ok(Helper.FlattenSlug(filtered));
    }

    [HttpGet("states")]
    public async Task<IActionResult> GetStatesAsync()
    {
        var models = await _stateRepository.GetAsync();
        
        return Ok(models.Select(r => r.Title));
    }

    [HttpGet("cities")]
    public async Task<IActionResult> GetCitiesAsync()
    {
        var models = await _cityRepository.GetAsync();

        var result = models
        .GroupBy(c => c.State.Title)
        .Select(g => new {
            State = g.Key,
            Cities = g.Select(c => c.Title).ToList()
        });

        return Ok(result);
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUserAsync()
    {
        var username = User.Identity!.Name!;
        var model = await _userRepository.GetAsync(username);
        if (model is null)
            return NotFound("Your profile does not exists.");

        return Ok(model.ToDto());
    }

    [Authorize]
    [HttpPost("user-address")]
    public async Task<IActionResult> PostUserAddressAsync([FromBody] UserAddressCreateDto dto)
    {
        var username = User.Identity!.Name!;
        var model = await _userRepository.GetAsync(username, track: true);
        if (model is null)
            return NotFound("Your profile does not exists.");

        var cityModel = await _cityRepository.GetAsync(dto.CityTitle, dto.StateTitle);

        if (cityModel is null)
            return BadRequest("city or state not found.");

        await _userAddressRepository.AddAsync(
            new()
            {
                Address = dto.Address,
                Title = dto.Title,
                Slug = string.Empty,
                PostalCode = dto.PostalCode,
                CityId = cityModel.Id,
                UserId = model.Id
            }
        );

        return Ok();
    }

    [Authorize]
    [HttpDelete("user-address/{id}")]
    public async Task<IActionResult> DeleteUserAddressAsync(long id)
    {
        var username = User.Identity!.Name!;
        var model = await _userAddressRepository.GetAsync(id);
        if (model is null)
            return NotFound("User address does not exists.");

        model.IsDeleted = true;
        await _userAddressRepository.UpdateAsync(model);

        return Ok();
    }
}
