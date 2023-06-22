using Microsoft.AspNetCore.Mvc;
using Sistema_de_Estoque.Models;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Sistema_de_Estoque.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProdutosDisp(string pesquisa)
        {
            SqlConnection conexao = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ListaProdutos;Integrated Security=True");
            string query = "SELECT * FROM Produtos";

            if (!string.IsNullOrEmpty(pesquisa))
            {
                query += $" WHERE Nome LIKE '%{pesquisa}%' OR Preco LIKE '%{pesquisa}%' OR categoria LIKE '%{pesquisa}%'";
            }

            SqlCommand comando = new SqlCommand(query, conexao);
            conexao.Open();
            SqlDataReader retorno = comando.ExecuteReader();

            List<Produto> lista = new List<Produto>();
            while (retorno.Read())
            {
                Produto produto = new Produto();
                produto.nome = retorno["Nome"].ToString();
                produto.Id = Convert.ToInt32(retorno["id"]);
                produto.img = retorno["Imagem"].ToString();
                produto.descricao = retorno["Decricao"].ToString();
                produto.categoria = retorno["Categoria"].ToString();
                produto.preco = retorno["Preco"].ToString();
                lista.Add(produto);
            }
            conexao.Close();
            ViewBag.novo = lista;
            return View();
        }

        public IActionResult FormCadastro()
        {
            return View();
        }

        public IActionResult Salvar(Produto produto)
        {
            SqlConnection conexao = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ListaProdutos;Integrated Security=True");
            SqlCommand comando = new SqlCommand("INSERT INTO PRODUTOS (NOME,IMAGEM,DECRICAO,CATEGORIA,PRECO) VALUES ('" + produto.nome + "','" + produto.img + "','" + produto.descricao + "','" + produto.categoria + "','" + produto.preco + "')", conexao);
            conexao.Open();
            comando.ExecuteNonQuery();
            conexao.Close();

            return RedirectToAction("ProdutosDisp");
        }

        public IActionResult Deletar(int id)
        {
            SqlConnection conexao = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ListaProdutos;Integrated Security=True");
            SqlCommand comando = new SqlCommand("DELETE FROM Produtos WHERE Id = @id", conexao);
            comando.Parameters.AddWithValue("@id", id);
            conexao.Open();
            int linhasAfetadas = comando.ExecuteNonQuery();
            conexao.Close();

            if (linhasAfetadas > 0)
            {
                return RedirectToAction("ProdutosDisp");
            }
            else
            {
                return View("Error");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
