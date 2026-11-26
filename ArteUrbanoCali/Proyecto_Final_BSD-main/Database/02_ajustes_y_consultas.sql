-- =====================================================================
--  Galería de Arte Urbano de Cali — Ajustes sobre la base EXISTENTE (MySQL 8.0.16+)
--
--  NO crea tablas: trabaja sobre arte_urbano_cali tal como está en
--  01_arte_urbano_cali.sql (ejecutar ese archivo primero, o usar la base actual).
--
--  Qué hace:
--    1. Hace que los recursos multimedia apunten a los archivos que SÍ existen en la
--       carpeta RecursosMultimedia/ (antes apuntaban a nombres como obra_001.jpg que no
--       estaban en la carpeta) y corrige su tamaño en KB.
--    2. Agrega 2 recursos más (audio y video) con archivos que estaban sin registrar.
--    3. Agrega el índice, los 2 procedimientos almacenados que pide la rúbrica
--       (la base ya tiene las vistas vw_detalle_obra y vw_ruta_detalle).
--    4. Consultas de ejemplo (JOIN, agregaciones, INSERT/UPDATE/DELETE).
--  Se puede ejecutar más de una vez sin duplicar datos. Usa DELIMITER (cliente mysql o Workbench).
-- =====================================================================

USE arte_urbano_cali;

-- ---------------------------------------------------------------------
-- 1. Alinear los recursos existentes con los archivos reales de la carpeta
--    (ruta relativa, formato y tamaño en KB). 15 recursos en total.
-- ---------------------------------------------------------------------
-- Imágenes (10)
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/arte1.jpg', tamanio = 1018.22,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = 'b2864d4c-713d-5868-830b-0e59af1274ea';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/arte2.jpg', tamanio = 117.71,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = '8ed63588-4621-5249-86a9-6d483605dd85';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/arte3.jpg', tamanio = 188.21,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = 'b7401feb-9832-5eef-a31f-ca507601c36b';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/arte4.jpg', tamanio = 106.73,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = 'be41a145-cf85-582b-bae8-5aa60fa19c03';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/images.jpg', tamanio = 45.71,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = '7a562724-b980-5af1-bccf-794dced8c77f';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/graffitis-polemica-calle-quinta-cali.jpg', tamanio = 82.29,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = 'a021f8e4-f04c-5bbd-ad42-13b66966821a';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/dwse0k92zabodsc3nu4i.jpg', tamanio = 138.26,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = '869c5fcd-8ebd-5409-bc86-eeefe4a4741f';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/galeria291139-1280x640.jpg', tamanio = 276.07,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = 'bc257f1d-5c24-59b2-b80b-d90dc4b5ea71';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/54278254_1477662065700945_6471878629688082432_n.jpg', tamanio = 279.67,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'JPG')
 WHERE idRecurso = '046f7981-8229-5d22-858f-c899a78a8a0c';
UPDATE recursos_multimedias SET rutaArchivo = 'Imagenes/Graficalia_0.png', tamanio = 1097.66,
       idFormatoFK = (SELECT idFormato FROM formatos_recursos WHERE extension = 'PNG')
 WHERE idRecurso = '609ee357-53f9-5a51-a17f-4e1f64a99f5a';

-- Modelo 3D, videos, audio y realidad aumentada
UPDATE recursos_multimedias SET rutaArchivo = 'Modelos3D/obra_004.glb', tamanio = 956.59
 WHERE idRecurso = '2ecf420d-b6ee-57b5-acb6-1f1b4c81eb29';

UPDATE recursos_multimedias SET rutaArchivo = 'Videos/obra_005.mp4', tamanio = 2348.81
 WHERE idRecurso = '5da3061e-1fcf-5549-a8eb-e3b560379ce5';

UPDATE recursos_multimedias SET tamanio = 1841.15        -- Videos/obra_001.mp4 ya existía
 WHERE idRecurso = 'd3c61225-198d-5400-8fdd-1c2752c807c9';

UPDATE recursos_multimedias SET tamanio = 1641.27        -- Audios/obra_002.mp3 ya existía
 WHERE idRecurso = '0512d504-4c8c-5fa9-b4eb-9fc14c863830';

-- La carpeta AR/obra_001 trae el descriptor de "Raíces del Pacífico", así que la referencia AR se asocia a esa obra.
UPDATE recursos_multimedias
   SET rutaArchivo = 'AR/obra_001', tamanio = 2.50,
       nombreRecurso = 'Raíces del Pacífico - AR',
       descripcionRecurso = 'Referencia a la carpeta de realidad aumentada (marcador y descriptor).',
       idObraFK = '7cf336cc-913c-5ce9-8c91-c4f42954f1f4'
 WHERE idRecurso = '726aa9bc-50a3-522b-a1fd-8598805d0b85';

-- ---------------------------------------------------------------------
-- 2. Dos recursos más con archivos que estaban en la carpeta sin registrar
-- ---------------------------------------------------------------------
INSERT INTO recursos_multimedias (idRecurso, nombreRecurso, descripcionRecurso, rutaArchivo, tamanio, duracion, idFormatoFK, idObraFK)
SELECT UUID(), 'Audioguía de la ladera', 'Audioguía de la obra.', 'Audios/obra_008.mp3', 938.00, NULL, f.idFormato,
       '9cc752bb-b09a-5fc3-8302-974a394f8896'
  FROM formatos_recursos f
 WHERE f.extension = 'MP3'
   AND NOT EXISTS (SELECT 1 FROM recursos_multimedias WHERE rutaArchivo = 'Audios/obra_008.mp3');

INSERT INTO recursos_multimedias (idRecurso, nombreRecurso, descripcionRecurso, rutaArchivo, tamanio, duracion, idFormatoFK, idObraFK)
SELECT UUID(), 'Documental del río', 'Video documental del mural.', 'Videos/video1.mp4', 61470.08, NULL, f.idFormato,
       'cc9b286b-43dd-5f53-bfbb-55f8adb3e5d4'
  FROM formatos_recursos f
 WHERE f.extension = 'MP4'
   AND NOT EXISTS (SELECT 1 FROM recursos_multimedias WHERE rutaArchivo = 'Videos/video1.mp4');

-- ---------------------------------------------------------------------
-- 3. Índice justificado
--    La aplicación busca y ordena obras por nombre (módulo Obras y Consultas);
--    el índice evita recorrer toda la tabla en esas búsquedas.
-- ---------------------------------------------------------------------
SET @existe = (SELECT COUNT(*) FROM information_schema.statistics
                WHERE table_schema = DATABASE() AND table_name = 'obras' AND index_name = 'idx_obras_nombre');
SET @sql = IF(@existe = 0, 'CREATE INDEX idx_obras_nombre ON obras (nombreObra)', 'SELECT ''indice ya existe''');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------
-- 4. Procedimientos almacenados (usan la vista vw_ruta_detalle de la base)
-- ---------------------------------------------------------------------
DROP PROCEDURE IF EXISTS sp_ruta_detalle;
DROP PROCEDURE IF EXISTS sp_artistas_con_mas_de;

DELIMITER $$

-- Obras de una ruta en orden de recorrido, con su ubicación
CREATE PROCEDURE sp_ruta_detalle(IN p_idRuta CHAR(36))
BEGIN
    SELECT nombreRuta, ordenVisita, nombreObra, nombreLugar, direccion,
           nombreBarrio, numeroComuna, tiempoPermanencia, recomendacion
      FROM vw_ruta_detalle
     WHERE idRuta = p_idRuta
     ORDER BY ordenVisita;
END$$

-- Artistas con más de N obras
CREATE PROCEDURE sp_artistas_con_mas_de(IN p_n INT)
BEGIN
    SELECT a.nombreArtistico, COUNT(o.idObra) AS totalObras
      FROM artistas a
      JOIN obras o ON o.idArtistaFK = a.idArtista
     GROUP BY a.idArtista, a.nombreArtistico
    HAVING COUNT(o.idObra) > p_n
     ORDER BY totalObras DESC;
END$$

DELIMITER ;

-- ---------------------------------------------------------------------
-- 5. Consultas de ejemplo
-- ---------------------------------------------------------------------

-- 5.1 Vistas existentes
SELECT * FROM vw_detalle_obra ORDER BY nombreObra;
SELECT * FROM vw_ruta_detalle ORDER BY nombreRuta, ordenVisita;

-- 5.2 JOIN múltiple: obra con sus recursos multimedia (tipo y formato vienen de las tablas de catálogo)
SELECT o.nombreObra, a.nombreArtistico, c.nombreCategoria, u.nombreLugar,
       t.nombreTipoRecurso, r.nombreRecurso, f.extension, r.rutaArchivo
  FROM obras o
  JOIN artistas a    ON a.idArtista   = o.idArtistaFK
  JOIN categorias c  ON c.idCategoria = o.idCategoriaFK
  JOIN ubicaciones u ON u.idUbicacion = o.idUbicacionFK
  LEFT JOIN recursos_multimedias r ON r.idObraFK = o.idObra
  LEFT JOIN formatos_recursos f    ON f.idFormato = r.idFormatoFK
  LEFT JOIN tipos_recursos t       ON t.idTipoRecurso = f.idTipoRecursoFK
 ORDER BY o.nombreObra, t.nombreTipoRecurso;

-- 5.3 Agregación: obras por comuna
SELECT cm.numeroComuna, COUNT(o.idObra) AS totalObras
  FROM comunas cm
  JOIN barrios b      ON b.idComunaFK = cm.idComuna
  JOIN ubicaciones u  ON u.idBarrioFK = b.idBarrio
  JOIN obras o        ON o.idUbicacionFK = u.idUbicacion
 GROUP BY cm.numeroComuna
 ORDER BY totalObras DESC;

-- 5.4 Agregación: obras por categoría
SELECT c.nombreCategoria, COUNT(o.idObra) AS totalObras
  FROM categorias c
  LEFT JOIN obras o ON o.idCategoriaFK = c.idCategoria
 GROUP BY c.idCategoria, c.nombreCategoria
 ORDER BY totalObras DESC;

-- 5.5 Agregación con HAVING: artistas con más de 1 obra
SELECT a.nombreArtistico, COUNT(o.idObra) AS totalObras
  FROM artistas a
  JOIN obras o ON o.idArtistaFK = a.idArtista
 GROUP BY a.idArtista, a.nombreArtistico
HAVING COUNT(o.idObra) > 1;

-- 5.6 Recursos por tipo
SELECT t.nombreTipoRecurso, COUNT(r.idRecurso) AS totalRecursos, ROUND(COALESCE(SUM(r.tamanio), 0) / 1024, 2) AS totalMB
  FROM tipos_recursos t
  LEFT JOIN formatos_recursos f    ON f.idTipoRecursoFK = t.idTipoRecurso
  LEFT JOIN recursos_multimedias r ON r.idFormatoFK = f.idFormato
 GROUP BY t.idTipoRecurso, t.nombreTipoRecurso
 ORDER BY totalRecursos DESC;

-- 5.7 Procedimientos almacenados
CALL sp_ruta_detalle('a95dbaca-8575-5226-bb7b-88f1ff1ab7a5');   -- Ruta del Centro Histórico
CALL sp_artistas_con_mas_de(1);

-- 5.8 INSERT / UPDATE / DELETE de ejemplo (sobre un recurso temporal)
INSERT INTO recursos_multimedias (idRecurso, nombreRecurso, rutaArchivo, tamanio, idFormatoFK, idObraFK)
SELECT UUID(), 'Recurso temporal', 'Imagenes/temporal.jpg', 10.00, f.idFormato, '7cf336cc-913c-5ce9-8c91-c4f42954f1f4'
  FROM formatos_recursos f WHERE f.extension = 'JPG';

UPDATE recursos_multimedias SET descripcionRecurso = 'Registro de prueba actualizado'
 WHERE rutaArchivo = 'Imagenes/temporal.jpg';

DELETE FROM recursos_multimedias WHERE rutaArchivo = 'Imagenes/temporal.jpg';
