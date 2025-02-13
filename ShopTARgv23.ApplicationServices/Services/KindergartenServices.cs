﻿using Microsoft.EntityFrameworkCore;
using ShopTARgv23.Core.Domain;
using ShopTARgv23.Core.Dto;
using ShopTARgv23.Core.ServiceInterface;
using ShopTARgv23.Data;

namespace ShopTARgv23.ApplicationServices.Services
{
    public class KindergartenServices : IKindergartenServices
    {
        private readonly ShopTARgv23Context _context;
        private readonly IFileServices _fileServices;
        public KindergartenServices(ShopTARgv23Context context, IFileServices fileServices)
        {
            _context = context;
            _fileServices = fileServices;
        }

        public async Task<Kindergarten> Details(Guid id)
        {
            var result = await _context.Kindergarten.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<Kindergarten> Create(KindergartenDto dto)
        {
            Kindergarten kindergarten = new Kindergarten();

            kindergarten.Id = Guid.NewGuid();
            kindergarten.GroupName = dto.GroupName;
            kindergarten.ChildrenCount = dto.ChildrenCount;
            kindergarten.KindergartenName = dto.KindergartenName;
            kindergarten.Teacher = dto.Teacher;
            kindergarten.CreatedAt = DateTime.Now;
            kindergarten.UpdatedAt = DateTime.Now;

            if (dto.Files != null)
            {
                _fileServices.UploadFilesToDatabase(dto, kindergarten);
            }

            await _context.Kindergarten.AddAsync(kindergarten);
            await _context.SaveChangesAsync();

            return kindergarten;
        }

        public async Task<Kindergarten> Update(KindergartenDto dto)
        {
            var domain = new Kindergarten()
            {
                Id = dto.Id,
                GroupName = dto.GroupName,
                ChildrenCount = dto.ChildrenCount,
                KindergartenName = dto.KindergartenName,
                Teacher = dto.Teacher,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = DateTime.Now,
            };

            if (dto.Files != null)
            {
                _fileServices.UploadFilesToDatabase(dto, domain);
            }

            _context.Kindergarten.Update(domain);
            await _context.SaveChangesAsync();

            return domain;
        }

        public async Task<Kindergarten> Delete(Guid id)
        {
            var result = await _context.Kindergarten
               .FirstOrDefaultAsync(x => x.Id == id);

            var images = await _context.FileToDatabases
                .Where(x => x.KindergartenId == id)
                .Select(y => new FileToDatabaseDto
                {
                    Id = y.Id,
                    ImageTitle = y.ImageTitle,
                    KindergartenId = y.KindergartenId
                }).ToArrayAsync();

            await _fileServices.RemoveImagesFromDatabase(images);
            _context.Kindergarten.Remove(result);
            await _context.SaveChangesAsync();
            return result;
        }
    }
}