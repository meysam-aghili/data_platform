using Api.Data;
using Shared.Models;

namespace Api.Services.Repositories;

public class StateRepository(ApiDbContext context) : DbRepository<State>(context);
