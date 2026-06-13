using System.Text.Json;
using System.Text.Json.Serialization;
using GerenciadorChamados.Dominio;

namespace GerenciadorChamados.Persistencia;

public class RepositorioChamados
{
    private readonly string _caminhoArquivo;
    private readonly JsonSerializerOptions _opcoesJson;

    public RepositorioChamados(string caminhoArquivo)
    {
        _caminhoArquivo = caminhoArquivo;
        _opcoesJson = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public List<Chamado> Carregar()
    {
        if (!File.Exists(_caminhoArquivo))
        {
            return new List<Chamado>();
        }

        try
        {
            string json = File.ReadAllText(_caminhoArquivo);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Chamado>();
            }

            return JsonSerializer.Deserialize<List<Chamado>>(json, _opcoesJson) ?? new List<Chamado>();
        }
        catch (JsonException)
        {
            return new List<Chamado>();
        }
    }

    public void Salvar(List<Chamado> chamados)
    {
        string json = JsonSerializer.Serialize(chamados, _opcoesJson);
        File.WriteAllText(_caminhoArquivo, json);
    }
}