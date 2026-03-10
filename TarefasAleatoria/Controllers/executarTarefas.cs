using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks.Dataflow;

namespace TarefasAleatoria.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class executarTarefas : ControllerBase
    {
        private static List<atributoTarefas> tarefa = new List<atributoTarefas>
        {
        
            new atributoTarefas { trfId = 1, trfNome = "Limpar quarto", trfDescricao = "Arrumar as camas e limpar o chão", trfTempo = 10, trfYoutube = "https://youtu.be/hcQyXZATshA?si=Vsu5ZqOlYyeiiZJk" },

            new atributoTarefas { trfId = 2, trfNome = "Lavar o carro", trfDescricao = "Limpar os Bancos e lataria", trfTempo = 30, trfYoutube = "Sem Video"},

            new atributoTarefas { trfId = 3, trfNome = "Cuidar das plantas", trfDescricao = "Regar as plantas e plantar cenoura", trfTempo = 40, trfYoutube = "Sem Video"}
        
        };

        private readonly ILogger<executarTarefas> _logger;

        public executarTarefas(ILogger<executarTarefas> logger)
        {
            _logger = logger;
        }

        [HttpGet("Sugerir/{tempo}", Name = "GetAtividadeSugerida")]
        public IActionResult GetAtividadeSugerida(float tempo)
        {
            if (tempo <= 0)
            {
                return new JsonResult("Você está é atrasado! (ou insira um valor maior que zero)");
            }

            Random rand = new Random();
            List<atributoTarefas> tarefasSugeridas = new List<atributoTarefas>();
            List<atributoTarefas> tarefasExibidas = new List<atributoTarefas>();  

            while (tempo > 0)
            {

                atributoTarefas confirmador = new atributoTarefas();

                var FazerTarefa = tarefa.Where(r => r.trfTempo <= tempo && !tarefasExibidas.Contains(r)).ToList();

                var tentar = FazerTarefa.Count;

                if (FazerTarefa.Count == 0)
                {
                    break;
                }

                atributoTarefas tarefaAleatoria = FazerTarefa[rand.Next(FazerTarefa.Count)];

                // Adicionar tarefa para mostrar ela (ou elas)
                tarefasSugeridas.Add(tarefaAleatoria);

                // Não mostrar denovo essa tarefa
                tarefasExibidas.Add(tarefaAleatoria);

                tempo -= tarefaAleatoria.trfTempo;
            }

            if (tarefasSugeridas.Count == 0)
            {
                return new JsonResult("Nenhuma tarefa encontrada para o tempo disponível");
            }

            return new JsonResult(tarefasSugeridas);
        }

        [HttpGet("Editar/{id}", Name = "GetTarefaEditar")]
        public IActionResult GetReceitaEditar(int id)
        {
            var tarefas = tarefa.FirstOrDefault(w => w.trfId == id);
            if (tarefas == null)
            {
                return NotFound("Receita não encontrada.");
            }
            return new JsonResult(tarefas);
        }

        [HttpGet] public IActionResult ListarTarefas()
        {
            return Ok(tarefa);
        }

        [HttpPost("Inserir")]
        public IActionResult Post([FromBody] atributoTarefas novaTarefa)
        {

            /* if (novaTarefa == null || string.IsNullOrEmpty(novaTarefa.trfNome))
            {
                return BadRequest("Insira dados válidos");
            }*/ 

            if (tarefa.Max(r => r.trfId) == 0)
            {

                novaTarefa.trfId = 1;

            }

            int novoId = tarefa.Max(r => r.trfId) + 1;

            novaTarefa.trfId = novoId;

            tarefa.Add(novaTarefa);

            return Ok(tarefa);

        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] atributoTarefas tarefaAtualizada)
        {

            if (tarefaAtualizada == null)
            {

                return BadRequest("Nenhuma tarefa encontrada com o ID: " + id);

            }

            var tarefaExistente = tarefa.FirstOrDefault(r => r.trfId == id);

            if (tarefaExistente == null)
            {

                return BadRequest("Nenhuma tarefa encontrada com o ID: " + id);

            }

            tarefaExistente.trfNome = tarefaAtualizada.trfNome;
            tarefaExistente.trfDescricao = tarefaAtualizada.trfDescricao;
            tarefaExistente.trfTempo = tarefaAtualizada.trfTempo;
            tarefaExistente.trfYoutube = tarefaAtualizada.trfYoutube;

            return Ok(tarefaExistente);
        }

        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {

            var tarefas = tarefa.FirstOrDefault(r => r.trfId == id);

            if (tarefa == null)
            {

                return BadRequest("Nunhuma tarefa encontrada com o ID: " + id);

            }

            tarefa.Remove(tarefas);

            return NoContent();

        }
    }
}
