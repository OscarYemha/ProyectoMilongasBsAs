# Agenda de Milongas de Buenos Aires

Aplicación de escritorio desarrollada en **C# y .NET 8** para consultar, filtrar y explorar la agenda de milongas de Buenos Aires.

La aplicación obtiene información pública desde **Hoy Milonga** mediante navegación automatizada y procesamiento de HTML, y presenta los eventos en una interfaz **Windows Forms** con carga automática y progresiva, filtros, vistas de detalle y cálculo de distancias.

El proyecto busca centralizar la información útil de cada milonga en una interfaz simple, permitiendo consultar rápidamente qué eventos hay disponibles, dónde se realizan, sus horarios, clases y demás información relevante.

## Capturas

### Agenda

![Agenda principal](docs/images/agenda-principal.png)

### Filtros

![Agenda filtrada por barrio y clase](docs/images/agenda-filtros.png)

### Detalle de una milonga

![Vista detallada de una milonga](docs/images/milonga-detalle.png)

## Funcionalidades

- Carga automática de la agenda al abrir la aplicación.
- Carga progresiva de los eventos para mostrar resultados sin esperar a que finalice todo el proceso.
- Navegación por las fechas disponibles.
- Selección automática del día actual al abrir la agenda.
- Búsqueda por nombre, salón o barrio.
- Filtro por barrio.
- Filtro por eventos con clase o sin clase.
- Ordenamiento cronológico automático de los eventos.
- Identificación de eventos destacados, abiertos, finalizados y cancelados.
- Visualización de modalidad de entrada y eventos especiales.
- Vista detallada de cada milonga.
- Extracción de horarios, dirección, coordenadas, organizadores, descripción, contactos e imágenes.
- Acceso al mapa y ubicación del evento.
- Cálculo de distancia disponible en la vista de detalle.
- Obtención de información detallada bajo demanda para evitar penalizar la carga inicial de la agenda.
- Recuperación mediante reintentos ante determinadas redirecciones inesperadas del sitio fuente.
- Manejo asincrónico de las operaciones de navegación y actualización de la interfaz.
- Cancelación de búsquedas anteriores para evitar actualizaciones innecesarias.

## Tecnologías

- C#
- .NET 8
- Windows Forms
- Microsoft Playwright
- HtmlAgilityPack
- System.Text.Json

## Arquitectura

La solución está dividida principalmente en dos proyectos con responsabilidades diferenciadas.

### Milongas.App

Aplicación Windows Forms responsable de la interfaz de usuario y de la interacción con la agenda.

Incluye:

- listado de milongas;
- tarjetas de eventos;
- filtros;
- búsqueda;
- navegación por fechas;
- vista de detalle;
- estados de carga;
- interacción del usuario.

### Milongas.Extractor

Biblioteca de clases responsable de obtener, procesar y organizar los datos utilizados por la aplicación.

Incluye:

- navegación automatizada con Playwright;
- obtención de HTML dinámico;
- parsing con HtmlAgilityPack;
- extracción de agenda y detalles;
- modelos de dominio;
- filtrado y ordenamiento;
- cálculo de distancias;
- manejo de contextos de navegación;
- caché local de datos de detalle.

## Flujo general

```text
Hoy Milonga
    ↓
BrowserService / Playwright
    ↓
HtmlExtractor / MilongaDetalleExtractor
    ↓
HoyMilongaService
    ↓
AgendaService / DistanciaService
    ↓
Milongas.App
```

La agenda comienza a cargarse automáticamente al abrir la pantalla de milongas.

Los eventos se obtienen progresivamente por fecha. Esto permite mostrar el primer conjunto de resultados mientras la aplicación continúa procesando el resto de los días disponibles.

La información que solamente existe en la página individual de una milonga se obtiene bajo demanda cuando el usuario abre su vista de detalle.

## Desafíos técnicos

Durante el desarrollo se abordaron distintos problemas relacionados con la navegación web, la extracción de contenido dinámico y la actualización de una aplicación de escritorio.

### Carga progresiva

La agenda se procesa por día para poder mostrar los primeros resultados antes de finalizar la obtención completa de los eventos disponibles.

Esto reduce el tiempo percibido de espera y permite comenzar a utilizar la aplicación mientras continúa el procesamiento.

### Contenido web dinámico

El sitio fuente genera parte de su contenido dinámicamente.

Para obtener los datos se utiliza **Microsoft Playwright**, que permite navegar e interactuar con el sitio como un navegador real antes de procesar el HTML resultante.

Posteriormente, **HtmlAgilityPack** se utiliza para recorrer y extraer la información relevante del documento.

### Extracción adaptable

Los datos de la agenda y los detalles se extraen utilizando la estructura semántica disponible en el HTML.

Durante el desarrollo fue necesario adaptar los extractores ante cambios en el formato de horarios, clases y direcciones del sitio fuente.

### Carga de detalles bajo demanda

Parte de la información de una milonga solamente se encuentra disponible en su página individual.

En lugar de descargar todos esos detalles durante la carga inicial, la aplicación los obtiene cuando el usuario abre un evento.

Esto evita realizar una gran cantidad de navegaciones innecesarias y permite mantener una carga inicial más rápida.

### Manejo de redirecciones

El sitio fuente puede redirigir de manera intermitente algunas solicitudes de detalle hacia una página de inicio de sesión.

La aplicación detecta esta situación y puede recrear el contexto de navegación y volver a intentar la solicitud de forma controlada.

Este mecanismo permite recuperarse de determinados fallos transitorios sin interrumpir inmediatamente la experiencia del usuario.

### Concurrencia y asincronismo

Las operaciones de navegación y actualización de la interfaz se coordinan de forma asincrónica para evitar bloquear innecesariamente la interfaz de Windows Forms.

También se controla el acceso a determinadas operaciones del navegador para evitar navegaciones simultáneas incompatibles.

### Búsqueda con debounce

Las búsquedas de texto esperan brevemente antes de actualizar los resultados.

Esto evita reprocesar la agenda por cada tecla presionada cuando el usuario está escribiendo una búsqueda.

## Distancias

La aplicación puede calcular la distancia entre un punto de origen y las coordenadas geográficas de una milonga utilizando la **fórmula de Haversine**.

Actualmente, el punto de origen se encuentra configurado temporalmente con las coordenadas del Obelisco de Buenos Aires.

La distancia se muestra en la vista de detalle de cada evento.

Una posible evolución del proyecto es utilizar la ubicación real del usuario como punto de origen.

## Caché

El proyecto incluye un mecanismo de caché local para información de detalle que puede resultar costosa de obtener mediante navegación.

Los datos se almacenan localmente en:

```text
detalles-cache.json
```

El archivo se genera en tiempo de ejecución y no forma parte del repositorio.

## Ejecución

### Requisitos

- Windows
- .NET 8 SDK
- Visual Studio 2022 o compatible
- Navegadores de Microsoft Playwright

### Pasos

Después de clonar el repositorio:

1. Abrir la solución en Visual Studio.
2. Restaurar los paquetes NuGet.
3. Asegurarse de que `Milongas.App` sea el proyecto de inicio.
4. Compilar la solución.
5. Verificar que Chromium para Playwright se encuentre instalado.
6. Ejecutar la aplicación.

Playwright necesita sus navegadores instalados para poder realizar la navegación automatizada.

Si fuera necesario, pueden instalarse utilizando el script de Playwright generado durante la compilación del proyecto.

## Limitaciones actuales

- La aplicación depende de la estructura HTML de Hoy Milonga, por lo que cambios en el sitio pueden requerir actualizar los extractores o selectores.
- Algunas solicitudes de detalle pueden ser redirigidas de manera intermitente por el sitio fuente.
- La aplicación implementa reintentos para recuperarse de determinados casos, pero su funcionamiento continúa dependiendo del comportamiento del servicio externo.
- La ubicación real del usuario todavía no se obtiene automáticamente.
- El punto utilizado para calcular distancias está configurado temporalmente.
- La versión actual está desarrollada como aplicación de escritorio para Windows.
- La carga de datos depende de la disponibilidad y los tiempos de respuesta del sitio fuente.

## Próximos pasos

Entre las posibles evoluciones del proyecto se encuentran:

- backend con ASP.NET Core Web API;
- extracción centralizada del lado servidor;
- persistencia en base de datos;
- geolocalización real del usuario;
- cliente web o móvil;
- soporte para Android e iOS;
- tests automatizados;
- logging estructurado;
- mayor desacoplamiento entre la extracción de datos y la interfaz de usuario.

## Estado

Proyecto en desarrollo activo.

La versión actual funciona como **MVP de escritorio** y forma parte de un portfolio de desarrollo de software.