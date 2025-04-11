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
            var nodo = new Nodo("PROGRAMA", "");
            while (posicion < tokens.Count)
            {
                nodo.Hijos.Add(CrearNodoDeclaracion());
            }
            return nodo;
        }

        private Nodo CrearNodoDeclaracion()
        {
            var token = tokens[posicion];
            switch (token.Valor)
            {
                case "si":
                    return CrearNodoSi();
                case "mientras":
                    return CrearNodoMientras();
                case "para":
                    return CrearNodoPara();
                default:
                    return CrearNodoExpresion();
            }
        }

        private Nodo CrearNodoExpresion()
        {
            var nodo = new Nodo("EXPRESION", "");
            
            // Si es una asignación
            if (posicion + 1 < tokens.Count && tokens[posicion + 1].Valor == "=")
            {
                nodo.Hijos.Add(new Nodo("IDENTIFICADOR", tokens[posicion].Valor, tokens[posicion]));
                posicion++; // Consumir el identificador
                nodo.Hijos.Add(new Nodo("OPERADOR", tokens[posicion].Valor, tokens[posicion]));
                posicion++; // Consumir el =
                nodo.Hijos.Add(CrearNodoExpresionAritmetica());
                return nodo;
            }

            // Si es una expresión aritmética
            nodo.Hijos.Add(CrearNodoExpresionAritmetica());
            return nodo;
        }

        private Nodo CrearNodoExpresionAritmetica()
        {
            var nodo = new Nodo("EXPRESION_ARITMETICA", "");
            
            // Manejar paréntesis
            if (tokens[posicion].Valor == "(")
            {
                posicion++; // Consumir el (
                nodo.Hijos.Add(CrearNodoExpresionAritmetica());
                if (posicion >= tokens.Count || tokens[posicion].Valor != ")")
                {
                    throw new Exception($"Se esperaba ')' en línea {tokens[posicion - 1].Linea}, columna {tokens[posicion - 1].Columna}");
                }
                posicion++; // Consumir el )
            }
            else
            {
                nodo.Hijos.Add(CrearNodoTermino());
            }

            // Manejar operadores con precedencia
            while (posicion < tokens.Count && EsOperador(tokens[posicion].Valor) && tokens[posicion].Valor != ")")
            {
                var operador = tokens[posicion].Valor;
                var precedencia = ObtenerPrecedencia(operador);
                
                // Si hay más operadores después
                if (posicion + 2 < tokens.Count)
                {
                    var siguienteToken = tokens[posicion + 2];
                    if (siguienteToken.Tipo == "OPERADOR" && EsOperador(siguienteToken.Valor))
                    {
                        var siguienteOperador = siguienteToken.Valor;
                        var siguientePrecedencia = ObtenerPrecedencia(siguienteOperador);
                        
                        if (siguientePrecedencia > precedencia)
                        {
                            // Guardar el primer operador y su operando izquierdo
                            var operadorToken = tokens[posicion];
                            posicion++; // Consumir el operador
                            
                            var operandoDerecho = new Nodo("EXPRESION_ARITMETICA", "");
                            // Añadir el primer término de la operación de mayor precedencia
                            operandoDerecho.Hijos.Add(CrearNodoTermino());
                            
                            // Añadir el operador de mayor precedencia
                            operandoDerecho.Hijos.Add(new Nodo("OPERADOR", siguienteOperador, siguienteToken));
                            posicion++; // Consumir el operador de mayor precedencia
                            
                            // Añadir el segundo término de la operación de mayor precedencia
                            operandoDerecho.Hijos.Add(CrearNodoTermino());
                            
                            // Ahora agregar el operador de menor precedencia al nodo principal
                            nodo.Hijos.Add(new Nodo("OPERADOR", operador, operadorToken));
                            
                            // Agregar la expresión de mayor precedencia como el operando derecho
                            nodo.Hijos.Add(operandoDerecho);
                            
                            continue;
                        }
                    }
                }

                // Si no hay operadores de mayor precedencia, agregar al nodo actual
                nodo.Hijos.Add(new Nodo("OPERADOR", operador, tokens[posicion]));
                posicion++;
                if (posicion < tokens.Count)
                {
                    nodo.Hijos.Add(CrearNodoTermino());
                }
            }

            return nodo;
        }

        private int ObtenerPrecedencia(string operador)
        {
            switch (operador)
            {
                case "*":
                case "/":
                case "%":
                    return 3;
                case "+":
                case "-":
                    return 2;
                case ">":
                case "<":
                case ">=":
                case "<=":
                case "==":
                case "!=":
                    return 1;
                default:
                    return 0;
            }
        }

        private Nodo CrearNodoTermino()
        {
            var token = tokens[posicion];
            switch (token.Tipo)
            {
                case "NUMERO":
                case "FLOTANTE":
                case "CADENA":
                case "IDENTIFICADOR":
                    posicion++;
                    return new Nodo(token.Tipo, token.Valor, token);
                case "OPERADOR":
                    if (token.Valor == "(")
                    {
                        posicion++;
                        var nodo = CrearNodoExpresionAritmetica();
                        if (posicion >= tokens.Count || tokens[posicion].Valor != ")")
                        {
                            throw new Exception($"Se esperaba ')' en línea {token.Linea}, columna {token.Columna}");
                        }
                        posicion++;
                        return nodo;
                    }
                    else if (EsOperador(token.Valor))
                    {
                        posicion++;
                        return new Nodo("OPERADOR", token.Valor, token);
                    }
                    break;
            }
            throw new Exception($"Token no esperado '{token.Valor}' en línea {token.Linea}, columna {token.Columna}");
        }

        private bool EsOperador(string valor)
        {
            return valor == "+" || valor == "-" || valor == "*" || valor == "/" || valor == "%" ||
                   valor == ">" || valor == "<" || valor == ">=" || valor == "<=" || valor == "==" || valor == "!=" ||
                   valor == "=";
        }

        private Nodo CrearNodoSi()
        {
            var nodo = new Nodo("si", "");
            posicion++; // Consumir "si"

            // Condición
            nodo.Hijos.Add(CrearNodoExpresionAritmetica());

            // Bloque
            if (posicion >= tokens.Count || tokens[posicion].Valor != "{")
            {
                throw new Exception($"Se esperaba '{{' después de la condición en línea {tokens[posicion - 1].Linea}");
            }
            posicion++; // Consumir "{"
            nodo.Hijos.Add(CrearNodoBloque());

            // Bloque "sino" opcional
            if (posicion < tokens.Count && tokens[posicion].Valor == "sino")
            {
                posicion++; // Consumir "sino"
                if (posicion >= tokens.Count || tokens[posicion].Valor != "{")
                {
                    throw new Exception($"Se esperaba '{{' después de 'sino' en línea {tokens[posicion - 1].Linea}");
                }
                posicion++; // Consumir "{"
                nodo.Hijos.Add(CrearNodoBloque());
            }

            return nodo;
        }

        private Nodo CrearNodoMientras()
        {
            var nodo = new Nodo("MIENTRAS", "");
            posicion++; // Consumir "mientras"

            // Condición
            nodo.Hijos.Add(CrearNodoExpresionAritmetica());

            // Bloque
            if (posicion >= tokens.Count || tokens[posicion].Valor != "{")
            {
                throw new Exception($"Se esperaba '{{' después de la condición en línea {tokens[posicion - 1].Linea}");
            }
            posicion++; // Consumir "{"
            nodo.Hijos.Add(CrearNodoBloque());

            return nodo;
        }

        private Nodo CrearNodoPara()
        {
            var nodo = new Nodo("PARA", "");
            posicion++; // Consumir "para"

            if (posicion >= tokens.Count || tokens[posicion].Valor != "(")
            {
                throw new Exception($"Se esperaba '(' después de 'para' en línea {tokens[posicion - 1].Linea}");
            }
            posicion++; // Consumir "("

            // Inicialización
            nodo.Hijos.Add(CrearNodoExpresion());
            if (posicion >= tokens.Count || tokens[posicion].Valor != ";")
            {
                throw new Exception($"Se esperaba ';' después de la inicialización en línea {tokens[posicion - 1].Linea}");
            }
            posicion++; // Consumir ";"

            // Condición
            nodo.Hijos.Add(CrearNodoExpresionAritmetica());
            if (posicion >= tokens.Count || tokens[posicion].Valor != ";")
            {
                throw new Exception($"Se esperaba ';' después de la condición en línea {tokens[posicion - 1].Linea}");
            }
            posicion++; // Consumir ";"

            // Incremento
            nodo.Hijos.Add(CrearNodoExpresion());
            if (posicion >= tokens.Count || tokens[posicion].Valor != ")")
            {
                throw new Exception($"Se esperaba ')' después del incremento en línea {tokens[posicion - 1].Linea}");
            }
            posicion++; // Consumir ")"

            // Bloque
            if (posicion >= tokens.Count || tokens[posicion].Valor != "{")
            {
                throw new Exception($"Se esperaba '{{' después de 'para' en línea {tokens[posicion - 1].Linea}");
            }
            posicion++; // Consumir "{"
            nodo.Hijos.Add(CrearNodoBloque());

            return nodo;
        }

        private Nodo CrearNodoBloque()
        {
            var nodo = new Nodo("BLOQUE", "");
            while (posicion < tokens.Count && tokens[posicion].Valor != "}")
            {
                nodo.Hijos.Add(CrearNodoDeclaracion());
            }
            if (posicion >= tokens.Count)
            {
                throw new Exception("Bloque no cerrado");
            }
            posicion++; // Consumir "}"
            return nodo;
        }
    }
}