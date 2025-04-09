using System;
using System.Collections.Generic;

namespace COMPILADOR.Estructuras
{
    public class TablaSimbolos
    {
        private Dictionary<string, object> simbolos;

        public TablaSimbolos()
        {
            simbolos = new Dictionary<string, object>();
        }

        public void Definir(string nombre, object valor)
        {
            simbolos[nombre] = valor;
        }

        public bool EstaDefinida(string nombre)
        {
            return simbolos.ContainsKey(nombre);
        }

        public object ObtenerValor(string nombre)
        {
            if (!EstaDefinida(nombre))
            {
                throw new Exception($"Variable '{nombre}' no está definida");
            }
            return simbolos[nombre];
        }

        public void ActualizarValor(string nombre, object valor)
        {
            if (!EstaDefinida(nombre))
            {
                throw new Exception($"Variable '{nombre}' no está definida");
            }
            simbolos[nombre] = valor;
        }
    }
} 