using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityControlApp.Models;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;

namespace QualityControlApp.Controllers
{
    public class CompanyQuestionContentController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<CompanyQuestionContent> _companyquestionContent;


        public CompanyQuestionContentController(
  ApplicationDbContext context,
                       IUnitOfWork<CompanyQuestionContent> companyquestionContent,
                            IWebHostEnvironment host) : base(host)
        {
            _context = context;
            _companyquestionContent = companyquestionContent;

            //_host = host; // نفعله فقط لو احتجناه في هذا الكونترولر
        }

       








    }
}
