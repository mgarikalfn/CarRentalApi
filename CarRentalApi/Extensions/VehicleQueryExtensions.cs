using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;

namespace CarRentalApi.Extensions
{
    // Extensions/VehicleQueryExtensions.cs
    public static class VehicleQueryExtensions
    {
        public static IQueryable<Vehicle> ApplyFilter(this IQueryable<Vehicle> query, VehicleFilterDto filter)
        {
            // Basic filters
            if (!string.IsNullOrEmpty(filter.Make))
                query = query.Where(v => v.Make.Contains(filter.Make));

            if (!string.IsNullOrEmpty(filter.Model))
                query = query.Where(v => v.Model.Contains(filter.Model));

            if (filter.MinYear.HasValue)
                query = query.Where(v => v.Year >= filter.MinYear);

            if (filter.MaxYear.HasValue)
                query = query.Where(v => v.Year <= filter.MaxYear);

            // Price range
            if (filter.MinDailyPrice.HasValue)
                query = query.Where(v => v.DailyPrice >= filter.MinDailyPrice);

            if (filter.MaxDailyPrice.HasValue)
                query = query.Where(v => v.DailyPrice <= filter.MaxDailyPrice);

            // Vehicle specs
            if (!string.IsNullOrEmpty(filter.TransmissionType))
                query = query.Where(v => v.TransmissionType == filter.TransmissionType);

            if (!string.IsNullOrEmpty(filter.FuelType))
                query = query.Where(v => v.FuelType == filter.FuelType);

            if (filter.MinSeats.HasValue)
                query = query.Where(v => v.Seats >= filter.MinSeats);

            // Availability filter
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                query = query.Where(v => v.Availabilities.Any(a =>
                    a.StartDate <= filter.StartDate &&
                    a.EndDate >= filter.EndDate &&
                    !v.Bookings.Any(b =>
                        (b.StartDate <= filter.EndDate && b.EndDate >= filter.StartDate))));
            }

            // Features filter
            if (filter.FeatureIds != null && filter.FeatureIds.Any())
            {
                query = query.Where(v => v.VehicleFeatures
                    .Select(vf => vf.FeatureId)
                    .All(fid => filter.FeatureIds.Contains(fid)));
            }

            return query;
        }

        public static IQueryable<Vehicle> ApplySorting(this IQueryable<Vehicle> query, VehicleFilterDto filter)
        {
            return filter.SortBy.ToLower() switch
            {
                "price" => filter.SortDescending
                    ? query.OrderByDescending(v => v.DailyPrice)
                    : query.OrderBy(v => v.DailyPrice),
                "year" => filter.SortDescending
                    ? query.OrderByDescending(v => v.Year)
                    : query.OrderBy(v => v.Year),
                _ => filter.SortDescending
                    ? query.OrderByDescending(v => v.Make)
                    : query.OrderBy(v => v.Make)
            };
        }

        public static IQueryable<Vehicle> ApplyPagination(this IQueryable<Vehicle> query, VehicleFilterDto filter)
        {
            return query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }
    }
}
