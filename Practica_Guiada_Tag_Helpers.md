# Práctica guiada: Tag Helpers en ASP.NET Core

**Web III · Razor Pages · Proyecto Héroes · .NET 10**

## Objetivo y resultado

Construir un formulario de inscripción de héroes a una misión utilizando Tag Helpers. Al terminar, el estudiante podrá relacionar controles HTML con propiedades C#, mostrar validaciones, cargar un selector y generar enlaces con parámetros.

Se continúa con el proyecto **HeroesWeb** de la práctica anterior. Se agrega una página de laboratorio independiente del CRUD: la inscripción se valida y se muestra en pantalla, pero **no se guarda en SQL Server**. Así se puede observar el trabajo de los Tag Helpers sin modificar las entidades generadas ni la configuración de Identity.


## 1. Preparar el proyecto

1. Abra HeroesWeb en Visual Studio o VS Code.
2. Abra una terminal en la carpeta que contiene `HeroesWeb.csproj`.
3. Compruebe la compilación:

```powershell
dotnet build
```

No se necesitan paquetes adicionales de Tag Helpers para este proyecto web. Conserve `Program.cs`, los contextos, Identity y la conexión existentes.

**Si no dispone del proyecto anterior**, puede realizar el laboratorio en un proyecto nuevo con el mismo nombre, desde otra carpeta:

```powershell
dotnet new webapp -n HeroesWeb -f net10.0
cd HeroesWeb
```

No ejecute este comando encima del proyecto existente.

**Resultado esperado:** un proyecto Razor Pages que compila.

## 2. Reconocer y habilitar los Tag Helpers

Los Tag Helpers son componentes ejecutados en el servidor que permiten generar o modificar HTML desde Razor. Muchos de los integrados se utilizan mediante atributos `asp-...`. El navegador recibe el HTML resultante.

Abra **`Pages/_ViewImports.cshtml`**. Compruebe que exista esta línea; agréguela únicamente si falta:

```cshtml
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

Conserve las demás líneas del archivo. Esta importación se aplica a las páginas de esa carpeta y sus subcarpetas.

Compare:

```html
<!-- HTML escrito manualmente -->
<label for="Inscripcion_Nombre">Nombre del héroe</label>
<input id="Inscripcion_Nombre" name="Inscripcion.Nombre" type="text">
```

```cshtml
@* Razor con Tag Helpers: la propiedad se creará en los siguientes pasos. *@
<label asp-for="Inscripcion.Nombre"></label>
<input asp-for="Inscripcion.Nombre">
```

**Observe:** `asp-for` referencia una propiedad, no contiene el texto que debe escribir el usuario. `[BindProperty]` y el model binding procesarán posteriormente los datos enviados.

## 3. Crear el modelo del formulario

Cree la carpeta **`Models`** si no existe. Agregue **`Models/InscripcionMisionInput.cs`**:

```csharp
using System.ComponentModel.DataAnnotations;

namespace HeroesWeb.Models;

public class InscripcionMisionInput
{
    [Required(ErrorMessage = "Ingrese el nombre del héroe.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    [Display(Name = "Nombre del héroe")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese un correo de contacto.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [Display(Name = "Correo de contacto")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione una ciudad.")]
    [Display(Name = "Ciudad de operación")]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione una misión.")]
    [Range(1, 3, ErrorMessage = "Seleccione una misión válida.")]
    [Display(Name = "Misión")]
    public int? MisionId { get; set; }

    [StringLength(250, ErrorMessage = "Use como máximo 250 caracteres.")]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }
}
```

Esta clase representa los datos de entrada de la actividad. No es una nueva tabla ni necesita agregarse al DbContext.

**Compruebe:** `MisionId` es nullable para representar la opción vacía. `[Required]` exige seleccionar y `[Range]` limita los valores admitidos.

## 4. Crear el PageModel

Cree la carpeta **`Pages/Laboratorio`**. Dentro, cree **`Inscripcion.cshtml.cs`** con este contenido completo:

```csharp
using HeroesWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeroesWeb.Pages.Laboratorio;

public class InscripcionModel : PageModel
{
    [BindProperty]
    public InscripcionMisionInput Inscripcion { get; set; } = new();

    public List<SelectListItem> Ciudades { get; private set; } = new();
    public List<SelectListItem> Misiones { get; private set; } = new();
    public string? Confirmacion { get; private set; }

    public void OnGet(int? misionId)
    {
        CargarOpciones();
        if (misionId is >= 1 and <= 3)
        {
            Inscripcion.MisionId = misionId;
        }
    }

    public IActionResult OnPostInscribir()
    {
        CargarOpciones();

        if (!string.IsNullOrWhiteSpace(Inscripcion.Ciudad)
            && !Ciudades.Any(c => c.Value == Inscripcion.Ciudad))
        {
            ModelState.AddModelError("Inscripcion.Ciudad",
                "Seleccione una ciudad de la lista.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var mision = Misiones.Single(m =>
            m.Value == Inscripcion.MisionId!.Value.ToString());

        Confirmacion = $"Inscripción válida: {Inscripcion.Nombre} — "
            + $"{mision.Text} en {Inscripcion.Ciudad}. "
            + "Demostración sin guardar en la base de datos.";

        return Page();
    }

    private void CargarOpciones()
    {
        Ciudades = new List<SelectListItem>
        {
            new() { Value = "Metrópolis", Text = "Metrópolis" },
            new() { Value = "Gotham", Text = "Gotham" },
            new() { Value = "Central City", Text = "Central City" }
        };

        Misiones = new List<SelectListItem>
        {
            new() { Value = "1", Text = "Rescate de civiles" },
            new() { Value = "2", Text = "Protección de la ciudad" },
            new() { Value = "3", Text = "Investigación de amenazas" }
        };
    }
}
```

| Elemento del laboratorio | Función |
|---|---|
| `OnGet(int? misionId)` | Recibe el parámetro del enlace y puede preseleccionar una misión |
| `[BindProperty]` | Permite enlazar los valores del POST con `Inscripcion` |
| `OnPostInscribir()` | Atiende el POST del handler `Inscribir` |
| `CargarOpciones()` | Construye las opciones tanto en GET como en POST |
| `ModelState.IsValid` | Permite decidir si el formulario puede aceptarse |
| `return Page()` | Vuelve a representar la misma página |

**Punto de control:** la lista debe reconstruirse también después de un POST. Las solicitudes HTTP no conservan automáticamente el PageModel de la solicitud anterior.

En este laboratorio se vuelve a mostrar la página para observar la validación. En un alta real con persistencia, después de guardar normalmente se redirige a un GET para evitar el reenvío accidental al actualizar.

## 5. Crear el formulario con Tag Helpers

Cree **`Pages/Laboratorio/Inscripcion.cshtml`**:

```cshtml
@page
@model HeroesWeb.Pages.Laboratorio.InscripcionModel
@{
    ViewData["Title"] = "Inscripción a una misión";
}

<h1>@ViewData["Title"]</h1>
<p>Complete los datos del héroe. Esta actividad no guarda registros.</p>

@if (Model.Confirmacion is not null)
{
    <div class="alert alert-success" role="status">
        @Model.Confirmacion
    </div>
}

<form method="post" asp-page="/Laboratorio/Inscripcion"
      asp-page-handler="Inscribir">

    <div asp-validation-summary="All" class="text-danger"></div>

    <div class="mb-3">
        <label asp-for="Inscripcion.Nombre" class="form-label"></label>
        <input asp-for="Inscripcion.Nombre" class="form-control" />
        <span asp-validation-for="Inscripcion.Nombre"
              class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Inscripcion.Email" class="form-label"></label>
        <input asp-for="Inscripcion.Email" class="form-control" />
        <span asp-validation-for="Inscripcion.Email"
              class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Inscripcion.Ciudad" class="form-label"></label>
        <select asp-for="Inscripcion.Ciudad" asp-items="Model.Ciudades"
                class="form-select">
            <option value="">-- Seleccione una ciudad --</option>
        </select>
        <span asp-validation-for="Inscripcion.Ciudad"
              class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Inscripcion.MisionId" class="form-label"></label>
        <select asp-for="Inscripcion.MisionId" asp-items="Model.Misiones"
                class="form-select">
            <option value="">-- Seleccione una misión --</option>
        </select>
        <span asp-validation-for="Inscripcion.MisionId"
              class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Inscripcion.Observaciones" class="form-label"></label>
        <textarea asp-for="Inscripcion.Observaciones" rows="3"
                  class="form-control"></textarea>
        <span asp-validation-for="Inscripcion.Observaciones"
              class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Validar inscripción</button>
    <a asp-page="/Laboratorio/Inscripcion"
       class="btn btn-outline-secondary">Nueva inscripción</a>
    <a asp-page="/Index" class="btn btn-secondary">Volver al inicio</a>
</form>

<hr />
<h2>Inscripción rápida</h2>
<a asp-page="/Laboratorio/Inscripcion" asp-route-misionId="1">
    Preparar inscripción a Rescate de civiles
</a>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

Los estilos `form-control`, `form-select` y `btn` corresponden a Bootstrap, incluido en la plantilla utilizada; no son Tag Helpers.

### Relacionar los atributos con su función

| Atributo o elemento | Qué observar en esta práctica |
|---|---|
| `label asp-for` | Texto de `[Display]` y asociación con el control |
| `input asp-for` | Nombre del campo y tipo de entrada |
| `textarea asp-for` | Edición de observaciones |
| `select asp-for` | Propiedad que recibe la selección |
| `asp-items` | Opciones disponibles |
| `asp-validation-for` | Error junto al campo |
| `asp-validation-summary="All"` | Resumen de errores, incluidos los de propiedades |
| `asp-page` | Página de destino |
| `asp-page-handler` | Handler de la solicitud |
| `asp-route-misionId` | Parámetro del enlace |
| `partial` | Inclusión de la vista parcial indicada |

**Importante:** los Tag Helpers no guardan en una base de datos. Tampoco sustituyen la validación del servidor. La plantilla debe cargar jQuery antes de los scripts de validación; conserve el layout original y su sección `Scripts`.

## 6. Agregar acceso desde el menú

Abra **`Pages/Shared/_Layout.cshtml`**. Dentro del `<ul>` del menú agregue:

```cshtml
<li class="nav-item">
    <a class="nav-link text-dark" asp-page="/Laboratorio/Inscripcion">
        Laboratorio Tag Helpers
    </a>
</li>
```

Conserve los enlaces de Héroes, Superpoderes e Identity si existen.

Ejecute:

```powershell
dotnet build
dotnet run
```

Abra la dirección indicada por `Now listening on:` y entre al nuevo menú. Si el proyecto exige autenticación, inicie sesión según la configuración de la práctica anterior.

**Resultado esperado:** formulario con nombre, correo, ciudad, misión, observaciones y botones.

## 7. Inspeccionar el HTML generado

Pulse **F12 → Elementos** y busque el control del nombre. Identifique estos atributos entre los generados:

```html
id="Inscripcion_Nombre"
name="Inscripcion.Nombre"
type="text"
```

1. Observe que el `label` apunta al identificador del control.
2. Inspeccione el correo: debe tener `type="email"`.
3. Inspeccione el selector: la misión Rescate de civiles tiene valor `1`.
4. Busque `__RequestVerificationToken` dentro del formulario. Es un campo oculto usado para la protección antifalsificación; no es una contraseña ni un dato de la inscripción.
5. Inspeccione el enlace de inscripción rápida. Con las rutas predeterminadas de este proyecto su destino incluirá `/Laboratorio/Inscripcion?misionId=1`.
6. Pulse ese enlace: Rescate de civiles debe quedar seleccionada.
7. Inspeccione el formulario y observe el destino del POST con `handler=Inscribir`.

**Distinción:** el enlace genera una solicitud GET. El formulario tiene `method="post"`, por lo que `Inscribir` corresponde a `OnPostInscribir`. El nombre del handler no determina por sí solo el método HTTP.

## 8. Ejecutar las pruebas guiadas

| N.º | Acción | Resultado esperado |
|---|---|---|
| 1 | Abrir el formulario desde el menú | Las listas tienen opciones y empiezan sin selección |
| 2 | Enviar los campos vacíos | Se impide aceptar la inscripción y aparecen avisos de validación |
| 3 | Escribir `AB` como nombre | El nombre no cumple la longitud mínima |
| 4 | Usar un correo sin formato válido | La inscripción no se acepta |
| 5 | Seleccionar ciudad y misión, dejando el nombre inválido | Las listas siguen disponibles y conservan la selección |
| 6 | Escribir Flash, flash@example.com, Central City y Rescate de civiles | Aparece la confirmación de inscripción válida |
| 7 | Dejar observaciones vacías con los demás datos válidos | Se acepta; observaciones es opcional |
| 8 | Pulsar Nueva inscripción | Se abre un GET con el formulario limpio |
| 9 | Pulsar el enlace de inscripción rápida | Se preselecciona la misión 1 |
| 10 | Abrir el CRUD de Héroes existente | No aparece una nueva fila por esta actividad |

Los avisos iniciales pueden provenir de la validación HTML del navegador o de jQuery. Para comprobar específicamente el servidor:

1. Desactive temporalmente JavaScript desde las herramientas de desarrollo y recargue.
2. En Elementos, agregue `novalidate` al `<form>` para desactivar también la validación nativa del navegador.
3. Envíe datos inválidos. La respuesta debe mostrar errores y no una confirmación.
4. Reactive JavaScript y recargue al terminar.

**Desafío de comprobación:** cambie desde Elementos el valor de una ciudad a `CiudadInventada` y envíe el formulario. El servidor debe rechazarla aunque el cliente envíe ese valor. Restaure la página al finalizar.

## 9. Comprender la relación con MVC

Los Tag Helpers de formularios también se utilizan en vistas MVC. Cambia la forma de indicar el destino y la organización del código:

| Propósito | Razor Pages, usado aquí | MVC |
|---|---|---|
| Abrir un formulario | `asp-page="/Laboratorio/Inscripcion"` | `asp-controller="Misiones" asp-action="Inscripcion"` |
| Enviar el formulario | `method="post" asp-page-handler="Inscribir"` | `method="post" asp-controller="Misiones" asp-action="Inscribir"` |
| Enviar un parámetro | `asp-route-misionId="1"` | `asp-route-misionId="1"` |
| Generar un control | `asp-for="Inscripcion.Nombre"` | `asp-for="Nombre"` si el modelo de la vista es directamente `InscripcionMisionInput` |

La columna MVC es una comparación: necesitaría un controlador y acciones reales. No copie esos destinos en la página de este laboratorio. No combine `asp-page` con `asp-controller` y `asp-action` en el mismo enlace.

## 10. Resolver el desafío adicional

**Tiempo adicional sugerido: 15 minutos.**

Agregue al formulario la cantidad de integrantes del equipo:

1. Cree la propiedad `int? Integrantes` en `InscripcionMisionInput`.
2. Exija un valor y permita números entre 1 y 10 mediante Data Annotations.
3. Asigne el nombre visible “Integrantes del equipo”.
4. Agregue label, input y span de validación mediante Tag Helpers.
5. Incluya el número en la confirmación.
6. Compruebe los casos vacío, 0, 1, 10 y 11.

**Resultado esperado:** solamente se aceptan cantidades entre 1 y 10 y el control se genera como entrada numérica.

## 11. Comprender y entregar

Responda con sus propias palabras:

1. ¿Dónde se ejecuta un Tag Helper y qué recibe el navegador?
2. ¿Qué diferencia hay entre `asp-for` y `[BindProperty]`?
3. ¿Por qué se escribe `Inscripcion.Nombre` en `asp-for`?
4. ¿Qué diferencia hay entre `asp-for` y `asp-items` en un selector?
5. ¿Por qué el selector muestra un texto pero envía un identificador?
6. ¿Qué método atiende `asp-page-handler="Inscribir"` en este formulario y por qué?
7. ¿Por qué se ejecuta `CargarOpciones()` tanto en GET como en POST?
8. ¿Por qué debe comprobarse `ModelState.IsValid` aunque exista validación en el navegador?
9. ¿Qué hace `asp-route-misionId` y cómo llega su valor al PageModel?
10. ¿Qué habría que agregar para guardar realmente una inscripción?

Entregue:

- Actualice su Repositorio de github y pase el enlace
- Respuestas a las diez preguntas.
- todo en un archivo texto 

**Evaluación sugerida de la actividad base:**

| Criterio | Puntaje |
|---|---:|
| modificacion del proyecto | 50 |
| Respuesta de preguntas  | 50 |
| **Total** | **100** |

El desafío se revisa como profundización y no altera el puntaje base.

## Problemas frecuentes

| Problema | Qué revisar |
|---|---|
| Aparecen atributos `asp-...` sin transformar en el HTML | Importación de Tag Helpers en `Pages/_ViewImports.cshtml` |
| No existe el espacio de nombres HeroesWeb | Utilizar el namespace real si su proyecto tiene otro nombre |
| Error al encontrar InscripcionModel | Coincidencia entre `@model`, namespace y clase del archivo `.cshtml.cs` |
| No se ejecuta el handler | `method="post"`, `asp-page-handler="Inscribir"` y `OnPostInscribir()` |
| Llega vacío el formulario | `[BindProperty]` y nombres de las propiedades referenciadas |
| El selector queda vacío después de un error | Llamada a `CargarOpciones()` al iniciar el POST |
| No hay validación en el navegador | Parcial de scripts, jQuery y renderizado de la sección Scripts en el layout |
| Aparecen mensajes repetidos | `All` muestra también errores de campos; para solo errores generales utilice `ModelOnly` |
| Se obtiene 404 | Ruta `/Laboratorio/Inscripcion`, archivo dentro de Pages y directiva `@page` |
| No aparece un registro nuevo en SQL Server | Es el comportamiento previsto: esta actividad valida, pero no persiste |

## Referencias y nota para el docente

Documentación oficial consultada:

- [Introducción a Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro?view=aspnetcore-10.0).
- [Tag Helpers en formularios](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-10.0).
- [Anchor Tag Helper](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper?view=aspnetcore-10.0).
- [Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-10.0).

