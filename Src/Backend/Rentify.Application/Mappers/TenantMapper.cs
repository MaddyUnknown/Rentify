using Rentify.Application.DTOs.Tenant;
using Rentify.Application.Enums;
using Rentify.Core.Entities;
using Rentify.Core.Enums;
using Rentify.Core.ValueObjects;
using Rentify.DataAccess.Core.QueryResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Mappers
{
    public class TenantMapper
    {
        public static TenantDto MapToTenantDto(Tenant tenant, IEnumerable<TenantEmergencyContact> tenantEmergencyContacts, IEnumerable<MediaFile> files)
        {
            var detailsDto = new GetTenantDetailsDto
            {
                Name = tenant.Name,
                Email = tenant.Email,
                PhoneNumber = tenant.PhoneNumber,
                Dob = DateOnly.FromDateTime(tenant.Dob),
                Employment = tenant.Employment,
                StreetName = tenant.TenantAddress.StreetName,
                City = tenant.TenantAddress.City,
                State = tenant.TenantAddress.State,
                ZipCode = tenant.TenantAddress.ZipCode,
                Note = tenant.Note ?? string.Empty
            };

            var emergencyContactDto = tenantEmergencyContacts.Select(c => MapToTenantEmergencyContactDto(c)).First();

            var filesDtos = files
                            .Where(f => !f.MediaFileLink.Tags.Any(f => f.Tag == MediaFileLinkTagEnum.ProfilePic)
                            )
                            .Select(f => MediaFileMapper.MapToMediaFileDto(f))
                            .ToList();

            var profilePic = files
                            .Where(f => f.MediaFileLink.Tags.Any(f => f.Tag == MediaFileLinkTagEnum.ProfilePic))
                            .Select(f => MediaFileMapper.MapToMediaFileDto(f))
                            .FirstOrDefault();

            return new TenantDto
            {
                Id = tenant.Id,
                Details = detailsDto,
                EmergencyContact = emergencyContactDto,
                ProfilePic = profilePic,
                MediaFiles = filesDtos
            };
        }

        public static TenantDetailsDto MapToTenantDetailsDto(Tenant tenant)
        {
            return new TenantDetailsDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Email = tenant.Email,
                PhoneNumber = tenant.PhoneNumber,
                Dob = DateOnly.FromDateTime(tenant.Dob),
                Employment = tenant.Employment,
                Note = tenant.Note ?? string.Empty,
                StreetName = tenant.TenantAddress.StreetName,
                City = tenant.TenantAddress.City,
                State = tenant.TenantAddress.State,
                ZipCode = tenant.TenantAddress.ZipCode
            };
        }

        public static TenantEmergencyContactDto MapToTenantEmergencyContactDto(TenantEmergencyContact tenantEmergencyContact)
        {
            return new TenantEmergencyContactDto
            {
                Id = tenantEmergencyContact.Id,
                Name = tenantEmergencyContact.Name,
                Email = tenantEmergencyContact.Email,
                PhoneNumber = tenantEmergencyContact.PhoneNumber,
                Relationship = tenantEmergencyContact.Relationship
            };
        }

        public static TenantSummaryDto MapToTenantSummaryDto(TenantSummaryQueryResult tenantSummaryQueryResult)
        {
            return new TenantSummaryDto
            {
                Id = tenantSummaryQueryResult.Id,
                Name = tenantSummaryQueryResult.Name,
                Email = tenantSummaryQueryResult.Email,
                PhoneNumber = tenantSummaryQueryResult.PhoneNumber,
                Status = TenantStatusEnum.Active,
                ProfilePic = tenantSummaryQueryResult.ProfilePic == null ? null : MediaFileMapper.MapToMediaFileDto(tenantSummaryQueryResult.ProfilePic)
            };
        }
    }
}
