using Shared.Dtos;
using Shared.Models;

namespace Shared.Extentions;

public static class Mapper
{
    public static CategoryDto ToDto(this Category model, List<Category>? categories = null) {
        var children = categories?
            .Where(c => c.ParentId == model.Id)
            .Select(c => c.ToDto(categories))
            .ToList() ?? [];

        return new()
        {
            Id = model.Id,
            Title = model.Title,
            Slug = model.Slug,
            Children = children
        };
    }

    public static ImageDto ToDto(this Image model) =>
        new()
        {
            Id = model.Id,
            Url = model.Url,
            Slug = model.Slug,
            Title = model.Title,
            SortNumber = model.SortNumber
        };

    public static ProductVariantDto ToDto(this ProductVariant model) =>
        new()
        {
            Id = model.Id,
            Price = model.Price,
            Title = model.Title,
            ColorTitle = model.Color.Title,
            SizeTitle = model.Size.Title,
            Description = model.Description,
            Slug = model.Slug,
            Images = model.Images?.Select(d => d.ToDto()).ToList(),
            Stock = model.Stock,
            SortNumber = model.SortNumber,
            Discount = model.Discount
        };

    public static ProductDto ToDto(this Product model) =>
        new()
        {
            Id = model.Id,
            Title = model.Title,
            Slug = model.Slug,
            Description = model.Description,
            ProductVariants = model.ProductVariants?.Select(pv => pv.ToDto()).ToList(),
            Category = model.Category.ToDto(),
            BrandTitle = model.Brand.Title
        };

    public static UserAddressDto ToDto(this UserAddress model) =>
        new()
        {
            Id = model.Id,
            Title = model.Title,
            Slug = model.Slug,
            Address = model.Address,
            CityTitle = model.City.Title,
            StateTitle = model.City.State.Title,
            PostalCode = model.PostalCode
        };

    public static UserDto ToDto(this User model) =>
        new()
        {
            Id = model.Id,
            Title = model.Title,
            Slug = model.Slug,
            Phone = model.Phone,
            UserAddresses = model.UserAddresses?.Select(ua => ua.ToDto()).ToList()
        };
}
