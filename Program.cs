using GerenciadorChamados.ConsoleUI;
using GerenciadorChamados.Persistencia;
using DominioGerenciador = GerenciadorChamados.Dominio.GerenciadorChamados;

var caminhoJson = Path.Combine(AppContext.BaseDirectory, "chamados.json");
var repositorio = new RepositorioChamados(caminhoJson);
var chamados = repositorio.Carregar();
var gerenciador = new DominioGerenciador(chamados);
var menu = new MenuConsole(gerenciador, repositorio);

menu.Executar();