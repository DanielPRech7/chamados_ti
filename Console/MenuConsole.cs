using GerenciadorChamados.Dominio;
using GerenciadorChamados.Persistencia;
using DominioGerenciador = GerenciadorChamados.Dominio.GerenciadorChamados;

namespace GerenciadorChamados.ConsoleUI;

public class MenuConsole
{
    private readonly DominioGerenciador _gerenciador;
    private readonly RepositorioChamados _repositorio;

    public MenuConsole(DominioGerenciador gerenciador, RepositorioChamados repositorio)
    {
        _gerenciador = gerenciador;
        _repositorio = repositorio;
    }

    public void Executar()
    {
        int opcao;

        do
        {
            ExibirMenuPrincipal();
            opcao = LerInteiro("Escolha uma opção: ");

            switch (opcao)
            {
                case 1:
                    ExibirNovoChamado();
                    break;
                case 2:
                    ExibirListaChamados();
                    break;
                case 0:
                    _repositorio.Salvar(_gerenciador.ListarTodos());
                    ExibirTelaSaida();
                    break;
                default:
                    ExibirMensagemTemporaria("Opção inválida.");
                    break;
            }
        }
        while (opcao != 0);
    }

    private void ExibirMenuPrincipal()
    {
        Console.Clear();
        ExibirCabecalho("MENU PRINCIPAL");
        Console.WriteLine("[1] Abrir novo chamado");
        Console.WriteLine("[2] Consultar chamados");
        Console.WriteLine("[0] Sair");
        Console.WriteLine();
        Console.WriteLine($"Total de chamados: {_gerenciador.TotalChamados()}  |  Abertos: {_gerenciador.TotalAbertos()}  |  Concluídos: {_gerenciador.TotalConcluidos()}");
        Console.WriteLine();
    }

    private void ExibirNovoChamado()
    {
        while (true)
        {
            Console.Clear();
            ExibirCabecalho("NOVO CHAMADO");
            Console.WriteLine("Descreva o problema ou solicitação:");
            Console.Write("> ");
            string? descricao = Console.ReadLine();

            try
            {
                var chamado = _gerenciador.AbrirChamado(descricao ?? string.Empty);
                _repositorio.Salvar(_gerenciador.ListarTodos());

                Console.Clear();
                ExibirCabecalho("NOVO CHAMADO");
                Console.WriteLine("Chamado criado com sucesso!");
                Console.WriteLine();
                Console.WriteLine($"ID...........: {chamado.Id}");
                Console.WriteLine($"Status.......: {chamado.Status}");
                Console.WriteLine($"Abertura.....: {chamado.DataAbertura:dd/MM/yyyy HH:mm}");
                Console.WriteLine("Descrição....: ");
                ExibirTextoComQuebra(chamado.Descricao, "               ");
                Console.WriteLine();
                AguardarTecla();
                return;
            }
            catch (ArgumentException ex)
            {
                ExibirMensagemTemporaria(ex.Message);
            }
        }
    }

    private void ExibirListaChamados()
    {
        while (true)
        {
            Console.Clear();
            ExibirCabecalho("CONSULTA DE CHAMADOS");

            var chamados = _gerenciador.ListarTodos();

            if (chamados.Count == 0)
            {
                Console.WriteLine("Nenhum chamado cadastrado.");
                Console.WriteLine();
                Console.WriteLine("[0] Voltar ao menu");
                Console.WriteLine();

                int opcao = LerInteiro("Escolha uma opção: ");
                if (opcao == 0)
                {
                    return;
                }

                ExibirMensagemTemporaria("Opção inválida.");
                continue;
            }

            Console.WriteLine($"{"ID",-4} {"Status",-12} {"Abertura",-12} Descrição");
            Console.WriteLine(new string('-', 60));

            foreach (var chamado in chamados)
            {
                Console.WriteLine($"{chamado.Id,-4} {chamado.Status,-12} {chamado.DataAbertura:dd/MM/yyyy,-12} {ResumirDescricao(chamado.Descricao, 28)}");
            }

            Console.WriteLine();
            int id = LerInteiro("Digite o ID para ver detalhes (0 = voltar): ");

            if (id == 0)
            {
                return;
            }

            if (_gerenciador.ObterPorId(id) is null)
            {
                ExibirMensagemTemporaria($"Chamado com ID {id} não encontrado.");
                continue;
            }

            ExibirDetalheChamado(id);
        }
    }

    private void ExibirDetalheChamado(int id)
    {
        while (true)
        {
            var chamado = _gerenciador.ObterPorId(id);

            Console.Clear();
            ExibirCabecalho("DETALHE DO CHAMADO");

            if (chamado is null)
            {
                Console.WriteLine($"Chamado com ID {id} não encontrado.");
                Console.WriteLine();
                AguardarTecla();
                return;
            }

            Console.WriteLine($"ID...........: {chamado.Id}");
            Console.WriteLine($"Status.......: {chamado.Status}");
            Console.WriteLine($"Abertura.....: {chamado.DataAbertura:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Conclusão....: {(chamado.DataConclusao.HasValue ? chamado.DataConclusao.Value.ToString("dd/MM/yyyy HH:mm") : "—")}");
            Console.WriteLine();
            Console.WriteLine("Descrição:");
            ExibirTextoComQuebra(chamado.Descricao, string.Empty);
            Console.WriteLine();

            if (chamado.EstaAberto())
            {
                Console.WriteLine("[1] Concluir chamado");
                Console.WriteLine("[0] Voltar");
                Console.WriteLine();
                int opcao = LerInteiro("Escolha uma opção: ");

                if (opcao == 0)
                {
                    return;
                }

                if (opcao == 1)
                {
                    bool concluiu = _gerenciador.ConcluirChamado(id);

                    if (concluiu)
                    {
                        _repositorio.Salvar(_gerenciador.ListarTodos());
                        ExibirMensagemTemporaria("Chamado concluído com sucesso!");
                    }
                    else
                    {
                        ExibirMensagemTemporaria("Não foi possível concluir o chamado.");
                    }

                    continue;
                }

                ExibirMensagemTemporaria("Opção inválida.");
            }
            else
            {
                Console.WriteLine("Este chamado já está concluído.");
                Console.WriteLine();
                Console.WriteLine("[0] Voltar");
                Console.WriteLine();

                int opcao = LerInteiro("Escolha uma opção: ");
                if (opcao == 0)
                {
                    return;
                }

                ExibirMensagemTemporaria("Opção inválida.");
            }
        }
    }

    private static void ExibirCabecalho(string titulo)
    {
        Console.WriteLine("GERENCIADOR DE CHAMADOS");
        Console.WriteLine($"=== {titulo} ===");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine();
    }

    private static int LerInteiro(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string? entrada = Console.ReadLine();

            if (int.TryParse(entrada, out int valor))
            {
                return valor;
            }

            Console.WriteLine("Entrada inválida. Digite um número inteiro.");
        }
    }

    private static string ResumirDescricao(string texto, int limite)
    {
        if (texto.Length <= limite)
        {
            return texto;
        }

        return texto[..limite].TrimEnd() + "...";
    }

    private static void ExibirTextoComQuebra(string texto, string recuo)
    {
        const int largura = 45;
        var palavras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string linhaAtual = recuo;

        foreach (var palavra in palavras)
        {
            string tentativa = string.IsNullOrWhiteSpace(linhaAtual.Trim())
                ? recuo + palavra
                : linhaAtual + " " + palavra;

            if (tentativa.Length > largura && linhaAtual.Trim().Length > 0)
            {
                Console.WriteLine(linhaAtual);
                linhaAtual = recuo + palavra;
            }
            else
            {
                linhaAtual = tentativa;
            }
        }

        if (!string.IsNullOrWhiteSpace(linhaAtual))
        {
            Console.WriteLine(linhaAtual);
        }
    }

    private static void ExibirMensagemTemporaria(string mensagem)
    {
        Console.WriteLine();
        Console.WriteLine(mensagem);
        AguardarTecla();
    }

    private static void AguardarTecla()
    {
        Console.WriteLine();
        Console.WriteLine("Pressione qualquer tecla para continuar...");
        Console.ReadKey(true);
    }

    private static void ExibirTelaSaida()
    {
        Console.Clear();
        ExibirCabecalho("ENCERRANDO");
        Console.WriteLine("Programa finalizado com sucesso.");
        Console.WriteLine();
        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey(true);
    }
}