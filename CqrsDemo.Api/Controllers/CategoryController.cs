using CqrsDemo.Api.Features.Commands.CategoryCommands;
using CqrsDemo.Api.Features.Queries.CategoryQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CqrsDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command, CancellationToken token)
        {
            try
            {
                return Ok(await _mediator.Send(command, token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryCommand command, CancellationToken token)
        {
            try
            {
                return Ok(await _mediator.Send(command, token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCategory(Guid id, CancellationToken token)
        {
            try
            {
                return Ok(await _mediator.Send(new RemoveCategoryCommand(id), token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories(CancellationToken token)
        {
            try
            {
                return Ok(await _mediator.Send(new GetAllCategorysQuery(), token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken token)
        {
            try
            {
                return Ok(await _mediator.Send(new GetCategoryByIdQuery(id), token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
