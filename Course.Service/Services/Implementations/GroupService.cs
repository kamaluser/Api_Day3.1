using Course.Core.Entities;
using Course.Data;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Exceptions;
using Course.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Services.Implementations
{
    public class GroupService:IGroupService
    {
        private readonly AppDbContext _context;

        public GroupService(AppDbContext context)
        {
            _context = context;
        }

        public int Create(GroupCreateDto dto)
        {
            if (_context.Groups.Any(x => x.No == dto.No))
                throw new DuplicateEntityException();

            Group group = new Group
            {
                No = dto.No,
                Limit = dto.Limit,
            };

            _context.Groups.Add(group);
            _context.SaveChanges();
            return group.Id;
        }

        public List<GroupGetDto> GetAll()
        {
            return _context.Groups.Where(x=>!x.IsDeleted).Select(x => new GroupGetDto
            {
                Id = x.Id,
                No = x.No,
                Limit = x.Limit,
            }).ToList();
        }

        public void Edit(int id, GroupEditDto dto)
        {
            Group group = _context.Groups.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (group == null)
                throw new EntityNotFoundException();

            group.No = dto.No;
            group.Limit = dto.Limit;

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            Group group = _context.Groups.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (group == null)
                throw new EntityNotFoundException();

            group.IsDeleted = true;
            _context.Groups.Remove(group);
            _context.SaveChanges();
        }

        public GroupGetDto GetById(int id)
        {
            Group group = _context.Groups.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (group == null)
                throw new EntityNotFoundException("Group with given id doesnt exist.");

            return new GroupGetDto
            {
                Id = group.Id,
                No = group.No,
                Limit = group.Limit,
            };
        }
    }
}
