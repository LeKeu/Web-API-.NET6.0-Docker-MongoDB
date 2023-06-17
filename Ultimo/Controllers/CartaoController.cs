using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ultimo.Models;
using Ultimo.Services;

namespace Ultimo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly CartoesService _cartoesService;

        public CartaoController(CartoesService cartoesService)
        {
            _cartoesService = cartoesService;
        }

        [HttpGet]
        public async Task<List<Cartao>> Get() => await _cartoesService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Cartao>> Get(string id)
        {
            var cartao = await _cartoesService.GetAsync(id);

            if (cartao is null)
            {
                return NotFound();
            }

            return cartao;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Cartao newCartao)
        {
            await _cartoesService.CreateAsync(newCartao);

            return CreatedAtAction(nameof(Get), new {id = newCartao.Id}, newCartao);

        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Cartao updateCartao)
        {
            var cartao = await _cartoesService.GetAsync(id);

            if (cartao is null)
            {
                return NotFound();
            }

            updateCartao.Id = cartao.Id;

            await _cartoesService.UpdateAsync(id, updateCartao);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cartao = await _cartoesService.GetAsync(id);

            if (cartao is null)
            {
                return NotFound();
            }

            await _cartoesService.RemoveAsync(cartao.Id!);

            return NoContent();
        }

    }
}
