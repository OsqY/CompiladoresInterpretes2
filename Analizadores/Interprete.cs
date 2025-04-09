using System;
using System.Collections.Generic;
using COMPILADOR.Estructuras;

namespace COMPILADOR.Analizadores
{
    public class Interprete
    {
        private TablaSimbolos tablaSimbolos;

        public Interprete()
        {
            tablaSimbolos = new TablaSimbolos();
        }

        public object Interpretar(Nodo nodo)
        {
            switch (nodo.Tipo)
            {
                case "PROGRAMA":
                    return InterpretarPrograma(nodo);
                case "EXPRESION":
                    return InterpretarExpresion(nodo);
                case "EXPRESION_ARITMETICA":
                    return InterpretarExpresionAritmetica(nodo);
                case "NUMERO":
                    return int.Parse(nodo.Valor);
                case "IDENTIFICADOR":
                    return tablaSimbolos.ObtenerValor(nodo.Valor);
                case "MIENTRAS":
                    return InterpretarMientras(nodo);
                case "BLOQUE":
                    return InterpretarBloque(nodo);
                default:
                    throw new Exception($"Tipo de nodo no soportado: {nodo.Tipo}");
            }
        }

        private object InterpretarPrograma(Nodo nodo)
        {
            object resultado = null;
            foreach (var hijo in nodo.Hijos)
            {
                resultado = Interpretar(hijo);
            }
            return resultado;
        }

        private object InterpretarExpresion(Nodo nodo)
        {
            // Si es una asignación
            if (nodo.Hijos.Count == 3 && nodo.Hijos[1].Valor == "=")
            {
                string nombreVariable = nodo.Hijos[0].Valor;
                object valorExpresion = Interpretar(nodo.Hijos[2]);

                if (tablaSimbolos.EstaDefinida(nombreVariable))
                {
                    tablaSimbolos.ActualizarValor(nombreVariable, valorExpresion);
                }
                else
                {
                    tablaSimbolos.Definir(nombreVariable, valorExpresion);
                }

                Console.WriteLine($"{nombreVariable} = {valorExpresion}");
                return valorExpresion;
            }

            // Si es una expresión aritmética
            return Interpretar(nodo.Hijos[0]);
        }

        private object InterpretarExpresionAritmetica(Nodo nodo)
        {
            if (nodo.Hijos.Count == 1)
            {
                return Interpretar(nodo.Hijos[0]);
            }

            var resultado = Interpretar(nodo.Hijos[0]);
            
            for (int i = 1; i < nodo.Hijos.Count; i += 2)
            {
                var operador = nodo.Hijos[i].Valor;
                var valorDerecho = Interpretar(nodo.Hijos[i + 1]);
                resultado = AplicarOperador(operador, resultado, valorDerecho);
            }

            return resultado;
        }

        private object AplicarOperador(string operador, object izquierdo, object derecho)
        {
            int izq = Convert.ToInt32(izquierdo);
            int der = Convert.ToInt32(derecho);

            switch (operador)
            {
                case "+":
                    return izq + der;
                case "-":
                    return izq - der;
                case "*":
                    return izq * der;
                case "/":
                    if (der == 0) throw new Exception("División por cero");
                    return izq / der;
                case "%":
                    if (der == 0) throw new Exception("Módulo por cero");
                    return izq % der;
                case ">":
                    return izq > der ? 1 : 0;
                case "<":
                    return izq < der ? 1 : 0;
                case ">=":
                    return izq >= der ? 1 : 0;
                case "<=":
                    return izq <= der ? 1 : 0;
                case "==":
                    return izq == der ? 1 : 0;
                case "!=":
                    return izq != der ? 1 : 0;
                default:
                    throw new Exception($"Operador no soportado: {operador}");
            }
        }

        private object InterpretarBloque(Nodo nodo)
        {
            object resultado = null;
            foreach (var hijo in nodo.Hijos)
            {
                resultado = Interpretar(hijo);
            }
            return resultado;
        }

        private object InterpretarMientras(Nodo nodo)
        {
            // nodo.Hijos[0] es la condición
            // nodo.Hijos[1] es el bloque
            while (Convert.ToInt32(Interpretar(nodo.Hijos[0])) > 0)
            {
                Interpretar(nodo.Hijos[1]);
            }
            return null;
        }
    }
} 