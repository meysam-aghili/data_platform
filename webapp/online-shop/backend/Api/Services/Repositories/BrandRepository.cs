using Api.Data;
using Shared.Models;

namespace Api.Services.Repositories;

public class BrandRepository(ApiDbContext context) : DbRepository<Brand>(context);
