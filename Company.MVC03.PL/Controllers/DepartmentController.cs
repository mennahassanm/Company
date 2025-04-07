using AutoMapper;
using Company.MVC.BLL.Interfaces;
using Company.MVC.BLL.Repositories;
using Company.MVC.DAL.Models;
using Company.MVC.PL.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Company.MVC.PL.Controllers
{
    [Authorize ]

    // MVC Controller
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IDepartmentRepositories _departmentRepositories;
        //private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        // ASK CLR Create Object From DepartmentRepositories
        public DepartmentController(
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            //_departmentRepositories = departmentRepositories;
            //_employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet] // Get: / Department /Index 
        public async Task<IActionResult> Index(string? SearchInput)
        {
            IEnumerable<Department> departments;
            if (string.IsNullOrEmpty(SearchInput))
            {
                departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            }
            else
            {
                departments = await _unitOfWork.DepartmentRepository.GetByNameAsync(SearchInput);
            }



            return View(departments);
        }

        [HttpGet] // GET : /Department/Index
        public async Task<IActionResult> Search(string? SearchInput)
        {
            IEnumerable<Department> departments;
            if (string.IsNullOrEmpty(SearchInput))
            {
                departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            }
            else
            {
                departments = await _unitOfWork.DepartmentRepository.GetByNameAsync(SearchInput);
            }
            return PartialView("DepartmentPartialView/DepartmentsTablePartialView", departments);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var employee = await _unitOfWork.EmployeeRepository.GetAllAsync();
            ViewData["employee"] = employee;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentDTO model)
        {
            if (ModelState.IsValid)
            {
                var department = _mapper.Map<Department>(model);
                await _unitOfWork.DepartmentRepository.AddAsync(department);
                var count = await _unitOfWork.CompleteAsync();
                if (count > 0)
                {
                    TempData["Message"] = "Department is Created !!";
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if (id is null) return BadRequest("Invalid Id");// 400

            var department = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            if (department is null) return NotFound(new { StatusCode = 400, message = $"Department With Id : {id} is not found" });

            return View(viewName, department);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id , string viewName = "Edit")
        {
            if (id is null) return BadRequest("Invalid Id");// 400
            var department = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            var employee = await _unitOfWork.EmployeeRepository.GetAllAsync();
            ViewData["employees"] = employee;

            if (department is null) return NotFound(new { StatusCode = 400, message = $"Employee With Id : {id} is not found" });
            var dto = _mapper.Map<CreateDepartmentDTO>(department);

            return View(viewName , dto);
        }

        //[HttpPost]
        //// [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit([FromRoute] int id, CreateDepartmentDTO model, string viewName = "Edit")
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var department = _mapper.Map<Department>(model);
        //        _unitOfWork.DepartmentRepository.Update(department);
        //        var count = await _unitOfWork.CompleteAsync();
        //        if (count > 0)
        //        {
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }

        //    return View(viewName, model);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, UpdateDepartmentDTO model)
        {

            if (ModelState.IsValid)
            {
                var department = new Department()
                {
                    Id = id,
                    Name = model.Name,
                    Code = model.Code,
                    CreateAt = model.CreateAt,

                };
                 _unitOfWork.DepartmentRepository.Update(department);
                var count = await _unitOfWork.CompleteAsync();
                if (count > 0)
                {
                    return RedirectToAction(nameof(Index));
                }

            }


            return View(model);
        }


        [HttpGet]
        public Task<IActionResult> Delete(int? id)
        {
            //if (id is null) return BadRequest("Invalid Id");// 400

            //var department = _departmentRepositories.Get(id.Value);
            //if (department is null) return NotFound(new { StatusCode = 400, message = $"Department With Id : {id} is not found" });

            return Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int? id, Department department)
        {
            //if (ModelState.IsValid)
            //{
            if (id is null) return BadRequest($" This Id = {id} InValid");


            var departmentDelete = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            _unitOfWork.DepartmentRepository.Delete(departmentDelete);

            var Count = await _unitOfWork.CompleteAsync();

            if (Count > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            //}
            return View(department);
        }



    }
}
