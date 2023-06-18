using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ultimo.Models;
using Ultimo.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Ultimo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly CartoesService _cartoesService;
        Extras extras;

        public CartaoController(CartoesService cartoesService)
        {
            _cartoesService = cartoesService;
        }

        private bool ChecarSenha(Cartao newCartao)
        {
            var tudoJunto = "" + (newCartao.DataNasc.Value.Year.ToString().Substring(2)) + "" +
                (newCartao.DataNasc.Value.Month.ToString().Length == 1 ? ("0" + newCartao.DataNasc.Value.Month) : (newCartao.DataNasc.Value.Month))
                + "" + (newCartao.DataNasc.Value.Day);

            var idade = DateTime.Today.Year - newCartao.DataNasc.Value.Year;
            var senha = newCartao.Senha.ToString();
            var senhaConfirm = newCartao.SenhaConfirm.ToString();

            //System.Diagnostics.Debug.WriteLine(extras.isConsecutive(senha));
            if (idade < 18)


            if (idade >= 18 && senha.Length == 6 && senha != tudoJunto){
                System.Diagnostics.Debug.WriteLine("Senha Linda");
                return true;
            }

            return false;
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

        [HttpPost("SolicitarCartão")]
        public async Task<IActionResult> Post(Cartao newCartao)
        {
            var senha_ok = ChecarSenha(newCartao);
            string aviso = null;

            if(DateTime.Today.Year - newCartao.DataNasc.Value.Year < 18)
                return BadRequest("É obrigatório ter 18 ou mais de idade para solicitar um cartão.");

            if (newCartao.Bandeira != "Mastercard" && newCartao.Bandeira != "Visa")
                aviso += "É necessário que a Bandeira seja 'Mastercard' ou 'Visa'.\n";

            if (newCartao.DataVenc != "5" && newCartao.DataVenc != "10" && newCartao.DataVenc != "15" && newCartao.DataVenc != "20")
                aviso += "É necessário que a Data de Vencimento seja '5', '10', '15' ou '20'.\n";

            if (newCartao.Tipo != "PLATINUM" && newCartao.Tipo != "GOLD" && newCartao.Tipo != "BLACK" && newCartao.Tipo != "DIAMOND")
                aviso += "É necessário que o Tipo seja 'PLATINUM', 'GOLD', 'BLACK' ou 'DIAMOND'.";

            await _cartoesService.CreateAsync(newCartao);
            if (!string.IsNullOrEmpty(aviso))
                return BadRequest(aviso);

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

        //ATIVAÇÃO DE CARTÃO
        [HttpPut("AtivarCartao")]
        public async Task<IActionResult> AtivarCartao(string id, string agencia, string conta, string senha)
        {
            var cartao = await _cartoesService.GetAsync(id);

            cartao.Id = cartao.Id;

            if (cartao.Agencia.ToString() == agencia && cartao.Conta.ToString() == conta && cartao.Senha.ToString() == senha)
            {
                System.Diagnostics.Debug.WriteLine("ATIVO");
                cartao.Status = "ATIVO";

                await _cartoesService.UpdateAsync(id, cartao);

                return CreatedAtAction(nameof(Get), new { id = cartao.Id }, cartao);
            }
            else
            {
                return BadRequest("Cartão não encontrado. Cheque se as informações estão corretas.");
            }
        }

        //BLOQUEAR CARTÃO
        [HttpPut("BloquearCartao")]
        public async Task<IActionResult> BloquearCartao(string id, string agencia, string conta, string senha, string motivo)
        {
            var cartao = await _cartoesService.GetAsync(id);

            cartao.Id = cartao.Id;

            if (cartao.Agencia.ToString() == agencia && cartao.Conta.ToString() == conta 
                && cartao.Senha.ToString() == senha && cartao.Status == "ATIVO")
            {
                cartao.Status = motivo;
                await _cartoesService.UpdateAsync(id, cartao);
                return CreatedAtAction(nameof(Get), new { id = cartao.Id }, cartao);
            }
            else
            {
                return BadRequest("Cartão não encontrado. Cheque se as informações estão corretas.");
            }

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
