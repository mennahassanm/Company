using System.Drawing;
using AutoMapper;
using Company.MVC.BLL.Interfaces;
using Company.MVC.DAL.Models;
using Company.MVC.PL.DTOS;
using Company.MVC.PL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Company.MVC.PL.Controllers
{

    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IEmployeeRepository _employeeRepository;
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        // ASK CLR Create Object From IEmployeeRepository

        public EmployeeController(
            //IEmployeeRepository employeeRepository , 
            //IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            //_employeeRepository = employeeRepository;
            //_departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        [HttpGet] // GET : /Department/Index
        public async Task<IActionResult> Index(string? SearchInput)
        {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(SearchInput))
            {
                employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
            }
            else
            {
                employees = await _unitOfWork.EmployeeRepository.GetByNameAsync(SearchInput);
            }

            // Dictionary   : 3 Property
            // 1. ViewData  : Transfer Extra Information From Controller (Action) To View

            //ViewData["Message"] = "Hello From ViewDat,a";

            // 2. ViewBag   : Transfer Extra Information From Controller (Action) To View

            //ViewBag.Massage = "Hello From ViewBag";

            //ViewBag.Message = new { Message = "Hello From ViewBag" };

            // 3. TempData  :
            return View(employees);


        }

        [HttpGet]
        public async Task<IActionResult> Search(string? SearchInput)
        {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(SearchInput))
            {
                employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
            }
            else
            {
                employees = await _unitOfWork.EmployeeRepository.GetByNameAsync(SearchInput);
            }
            return PartialView("EmployeePartialView/EmployeesTablePartialView", employees);

        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["departments"] = departments;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDTO model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                try
                {
                    if (model.Image is not null)
                    {
                        model.ImageName = DocumentSettings.UploadFile(model.Image, "Images");

                    }

                    var employee = _mapper.Map<Employee>(model);

                    await _unitOfWork.EmployeeRepository.AddAsync(employee);

                    var Count = await _unitOfWork.CompleteAsync();

                    if (Count > 0)
                    {
                        TempData["Message"] = "Employee Is Created !! ";
                        return RedirectToAction(nameof(Index));
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }



            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return BadRequest("Invalid Id");

            var employee = await _unitOfWork.EmployeeRepository.GetAsync(id.Value);

            if (employee is null)
                return NotFound(new { StatusCode = 404, message = $"Employee With Id {id} Is Not Found :(" });

            var dto = _mapper.Map<CreateEmployeeDTO>(employee);

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string viewName = "Edit")
        {

            if (id is null)
                return BadRequest("Invalid Id");
            var employee = await _unitOfWork.EmployeeRepository.GetAsync(id.Value);
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["departments"] = departments;
            if (employee is null)
                return NotFound(new { StatusCode = 404, message = $"Department With Id {id} Is Not Found :(" });

            var dto = _mapper.Map<CreateEmployeeDTO>(employee);

            return View(viewName, dto);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, CreateEmployeeDTO model, string viewName = "Edit")
        {
            if (ModelState.IsValid)
            {

                if (model.ImageName is not null)
                {
                    DocumentSettings.DeleteFile(model.ImageName, "Images");

                }
                if (model.Image is not null && model.Image is not null)
                {
                    model.ImageName = DocumentSettings.UploadFile(model.Image, "Images");
                }

                //if (id != model.Id) return BadRequest();

                var employee = _mapper.Map<Employee>(model);
                employee.Id = id;

                _unitOfWork.EmployeeRepository.Update(employee);
                var Count = await _unitOfWork.CompleteAsync();
                if (Count > 0)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(viewName, model);
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Edit(id, "Delete");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, CreateEmployeeDTO model)
        {
            if (ModelState.IsValid)
            {
                var employee = _mapper.Map<Employee>(model);
                employee.Id = id;
                _unitOfWork.EmployeeRepository.Delete(employee);

                var Count = await _unitOfWork.CompleteAsync();

                if (Count > 0)
                {
                    if (model.ImageName is not null)
                    {
                        DocumentSettings.DeleteFile(model.ImageName, "Images");
                    }
                    return RedirectToAction(nameof(Index));
                }

            }

            return View(model);
        }


    }
}
