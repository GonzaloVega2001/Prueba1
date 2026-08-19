# Commit;Systems — ASP.NET Core MVC

Versión del sitio en C# / ASP.NET Core MVC (.NET 8), con Modelo,
Vista y Controlador separados como pediste, y el formulario de
contacto enviando el correo desde el servidor (SMTP) en vez de
depender del cliente de correo del visitante.

**Importante:** este proyecto fue escrito a mano en este entorno,
que no tiene el SDK de .NET instalado — no pude compilarlo ni
correrlo acá para verificarlo. Está escrito siguiendo la estructura
y convenciones estándar de ASP.NET Core MVC, pero **corre `dotnet build`
en tu máquina antes de darlo por bueno** y avísame si sale algún error
para corregirlo.

## Estructura (Modelo — Vista — Controlador)

```
Controllers/
  HomeController.cs      ← Index (GET) y Contact (POST, envía el correo)
Models/
  ContactFormModel.cs     ← datos + validaciones del formulario
  SiteData.cs             ← registros (Service, ProcessStep, etc.) y el ViewModel
  ErrorViewModel.cs
Views/
  Shared/_Layout.cshtml   ← <head>, meta SEO, carga CSS/JS
  Home/Index.cshtml       ← toda la página, iterando el modelo
  Home/Error.cshtml
wwwroot/
  css/site.css            ← mismo diseño que la versión React
  js/site.js               ← menú móvil + animación de scroll
  robots.txt / sitemap.xml
```

## 1. Requisitos

- .NET 8 SDK: https://dotnet.microsoft.com/download

## 2. Correr en local

```bash
dotnet restore
dotnet run
```

Abre la URL que muestra la terminal (algo como `https://localhost:5001`).

## 3. Configurar el envío de correo (Resend, gratis)

El formulario usa el **SDK oficial de Resend para .NET** (paquete NuGet
`Resend`) — gratis hasta 100 correos/día, sin tarjeta de crédito.

### 3.1. Instalar el paquete

Antes de compilar, agrega la librería (esto también actualiza el
`.csproj` automáticamente con la versión correcta):

```bash
cd CommitSystemsMvc
dotnet add package Resend
```

### 3.2. Configurar tu API key

Los datos van en `appsettings.json`, sección `EmailSettings`:

```json
"EmailSettings": {
  "ResendApiKey": "TU_API_KEY_DE_RESEND",
  "FromAddress": "onboarding@resend.dev",
  "FromName": "Commit;Systems — Formulario web",
  "ToAddress": "gonzalovr2001@gmail.com"
}
```

Pasos:

1. Crea una cuenta gratis en https://resend.com (con tu correo, sin
   tarjeta).
2. En el panel, ve a **API Keys** → **Create API Key**. Dale un nombre
   como "commit-systems-web" y copia la clave que empieza con `re_`.
   Solo se muestra una vez.
3. Pégala en `ResendApiKey`.
4. **Sobre `ToAddress` (importante):** mientras el dominio `comsys.cl`
   no esté verificado en Resend, la cuenta está en modo de prueba y
   **solo permite enviar al mismo correo con el que te registraste**
   (`gonzalovr2001@gmail.com`). Enviar a cualquier otro correo (como
   `gvega@comsys.cl`) falla silenciosamente en ese modo — por eso
   `ToAddress` queda así por defecto.
5. **Sobre el remitente (`FromAddress`)**: mismo caso — mientras no
   verifiques un dominio propio, solo puedes enviar usando
   `onboarding@resend.dev` como remitente.
6. Para levantar ambas limitaciones (enviar a `gvega@comsys.cl` y que
   el remitente se vea como algo de `comsys.cl`), en Resend ve a
   **Domains** → **Add Domain**, escribe `comsys.cl`, y agrega los
   registros DNS (TXT/MX) que te muestre en el proveedor donde tengas
   contratado el dominio. Toma desde minutos hasta unas horas en
   verificarse. Una vez verificado, cambia `ToAddress` de vuelta a
   `gvega@comsys.cl` y `FromAddress` a algo como
   `formulario@comsys.cl`.

**Nunca subas `appsettings.json` con la API key real a un repositorio

público** — para producción, usa variables de entorno o `dotnet user-secrets`
en vez de dejarla en el archivo.

## 4. Editar el contenido del sitio

A diferencia de la versión React (que tenía un archivo `siteData.js`
separado), acá el contenido vive directamente en
`Controllers/HomeController.cs`, en el método `BuildViewModel` —
ahí están los servicios, el proceso, el stack y los datos de contacto,
como listas de objetos C#. Edita esos valores y el sitio se actualiza.

## 5. Publicar (deploy)

```bash
dotnet publish -c Release -o ./publish
```

Esto genera una carpeta lista para subir a un hosting con soporte
.NET — Azure App Service, un VPS con IIS/Kestrel, o similar.

## 6. SEO / Google Business Profile

Igual que en la versión React: los meta tags, datos estructurados
(`LocalBusiness`), `robots.txt` y `sitemap.xml` ya están en el
`_Layout.cshtml` y `wwwroot/`, apuntando a `commitsystems.cl` como
dominio de ejemplo — reemplázalo por tu dominio real cuando lo tengas.
Para aparecer en búsquedas como "desarrollador San Felipe" sigue
siendo indispensable crear tu **Google Business Profile** en
business.google.com (requiere tu propia verificación).
