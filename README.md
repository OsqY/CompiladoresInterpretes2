# Compilador Básico

Un compilador simple implementado en C# que soporta operaciones aritméticas básicas, manejo de variables y estructuras de control.

## Características

- Análisis léxico y sintáctico
- Evaluación de expresiones aritméticas
- Manejo de variables
- Estructuras de control (if/else, while)
- Operadores soportados: +, -, *, /, ==, !=, <, >, <=, >=, &&, ||
- Mensajes de error en español

## Sintaxis

### Variables
```c
x = 5
y = 10
```

### Expresiones aritméticas
```c
resultado = x + y * 2
```

### Estructuras de control
```c
// If-else
si (x > 5) {
    x = x + 1
} sino {
    x = x - 1
}

// While
mientras (x < 10) {
    x = x + 1
}
```

### Operadores soportados
- Aritméticos: `+`, `-`, `*`, `/`
- Comparación: `==`, `!=`, `<`, `>`, `<=`, `>=`
- Lógicos: `&&` (y), `||` (o)

## Ejemplos

### Ejemplo básico
```c
a = 5
b = 10
c = a + b
```

### Ejemplo con estructuras de control
```c
x = 0
mientras (x < 5) {
    si (x % 2 == 0) {
        x = x + 2
    } sino {
        x = x + 1
    }
}
```

### Ejemplo complejo
```c
a = 10
b = 5
c = a * b
d = (a + b) * c
e = d / (a - b)
f = e + c * 2
```

## Uso

1. Compila el proyecto:
```bash
dotnet build
```

2. Ejecuta el compilador:
```bash
dotnet run
```

3. Ingresa tu código y presiona Enter dos veces para procesarlo.

## Estructura del Proyecto

- `Program.cs`: Punto de entrada del programa
- `Estructuras/`: Contiene las clases de datos
  - `Token.cs`: Representa los elementos básicos del código
  - `Nodo.cs`: Representa los nodos del árbol de sintaxis
- `Analizadores/`: Contiene los analizadores
  - `AnalizadorLexico.cs`: Convierte el código en tokens
  - `AnalizadorSintactico.cs`: Construye el árbol de sintaxis
  - `Interprete.cs`: Ejecuta el código

## Requisitos

- .NET 8.0 o superior
- Visual Studio 2022 o Visual Studio Code

## Licencia

Este proyecto está bajo la Licencia MIT. 