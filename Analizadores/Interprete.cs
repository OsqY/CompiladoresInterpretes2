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

        public object? Interpretar(Nodo nodo)
        {
            switch (nodo.Tipo)
            {
                case "PROGRAMA":
                    return InterpretarPrograma(nodo);
                case "EXPRESION":
                    return InterpretarExpresion(nodo);
                case "EXPRESION_ARITMETICA":
                    return InterpretarExpresionAritmetica(nodo);
                case "EXPRESION_RELACIONAL":
                    return InterpretarExpresionRelacional(nodo);
                case "EXPRESION_LOGICA":
                    return InterpretarExpresionLogica(nodo);
                case "NEGACION":
                    return InterpretarNegacion(nodo);
                case "NUMERO":
                    return int.Parse(nodo.Valor);
                case "FLOTANTE":
                    return float.Parse(nodo.Valor);
                case "CADENA":
                    return nodo.Valor;
                case "IDENTIFICADOR":
                    return tablaSimbolos.ObtenerValor(nodo.Valor);
                case "MIENTRAS":
                    return InterpretarMientras(nodo);
                case "PARA":
                    return InterpretarPara(nodo);
                case "BLOQUE":
                    return InterpretarBloque(nodo);
                case "si":
                    return InterpretarSi(nodo);
                default:
                    throw new Exception($"Tipo de nodo no soportado: {nodo.Tipo}");
            }
        }

        private object? InterpretarPrograma(Nodo nodo)
        {
            object? resultado = null;
            foreach (var hijo in nodo.Hijos)
            {
                resultado = Interpretar(hijo);
            }
            return resultado;
        }

        private object? InterpretarExpresion(Nodo nodo)
        {
            // Si es una asignación
            if (nodo.Hijos.Count == 3 && nodo.Hijos[1].Valor == "=")
            {
                string nombreVariable = nodo.Hijos[0].Valor;
                object? valorExpresion = Interpretar(nodo.Hijos[2]);

                if (valorExpresion != null)
                {
                    if (tablaSimbolos.EstaDefinida(nombreVariable))
                    {
                        tablaSimbolos.ActualizarValor(nombreVariable, valorExpresion);
                    }
                    else
                    {
                        tablaSimbolos.Definir(nombreVariable, valorExpresion);
                    }

                    Console.WriteLine($"{nombreVariable} = {valorExpresion}");
                }
                return valorExpresion;
            }

            // Si es una expresión aritmética
            return Interpretar(nodo.Hijos[0]);
        }

        private object? InterpretarExpresionAritmetica(Nodo nodo)
        {
            if (nodo.Hijos.Count == 1)
            {
                return Interpretar(nodo.Hijos[0]);
            }

            var resultado = Interpretar(nodo.Hijos[0]);
            if (resultado == null) return null;
            
            for (int i = 1; i < nodo.Hijos.Count; i += 2)
            {
                var operador = nodo.Hijos[i].Valor;
                var valorDerecho = Interpretar(nodo.Hijos[i + 1]);
                if (valorDerecho == null) return null;
                
                resultado = AplicarOperador(operador, resultado, valorDerecho);
            }

            return resultado;
        }

        private object? InterpretarExpresionRelacional(Nodo nodo)
        {
            if (nodo.Hijos.Count == 3)
            {
                object? valorIzquierdo = Interpretar(nodo.Hijos[0]);
                string operador = nodo.Hijos[1].Valor;
                object? valorDerecho = Interpretar(nodo.Hijos[2]);
                
                return AplicarOperador(operador, valorIzquierdo, valorDerecho);
            }
            return null;
        }

        private object? AplicarOperador(string operador, object? izquierdo, object? derecho)
        {
            // Verificar que los valores no sean null
            if (izquierdo == null || derecho == null)
                throw new Exception($"No se puede aplicar el operador '{operador}' en valores nulos");

            // Si ambos son cadenas
            if (izquierdo is string izqStr && derecho is string derStr)
            {
                switch (operador)
                {
                    case "+": return izqStr + derStr;
                    case "==": return izqStr == derStr ? 1 : 0;
                    case "!=": return izqStr != derStr ? 1 : 0;
                }
            }
            // Si al menos uno es cadena, convertir ambos a cadena y concatenar
            else if (izquierdo is string || derecho is string)
            {
                if (operador == "+")
                {
                    return izquierdo.ToString() + derecho.ToString();
                }
            }
            // Si ambos son números
            else if (izquierdo is int izqInt && derecho is int derInt)
            {
                switch (operador)
                {
                    case "+": return izqInt + derInt;
                    case "-": return izqInt - derInt;
                    case "*": return izqInt * derInt;
                    case "/": return izqInt / derInt;
                    case "%": return izqInt % derInt;
                    case ">": return izqInt > derInt ? 1 : 0;
                    case "<": return izqInt < derInt ? 1 : 0;
                    case ">=": return izqInt >= derInt ? 1 : 0;
                    case "<=": return izqInt <= derInt ? 1 : 0;
                    case "==": return izqInt == derInt ? 1 : 0;
                    case "!=": return izqInt != derInt ? 1 : 0;
                }
            }
            // Si al menos uno es flotante
            else if (izquierdo is float izqFloat || derecho is float derFloat)
            {
                float izq = Convert.ToSingle(izquierdo);
                float der = Convert.ToSingle(derecho);
                switch (operador)
                {
                    case "+": return izq + der;
                    case "-": return izq - der;
                    case "*": return izq * der;
                    case "/": return izq / der;
                    case ">": return izq > der ? 1 : 0;
                    case "<": return izq < der ? 1 : 0;
                    case ">=": return izq >= der ? 1 : 0;
                    case "<=": return izq <= der ? 1 : 0;
                    case "==": return izq == der ? 1 : 0;
                    case "!=": return izq != der ? 1 : 0;
                }
            }

            throw new Exception($"Operación no soportada entre {izquierdo.GetType()} y {derecho.GetType()}");
        }

        private object? InterpretarExpresionLogica(Nodo nodo)
        {
            if (nodo.Hijos.Count == 3)
            {
                object? valorIzquierdo = Interpretar(nodo.Hijos[0]);
                string operador = nodo.Hijos[1].Valor;
                
                // Evaluación en cortocircuito para && y ||
                if (operador == "&&")
                {
                    // Si el izquierdo es falso, no necesitamos evaluar el derecho
                    if (ConvertirABooleano(valorIzquierdo) == false)
                        return 0; 

                    object? valorDerechoAnd = Interpretar(nodo.Hijos[2]);
                    return ConvertirABooleano(valorDerechoAnd) ? 1 : 0;
                }
                else if (operador == "||")
                {
                    // Si el izquierdo es verdadero, no necesitamos evaluar el derecho
                    if (ConvertirABooleano(valorIzquierdo) == true)
                        return 1; 
                        
                    object? valorDerechoOr = Interpretar(nodo.Hijos[2]);
                    return ConvertirABooleano(valorDerechoOr) ? 1 : 0;
                }
                
                object? valorDerecho = Interpretar(nodo.Hijos[2]);
                return AplicarOperadorLogico(operador, valorIzquierdo, valorDerecho);
            }
            
            return null;
        }

        private object? InterpretarNegacion(Nodo nodo)
        {
            if (nodo.Hijos.Count == 2)
            {
                object? valor = Interpretar(nodo.Hijos[1]);
                return ConvertirABooleano(valor) ? 0 : 1;
            }
            return null;
        }

        private bool ConvertirABooleano(object? valor)
        {
            if (valor == null) return false;
            
            if (valor is int entero)
                return entero != 0;
                
            if (valor is double flotante)
                return flotante != 0;
                
            if (valor is string cadena)
                return !string.IsNullOrEmpty(cadena);
                
            return false;
        }

        private object? AplicarOperadorLogico(string operador, object? izquierdo, object? derecho)
        {
            bool valorIzquierdoBool = ConvertirABooleano(izquierdo);
            bool valorDerechoBool = ConvertirABooleano(derecho);
            
            switch (operador)
            {
                case "&&": return valorIzquierdoBool && valorDerechoBool ? 1 : 0;
                case "||": return valorIzquierdoBool || valorDerechoBool ? 1 : 0;
                default: return null;
            }
        }

        private object? InterpretarMientras(Nodo nodo)
        {
            while (Convert.ToInt32(Interpretar(nodo.Hijos[0])) > 0)
            {
                Interpretar(nodo.Hijos[1]);
            }
            return null;
        }

        private object? InterpretarPara(Nodo nodo)
        {
            Interpretar(nodo.Hijos[0]); 
            while (Convert.ToInt32(Interpretar(nodo.Hijos[1])) > 0) 
            {
                Interpretar(nodo.Hijos[3]); 
                Interpretar(nodo.Hijos[2]);
            }
            return null;
        }

        private object? InterpretarBloque(Nodo nodo)
        {
            object? resultado = null;
            foreach (var hijo in nodo.Hijos)
            {
                resultado = Interpretar(hijo);
            }
            return resultado;
        }

        private object? InterpretarSi(Nodo nodo)
        {
            var condicion = Interpretar(nodo.Hijos[0]);
            
            bool condicionCumplida = false;
            
            if (condicion is int intValue)
            {
                condicionCumplida = intValue != 0;
            }
            else if (condicion is float floatValue)
            {
                condicionCumplida = floatValue != 0;
            }
            else if (condicion is bool boolValue)
            {
                condicionCumplida = boolValue;
            }
            else if (condicion != null)
            {
                condicionCumplida = true;
            }
            
            if (condicionCumplida)
            {
                return Interpretar(nodo.Hijos[1]);
            }
            else if (nodo.Hijos.Count > 2)
            {
                return Interpretar(nodo.Hijos[2]);
            }
            
            return null;
        }
    }
}