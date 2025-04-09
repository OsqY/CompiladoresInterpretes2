using System;
using System.Collections.Generic;
using COMPILADOR.Estructuras;

namespace COMPILADOR.Analizadores
{
    public class AnalizadorSintactico
    {
        private List<Token> tokens;
        private int posicion;

        public AnalizadorSintactico(List<Token> tokens)
        {
            this.tokens = tokens;
            this.posicion = 0;
        }

        public Nodo Analizar()
        {
            var programa = CrearNodoPrograma();
            if (posicion < tokens.Count)
            {
                throw new Exception($"Error de sintaxis en línea {tokens[posicion].Linea}, columna {tokens[posicion].Columna}");
            }
            return programa;
        }

        private Nodo CrearNodoPrograma()
        {
            var programa = new Nodo("PROGRAMA", "programa");
            while (posicion < tokens.Count)
            {
                programa.AgregarHijo(CrearNodoDeclaracion());
            }
            return programa;
        }

        private Nodo CrearNodoDeclaracion()
        {
            var token = tokens[posicion];
            if (token.Tipo == "PALABRA_RESERVADA")
            {
                switch (token.Valor)
                {
                    case "si":
                        return CrearNodoSi();
                    case "mientras":
                        return CrearNodoMientras();
                    default:
                        throw new Exception($"Palabra reservada no esperada '{token.Valor}' en línea {token.Linea}, columna {token.Columna}");
                }
            }
            return CrearNodoExpresion();
        }

        private Nodo CrearNodoSi()
        {
            var nodoSi = new Nodo("SI", "si", tokens[posicion++]);
            ConsumirToken("(");
            nodoSi.AgregarHijo(CrearNodoExpresion());
            ConsumirToken(")");
            nodoSi.AgregarHijo(CrearNodoBloque());
            
            if (posicion < tokens.Count && tokens[posicion].Valor == "sino")
            {
                posicion++;
                nodoSi.AgregarHijo(CrearNodoBloque());
            }
            
            return nodoSi;
        }

        private Nodo CrearNodoMientras()
        {
            var nodoMientras = new Nodo("MIENTRAS", "mientras", tokens[posicion++]);
            ConsumirToken("(");
            nodoMientras.AgregarHijo(CrearNodoExpresion());
            ConsumirToken(")");
            nodoMientras.AgregarHijo(CrearNodoBloque());
            return nodoMientras;
        }

        private Nodo CrearNodoBloque()
        {
            var nodoBloque = new Nodo("BLOQUE", "bloque");
            ConsumirToken("{");
            while (tokens[posicion].Valor != "}")
            {
                nodoBloque.AgregarHijo(CrearNodoDeclaracion());
            }
            ConsumirToken("}");
            return nodoBloque;
        }

        private Nodo CrearNodoExpresion()
        {
            var nodoExpresion = new Nodo("EXPRESION", "expresion");
            
            // Si es una asignación
            if (posicion + 1 < tokens.Count && tokens[posicion + 1].Valor == "=")
            {
                nodoExpresion.AgregarHijo(new Nodo("IDENTIFICADOR", tokens[posicion].Valor, tokens[posicion++]));
                nodoExpresion.AgregarHijo(new Nodo("OPERADOR", tokens[posicion].Valor, tokens[posicion++]));
                nodoExpresion.AgregarHijo(CrearNodoExpresionAritmetica());
            }
            else
            {
                // Es una expresión aritmética
                return CrearNodoExpresionAritmetica();
            }
            
            return nodoExpresion;
        }

        private Nodo CrearNodoExpresionAritmetica()
        {
            var nodoExpresion = new Nodo("EXPRESION_ARITMETICA", "expresion_aritmetica");
            nodoExpresion.AgregarHijo(CrearNodoTermino());
            
            while (posicion < tokens.Count && EsOperador(tokens[posicion].Valor))
            {
                nodoExpresion.AgregarHijo(new Nodo("OPERADOR", tokens[posicion].Valor, tokens[posicion++]));
                nodoExpresion.AgregarHijo(CrearNodoTermino());
            }
            
            return nodoExpresion;
        }

        private Nodo CrearNodoTermino()
        {
            var token = tokens[posicion++];
            switch (token.Tipo)
            {
                case "NUMERO":
                case "IDENTIFICADOR":
                    return new Nodo(token.Tipo, token.Valor, token);
                default:
                    throw new Exception($"Token no esperado '{token.Valor}' en línea {token.Linea}, columna {token.Columna}");
            }
        }

        private void ConsumirToken(string valorEsperado)
        {
            if (posicion >= tokens.Count || tokens[posicion].Valor != valorEsperado)
            {
                throw new Exception($"Se esperaba '{valorEsperado}' en línea {tokens[posicion-1].Linea}, columna {tokens[posicion-1].Columna}");
            }
            posicion++;
        }

        private bool EsOperador(string valor)
        {
            return "+-*/%=!<>&|".Contains(valor) || 
                   valor == "==" || valor == "!=" || 
                   valor == "<=" || valor == ">=" || 
                   valor == "&&" || valor == "||";
        }
    }
} 