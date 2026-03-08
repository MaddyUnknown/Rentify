using Rentify.Application.DTOs;
using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Property;
using Rentify.Application.DTOs.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Interfaces.Services
{
    public interface ITenantService
    {
        Task<PaginatedList<TenantSummaryDto>> GetAllTenantAsync(TenantSearchDto tenantSearchDto);
        Task<TenantDto> GetTenantByIdAsync(int id);
        Task<TenantDetailsDto> UpdateTenantAsync(int id, UpdateTenantDetailsDto updateTenantDto);
        Task<TenantDetailsDto> DeleteTenantAsync(int id);
        Task<TenantEmergencyContactDto> UpdateTenantEmergencyContactAsync(int tenantId, int tenantEmergencyContactId, UpdateTenantEmergencyContactDto updateTenantEmergencyContactDto);
        Task<TenantDto> CreateTenantAsync(CreateTenantDto createTenantDto);
        Task<MediaFileDto> UpdateProfilePic(UpdateTenantProfilePicDto updateTenantProfilePic, CancellationToken ct = default);
    }
}
