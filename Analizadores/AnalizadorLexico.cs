using System;
using System.Collections.Generic;
using System.Text;
using COMPILADOR.Estructuras;

namespace COMPILADOR.Analizadores
{
    public class AnalizadorLexico
    {
        private string codigo;
        private int posicion;
        private int linea;
        private int columna;

        private static readonly HashSet<string> palabrasReservadas = new HashSet<string>
        {
            "si", "sino", "mientras"
        };

        private static readonly HashSet<string> operadores = new HashSet<string>
        {
            "+", "-", "*", "/", "%", "=", "==", "!=", "<", ">", "<=", ">=", "&&", "||"
        };

        public AnalizadorLexico(string codigo)
        {
            this.codigo = codigo;
            this.posicion = 0;
            this.linea = 1;
            this.columna = 1;
        }

        public List<Token> Analizar()
        {
            var tokens = new List<Token>();
            while (posicion < codigo.Length)
            {
                char caracter = codigo[posicion];
                
                if (char.IsWhiteSpace(caracter))
                {
                    if (caracter == '\n')
                    {
                        linea++;
                        columna = 1;
                    }
                    else
                    {
                        columna++;
                    }
                    posicion++;
                    continue;
                }

                if (caracter == '{' || caracter == '}' || caracter == '(' || caracter == ')')
                {
                    tokens.Add(new Token("SIMBOLO", caracter.ToString(), linea, columna));
                    posicion++;
                    columna++;
                    continue;
                }

                if (char.IsDigit(caracter))
                {
                    tokens.Add(LeerNumero());
                }
                else if (char.IsLetter(caracter))
                {
                    tokens.Add(LeerIdentificador());
                }
                else if (EsOperador(caracter))
                {
                    tokens.Add(LeerOperador());
                }
                else
                {
                    throw new Exception($"Carácter no reconocido '{caracter}' en línea {linea}, columna {columna}");
                }
            }
            return tokens;
        }

        private Token LeerNumero()
        {
            var numero = new StringBuilder();
            int inicioColumna = columna;
            
            while (posicion < codigo.Length && char.IsDigit(codigo[posicion]))
            {
                numero.Append(codigo[posicion]);
                posicion++;
                columna++;
            }
            return new Token("NUMERO", numero.ToString(), linea, inicioColumna);
        }

        private Token LeerIdentificador()
        {
            var identificador = new StringBuilder();
            int inicioColumna = columna;
            
            while (posicion < codigo.Length && char.IsLetterOrDigit(codigo[posicion]))
            {
                identificador.Append(codigo[posicion]);
                posicion++;
                columna++;
            }

            string valor = identificador.ToString();
            if (palabrasReservadas.Contains(valor))
            {
                return new Token("PALABRA_RESERVADA", valor, linea, inicioColumna);
            }
            return new Token("IDENTIFICADOR", valor, linea, inicioColumna);
        }

        private Token LeerOperador()
        {
            var operador = new StringBuilder();
            int inicioColumna = columna;
            
            operador.Append(codigo[posicion]);
            posicion++;
            columna++;
            
            if (posicion < codigo.Length)
            {
                string posibleOperador = operador.ToString() + codigo[posicion];
                if (operadores.Contains(posibleOperador))
                {
                    operador.Append(codigo[posicion]);
                    posicion++;
                    columna++;
                }
            }
            
            string operadorFinal = operador.ToString();
            if (!operadores.Contains(operadorFinal))
            {
                throw new Exception($"Operador no válido '{operadorFinal}' en línea {linea}, columna {inicioColumna}");
            }
            
            return new Token("OPERADOR", operadorFinal, linea, inicioColumna);
        }

        private bool EsOperador(char c)
        {
            return "+-*/%=!<>&|".Contains(c);
        }
    }
} 