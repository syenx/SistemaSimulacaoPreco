using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteDLL
{
    class Program
    {
        static void Main(string[] args)
        {
            KS.SimuladorPreco.DLL.SimuladorPreco sp = new KS.SimuladorPreco.DLL.SimuladorPreco();

            var teste = sp.ConsultarItem("1115","thiagoa","SP", "3","","","0,00","");

            Console.WriteLine("estabelecimentoId -  " + teste.estabelecimentoId     );
            Console.WriteLine("itemId -  " + teste.itemId                           );
            Console.WriteLine("ufIdOrigem -  " + teste.ufIdOrigem                   );
            Console.WriteLine("itemDescricao -  " + teste.itemDescricao             );
            Console.WriteLine("laboratorioNome -  " + teste.laboratorioNome         );
            Console.WriteLine("tipo -  " + teste.tipo                               );
            Console.WriteLine("exclusivoHospitalar -  " + teste.exclusivoHospitalar );
            Console.WriteLine("NCM -  " + teste.NCM                                 );
            Console.WriteLine("listaDescricao - " + teste.listaDescricao           );
            Console.WriteLine("categoria -" + teste.categoria                     );
            Console.WriteLine("resolucao13 - " + teste.resolucao13                 );
            Console.WriteLine("tratamentoICMSEstab - " + teste.tratamentoICMSEstab );
            Console.WriteLine("aplicacaoRepasse - " + teste.aplicacaoRepasse       );
            Console.WriteLine("reducaoST_MVA - " + teste.reducaoST_MVA             );
            Console.WriteLine("precoFabrica - " + teste.precoFabrica               );
            Console.WriteLine("descontoComercial   " + teste.descontoComercial     );
            Console.WriteLine("descontoAdicional   " + teste.descontoAdicional     );
            Console.WriteLine("percRepasse   " + teste.percRepasse                 );
            Console.WriteLine("precoAquisicao   " + teste.precoAquisicao           );
            Console.WriteLine("percReducaoBase   " + teste.percReducaoBase         );
            Console.WriteLine("percICMSe   " + teste.percICMSe                     );
            Console.WriteLine("valorCreditoICMS   " + teste.valorCreditoICMS       );
            Console.WriteLine("percIPI   " + teste.percIPI                         );
            Console.WriteLine("valorIPI   " + teste.valorIPI                       );
            Console.WriteLine("percPisCofins   " + teste.percPisCofins             );
            Console.WriteLine("valorPisCofins   " + teste.valorPisCofins           );
            Console.WriteLine("pmc17   " + teste.pmc17                             );
            Console.WriteLine("descST   " + teste.descST                           );
            Console.WriteLine("mva   " + teste.mva                                 );
            Console.WriteLine("valorICMSST   " + teste.valorICMSST                 );
            Console.WriteLine("estabelecimentoNome   " + teste.estabelecimentoNome );
            Console.WriteLine("estabelecimentoUf   " + teste.estabelecimentoUf     );
            Console.WriteLine("custoPadrao   " + teste.custoPadrao                 );
            Console.WriteLine("aliquotaInternaICMS   " + teste.aliquotaInternaICMS );
            Console.WriteLine("percPmc   " + teste.percPmc                         );
            Console.WriteLine("itemControlado   " + teste.itemControlado           );
            Console.WriteLine("unidadeNegocioId   " + teste.unidadeNegocioId       );
            Console.WriteLine("capAplicado   " + teste.capAplicado                 );
            Console.WriteLine("capDescontoPrc   " + teste.capDescontoPrc           );
            Console.WriteLine("usuarioId   " + teste.usuarioId                     );


            Console.ReadLine();
        }
    }
}
