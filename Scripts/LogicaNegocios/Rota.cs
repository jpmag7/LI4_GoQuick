

public class Rota
{
    private string id;
    private string nomeOrigem;
    private string nomeDestino;
    private string horaInicio;
    private string horaFim;
    private string duracao;
    private string numeroAutocarro;
    private string funcionamento;

    public Rota(string id, string nomeParagemOrigem, string nomeParagemDestino, string horaInicio, string horaFim, string duracao, string numeroAutocarro, string funcionamento)
    {
        this.id = id;
        this.nomeOrigem = nomeParagemOrigem;
        this.nomeDestino = nomeParagemDestino;
        this.horaInicio = horaInicio;
        this.horaFim = horaFim;
        this.duracao = duracao;
        this.numeroAutocarro = numeroAutocarro;
        this.funcionamento = funcionamento;
    }

    public string getId() { return id; }
    public string getOrigem() { return nomeOrigem; }
    public string getDestino() { return nomeDestino; }
    public string getInicio() { return horaInicio; }
    public string getFim() { return horaFim; }
    public string getDuracao() { return duracao; }
    public string getNumeroAutocarro() { return numeroAutocarro; }
    public string getFuncionamento() { return funcionamento; }
}
