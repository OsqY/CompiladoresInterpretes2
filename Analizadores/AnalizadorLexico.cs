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
        private HashSet<string> palabrasReservadas;

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
            this.palabrasReservadas = new HashSet<string> { "si", "sino", "mientras", "para", "TATIANA", "OSCAR" };
        }

        public List<Token> Analizar()
        {
            List<Token> tokens = new List<Token>();
            Token token;

            while ((token = SiguienteToken()) != null)
            {
                tokens.Add(token);
            }

            return tokens;
        }

        private Token SiguienteToken()
        {
            // Ignorar espacios en blanco
            while (posicion < codigo.Length && char.IsWhiteSpace(codigo[posicion]))
            {
                if (codigo[posicion] == '\n')
                {
                    linea++;
                    columna = 1;
                }
                else
                {
                    columna++;
                }
                posicion++;
            }

            if (posicion >= codigo.Length)
            {
                return null;
            }

            char caracter = codigo[posicion];

            // Comentarios de una línea
            if (caracter == 'T' && posicion + 6 < codigo.Length && codigo.Substring(posicion, 7) == "TATIANA")
            {
                posicion += 7;
                columna += 7;
                while (posicion < codigo.Length && codigo[posicion] != '\n')
                {
                    posicion++;
                }
                return SiguienteToken();
            }

            // Comentarios de múltiples líneas
            if (caracter == 'O' && posicion + 4 < codigo.Length && codigo.Substring(posicion, 5) == "OSCAR")
            {
                posicion += 5;
                columna += 5;
                while (posicion < codigo.Length && !(codigo[posicion] == 'O' && posicion + 4 < codigo.Length && codigo.Substring(posicion, 5) == "OSCAR"))
                {
                    if (codigo[posicion] == '\n')
                    {
                        linea++;
                        columna = 1;
                    }
                    else
                    {
                        columna++;
                    }
                    posicion++;
                }
                if (posicion < codigo.Length)
                {
                    posicion += 5;
                    columna += 5;
                }
                return SiguienteToken();
            }

            // Números
            if (char.IsDigit(codigo[posicion]))
            {
                return LeerNumero();
            }

            // Identificadores y palabras reservadas
            if (char.IsLetter(codigo[posicion]) || codigo[posicion] == '_')
            {
                return LeerIdentificador();
            }

            // Cadenas
            if (codigo[posicion] == '"')
            {
                return LeerCadena();
            }

            // Operadores y símbolos
            switch (caracter)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                case '=':
                case '>':
                case '<':
                case '!':
                case ';':
                case '{':
                case '}':
                case '(':
                case ')':
                    posicion++;
                    columna++;
                    return new Token("OPERADOR", caracter.ToString(), linea, columna - 1);
            }

            throw new Exception($"Carácter no reconocido '{caracter}' en línea {linea}, columna {columna}");
        }

        private Token LeerNumero()
        {
            int inicio = posicion;
            bool tienePunto = false;

            while (posicion < codigo.Length && (char.IsDigit(codigo[posicion]) || codigo[posicion] == '.'))
            {
                if (codigo[posicion] == '.')
                {
                    if (tienePunto)
                    {
                        throw new Exception($"Número inválido en línea {linea}, columna {columna}");
                    }
                    tienePunto = true;
                }
                posicion++;
                columna++;
            }

            string valor = codigo.Substring(inicio, posicion - inicio);
            return new Token(tienePunto ? "FLOTANTE" : "NUMERO", valor, linea, columna - valor.Length);
        }

        private Token LeerIdentificador()
        {
            int inicio = posicion;
            while (posicion < codigo.Length && (char.IsLetterOrDigit(codigo[posicion]) || codigo[posicion] == '_'))
            {
                posicion++;
                columna++;
            }

            string valor = codigo.Substring(inicio, posicion - inicio);
            string tipo = palabrasReservadas.Contains(valor) ? "PALABRA_RESERVADA" : "IDENTIFICADOR";
            return new Token(tipo, valor, linea, columna - valor.Length);
        }

        private Token LeerCadena()
        {
            StringBuilder valor = new StringBuilder();
            posicion++; // Saltar la comilla inicial
            columna++;
            int inicioColumna = columna;

            while (posicion < codigo.Length && codigo[posicion] != '"')
            {
                if (codigo[posicion] == '\\')
                {
                    posicion++;
                    columna++;
                    if (posicion >= codigo.Length)
                    {
                        throw new Exception($"Cadena no terminada en línea {linea}, columna {columna}");
                    }
                    switch (codigo[posicion])
                    {
                        case 'n': valor.Append('\n'); break;
                        case 't': valor.Append('\t'); break;
                        case '\\': valor.Append('\\'); break;
                        case '"': valor.Append('"'); break;
                        default: valor.Append(codigo[posicion]); break;
                    }
                }
                else
                {
                    valor.Append(codigo[posicion]);
                }
                posicion++;
                columna++;
            }

            if (posicion >= codigo.Length)
            {
                throw new Exception($"Cadena no terminada en línea {linea}, columna {columna}");
            }

            posicion++; // Saltar la comilla final
            columna++;
            return new Token("CADENA", valor.ToString(), linea, inicioColumna - 1);
        }

        private Token LeerOperador()
        {
            var operador = new StringBuilder();
            int inicioColumna = columna;
            
            while (posicion < codigo.Length && EsOperador(codigo[posicion]))
            {
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
            }
            return new Token("OPERADOR", operador.ToString(), linea, inicioColumna);
        }

        private bool EsOperador(char c)
        {
            return "+-*/%=!<>&|".Contains(c);
        }
    }
} 