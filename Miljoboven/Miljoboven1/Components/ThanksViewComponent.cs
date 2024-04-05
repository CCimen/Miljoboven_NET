using Microsoft.AspNetCore.Mvc;
using Miljoboven2.Models;

namespace YourAppName.ViewComponents
{
    public class ThanksViewComponent : ViewComponent
    {
        private readonly IMiljobovenRepository _repository;

        public ThanksViewComponent(IMiljobovenRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            return View("Default");
        }
    }
}