using AjaxCRUD.Data;
using AjaxCRUD.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjaxCRUD.Controllers
{
    public class HomeController : Controller
    {
        StudentDataEntities studentDataEntities = new StudentDataEntities();


        public ActionResult Index()
        {
            List<tbldepartment> DeptList = studentDataEntities.tbldepartments.ToList();
            ViewBag.ListOfDepartment = new SelectList(DeptList, "DepartmentID", "DepartmentName");
            return View();
        }

        public JsonResult GetStudentList()
        {
            
            List<StudentViewModel> StuList = studentDataEntities.tblStudents.Where(x => x.IsDeleted == false).Select(x => new StudentViewModel
            {
                StudentID = x.StudentID,
                StudentName = x.StudentName,
                Email = x.Email,
                DepartmentName = x.tbldepartment.DepartmentName
            }).ToList();

            return Json(StuList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentById(int StudentId)
        {
            tblStudent model = studentDataEntities.tblStudents.Where(x => x.StudentID == StudentId).SingleOrDefault();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDataInDatabase(StudentViewModel model)
        {
            var result = false;
            try
            {
                if (model.StudentID > 0)
                {
                    tblStudent Stu = studentDataEntities.tblStudents.SingleOrDefault(x => x.IsDeleted == false && x.StudentID == model.StudentID);
                    Stu.StudentName = model.StudentName;
                    Stu.Email = model.Email;
                    Stu.DepartmentID = model.DepartmentID;
                    studentDataEntities.SaveChanges();
                    result = true;
                }
                else
                {
                    tblStudent Stu = new tblStudent();
                    Stu.StudentName = model.StudentName;
                    Stu.Email = model.Email;
                    Stu.DepartmentID = model.DepartmentID;
                    Stu.IsDeleted = false;
                    studentDataEntities.tblStudents.Add(Stu);
                    studentDataEntities.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteStudentRecord(int StudentId)
        {
            bool result = false;
            tblStudent Stu = studentDataEntities.tblStudents.SingleOrDefault(x => x.IsDeleted == false && x.StudentID == StudentId);
            if (Stu != null)
            {
                Stu.IsDeleted = true;
                studentDataEntities.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}