using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.Property;
using Rentify.Core.Entities;
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
