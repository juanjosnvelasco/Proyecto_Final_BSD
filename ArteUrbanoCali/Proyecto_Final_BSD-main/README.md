# Galería de Arte Urbano de Cali

Aplicación de escritorio en **C# (WinForms, .NET 10)** conectada a **MySQL 8** para registrar y consultar obras de arte
urbano de Cali, sus artistas, ubicaciones, categorías, rutas y recursos multimedia (fotos, videos, audios,
modelos 3D y referencias de realidad aumentada).

Cali está llena de arte en sus paredes: murales que cuentan historias de barrio, grafitis que cambian de una semana
a otra, retratos, stencils, letras que se vuelven imagen. Casi todo eso vive en la calle y casi nada está registrado
en un solo lugar.

## Requisitos
- Windows 10/11, .NET SDK 10 (o Visual Studio con carga de trabajo de escritorio .NET)
- MySQL Server 8.0.16 o superior (necesario para las restricciones CHECK)

## Cómo ejecutar
1. Importar la base en MySQL, **en este orden** (carpeta `Database/`):
   1. `01_arte_urbano_cali.sql` → la base `arte_urbano_cali` completa (estructura y datos). Si ya tienes la base cargada, puedes omitirlo.
   2. `02_ajustes_y_consultas.sql` → **no crea tablas**: alinea los recursos con los archivos reales de `RecursosMultimedia/`, agrega el índice y los procedimientos almacenados, e incluye las consultas de ejemplo.
2. Ajustar la cadena de conexión en `ArteUrbanoCali/Datos/conexion.cs`:
   ```
   Server=localhost;Port=3306;Database=arte_urbano_cali;User ID=root;Password=root;CharSet=utf8mb4;GuidFormat=None;
   ```
3. Abrir `ArteUrbanoCali/ArteUrbanoCali.csproj` y ejecutar (F5), o desde consola: `dotnet run --project ArteUrbanoCali`.
4. Iniciar sesión con un usuario **Administrador** (gestiona información) o **Visitante** (solo consulta).
   Usuarios de prueba: `admin@arteurbanocali.test` y `visitante@arteurbanocali.test`.

## Estructura del proyecto (capas)
| Carpeta     | Contenido                                                                 |
|-------------|---------------------------------------------------------------------------|
| `Entidades` | Clases del dominio (Obra, Artista, RecursoMultimedia, Ruta, RutaObra…)     |
| `Datos`     | Acceso a MySQL, consultas **parametrizadas** (MySqlConnector)             |
| `Negocio`   | Validaciones, permisos, reglas de negocio y manejo de archivos multimedia |
| `UI`        | Formularios WinForms (sin lógica de negocio)                              |

## Módulos
- **Artistas / Categorías / Ubicaciones / Obras**: registrar, consultar, modificar y activar/desactivar.
- **Multimedia**: elegir el archivo del computador, copiarlo a `RecursosMultimedia/`, registrar su ruta relativa,
  asociarlo a una obra y verlo o reproducirlo. Imágenes JPG/PNG con vista previa; video, audio, modelo 3D y AR se abren
  con el programa predeterminado del equipo (las imágenes AVIF también).
- **Rutas**: crear rutas, agregar y retirar obras, definir y reordenar el recorrido (Subir/Bajar), tiempo de
  permanencia y recomendación por obra. Una ruta activa requiere al menos 2 obras; una obra no se repite en la ruta
  ni se repite el orden.
- **Consultas**: consulta de obra, obras por artista / categoría / ubicación, consulta de ruta, recursos de una obra y
  estadísticas (obras por comuna, obras por categoría, artistas con más de N obras, recursos por tipo).

## Carpeta de recursos multimedia
```
RecursosMultimedia/
  Imagenes/   *.jpg  *.png  *.avif
  Videos/     *.mp4
  Audios/     *.mp3
  Modelos3D/  *.glb
  AR/         obra_001/ (marcador.png, descriptor.json, LEEME.txt)
```
- Los archivos **no** se guardan en MySQL: solo su ruta **relativa** a `RecursosMultimedia` (ej. `Imagenes/arte1.jpg`).
- La aplicación localiza la carpeta junto al ejecutable o subiendo por las carpetas del proyecto, por lo que
  funciona en otro equipo sin cambiar rutas.
- Formatos controlados: IMAGEN (JPG, PNG), VIDEO (MP4), AUDIO (MP3), MODELO3D (GLB), AR (referencia a carpeta).
  Tamaño máximo por archivo: 200 MB. El formato se valida en la aplicación y con CHECK en la base de datos.
- Al "Quitar registro" de un recurso se borra solo la fila de la base de datos; el archivo se conserva.

## Cómo se guarda lo multimedia en la base
- `tipos_recursos(idTipoRecurso, nombreTipoRecurso, carpetaBase)`: IMAGEN → `Imagenes/`, VIDEO → `Videos/`, AUDIO → `Audios/`, MODELO3D → `Modelos3D/`, AR → `AR/`.
- `formatos_recursos(idFormato, extension, idTipoRecursoFK)`: JPG, PNG, MP4, MP3, GLB, AR. **La aplicación lee estos catálogos**, así que los tipos y formatos permitidos se controlan desde la base de datos.
- `recursos_multimedias(idRecurso, nombreRecurso, descripcionRecurso, rutaArchivo UNIQUE, tamanio (KB), duracion, fechaRegistroRecurso, idFormatoFK, idObraFK)`.
- `rutas` y `rutas_obras` (entidad asociativa con `ordenVisita`, `recomendacion`, `tiempoPermanencia`; `UNIQUE (idRutaFK, idObraFK)` y `UNIQUE (idRutaFK, ordenVisita)`).
- Vistas que usa la aplicación: `vw_ruta_detalle` (consulta de ruta). También existe `vw_detalle_obra`.

## Seguridad
- Contraseñas con PBKDF2-SHA256 (100 000 iteraciones y sal aleatoria); nunca en texto plano.
- Consultas parametrizadas en toda la capa de datos (sin concatenar texto del usuario).
- Los permisos se verifican en la capa de negocio (`Validador.ExigirAdministrador`), no solo en la interfaz.
