<div align="center">

# Red Sismológica · Revisión manual de eventos

**Aplicación de escritorio para que los analistas de una red sismológica revisen los sismos detectados automáticamente y registren el resultado: confirmar, rechazar o derivar a un experto.**

[![CI](https://github.com/Alelavini/Sismografo/actions/workflows/ci.yml/badge.svg)](https://github.com/Alelavini/Sismografo/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white)
![Windows Forms](https://img.shields.io/badge/UI-Windows%20Forms-0078D4?logo=windows&logoColor=white)
![xUnit](https://img.shields.io/badge/Tests-xUnit-5E1F87)
![License](https://img.shields.io/badge/License-MIT-yellow)

<br />

<img src="docs/captura.png" alt="Pantalla de revisión manual de eventos sísmicos" width="100%" />

</div>

---

## ⚡ Probarlo

### Opción A: descargar el ejecutable (sin instalar nada)

1. Entrá a **[Releases](https://github.com/Alelavini/Sismografo/releases/latest)** y bajá `RedSismica-win-x64.zip`.
2. Descomprimilo y abrí `RedSismica.exe`.

Es un único `.exe` autocontenido: no hace falta tener .NET instalado.

### Opción B: desde el código (Windows + [.NET SDK 8 o superior](https://dotnet.microsoft.com/download))

```bash
git clone https://github.com/Alelavini/Sismografo.git
cd Sismografo
dotnet run --project src/RedSismica
```

También se puede abrir `RedSismica.sln` en Visual Studio 2022 y presionar <kbd>F5</kbd>.

> La aplicación arranca con una sesión de analista y un conjunto de eventos simulados, registrados por estaciones ubicadas en localidades de Cuyo, el NOA y la Patagonia, así que se puede recorrer el caso de uso completo de inmediato.

---

## 🧭 Qué hace

Implementa el caso de uso **"Registrar resultado de revisión manual"** de un sistema de gestión para una red sismológica:

1. Lista los **eventos auto detectados** pendientes de revisión, ordenados por fecha y hora de ocurrencia.
2. Al seleccionar uno, el evento queda **bloqueado en revisión** para que ningún otro analista lo tome en paralelo. Si se elige otro, el anterior se libera.
3. Muestra el **detalle del evento**: magnitud y su descripción en la escala de Richter, clasificación por profundidad, epicentro, hipocentro, alcance y origen de generación.
4. Muestra las **series temporales** registradas por cada estación sismológica y sismógrafo, con la velocidad de onda máxima y si se superó el umbral de alarma.
5. Permite **ver el epicentro en un mapa** (OpenStreetMap) y **modificar los datos** del evento (magnitud, alcance, origen).
6. Valida que el evento tenga los datos completos y **registra el resultado**. Cada cambio de estado queda en el **historial** con la fecha y el usuario responsable.

### Ciclo de vida de un evento

```mermaid
stateDiagram-v2
    direction LR
    [*] --> AutoDetectado: la red detecta el sismo
    AutoDetectado --> BloqueadoEnRevision: el analista lo selecciona
    BloqueadoEnRevision --> AutoDetectado: selecciona otro evento o cierra la pantalla
    BloqueadoEnRevision --> Confirmado: confirmar
    BloqueadoEnRevision --> Rechazado: rechazar
    BloqueadoEnRevision --> DerivadoAExperto: solicitar revisión a experto
    Confirmado --> [*]
    Rechazado --> [*]
    DerivadoAExperto --> [*]
```

---

## 🏗 Diseño

La aplicación sigue el patrón **Pantalla / Gestor / Entidades** (Boundary-Control-Entity):

- **`PantallaRegistrarResultado`** (boundary): sólo presenta datos y captura las acciones del analista. No contiene reglas de negocio.
- **`GestorRegistrarResultado`** (control): coordina el caso de uso. Filtra y ordena eventos, bloquea y libera, valida y registra el resultado.
- **Entidades de dominio**: encapsulan las reglas. Por ejemplo, `EventoSismico` administra sus cambios de estado garantizando que haya un único estado vigente, `MagnitudRichter` deriva su descripción del valor y `ClasificacionSismo` se calcula a partir de la profundidad del hipocentro.

```mermaid
classDiagram
    direction LR
    class GestorRegistrarResultado {
        +tomarRegistroResultadoRevisionManual()
        +tomarSeleccionEvento(evento)
        +modificarDatosEvento(alcance, magnitud, origen)
        +tomarAccionConEvento(accion)
    }
    class EventoSismico {
        +setEstado(estado, usuario)
        +bloquearEventoSismico(usuario)
        +tieneDatosCompletos()
    }
    class CambioEstado {
        +sosActual()
    }
    class SerieTemporal
    class MuestraSismica
    class DetalleMuestraSismica
    class TipoDeDato
    class EstacionSismologica
    class Sismografo

    GestorRegistrarResultado --> "*" EventoSismico
    GestorRegistrarResultado --> Sesion
    Sesion --> Usuario
    EventoSismico --> "1..*" CambioEstado
    CambioEstado --> Estado
    CambioEstado --> Usuario
    EventoSismico --> MagnitudRichter
    EventoSismico --> ClasificacionSismo
    EventoSismico --> AlcanceSismo
    EventoSismico --> OrigenDeGeneracion
    EventoSismico --> "*" SerieTemporal
    SerieTemporal --> EstacionSismologica
    SerieTemporal --> Sismografo
    SerieTemporal --> "*" MuestraSismica
    MuestraSismica --> "*" DetalleMuestraSismica
    DetalleMuestraSismica --> TipoDeDato
```

### Estructura

```
Sismografo/
├── src/RedSismica/
│   ├── Dominio/                  Entidades: EventoSismico, SerieTemporal, Estado, CambioEstado...
│   ├── Interfaz/                 Pantalla principal y diálogo de modificación
│   ├── GestorRegistrarResultado.cs   Controlador del caso de uso
│   ├── DatosDePrueba.cs          Eventos, estaciones y sismógrafos simulados
│   └── Program.cs
├── tests/RedSismica.Tests/       Tests unitarios (xUnit)
└── .github/workflows/            CI y publicación automática del ejecutable
```

---

## 🧪 Tests

```bash
dotnet test
```

La suite cubre las reglas del caso de uso: filtrado y orden de eventos, bloqueo y liberación, transición a cada estado final, historial con un único estado vigente, validación de datos incompletos, acciones inválidas y clasificación por profundidad.

En cada *push*, **GitHub Actions** compila la solución y corre los tests. Al publicar un tag `v*`, otro workflow genera el `.exe` autocontenido y lo adjunta a un Release.

---

## 🛠 Stack

| | |
|---|---|
| **Lenguaje** | C# 12 con *nullable reference types* |
| **Plataforma** | .NET 8 (LTS) |
| **Interfaz** | Windows Forms |
| **Tests** | xUnit |
| **CI/CD** | GitHub Actions: build, tests y publicación de un ejecutable *single-file* |

---

## 👤 Autor

**Alejandro Lavini**

[![GitHub](https://img.shields.io/badge/GitHub-Alelavini-181717?logo=github)](https://github.com/Alelavini)

<div align="center">
<sub>Distribuido bajo licencia <a href="LICENSE">MIT</a>.</sub>
</div>
