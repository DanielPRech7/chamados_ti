namespace GerenciadorChamados.Dominio;

public class GerenciadorChamados
{
    private readonly List<Chamado> _chamados;

    public GerenciadorChamados(List<Chamado>? chamados = null)
    {
        _chamados = chamados ?? new List<Chamado>();
    }

    public Chamado AbrirChamado(string descricao)
    {
        var chamado = new Chamado(ObterProximoId(), descricao);
        _chamados.Add(chamado);
        return chamado;
    }

    public List<Chamado> ListarTodos()
    {
        return _chamados
            .OrderBy(c => c.Id)
            .ToList();
    }

    public Chamado? ObterPorId(int id)
    {
        return _chamados.FirstOrDefault(c => c.Id == id);
    }

    public bool ConcluirChamado(int id)
    {
        var chamado = ObterPorId(id);

        if (chamado is null || !chamado.EstaAberto())
        {
            return false;
        }

        chamado.Concluir();
        return true;
    }

    public int TotalChamados()
    {
        return _chamados.Count;
    }

    public int TotalAbertos()
    {
        return _chamados.Count(c => c.EstaAberto());
    }

    public int TotalConcluidos()
    {
        return _chamados.Count(c => !c.EstaAberto());
    }

    private int ObterProximoId()
    {
        return _chamados.Count == 0 ? 1 : _chamados.Max(c => c.Id) + 1;
    }
}