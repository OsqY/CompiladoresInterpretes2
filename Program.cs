using System;
using COMPILADOR.Analizadores;
using COMPILADOR.Estructuras;

namespace COMPILADOR
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Compilador Básico");
            Console.WriteLine("Ingrese el código a compilar (presione Enter dos veces para terminar):");

            string codigo = "";
            string linea;
            while (!string.IsNullOrEmpty(linea = Console.ReadLine()))
            {
                codigo += linea + "\n";
            }

            try
            {
                var analizadorLexico = new AnalizadorLexico(codigo);
                var tokens = analizadorLexico.Analizar();

                Console.WriteLine("\nTokens encontrados:");
                foreach (var token in tokens)
                {
                    Console.WriteLine(token);
                }

                var analizadorSintactico = new AnalizadorSintactico(tokens);
                var arbolSintactico = analizadorSintactico.Analizar();

                var interprete = new Interprete();
                interprete.Interpretar(arbolSintactico);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
    }
}
