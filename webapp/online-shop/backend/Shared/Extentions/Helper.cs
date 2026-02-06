using Shared.Dtos;
using Shared.Models;

namespace Shared.Extentions;

public static class Helper
{
    public static List<long> Flatten(List<CategoryDto> children)
    {
        return children
            .SelectMany(c => new[] { c.Id }.Concat(Flatten(c.Children)))
            .ToList();
    }

    public static List<string> FlattenSlug(List<CategoryDto> children)
    {
        var result = new List<string>();

        if (children == null)
            return result;

        foreach (var c in children)
        {
            result.Add(c.Slug);
            result.AddRange(FlattenSlug(c.Children));
        }

        return result;
    }
        public static CategoryDto? FilterBySlug(this CategoryDto category, string slug)
        {
            if (category.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
            {
                return new CategoryDto
                {
                    Id = category.Id,
                    Title = category.Title,
                    Slug = category.Slug,
                    Children = []
                };
            }

            // Filter children recursively using LINQ
            var matchingChildren = category.Children
                .Select(c => c.FilterBySlug(slug))
                .Where(c => c != null)
                .Cast<CategoryDto>()
                .ToList();

            // If any child contains the slug, propagate this branch upward
            if (matchingChildren.Any())
            {
                return new CategoryDto
                {
                    Id = category.Id,
                    Title = category.Title,
                    Slug = category.Slug,
                    Children = matchingChildren
                };
            }

            return null;
        }

        public static List<CategoryDto> FilterTreeBySlug(this IEnumerable<CategoryDto> root, string slug)
        {
            return root
                .Select(c => c.FilterBySlug(slug))
                .Where(c => c != null)
                .Cast<CategoryDto>()
                .ToList();
        }
    

}
