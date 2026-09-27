# Práctica guiada: héroes y superpoderes con Razor Pages

**Web III · Database First · SQL Server · .NET 10**

## Objetivo y resultado

Crear una aplicación web que permita registrar, listar, consultar, modificar y eliminar héroes y sus superpoderes, partiendo de una base de datos creada en SQL Server Management Studio (SSMS).

Esta guía conserva la base `HeroesDb`, las tablas, los campos y la relación del chat [SQL y scaffolding Razor](https://chatgpt.com/share/6aa74f18-331c-83e9-9593-8eaf5903d441). En ese chat, el procedimiento de generación se mostró con Estudiantes; aquí se aplica a las dos tablas de héroes y se completa hasta probar ambos CRUD.

Un héroe puede tener cero o muchos superpoderes. Cada superpoder pertenece a un único héroe. Al eliminar un héroe, se eliminan también sus poderes, de acuerdo con el `ON DELETE CASCADE` del SQL original. En este ejercicio, las habilidades de Batman también se registran en la tabla SuperPoderes.

**Duración estimada:** 2 a 3 horas con el software instalado.

## 1. Preparar el equipo

Necesita Windows, el SDK de .NET 10, SQL Server con una instancia accesible, SSMS y un editor: Visual Studio compatible con .NET 10 o VS Code. SSMS administra SQL Server; instalar solamente SSMS no instala el motor de base de datos.

En PowerShell compruebe:

```powershell
dotnet --list-sdks
```

Debe aparecer un SDK `10.0.xxx`. Los comandos de esta guía se ejecutan en PowerShell; los bloques SQL se ejecutan en SSMS.

## 2. Conectarse a SQL Server desde SSMS

1. Abra SQL Server Management Studio.
2. Seleccione **Motor de base de datos**.
3. Introduzca el nombre de la instancia instalada.
4. Seleccione **Autenticación de Windows**.
5. Pulse **Conectar**.

El ejemplo original utiliza `(localdb)\MSSQLLocalDB`. Úselo si tiene LocalDB instalado. Si su instalación es SQL Server Express, podría utilizar `.\SQLEXPRESS`; el nombre depende de su equipo.

**Anote el servidor exacto con el que se conectó.** Tanto el comando de ingeniería inversa como la aplicación deben utilizar esa misma instancia. Su usuario de Windows debe tener permisos para crear la base y trabajar con sus tablas. Si el servidor es compartido, el docente debe indicar la instancia y asignar los permisos.

## 3. Crear HeroesDb y sus tablas

Pulse **Nueva consulta**, copie el siguiente SQL y ejecútelo con **F5**. El script se ejecuta una sola vez y supone que no existe todavía `HeroesDb`. Si ya realizó el ejemplo original, conserve esa base y compruebe sus tablas en lugar de recrearla.

```sql
CREATE DATABASE HeroesDb;
GO

USE HeroesDb;
GO

CREATE TABLE Heroes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Ciudad NVARCHAR(100) NOT NULL,
    IdentidadSecreta NVARCHAR(100) NULL
);
GO

CREATE TABLE SuperPoderes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    HeroeId INT NOT NULL,

    CONSTRAINT FK_SuperPoderes_Heroes
        FOREIGN KEY (HeroeId)
        REFERENCES Heroes(Id)
        ON DELETE CASCADE
);
GO

INSERT INTO Heroes (Nombre, Ciudad, IdentidadSecreta)
VALUES
(N'Superman', N'Metrópolis', N'Clark Kent'),
(N'Batman', N'Gotham', N'Bruce Wayne');
GO

INSERT INTO SuperPoderes (Nombre, Descripcion, HeroeId)
VALUES
(N'Volar', N'Puede desplazarse por el aire.', 1),
(N'Superfuerza', N'Tiene una fuerza extraordinaria.', 1),
(N'Visión de calor', N'Emite rayos de energía desde los ojos.', 1),
(N'Inteligencia estratégica', N'Planifica y analiza situaciones complejas.', 2),
(N'Artes marciales', N'Tiene entrenamiento físico y de combate.', 2);
GO
```

Se conserva el esquema original. Se antepone `N` a los textos de ejemplo para representar explícitamente literales Unicode.

Actualice **Bases de datos** en el Explorador de objetos y ubique `HeroesDb > Tablas`. Deben aparecer `dbo.Heroes` y `dbo.SuperPoderes`.

Compruebe los registros:

```sql
USE HeroesDb;
GO
SELECT * FROM dbo.Heroes;
SELECT * FROM dbo.SuperPoderes;

SELECT h.Nombre AS Heroe, p.Nombre AS SuperPoder
FROM dbo.Heroes AS h
INNER JOIN dbo.SuperPoderes AS p ON p.HeroeId = h.Id;
```

**Resultado esperado:** dos héroes y cinco poderes: tres de Superman y dos de Batman.

| Elemento | Función |
|---|---|
| Heroes.Id | Clave primaria del héroe |
| SuperPoderes.Id | Clave primaria del poder |
| SuperPoderes.HeroeId | Clave foránea que identifica al dueño del poder |
| IDENTITY(1,1) | Genera los identificadores automáticamente |
| NOT NULL | Exige un valor |
| NULL | Permite omitir identidad secreta o descripción |
| ON DELETE CASCADE | Elimina los poderes cuando se elimina su héroe |

## 4. Crear el proyecto Razor Pages

Abra PowerShell en la carpeta donde guarda sus proyectos:

```powershell
dotnet new webapp -n HeroesWeb -f net10.0
cd HeroesWeb
```

Abra la carpeta en su editor. En VS Code puede ejecutar `code .`.

**Todos los comandos siguientes se ejecutan en la carpeta que contiene `HeroesWeb.csproj`.** Mantenga el nombre del proyecto para poder copiar los ejemplos de espacios de nombres.

## 5. Instalar paquetes y herramientas

Se conserva el procedimiento del chat: paquetes en el proyecto y herramientas globales. La versión se limita a la familia 10.0 para que corresponda con .NET 10.

Ejecute cada línea:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version "10.0.*"
dotnet add package Microsoft.EntityFrameworkCore.Design --version "10.0.*"
dotnet add package Microsoft.EntityFrameworkCore.Tools --version "10.0.*"
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version "10.0.*"

dotnet tool install -g dotnet-ef --version "10.0.*"
dotnet tool install -g dotnet-aspnet-codegenerator --version "10.0.*"
```

Si una herramienta ya está instalada, consulte `dotnet tool list -g`. Si necesita actualizarla a la familia 10.0, use el comando correspondiente:

```powershell
dotnet tool update -g dotnet-ef --version "10.0.*"
dotnet tool update -g dotnet-aspnet-codegenerator --version "10.0.*"
```

| Componente | Función |
|---|---|
| EntityFrameworkCore.SqlServer | Conectar EF Core con SQL Server |
| EntityFrameworkCore.Design | Operaciones de diseño e ingeniería inversa |
| EntityFrameworkCore.Tools | Comandos de EF en la consola de NuGet de Visual Studio; se conserva del ejemplo, aunque no es indispensable para estos comandos CLI |
| VisualStudio.Web.CodeGeneration.Design | Dependencias para generar el CRUD web |
| dotnet-ef | Generar modelos y contexto desde la base |
| dotnet-aspnet-codegenerator | Generar las páginas Razor |

Compruebe:

```powershell
dotnet ef --version
dotnet tool list -g
dotnet build
```

**Resultado esperado:** herramientas 10.0 y compilación sin errores.

## 6. Generar los modelos y el DbContext

Ejecute este comando en una sola línea. Reemplace el servidor si utilizó otra instancia en SSMS:

```powershell
dotnet ef dbcontext scaffold "data source=localhost;initial catalog=2026web3;integrated security=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --context HeroesContext --context-dir Data --output-dir Models --no-onconfiguring --no-pluralize --data-annotations
```

Este paso se llama **ingeniería inversa** y forma parte del enfoque **Database First**. Lee el esquema existente para crear clases C#; no crea nuevamente la base ni copia sus registros dentro de las clases.

| Opción | Resultado |
|---|---|
| --context HeroesContext | Nombre del contexto |
| --context-dir Data | Guarda el contexto en Data |
| --output-dir Models | Guarda las entidades en Models |
| --no-onconfiguring | La conexión se configurará en Program.cs |
| --no-pluralize | Mantiene los nombres Heroes y SuperPoderes en las clases |
| --data-annotations | Genera atributos de mapeo cuando corresponde |

Las dos últimas opciones complementan el comando original: evitan nombres inesperados al singularizar palabras españolas y aportan atributos útiles para los formularios. Se omite `-f` en la primera ejecución para no sobrescribir archivos por accidente. Si necesita regenerar, revise primero sus modificaciones: `-f` las sobrescribe.

**Resultado esperado:**

- `Models/Heroes.cs`
- `Models/SuperPoderes.cs`
- `Data/HeroesContext.cs`

Abra `SuperPoderes.cs` y localice `HeroeId` y la propiedad de navegación `Heroe`. En `Heroes.cs`, localice la colección de poderes. La clave foránea es el identificador; la navegación permite acceder al objeto relacionado.

[Referencia de las opciones: Microsoft, herramientas CLI de EF Core](https://learn.microsoft.com/en-us/ef/core/cli/dotnet).

## 7. Configurar appsettings.json

Reemplace el contenido de `appsettings.json` por:

```json
{
  "ConnectionStrings": {
    "HeroesDb": "Server=(localdb)\\MSSQLLocalDB;Database=HeroesDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Use exactamente la misma instancia del paso 6. En JSON, una barra invertida se escribe `\\`; en el comando de PowerShell anterior se escribe `\`.

Por ejemplo, para Express, la parte del servidor en el JSON sería `Server=.\\SQLEXPRESS;`.

`Trusted_Connection=True` utiliza su identidad de Windows. `TrustServerCertificate=True` permite trabajar con el certificado del servidor en este entorno de práctica local; al publicar, se debe configurar un certificado válido.

## 8. Registrar HeroesContext en Program.cs

Reemplace `Program.cs` por:

```csharp
using Microsoft.EntityFrameworkCore;
using HeroesWeb.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<HeroesContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("HeroesDb")
        ?? throw new InvalidOperationException("Falta la conexión HeroesDb.")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.Run();
```

El registro del contexto debe estar **antes de `builder.Build()`**. Permite que las páginas reciban el contexto mediante inyección de dependencias.

Compile:

```powershell
dotnet build
```

No ejecute migraciones ni `dotnet ef database update`: las tablas ya existen en SQL Server.

## 9. Generar los dos CRUD

Ejecute cada comando en una sola línea:

```powershell
dotnet aspnet-codegenerator razorpage -m HeroesWeb.Models.Heroes -dc HeroesWeb.Data.HeroesContext -udl -outDir Pages/Heroes --referenceScriptLibraries

dotnet aspnet-codegenerator razorpage -m HeroesWeb.Models.SuperPoderes -dc HeroesWeb.Data.HeroesContext -udl -outDir Pages/SuperPoderes --referenceScriptLibraries
```

`-m` selecciona el modelo; `-dc`, el contexto; `-udl` reutiliza el layout, y `-outDir` indica la carpeta de destino. La última opción incorpora las referencias a scripts de validación en los formularios.

En **cada carpeta** se crean estos pares:

| Interfaz | Lógica | Operación |
|---|---|---|
| Index.cshtml | Index.cshtml.cs | Listado |
| Create.cshtml | Create.cshtml.cs | Alta |
| Details.cshtml | Details.cshtml.cs | Consulta individual |
| Edit.cshtml | Edit.cshtml.cs | Modificación |
| Delete.cshtml | Delete.cshtml.cs | Confirmación y eliminación |

La ingeniería inversa del paso 6 generó las entidades; estos comandos generan la interfaz y los handlers. [Referencia del generador de Microsoft](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator?view=aspnetcore-10.0).

## 10. Ajustar la relación en los formularios de poderes

### 10.1. Mostrar el nombre del héroe en el selector

Abra `Pages/SuperPoderes/Create.cshtml.cs` y `Pages/SuperPoderes/Edit.cshtml.cs`.

Busque **todas** las expresiones `new SelectList(...)` que cargan `HeroeId`. Compruebe que utilicen `"Id"` como valor y `"Nombre"` como texto:

```csharp
ViewData["HeroeId"] = new SelectList(_context.Heroes, "Id", "Nombre");
```

En Edit, o cuando se vuelve a mostrar un formulario enviado, preserve la selección:

```csharp
ViewData["HeroeId"] = new SelectList(
    _context.Heroes, "Id", "Nombre", SuperPoderes.HeroeId);
```

Conserve el nombre real de la variable del contexto que creó el generador, normalmente `_context`. Si ya usa `Nombre`, no es necesario cambiarlo.

En los archivos `Create.cshtml` y `Edit.cshtml` de poderes debe existir un control como este:

```html
<select asp-for="SuperPoderes.HeroeId" class="form-control"
        asp-items="ViewBag.HeroeId"></select>
<span asp-validation-for="SuperPoderes.HeroeId" class="text-danger"></span>
```

El usuario ve Superman o Batman; lo que se envía al servidor es su Id.

### 10.2. Evitar que se exija enviar el objeto Heroe completo

El formulario envía `HeroeId`, pero no el objeto de navegación `Heroe`. Para que la validación no exija ese objeto, abra **`Models/SuperPoderes.cs`** y agregue este using:

```csharp
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
```

Localice la propiedad de navegación generada y agregue `[ValidateNever]`, conservando los demás atributos que tenga:

```csharp
[ValidateNever]
public virtual Heroes Heroe { get; set; } = null!;
```

No aplique ese atributo a `HeroeId` ni a toda la entidad. Este ajuste excluye únicamente la navegación de la validación del formulario; la relación obligatoria y la clave foránea siguen existiendo.

Este cambio es manual sobre un archivo generado: si repite la ingeniería inversa con `-f`, deberá reaplicarlo. Para esta práctica inicial se muestra directamente sobre la propiedad para que el estudiante identifique su función.

### 10.3. Validar el héroe seleccionado y recargar el selector

En **Create.cshtml.cs y Edit.cshtml.cs de poderes**, dentro de `OnPostAsync`, reemplace el bloque inicial que comprueba `ModelState.IsValid` por:

```csharp
if (!await _context.Heroes.AnyAsync(h => h.Id == SuperPoderes.HeroeId))
{
    ModelState.AddModelError("SuperPoderes.HeroeId", "Seleccione un héroe válido.");
}

if (!ModelState.IsValid)
{
    ViewData["HeroeId"] = new SelectList(
        _context.Heroes, "Id", "Nombre", SuperPoderes.HeroeId);
    return Page();
}
```

Conserve el resto del handler generado, incluido el guardado y el tratamiento de concurrencia de Edit. Verifique que el archivo tenga los using de `Microsoft.EntityFrameworkCore` y `Microsoft.AspNetCore.Mvc.Rendering`, normalmente incluidos por el generador.

Así se comprueba que el héroe existe y se reconstruye el selector si el formulario contiene errores. Cree primero un héroe si no hay ninguno disponible.

### 10.4. Mostrar el nombre en las consultas

En `Index.cshtml`, `Details.cshtml` y `Delete.cshtml` de poderes, revise cómo se muestra la navegación. Si aparece `Heroe.Id`, cambie solamente esa propiedad por `Heroe.Nombre`. Por ejemplo, en el listado:

```csharp
@Html.DisplayFor(modelItem => item.Heroe.Nombre)
```

Conserve el resto de la expresión que corresponda a cada página. Compruebe que las consultas generadas carguen la navegación mediante `.Include(s => s.Heroe)`.

## 11. Añadir enlaces al menú

Abra `Pages/Shared/_Layout.cshtml`. Dentro de la lista `<ul>` del menú, junto a los enlaces existentes, agregue:

```html
<li class="nav-item">
    <a class="nav-link text-dark" asp-page="/Heroes/Index">Héroes</a>
</li>
<li class="nav-item">
    <a class="nav-link text-dark" asp-page="/SuperPoderes/Index">Superpoderes</a>
</li>
```

En `Pages/Heroes/Delete.cshtml`, agregue antes del formulario de confirmación:

```html
<p class="text-danger">
    Al eliminar este héroe también se eliminarán todos sus superpoderes.
</p>
```

Esta explicación refleja el comportamiento real de la base de datos.

## 12. Ejecutar la aplicación

```powershell
dotnet build
dotnet dev-certs https --trust
dotnet run --launch-profile https
```

Acepte el certificado de desarrollo cuando Windows lo solicite. Abra la dirección HTTPS mostrada en `Now listening on:`. El puerto varía según el proyecto.

Acceda a los menús Héroes y Superpoderes. Para detener la aplicación, use `Ctrl+C`.

## 13. Pruebas guiadas

| Paso | Acción | Resultado esperado |
|---|---|---|
| 1 | Abrir Héroes | Aparecen Superman y Batman |
| 2 | Abrir Superpoderes | Aparecen cinco poderes con el nombre de su héroe |
| 3 | Crear Flash, ciudad Central City, identidad Barry Allen | Se genera su Id automáticamente |
| 4 | Crear Supervelocidad y seleccionar Flash | Se guarda con el HeroeId de Flash |
| 5 | Consultar los detalles del poder | Aparecen sus datos y Flash |
| 6 | Editar su descripción | Se conserva el cambio |
| 7 | Editar el poder y asignarlo temporalmente a Superman | Cambia la asociación |
| 8 | Volver a asignarlo a Flash | Se restablece la asociación |
| 9 | Intentar crear un poder sin nombre | No se guarda y el selector sigue disponible |
| 10 | Crear y eliminar otro poder de prueba | Solo desaparece ese poder; su héroe permanece |
| 11 | Eliminar Flash y confirmar | Desaparecen Flash y Supervelocidad por la cascada |
| 12 | Reiniciar la aplicación | Los datos y cambios se conservan |

Compruebe también que identidad secreta y descripción se pueden dejar vacías. Para la prueba de cascada utilice el héroe Flash creado en la actividad, de modo que conserve los datos iniciales.

En SSMS ejecute:

```sql
USE HeroesDb;
GO
SELECT h.Id AS HeroeId, h.Nombre AS Heroe,
       p.Id AS PoderId, p.Nombre AS SuperPoder
FROM dbo.Heroes AS h
LEFT JOIN dbo.SuperPoderes AS p ON p.HeroeId = h.Id
ORDER BY h.Id, p.Id;
```

El `LEFT JOIN` también permite ver héroes que todavía no tienen poderes.

## 14. Comprender y entregar

Localice en el código generado: `OnGetAsync`, `OnPostAsync`, `[BindProperty]`, `ModelState.IsValid`, `SaveChangesAsync`, `asp-for`, `asp-route-id` e `Include`.

Responda:

1. ¿Qué significa Database First?
2. ¿Qué genera `dotnet ef dbcontext scaffold` y qué genera `dotnet aspnet-codegenerator`?
3. ¿Qué diferencia hay entre `HeroeId` y `Heroe`?
4. ¿Por qué el selector muestra un nombre, pero guarda un número?
5. ¿Qué sucede con los poderes al eliminar un héroe? ¿Dónde se definió ese comportamiento?
6. ¿Por qué no se ejecutaron migraciones?
7. ¿Por qué abrir Delete no elimina el registro hasta confirmar el formulario?

Entregue un pdf con   las respuestas y capturas de ambos listados, del selector de héroes y de una validación. Puede excluir `bin` y `obj` del proyecto y pasar la ruta de github.

**Evaluación :** base y relación 20%; ingeniería inversa y conexión 25%; ambos CRUD 30%; validación y pruebas 15%; explicación y presentación 10%.

## Problemas frecuentes

| Problema | Revisión |
|---|---|
| No se reconoce dotnet | Instalar SDK y volver a abrir la terminal |
| No se encuentra un proyecto | Ubicarse en la carpeta de HeroesWeb.csproj |
| Error de conexión | Usar la instancia exacta de SSMS y comprobar servicio y permisos |
| La base ya existe | Continuar con la base original; no volver a ejecutar CREATE DATABASE |
| Falta Web.CodeGeneration.Design | Instalar Microsoft.VisualStudio.Web.CodeGeneration.Design en el proyecto |
| No existe HeroesContext | Ejecutar la ingeniería inversa antes de modificar Program.cs |
| No existe el modelo indicado | Comprobar que se utilizó --no-pluralize y revisar Models |
| El poder no se guarda y la navegación aparece como obligatoria | Revisar ValidateNever sobre Heroe y los mensajes de ModelState |
| El selector queda vacío al fallar la validación | Reconstruir SelectList antes de return Page() |
| Se ve el Id en lugar del nombre del héroe | Revisar el texto de SelectList y Heroe.Nombre en las vistas |
| Datos diferentes entre SSMS y la web | Comprobar instancia y nombre de base en ambas conexiones |

