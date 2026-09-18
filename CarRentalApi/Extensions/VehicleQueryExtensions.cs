using Application.Dto.vehicle;
using Domain.Entities;

namespace CarRentalApi.Extensions
{
    public static class VehicleQueryExtensions
    {
        public static IQueryable<Domain.Entities.Vehicle> ApplyFilter(
            this IQueryable<Domain.Entities.Vehicle> query, VehicleFilterDto filter)
        {
            if (!string.IsNullOrEmpty(filter.Make))
                query = query.Where(v => v.Specification.Brand.Contains(filter.Make));

            if (!string.IsNullOrEmpty(filter.Model))
                query = query.Where(v => v.Specification.Model.Contains(filter.Model));

            if (filter.MinYear.HasValue)
                query = query.Where(v => v.Specification.Year >= filter.MinYear);

            if (filter.MaxYear.HasValue)
                query = query.Where(v => v.Specification.Year <= filter.MaxYear);

            if (filter.MinDailyPrice.HasValue)
                query = query.Where(v => v.Price.Amount >= filter.MinDailyPrice);

            if (filter.MaxDailyPrice.HasValue)
                query = query.Where(v => v.Price.Amount <= filter.MaxDailyPrice);

            if (!string.IsNullOrEmpty(filter.TransmissionType))
                query = query.Where(v => v.Specification.Transmission.ToString() == filter.TransmissionType);

            if (!string.IsNullOrEmpty(filter.FuelType))
                query = query.Where(v => v.Specification.FuelType.ToString() == filter.FuelType);

            if (filter.MinSeats.HasValue)
                query = query.Where(v => v.Specification.SeatCount >= filter.MinSeats);

            return query;
        }

        public static IQueryable<Domain.Entities.Vehicle> ApplySorting(
            this IQueryable<Domain.Entities.Vehicle> query, VehicleFilterDto filter)
        {
            return filter.SortBy?.ToLower() switch
            {
                "price" => filter.SortDescending
                    ? query.OrderByDescending(v => v.Price.Amount)
                    : query.OrderBy(v => v.Price.Amount),
                "year" => filter.SortDescending
                    ? query.OrderByDescending(v => v.Specification.Year)
                    : query.OrderBy(v => v.Specification.Year),
                _ => filter.SortDescending
                    ? query.OrderByDescending(v => v.Specification.Brand)
                    : query.OrderBy(v => v.Specification.Brand)
            };
        }

        public static IQueryable<Domain.Entities.Vehicle> ApplyPagination(
            this IQueryable<Domain.Entities.Vehicle> query, VehicleFilterDto filter)
        {
            return query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }
    }
}
