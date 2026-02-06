using Api.Data;
using Shared.Models;

namespace Api.Services.Repositories;

public class ColorRepository(ApiDbContext context) : DbRepository<Color>(context);
