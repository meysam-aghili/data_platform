using Api.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Api.Services.Repositories;

public class UserAddressRepository(ApiDbContext context) : DbRepository<UserAddress>(context)
{
    public IQueryable<UserAddress> GetBaseQueryable() =>
        GetQueryable()
        .AsNoTracking()
        .AsSplitQuery();
}
