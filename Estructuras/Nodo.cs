using System;
using System.Collections.Generic;

namespace COMPILADOR.Estructuras
{
    // Nodo del árbol de sintaxis
    public class Nodo
    {
        public string Tipo { get; set; }
        public string Valor { get; set; }
        public List<Nodo> Hijos { get; set; }
        public Token Token { get; set; }

        public Nodo(string tipo, string valor, Token token = null)
        {
            Tipo = tipo;
            Valor = valor;
            Hijos = new List<Nodo>();
            Token = token;
        }

        public void AgregarHijo(Nodo hijo)
        {
            Hijos.Add(hijo);
        }

        public override string ToString()
        {
            return $"{Tipo}({Valor})";
        }
    }
} 