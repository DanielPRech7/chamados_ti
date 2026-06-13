using System.Text.Json.Serialization;

namespace GerenciadorChamados.Dominio;

public class Chamado
{
    public int Id { get; private set; }
    public string Descricao { get; private set; }
    public StatusChamado Status { get; private set; }
    public DateTime DataAbertura { get; private set; }
    public DateTime? DataConclusao { get; private set; }

    public Chamado(int id, string descricao)
    {
        ValidarDescricao(descricao);

        Id = id;
        Descricao = descricao.Trim();
        Status = StatusChamado.Aberto;
        DataAbertura = DateTime.Now;
        DataConclusao = null;
    }

    [JsonConstructor]
    public Chamado(int id, string descricao, StatusChamado status, DateTime dataAbertura, DateTime? dataConclusao)
    {
        ValidarDescricao(descricao);

        Id = id;
        Descricao = descricao.Trim();
        Status = status;
        DataAbertura = dataAbertura;
        DataConclusao = dataConclusao;
    }

    public void Concluir()
    {
        if (!EstaAberto())
        {
            throw new InvalidOperationException("O chamado já está concluído.");
        }

        Status = StatusChamado.Concluido;
        DataConclusao = DateTime.Now;
    }

    public bool EstaAberto()
    {
        return Status == StatusChamado.Aberto;
    }

    private static void ValidarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("A descrição do chamado não pode ser vazia.");
        }
    }
}