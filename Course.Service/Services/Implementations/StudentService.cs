using Course.Core.Entities;
using Course.Data;
using Course.Service.Dtos.StudentDtos;
using Course.Service.Exceptions;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public StudentService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public int Create(StudentCreateDto dto)
        {
            if (_context.Students.Any(x => x.Email == dto.Email))
                throw new DuplicateEntityException($"Student with email {dto.Email} already exists.");

            Group group = _context.Groups.Include(x => x.Students).FirstOrDefault(x => x.Id == dto.GroupId && !x.IsDeleted);

            if (group == null)
                throw new EntityNotFoundException($"Group with ID {dto.GroupId} not found.");

            if (group.Limit <= group.Students.Count)
                throw new GroupLimitException($"Group is full!");

            string file = null;
            if (dto.File != null)
            {
                file = SaveFile(dto.File);
            }

            Student student = new Student
            {
                FullName = dto.FullName,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                GroupId = dto.GroupId,
                Photo = file
            };

            _context.Students.Add(student);
            _context.SaveChanges();
            return student.Id;
        }

        public List<StudentGetDto> GetAll()
        {
            var students = _context.Students.Include(x => x.Group).Where(x=>!x.IsDeleted).Select(x => new StudentGetDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                BirthDate = x.BirthDate,
                GroupId = x.GroupId,
                GroupName = x.Group.No,
                PhotoUrl = x.Photo!=null?null:$"/uploads/student/{x.Photo}"
            }).ToList();
            return students;
        }

        public StudentGetDto GetById(int id)
        {
            var student = _context.Students.Include(x => x.Group).FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (student == null)
                throw new EntityNotFoundException($"Student with {id} ID not found.");

            return new StudentGetDto
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                BirthDate = student.BirthDate,
                GroupId = student.GroupId,
                GroupName = student.Group.No,
                PhotoUrl = student.Photo != null ? null : $"uploads/student/{student.Photo}"
            };
        }
        public void Edit(int id, StudentEditDto dto)
        {
            var student = _context.Students.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (student == null)
                throw new EntityNotFoundException($"Student with {id} ID not found.");

            Group group = _context.Groups.Include(x => x.Students).FirstOrDefault(x => x.Id == dto.GroupId && !x.IsDeleted);

            if (group == null)
                throw new EntityNotFoundException($"Group with ID {dto.GroupId} not found.");

            if (group.Limit <= group.Students.Count)
                throw new GroupLimitException($"Group is full!");

            if (dto.File != null)
            {
                string path = SaveFile(dto.File);
                student.Photo = path;
            }

            student.FullName = dto.FullName;
            student.Email = dto.Email;
            student.BirthDate = dto.BirthDate;
            student.GroupId = dto.GroupId;

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var student = _context.Students.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (student == null)
                throw new EntityNotFoundException($"Student with {id} ID not found.");

            student.IsDeleted = true;
            _context.Students.Remove(student);
            _context.SaveChanges();
        }

        private string SaveFile(IFormFile file)
        {
            string uploadDir = Path.Combine(_environment.WebRootPath, "uploads/student");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadDir, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return fileName;
        }
    }
}
