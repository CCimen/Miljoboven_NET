using Microsoft.AspNetCore.Mvc;
using Miljoboven2.Models;

namespace Miljoboven2.Components;

public class ErrandDataViewComponent : ViewComponent
{
    private readonly IMiljobovenRepository _repository;

    public ErrandDataViewComponent(IMiljobovenRepository repository)
    {
        _repository = repository;
    }

    public async Task<IViewComponentResult> InvokeAsync(int id)
    {
        var errandDetails = await _repository.ShowErrandData(id);
        return View(errandDetails);
    }
}