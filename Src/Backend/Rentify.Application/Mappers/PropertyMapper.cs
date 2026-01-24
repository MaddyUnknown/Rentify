using Rentify.Application.DTOs;
using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.Property;
using Rentify.Core.Entities;
using Rentify.DataAccess.Core.QueryResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Mappers
{
    public static class PropertyMapper
    {
        public static PropertyDto MapToPropertyDto(Property property, IEnumerable<Unit> units, IEnumerable<MediaFile> files)
        {
            var propertyDetailDto = new GetPropertyDetailsDto
            {
                Name = property.Name,
                StreetName = property.PropertyAddress?.StreetName ?? string.Empty,
                City = property.PropertyAddress?.City ?? string.Empty,
                State = property.PropertyAddress?.State ?? string.Empty,
                ZipCode = property.PropertyAddress?.ZipCode ?? string.Empty,
                Description = property.Description ?? string.Empty,
            };

            LocationDto? locationDto = (property.PropertyLocation?.Latitude == null || property.PropertyLocation?.Longitude == null)
                ? null
                : new LocationDto
                {
                    Latitude = property.PropertyLocation.Latitude,
                    Longitude = property.PropertyLocation.Longitude,
                };

            var unitDtos = units.Select(u => UnitMapper.MapToUnitDto(u));

            var filesDtos = files.Select(f => MediaFileMapper.MapToMediaFileDto(f));

            return new PropertyDto
            {
                Id = property.Id,
                GeneralDetails = propertyDetailDto,
                MediaFiles = filesDtos,
                Location = locationDto,
                Units = unitDtos

            };
        }

        public static PropertyDetailsDto MapToPropertyDetailsDto(Property property)
        {
            return new PropertyDetailsDto
            {
                Id = property.Id,
                Name = property.Name,
                StreetName = property.PropertyAddress?.StreetName ?? string.Empty,
                City = property.PropertyAddress?.City ?? string.Empty,
                State = property.PropertyAddress?.State ?? string.Empty,
                ZipCode = property.PropertyAddress?.ZipCode ?? string.Empty,
                Description = property.Description ?? string.Empty,
            };
        }

        public static PropertySummaryDto MapToPropertySummaryDto(PropertySummaryQueryResult property)
        {
            return new PropertySummaryDto
            {
                Id = property.Id,
                Name = property.Name,
                Address = $"{property.Address.StreetName}, {property.Address.City}, {property.Address.State}, {property.Address.ZipCode}",
                NumberOfUnits = property.NumberOfUnits,
                NumberOfVacantUnits = 1,
                CoverImage = property.CoverImage == null ? null : MediaFileMapper.MapToMediaFileDto(property.CoverImage),
            };
        }

        public static PropertyLocationDto MapToPropertyLocationDto(Property property)
        {
            return new PropertyLocationDto
            {
                PropertyId = property.Id,
                Latitude = property.PropertyLocation?.Latitude,
                Longitude = property.PropertyLocation?.Longitude,
            };
        }
    }
}
