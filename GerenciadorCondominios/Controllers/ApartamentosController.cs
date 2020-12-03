using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorCondominios.Controllers
{
    public class ApartamentosController : Controller
    {
        // precisa do web para trabalhar com as fotos
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IApartamentoRepositorio _apartamentoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public ApartamentosController(IWebHostEnvironment webHostEnvironment, IApartamentoRepositorio apartamentoRepositorio, IUsuarioRepositorio usuarioRepositorio)
        {
            _webHostEnvironment = webHostEnvironment;
            _apartamentoRepositorio = apartamentoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<IActionResult> Index()
        {
            // User possui as informações do usuário logado
            Usuario usuario = await _usuarioRepositorio.PegarUsuarioPeloNome(User);

            if (await _usuarioRepositorio.VerificarSeUsuarioEstaEmFuncao(usuario, "Sindico"))
            {
                return View(await _apartamentoRepositorio.PegarTodos());
            }

            return View(await _apartamentoRepositorio.PegarApartamentoPeloUsuario(usuario.Id));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["MoradorId"] = new SelectList(await _usuarioRepositorio.PegarTodos(), "Id", "UserName");
            ViewData["ProprietarioId"] = new SelectList(await _usuarioRepositorio.PegarTodos(), "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApartamentoId,Numero,Andar,Foto,MoradorId,ProprietarioId")] Apartamento apartamento, IFormFile foto)
        {
            if (ModelState.IsValid)
            {
                if (foto != null)
                {
                    string diretorio = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
                    string nomeFoto = Guid.NewGuid().ToString() + foto.FileName;

                    using (FileStream fileStream = new FileStream(Path.Combine(diretorio, nomeFoto), FileMode.Create))
                    {
                        await foto.CopyToAsync(fileStream);
                        apartamento.Foto = "~/imagens/" + nomeFoto;
                    }
                }
                
                await _apartamentoRepositorio.Inserir(apartamento);
                TempData["NovoRegistro"] = $"Apartamento número {apartamento.Numero} registrado com sucesso";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MoradorId"] = new SelectList(await _usuarioRepositorio.PegarTodos(), "Id", "UserName", apartamento.MoradorId);
            ViewData["ProprietarioId"] = new SelectList(await _usuarioRepositorio.PegarTodos(), "Id", "UserName", apartamento.ProprietarioId);
            return View(apartamento);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Apartamento apartamento = await _apartamentoRepositorio.PegarPeloId(id);
            if (apartamento == null)
            {
                return NotFound();
            }
            TempData["Foto"] = apartamento.Foto;
            ViewData["MoradorId"] = new SelectList(await _usuarioRepositorio.PegarTodos(), "Id", "UserName", apartamento.MoradorId);
            ViewData["ProprietarioId"] = new SelectList(await _usuarioRepositorio.PegarTodos(), "Id", "UserName", apartamento.ProprietarioId);
            return View(apartamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApartamentoId,Numero,Andar,Foto,MoradorId,ProprietarioId")] Apartamento apartamento, IFormFile foto)
        {
            if (id != apartamento.ApartamentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (foto != null)
                {
                    string diretorio = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
                    string nomeFoto = Guid.NewGuid().ToString() + foto.FileName;

                    using (FileStream fileStream = new FileStream(Path.Combine(diretorio, nomeFoto), FileMode.Create))
                    {
                        await foto.CopyToAsync(fileStream);
                        apartamento.Foto = "~/imagens/" + nomeFoto;
                        System.IO.File.Delete(TempData["Foto"].ToString().Replace("~", "wwwroot"));
                    }
                }
                else
                    apartamento.Foto = TempData["Foto"].ToString();

                await _apartamentoRepositorio.Atualizar(apartamento);
                TempData["Atualizacao"] = $"Apartamento número {apartamento.Numero} atualizado com sucesso";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MoradorId"] = new SelectList(await _usuarioRepositorio.PegarTodos(), "Id", "UserName", apartamento.MoradorId);
            ViewData["ProprietarioId"] = new SelectList(await _usuarioRepositorio.PegarTodos(), "Id", "UserName", apartamento.ProprietarioId);
            return View(apartamento);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            Apartamento apartamento = await _apartamentoRepositorio.PegarPeloId(id);
            System.IO.File.Delete(apartamento.Foto.Replace("~", "wwwroot"));
            await _apartamentoRepositorio.Excluir(apartamento);
            TempData["Exclusao"] = $"Apartamento número {apartamento.Numero} excluído com sucesso";
            return Json("Apartamento excluído");
        }
    }
}
