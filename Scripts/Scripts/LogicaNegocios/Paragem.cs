using System.Collections.Generic;


public class Paragem
{
    private string id;
    private GPS gps;
    private string nome;
    private string linkImage;
    private string descricao;
    private string endereco;
    private List<Rota> rotas;

    public Paragem(string id, GPS gps, string nome, string linkImage, string descricao, string endereco, List<Rota> rotas)
    {
        this.id = id;
        this.gps = gps;
        this.nome = nome;
        this.linkImage = linkImage;
        this.descricao = descricao;
        this.endereco = endereco;
        this.rotas = rotas;
    }

    public string getId() { return id; }
    public GPS getGPS() { return gps; }
    public string getNome() { return nome; }
    public string getLinkImage() { return linkImage; }
    public string getDescricao() { return descricao; }
    public string getEndereco() { return endereco; }
    public List<Rota> getRotas() { return rotas; }

}
