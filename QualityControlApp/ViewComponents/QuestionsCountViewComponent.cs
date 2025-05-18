using Microsoft.AspNetCore.Mvc;
using QualityControlApp.Models.Entities;
using QualityControlApp.Models.Interfaces;

namespace QualityControlApp.ViewComponents
{
    public class QuestionCountViewComponent : ViewComponent
    {
        private readonly IUnitOfWork<Question > _question;
        private readonly IUnitOfWork<QuestionType > _questionType;

        public QuestionCountViewComponent(IUnitOfWork<Question> question,
                                      IUnitOfWork<QuestionType> questiontype)
        {
            _question  = question;
            _questionType  = questiontype;
        }

        public IViewComponentResult Invoke()
        {
            var questiontype = _questionType.Entity.GetAll().ToList();

            int count = 0;
            foreach (var section in questiontype)
            {
                section.QuestionsCount  = _question .Entity.Include(n => n.QuestionType ).Where(n => n.QuestionTypeId  == section.Id).Count();
                count += section.QuestionsCount;
            }

            ViewBag.AllNewsCount = count;    //ViewBag.AllNewsCount = _news.Entity.GetAll().Count();
            return View(questiontype);
        }


    }
}
