CREATE DATABASE  IF NOT EXISTS `arte_urbano_cali` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `arte_urbano_cali`;
-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: arte_urbano_cali
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `artistas`
--

DROP TABLE IF EXISTS `artistas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `artistas` (
  `idArtista` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreArtistico` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombre1Real` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombre2Real` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `apellido1Real` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `apellido2Real` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `biografia` text COLLATE utf8mb4_unicode_ci,
  `perfilRedSocial` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `estadoArtista` tinyint(1) NOT NULL DEFAULT '1',
  `idPaisFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idArtista`),
  UNIQUE KEY `uq_artistas_nombreartistico` (`nombreArtistico`),
  KEY `fk_artistas_pais` (`idPaisFK`),
  CONSTRAINT `fk_artistas_pais` FOREIGN KEY (`idPaisFK`) REFERENCES `paises` (`idPais`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `artistas`
--

LOCK TABLES `artistas` WRITE;
/*!40000 ALTER TABLE `artistas` DISABLE KEYS */;
INSERT INTO `artistas` VALUES ('0c1ebb1a-0f52-5e69-92cd-bb9d9f75bb5a','Colibrí Urbano','Santiago',NULL,'Ortega','Mejía','Combina grafiti y tipografía inspirado en el arte callejero mexicano.','@colibri.urbano',1,'caefa889-0731-52c1-bb91-6e6b1ef8fb10'),('40c469a2-bcab-5ffc-ae3f-02eb9917817e','Pared Viva','Lucía','Fernanda','Paz','Herrera','Especialista en stencil y personajes de animales urbanos.','@paredviva',1,'8268b211-04fa-5491-80c9-ef583fab77c6'),('564219ed-038a-55eb-99bb-5cf0d3d2d81a','Sombra Azul','Mateo',NULL,'Aranda',NULL,'Grafitero de laderas; actualmente inactivo en el circuito.',NULL,0,'ac36682a-7564-509b-bbf5-1dbaefb7d8ba'),('cbb74791-a2f2-5c67-b988-f5443e57767d','Eco Gris','Inés','Paula','Navarro','Sala','Letrista y diseñadora gráfica, invitada a intervenir fachadas del sur.','@ecogris',1,'c463d203-23a3-5bcc-98fd-c395c180358f'),('cfabb243-7cf4-59d5-9cf7-3547b0047fef','Tinta Roja','Valentina',NULL,'Ríos',NULL,'Artista autodidacta, trabaja el color y la música popular en gran formato.','@tintaroja.art',1,'e1e2583c-00e7-50da-9a28-d7e7ac0d243b'),('e5bd92ae-51c6-58bd-85c5-e2b4cffcee60','Nómada','Camilo','Andrés','Restrepo','Lozano','Muralista caleño que retrata la identidad del Pacífico colombiano.','@nomada.cali',1,'e1e2583c-00e7-50da-9a28-d7e7ac0d243b');
/*!40000 ALTER TABLE `artistas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `barrios`
--

DROP TABLE IF EXISTS `barrios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `barrios` (
  `idBarrio` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreBarrio` varchar(80) COLLATE utf8mb4_unicode_ci NOT NULL,
  `idComunaFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idBarrio`),
  UNIQUE KEY `uq_barrios_nombre_comuna` (`nombreBarrio`,`idComunaFK`),
  KEY `fk_barrios_comuna` (`idComunaFK`),
  CONSTRAINT `fk_barrios_comuna` FOREIGN KEY (`idComunaFK`) REFERENCES `comunas` (`idComuna`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `barrios`
--

LOCK TABLES `barrios` WRITE;
/*!40000 ALTER TABLE `barrios` DISABLE KEYS */;
INSERT INTO `barrios` VALUES ('bf8f4dc6-1a2e-56d6-aefe-1e964bb6ac2f','Ciudad Jardín','1350d37a-9e2e-56ed-bcee-987dcdf94d30'),('6cfe94c7-c4fc-55c8-9c2e-ab01235d789e','Cristo Rey','eed94f28-c00b-537c-8431-b59f0e5fee4a'),('63407f59-57aa-521d-a072-75657854423f','El Calvario','35b5a8c7-eafa-5aeb-9112-fc0d10947549'),('392973ad-5d7b-5de4-bc6b-7fda9a0d4943','El Peñón','80cac1e3-d961-567f-8f4a-2ba6e5aadf3d'),('f4ad0b44-9470-54aa-8e09-f911906c09a0','Granada','f0e9efaa-c606-5ddb-8330-e09f1cecefc5'),('8a21c980-9e25-5943-b86d-2353667e61d1','San Antonio','35b5a8c7-eafa-5aeb-9112-fc0d10947549'),('62388070-251a-5b9b-bd48-5761321f49f5','San Fernando','80cac1e3-d961-567f-8f4a-2ba6e5aadf3d'),('e04908eb-f718-5b02-b3d0-b900659333b2','San Pedro','35b5a8c7-eafa-5aeb-9112-fc0d10947549'),('f220a5df-6b80-577a-87d4-903aafc20eaf','Siloé','eed94f28-c00b-537c-8431-b59f0e5fee4a'),('d41c3ceb-777b-5852-a9cd-c46b708b58f3','Versalles','f0e9efaa-c606-5ddb-8330-e09f1cecefc5');
/*!40000 ALTER TABLE `barrios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categorias`
--

DROP TABLE IF EXISTS `categorias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categorias` (
  `idCategoria` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreCategoria` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `descripcionCategoria` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `estadoCategoria` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`idCategoria`),
  UNIQUE KEY `uq_categorias_nombre` (`nombreCategoria`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categorias`
--

LOCK TABLES `categorias` WRITE;
/*!40000 ALTER TABLE `categorias` DISABLE KEYS */;
INSERT INTO `categorias` VALUES ('240d1dc2-70c1-5ea4-a1ce-8c198f5e1dac','Retrato','Rostros y figuras humanas del barrio.',1),('284eaf44-0a29-59ed-9e18-b8d164e58e88','Arte tipográfico','Letras y frases como elemento central.',1),('515b0e46-fc6b-5ee0-a33c-90e0c71f8c7b','Instalación','Intervenciones tridimensionales en el espacio público.',0),('61c96333-ea77-5c82-bc27-61882ae38143','Mural','Obras de gran formato pintadas sobre muros y fachadas.',1),('a0211609-bab8-5481-812c-2b0f7b42d9e3','Grafiti','Intervenciones con aerosol, firmas y personajes.',1),('b3db605b-c157-5444-b12b-01dcca7b89d5','Stencil','Imágenes hechas con plantillas recortadas.',1);
/*!40000 ALTER TABLE `categorias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `comunas`
--

DROP TABLE IF EXISTS `comunas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comunas` (
  `idComuna` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `numeroComuna` int NOT NULL,
  PRIMARY KEY (`idComuna`),
  UNIQUE KEY `uq_comunas_numero` (`numeroComuna`),
  CONSTRAINT `ck_comunas_numero` CHECK ((`numeroComuna` between 1 and 22))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comunas`
--

LOCK TABLES `comunas` WRITE;
/*!40000 ALTER TABLE `comunas` DISABLE KEYS */;
INSERT INTO `comunas` VALUES ('8076caa8-d667-5729-860e-7d932f536ad0',1),('f0e9efaa-c606-5ddb-8330-e09f1cecefc5',2),('35b5a8c7-eafa-5aeb-9112-fc0d10947549',3),('be6af2ec-0904-5529-9b53-5f3c552a3e9a',4),('a6b30273-e5c7-5e0c-a442-4e70adca788f',5),('abc3925b-8da8-5a5f-8c44-b78f62e80e79',6),('71139d61-75ca-56c8-829d-c6c4438a9b25',7),('0e106f63-6018-5353-8c51-98f8ed1fc62a',8),('db85b5be-7d81-5ef2-9143-59d45ffe5e4d',9),('52739aeb-3020-5c0f-9d10-1b24e5c00c74',10),('4bd0b7f9-6e47-549e-b2ae-b1d877e38be2',11),('0d202001-c23a-59d6-bc71-9cccba41fe94',12),('a3812d39-bb93-5ed7-bf5f-807d52737042',13),('7e76d00b-baff-5dfc-b874-c8b0f35ef195',14),('4ab84fb8-39a5-5e64-83e6-fece7fbaa319',15),('d226ab0d-868d-54f5-b177-d35f37782f27',16),('0246107e-5e99-5d49-a5e3-17fe9d251f60',17),('2c807010-ed5d-58df-a2f0-de8e71e67f82',18),('80cac1e3-d961-567f-8f4a-2ba6e5aadf3d',19),('eed94f28-c00b-537c-8431-b59f0e5fee4a',20),('09f8f2a3-40af-5221-88e6-76abad7c334f',21),('1350d37a-9e2e-56ed-bcee-987dcdf94d30',22);
/*!40000 ALTER TABLE `comunas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `formatos_recursos`
--

DROP TABLE IF EXISTS `formatos_recursos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `formatos_recursos` (
  `idFormato` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `extension` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `idTipoRecursoFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idFormato`),
  UNIQUE KEY `uq_formatos_extension` (`extension`),
  KEY `fk_formatos_tipo` (`idTipoRecursoFK`),
  CONSTRAINT `fk_formatos_tipo` FOREIGN KEY (`idTipoRecursoFK`) REFERENCES `tipos_recursos` (`idTipoRecurso`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `formatos_recursos`
--

LOCK TABLES `formatos_recursos` WRITE;
/*!40000 ALTER TABLE `formatos_recursos` DISABLE KEYS */;
INSERT INTO `formatos_recursos` VALUES ('2d851715-fd65-5151-8438-72c110cae34c','JPG','c9d11a2d-4c43-5988-b9e1-7ffbb52e430b'),('2f14602e-6520-5455-80ba-667afd4cfc8b','GLB','5f17070a-2ec0-536c-90a2-0341b52c9e75'),('3c95e9c4-b3b0-5dfb-b5f9-f88feb7b7970','MP3','f84d806b-dad3-5109-8acc-120c68a4d479'),('3e7e29d6-3402-58cf-a415-2c022696daf3','AR','eb633e47-d274-5851-a46c-1f16468b4dec'),('48f9b86d-04d3-5716-8011-065a5b03c59f','MP4','2d075659-26b1-5dec-a759-666100e8e544'),('96007b85-25f3-5b3e-81ca-a7ef7b99a0ef','PNG','c9d11a2d-4c43-5988-b9e1-7ffbb52e430b');
/*!40000 ALTER TABLE `formatos_recursos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `obras`
--

DROP TABLE IF EXISTS `obras`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `obras` (
  `idObra` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreObra` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `descripcionObra` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `fechaCreacion` date NOT NULL,
  `estadoConservacion` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `fechaRegistroObra` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `idArtistaFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `idCategoriaFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `idUbicacionFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `idUsuarioFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idObra`),
  KEY `fk_obras_artista` (`idArtistaFK`),
  KEY `fk_obras_categoria` (`idCategoriaFK`),
  KEY `fk_obras_ubicacion` (`idUbicacionFK`),
  KEY `fk_obras_usuario` (`idUsuarioFK`),
  CONSTRAINT `fk_obras_artista` FOREIGN KEY (`idArtistaFK`) REFERENCES `artistas` (`idArtista`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_obras_categoria` FOREIGN KEY (`idCategoriaFK`) REFERENCES `categorias` (`idCategoria`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_obras_ubicacion` FOREIGN KEY (`idUbicacionFK`) REFERENCES `ubicaciones` (`idUbicacion`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_obras_usuario` FOREIGN KEY (`idUsuarioFK`) REFERENCES `usuarios` (`idUsuario`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `ck_obras_estado` CHECK ((`estadoConservacion` in (_cp850'bueno',_cp850'regular',_cp850'deteriorado')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `obras`
--

LOCK TABLES `obras` WRITE;
/*!40000 ALTER TABLE `obras` DISABLE KEYS */;
INSERT INTO `obras` VALUES ('19f38195-8b1e-50d8-b45c-45a5d0a233fd','Letras del Jardín','Palabras en letra cursiva sobre una fachada del sur.','2023-12-01','bueno','2026-11-15 23:37:54','cbb74791-a2f2-5c67-b988-f5443e57767d','284eaf44-0a29-59ed-9e18-b8d164e58e88','2044e229-0958-549d-b711-a2d98baf110a','18c962bb-b12a-5bdb-bd73-7bd8d7244d08'),('1ad40104-e089-549c-ba84-62594b2718f1','Mirada del Cerro','Retrato en gran formato frente al cerro.','2020-02-27','regular','2026-11-15 23:37:54','e5bd92ae-51c6-58bd-85c5-e2b4cffcee60','240d1dc2-70c1-5ea4-a1ce-8c198f5e1dac','d95561e3-f297-57ec-a7af-ab8c8592bf22','be963830-5544-58a4-99c4-65abbd5cd0f0'),('2b93a2d9-cdb6-5d19-b264-0fa1b009fe0e','Salsa en la Pared','Homenaje a la salsa caleña con instrumentos y bailarines.','2020-08-22','regular','2026-11-15 23:37:54','cfabb243-7cf4-59d5-9cf7-3547b0047fef','61c96333-ea77-5c82-bc27-61882ae38143','46677b6d-8275-57c9-926b-5d7df8052c4e','be963830-5544-58a4-99c4-65abbd5cd0f0'),('3778a553-1e03-597e-8638-3c2622ae278e','Rostro de Barrio','Retrato de una vecina del barrio.','2022-01-10','bueno','2026-11-15 23:37:54','e5bd92ae-51c6-58bd-85c5-e2b4cffcee60','240d1dc2-70c1-5ea4-a1ce-8c198f5e1dac','031f4640-92ba-5386-b140-42622387724e','be963830-5544-58a4-99c4-65abbd5cd0f0'),('48aa7894-2469-5f05-8355-f3999fa47cd8','Gato Guardián','Stencil de un gato que vigila la esquina.','2023-05-18','bueno','2026-11-15 23:37:54','40c469a2-bcab-5ffc-ae3f-02eb9917817e','b3db605b-c157-5444-b12b-01dcca7b89d5','eba24c3e-0692-5990-9a64-84c6ff5dfb47','be963830-5544-58a4-99c4-65abbd5cd0f0'),('55328064-c5c0-566f-b118-5ade7f12f676','Palabras al Viento','Frase tipográfica sobre la memoria de la ciudad.','2019-11-05','deteriorado','2026-11-15 23:37:54','0c1ebb1a-0f52-5e69-92cd-bb9d9f75bb5a','284eaf44-0a29-59ed-9e18-b8d164e58e88','f29b90e6-ffb8-52b5-9793-ce8e85fe4ea1','18c962bb-b12a-5bdb-bd73-7bd8d7244d08'),('6054fcf9-dee2-5df1-8f59-972b58baf45d','Lenguaje de Esquina','Composición de grafiti con firmas y personajes.','2022-10-02','bueno','2026-11-15 23:37:54','0c1ebb1a-0f52-5e69-92cd-bb9d9f75bb5a','a0211609-bab8-5481-812c-2b0f7b42d9e3','3e85858a-a338-5016-b3d7-0db16e9f548f','18c962bb-b12a-5bdb-bd73-7bd8d7244d08'),('7cf336cc-913c-5ce9-8c91-c4f42954f1f4','Raíces del Pacífico','Mural sobre las raíces afro del Pacífico colombiano.','2021-03-14','bueno','2026-11-15 23:37:54','e5bd92ae-51c6-58bd-85c5-e2b4cffcee60','61c96333-ea77-5c82-bc27-61882ae38143','0f6ce8ce-d1f9-55e8-9766-0a61a1e747e1','be963830-5544-58a4-99c4-65abbd5cd0f0'),('9cc752bb-b09a-5fc3-8302-974a394f8896','Memoria de la Loma','Grafiti con figuras de la ladera de Siloé.','2018-06-12','regular','2026-11-15 23:37:54','564219ed-038a-55eb-99bb-5cf0d3d2d81a','a0211609-bab8-5481-812c-2b0f7b42d9e3','1447a9d8-d272-5dc4-bc32-07d8254f93dd','be963830-5544-58a4-99c4-65abbd5cd0f0'),('cc9b286b-43dd-5f53-bfbb-55f8adb3e5d4','Río de Colores','Mural inspirado en el río Cali y su vegetación.','2021-09-30','bueno','2026-11-15 23:37:54','cfabb243-7cf4-59d5-9cf7-3547b0047fef','61c96333-ea77-5c82-bc27-61882ae38143','4612d7e0-e643-5f95-a140-1c92b55ad0c4','18c962bb-b12a-5bdb-bd73-7bd8d7244d08');
/*!40000 ALTER TABLE `obras` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `paises`
--

DROP TABLE IF EXISTS `paises`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `paises` (
  `idPais` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombrePais` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idPais`),
  UNIQUE KEY `uq_paises_nombre` (`nombrePais`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `paises`
--

LOCK TABLES `paises` WRITE;
/*!40000 ALTER TABLE `paises` DISABLE KEYS */;
INSERT INTO `paises` VALUES ('8268b211-04fa-5491-80c9-ef583fab77c6','Argentina'),('235a926f-b307-5932-a085-5c09209968f3','Brasil'),('ac36682a-7564-509b-bbf5-1dbaefb7d8ba','Chile'),('e1e2583c-00e7-50da-9a28-d7e7ac0d243b','Colombia'),('c463d203-23a3-5bcc-98fd-c395c180358f','España'),('caefa889-0731-52c1-bb91-6e6b1ef8fb10','México');
/*!40000 ALTER TABLE `paises` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recursos_multimedias`
--

DROP TABLE IF EXISTS `recursos_multimedias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recursos_multimedias` (
  `idRecurso` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreRecurso` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `descripcionRecurso` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rutaArchivo` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ruta relativa a RecursosMultimedia/',
  `tamanio` decimal(12,2) NOT NULL COMMENT 'KB',
  `duracion` int DEFAULT NULL COMMENT 'segundos; solo video y audio',
  `fechaRegistroRecurso` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `idFormatoFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `idObraFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idRecurso`),
  UNIQUE KEY `uq_recursos_ruta` (`rutaArchivo`),
  KEY `fk_recursos_formato` (`idFormatoFK`),
  KEY `fk_recursos_obra` (`idObraFK`),
  CONSTRAINT `fk_recursos_formato` FOREIGN KEY (`idFormatoFK`) REFERENCES `formatos_recursos` (`idFormato`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_recursos_obra` FOREIGN KEY (`idObraFK`) REFERENCES `obras` (`idObra`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `ck_recursos_duracion` CHECK (((`duracion` is null) or (`duracion` > 0))),
  CONSTRAINT `ck_recursos_ruta_relativa` CHECK (((not(regexp_like(`rutaArchivo`,_cp850'^([A-Za-z]:|/)'))) and (locate(char(92),`rutaArchivo`) = 0))),
  CONSTRAINT `ck_recursos_tamanio` CHECK ((`tamanio` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recursos_multimedias`
--

LOCK TABLES `recursos_multimedias` WRITE;
/*!40000 ALTER TABLE `recursos_multimedias` DISABLE KEYS */;
INSERT INTO `recursos_multimedias` VALUES ('046f7981-8229-5d22-858f-c899a78a8a0c','Letras del Jardín - foto','Fotografía de la fachada.','Imagenes/obra_010.jpg',1875.30,NULL,'2026-11-15 23:37:54','2d851715-fd65-5151-8438-72c110cae34c','19f38195-8b1e-50d8-b45c-45a5d0a233fd'),('0512d504-4c8c-5fa9-b4eb-9fc14c863830','Entrevista a la artista','Audio con la artista.','Audios/obra_002.mp3',4300.00,210,'2026-11-15 23:37:54','3c95e9c4-b3b0-5dfb-b5f9-f88feb7b7970','2b93a2d9-cdb6-5d19-b264-0fa1b009fe0e'),('2ecf420d-b6ee-57b5-acb6-1f1b4c81eb29','Rostro de Barrio - modelo 3D','Modelo 3D del retrato.','Modelos3D/obra_003.glb',8900.00,NULL,'2026-11-15 23:37:54','2f14602e-6520-5455-80ba-667afd4cfc8b','3778a553-1e03-597e-8638-3c2622ae278e'),('5da3061e-1fcf-5549-a8eb-e3b560379ce5','Río de Colores - recorrido','Video recorriendo el mural.','Videos/obra_006.mp4',15400.00,60,'2026-11-15 23:37:54','48f9b86d-04d3-5716-8011-065a5b03c59f','cc9b286b-43dd-5f53-bfbb-55f8adb3e5d4'),('609ee357-53f9-5a51-a17f-4e1f64a99f5a','Palabras al Viento - foto','Estado actual de la obra.','Imagenes/obra_004.png',2750.00,NULL,'2026-11-15 23:37:54','96007b85-25f3-5b3e-81ca-a7ef7b99a0ef','55328064-c5c0-566f-b118-5ade7f12f676'),('726aa9bc-50a3-522b-a1fd-8598805d0b85','Gato Guardián - AR','Referencia a la carpeta de realidad aumentada.','AR/obra_005/',5200.00,NULL,'2026-11-15 23:37:54','3e7e29d6-3402-58cf-a415-2c022696daf3','48aa7894-2469-5f05-8355-f3999fa47cd8'),('7a562724-b980-5af1-bccf-794dced8c77f','Mirada del Cerro - foto','Fotografía del retrato.','Imagenes/obra_009.jpg',2330.90,NULL,'2026-11-15 23:37:54','2d851715-fd65-5151-8438-72c110cae34c','1ad40104-e089-549c-ba84-62594b2718f1'),('869c5fcd-8ebd-5409-bc86-eeefe4a4741f','Memoria de la Loma - foto','Foto del grafiti.','Imagenes/obra_007.jpg',2010.00,NULL,'2026-11-15 23:37:54','2d851715-fd65-5151-8438-72c110cae34c','9cc752bb-b09a-5fc3-8302-974a394f8896'),('8ed63588-4621-5249-86a9-6d483605dd85','Detalle del mural','Detalle de la pintura.','Imagenes/obra_001_detalle.png',3120.00,NULL,'2026-11-15 23:37:54','96007b85-25f3-5b3e-81ca-a7ef7b99a0ef','7cf336cc-913c-5ce9-8c91-c4f42954f1f4'),('a021f8e4-f04c-5bbd-ad42-13b66966821a','Gato Guardián - foto','Fotografía del stencil.','Imagenes/obra_005.jpg',1640.20,NULL,'2026-11-15 23:37:54','2d851715-fd65-5151-8438-72c110cae34c','48aa7894-2469-5f05-8355-f3999fa47cd8'),('b2864d4c-713d-5868-830b-0e59af1274ea','Mural frontal','Vista general del mural.','Imagenes/obra_001.jpg',2450.50,NULL,'2026-11-15 23:37:54','2d851715-fd65-5151-8438-72c110cae34c','7cf336cc-913c-5ce9-8c91-c4f42954f1f4'),('b7401feb-9832-5eef-a31f-ca507601c36b','Salsa en la Pared - foto','Foto de la obra completa.','Imagenes/obra_002.jpg',2210.75,NULL,'2026-11-15 23:37:54','2d851715-fd65-5151-8438-72c110cae34c','2b93a2d9-cdb6-5d19-b264-0fa1b009fe0e'),('bc257f1d-5c24-59b2-b80b-d90dc4b5ea71','Río de Colores - foto','Vista general del mural.','Imagenes/obra_006.jpg',2890.60,NULL,'2026-11-15 23:37:54','2d851715-fd65-5151-8438-72c110cae34c','cc9b286b-43dd-5f53-bfbb-55f8adb3e5d4'),('be41a145-cf85-582b-bae8-5aa60fa19c03','Rostro de Barrio - foto','Fotografía del retrato.','Imagenes/obra_003.jpg',1980.40,NULL,'2026-11-15 23:37:54','2d851715-fd65-5151-8438-72c110cae34c','3778a553-1e03-597e-8638-3c2622ae278e'),('d3c61225-198d-5400-8fdd-1c2752c807c9','Proceso de pintura','Video corto del proceso.','Videos/obra_001.mp4',18500.00,95,'2026-11-15 23:37:54','48f9b86d-04d3-5716-8011-065a5b03c59f','7cf336cc-913c-5ce9-8c91-c4f42954f1f4');
/*!40000 ALTER TABLE `recursos_multimedias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rutas`
--

DROP TABLE IF EXISTS `rutas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rutas` (
  `idRuta` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreRuta` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `descripcionRuta` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `duracionEstimada` int NOT NULL COMMENT 'minutos',
  `distanciaEstimada` decimal(6,2) NOT NULL COMMENT 'km',
  `estadoRuta` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`idRuta`),
  UNIQUE KEY `uq_rutas_nombre` (`nombreRuta`),
  CONSTRAINT `ck_rutas_distancia` CHECK ((`distanciaEstimada` > 0)),
  CONSTRAINT `ck_rutas_duracion` CHECK ((`duracionEstimada` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rutas`
--

LOCK TABLES `rutas` WRITE;
/*!40000 ALTER TABLE `rutas` DISABLE KEYS */;
INSERT INTO `rutas` VALUES ('2b800ee5-d45f-5d68-9fd9-b23b886ebc56','Ruta Norte: Granada y Versalles','Murales del norte de la ciudad.',90,4.50,1),('6a7dd631-77c6-599a-b5aa-e4e672cc4287','Ruta de las Laderas','Arte en San Fernando y Siloé.',150,6.80,1),('a95dbaca-8575-5226-bb7b-88f1ff1ab7a5','Ruta del Centro Histórico','Recorrido por San Antonio, San Pedro y El Calvario.',120,3.20,1);
/*!40000 ALTER TABLE `rutas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rutas_obras`
--

DROP TABLE IF EXISTS `rutas_obras`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rutas_obras` (
  `idRutaObra` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ordenVisita` int NOT NULL,
  `recomendacion` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tiempoPermanencia` int NOT NULL COMMENT 'minutos',
  `idRutaFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `idObraFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idRutaObra`),
  UNIQUE KEY `uq_rutas_obras_ruta_obra` (`idRutaFK`,`idObraFK`),
  UNIQUE KEY `uq_rutas_obras_ruta_orden` (`idRutaFK`,`ordenVisita`),
  KEY `fk_rutas_obras_obra` (`idObraFK`),
  CONSTRAINT `fk_rutas_obras_obra` FOREIGN KEY (`idObraFK`) REFERENCES `obras` (`idObra`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_rutas_obras_ruta` FOREIGN KEY (`idRutaFK`) REFERENCES `rutas` (`idRuta`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `ck_rutas_obras_orden` CHECK ((`ordenVisita` > 0)),
  CONSTRAINT `ck_rutas_obras_tiempo` CHECK ((`tiempoPermanencia` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rutas_obras`
--

LOCK TABLES `rutas_obras` WRITE;
/*!40000 ALTER TABLE `rutas_obras` DISABLE KEYS */;
INSERT INTO `rutas_obras` VALUES ('27fbe7ef-7021-5a14-8504-80f415015d6e',3,'Obra delicada: observar sin tocar.',10,'a95dbaca-8575-5226-bb7b-88f1ff1ab7a5','55328064-c5c0-566f-b118-5ade7f12f676'),('3bf04935-04c8-5f85-bd57-4ef4631b5992',3,'Hay buen mirador a pocos metros.',25,'6a7dd631-77c6-599a-b5aa-e4e672cc4287','1ad40104-e089-549c-ba84-62594b2718f1'),('56924bf1-d43e-5672-8ee0-330625eb5ee2',1,'Empieza en el parque de San Fernando.',15,'6a7dd631-77c6-599a-b5aa-e4e672cc4287','6054fcf9-dee2-5df1-8f59-972b58baf45d'),('5b18f404-987a-5d11-8ae2-5bd4f2047d9d',2,'Subir con acompañante; terreno inclinado.',30,'6a7dd631-77c6-599a-b5aa-e4e672cc4287','9cc752bb-b09a-5fc3-8302-974a394f8896'),('5cef3157-5cc1-5b60-ad69-5484071bde6a',2,'Buen punto para fotografías de cerca.',15,'a95dbaca-8575-5226-bb7b-88f1ff1ab7a5','3778a553-1e03-597e-8638-3c2622ae278e'),('5e505488-fd59-5ee0-9ab6-918d084e0dc7',2,'Vale la pena ver el detalle de la vegetación.',20,'2b800ee5-d45f-5d68-9fd9-b23b886ebc56','cc9b286b-43dd-5f53-bfbb-55f8adb3e5d4'),('928848c9-2cdd-5d92-ab14-47e3bbd329ed',1,'Llegar temprano para ver la luz sobre el mural.',20,'a95dbaca-8575-5226-bb7b-88f1ff1ab7a5','2b93a2d9-cdb6-5d19-b264-0fa1b009fe0e'),('bc774db4-d4ca-5865-aafc-82aab666c5c5',1,'Mejor hora: media mañana.',20,'2b800ee5-d45f-5d68-9fd9-b23b886ebc56','7cf336cc-913c-5ce9-8c91-c4f42954f1f4'),('bf78c54e-2ecc-5389-a0f0-d4f73b141415',4,'Cierre del recorrido, ideal para descansar.',15,'a95dbaca-8575-5226-bb7b-88f1ff1ab7a5','48aa7894-2469-5f05-8355-f3999fa47cd8'),('f69d500b-4c88-5531-b46a-ea7af09a6d41',3,'Cerrar con un café en la plazoleta.',25,'2b800ee5-d45f-5d68-9fd9-b23b886ebc56','2b93a2d9-cdb6-5d19-b264-0fa1b009fe0e');
/*!40000 ALTER TABLE `rutas_obras` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipos_recursos`
--

DROP TABLE IF EXISTS `tipos_recursos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipos_recursos` (
  `idTipoRecurso` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreTipoRecurso` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `carpetaBase` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'carpeta dentro de RecursosMultimedia/',
  PRIMARY KEY (`idTipoRecurso`),
  UNIQUE KEY `uq_tipos_recursos_nombre` (`nombreTipoRecurso`),
  UNIQUE KEY `uq_tipos_recursos_carpeta` (`carpetaBase`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipos_recursos`
--

LOCK TABLES `tipos_recursos` WRITE;
/*!40000 ALTER TABLE `tipos_recursos` DISABLE KEYS */;
INSERT INTO `tipos_recursos` VALUES ('2d075659-26b1-5dec-a759-666100e8e544','VIDEO','Videos/'),('5f17070a-2ec0-536c-90a2-0341b52c9e75','MODELO3D','Modelos3D/'),('c9d11a2d-4c43-5988-b9e1-7ffbb52e430b','IMAGEN','Imagenes/'),('eb633e47-d274-5851-a46c-1f16468b4dec','AR','AR/'),('f84d806b-dad3-5109-8acc-120c68a4d479','AUDIO','Audios/');
/*!40000 ALTER TABLE `tipos_recursos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipos_usuarios`
--

DROP TABLE IF EXISTS `tipos_usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipos_usuarios` (
  `idTipoUsuario` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreTipoUsuario` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idTipoUsuario`),
  UNIQUE KEY `uq_tipos_usuarios_nombre` (`nombreTipoUsuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipos_usuarios`
--

LOCK TABLES `tipos_usuarios` WRITE;
/*!40000 ALTER TABLE `tipos_usuarios` DISABLE KEYS */;
INSERT INTO `tipos_usuarios` VALUES ('5f80ec92-3db5-5844-a90b-8aaa44eb5b75','Administrador'),('e056adb5-2474-57f1-b8b6-241b7548aded','Visitante');
/*!40000 ALTER TABLE `tipos_usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ubicaciones`
--

DROP TABLE IF EXISTS `ubicaciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ubicaciones` (
  `idUbicacion` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombreLugar` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `direccion` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `latitud` decimal(9,6) NOT NULL,
  `longitud` decimal(9,6) NOT NULL,
  `idBarrioFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idUbicacion`),
  KEY `fk_ubicaciones_barrio` (`idBarrioFK`),
  CONSTRAINT `fk_ubicaciones_barrio` FOREIGN KEY (`idBarrioFK`) REFERENCES `barrios` (`idBarrio`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `ck_ubicaciones_latitud` CHECK ((`latitud` between -(90) and 90)),
  CONSTRAINT `ck_ubicaciones_longitud` CHECK ((`longitud` between -(180) and 180))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ubicaciones`
--

LOCK TABLES `ubicaciones` WRITE;
/*!40000 ALTER TABLE `ubicaciones` DISABLE KEYS */;
INSERT INTO `ubicaciones` VALUES ('031f4640-92ba-5386-b140-42622387724e','Escalinata de San Antonio','Calle 2 # 10-12',3.446600,-76.543100,'8a21c980-9e25-5943-b86d-2353667e61d1'),('0f6ce8ce-d1f9-55e8-9766-0a61a1e747e1','Muro del Parque del Perro','Av. 9 Norte # 14-30',3.458500,-76.539000,'f4ad0b44-9470-54aa-8e09-f911906c09a0'),('1447a9d8-d272-5dc4-bc32-07d8254f93dd','Mirador de Siloé','Carrera 52 # 2-30',3.438000,-76.560000,'f220a5df-6b80-577a-87d4-903aafc20eaf'),('2044e229-0958-549d-b711-a2d98baf110a','Avenida Ciudad Jardín','Calle 18 # 122-10',3.372000,-76.540000,'bf8f4dc6-1a2e-56d6-aefe-1e964bb6ac2f'),('3e85858a-a338-5016-b3d7-0db16e9f548f','Parque de San Fernando','Calle 5 # 34-10',3.434000,-76.546000,'62388070-251a-5b9b-bd48-5761321f49f5'),('4612d7e0-e643-5f95-a140-1c92b55ad0c4','Fachada de la Calle del Sabor','Calle 17 Norte # 8-20',3.462000,-76.533000,'d41c3ceb-777b-5852-a9cd-c46b708b58f3'),('46677b6d-8275-57c9-926b-5d7df8052c4e','Plazoleta de San Antonio','Carrera 10 # 1-45',3.447000,-76.542500,'8a21c980-9e25-5943-b86d-2353667e61d1'),('d95561e3-f297-57ec-a7af-ab8c8592bf22','Plaza de Cristo Rey','Carrera 60 # 1-15',3.430000,-76.564000,'6cfe94c7-c4fc-55c8-9c2e-ab01235d789e'),('eba24c3e-0692-5990-9a64-84c6ff5dfb47','Calle del Calvario','Carrera 4 # 6-20',3.451000,-76.534000,'63407f59-57aa-521d-a072-75657854423f'),('f29b90e6-ffb8-52b5-9793-ce8e85fe4ea1','Muro del Pasaje San Pedro','Carrera 3 # 11-18',3.453500,-76.533000,'e04908eb-f718-5b02-b3d0-b900659333b2');
/*!40000 ALTER TABLE `ubicaciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `idUsuario` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombre1Usuario` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombre2Usuario` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `apellido1Usuario` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `apellido2Usuario` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `correoUsuario` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `contrasenaHash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'hash con sal; nunca texto plano',
  `fechaRegistroUsuario` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `estadoUsuario` tinyint(1) NOT NULL DEFAULT '1',
  `idTipoUsuarioFK` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`idUsuario`),
  UNIQUE KEY `uq_usuarios_correo` (`correoUsuario`),
  KEY `fk_usuarios_tipo` (`idTipoUsuarioFK`),
  CONSTRAINT `fk_usuarios_tipo` FOREIGN KEY (`idTipoUsuarioFK`) REFERENCES `tipos_usuarios` (`idTipoUsuario`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES ('18c962bb-b12a-5bdb-bd73-7bd8d7244d08','Andrés',NULL,'Marín',NULL,'admin2@arteurbanocali.test','PBKDF2$100000$NO9QZWXHGJbT4MEf3irM8w==$v912gX1oJNhImhWzAbnPUMhHENlZUJnrvATynKKSC5w=','2026-11-15 23:37:54',1,'5f80ec92-3db5-5844-a90b-8aaa44eb5b75'),('66c785da-d4a5-5dfe-901d-165bb0ca5d48','Valeria',NULL,'Ruiz','Sánchez','visitante@arteurbanocali.test','PBKDF2$100000$sVlrmI8iKE9SW4uPrYzc5A==$8zwt6xnzS7lzp89EZdApcjsrqrGlK/OtxiRfRuaVBXg=','2026-11-15 23:37:54',1,'e056adb5-2474-57f1-b8b6-241b7548aded'),('be963830-5544-58a4-99c4-65abbd5cd0f0','Laura','Camila','Gómez','Pérez','admin@arteurbanocali.test','PBKDF2$100000$NO9QZWXHGJbT4MEf3irM8w==$v912gX1oJNhImhWzAbnPUMhHENlZUJnrvATynKKSC5w=','2026-11-15 23:37:54',1,'5f80ec92-3db5-5844-a90b-8aaa44eb5b75');
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `vw_detalle_obra`
--

DROP TABLE IF EXISTS `vw_detalle_obra`;
/*!50001 DROP VIEW IF EXISTS `vw_detalle_obra`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_detalle_obra` AS SELECT 
 1 AS `idObra`,
 1 AS `nombreObra`,
 1 AS `fechaCreacion`,
 1 AS `estadoConservacion`,
 1 AS `nombreArtistico`,
 1 AS `nombreCategoria`,
 1 AS `nombreLugar`,
 1 AS `direccion`,
 1 AS `nombreBarrio`,
 1 AS `numeroComuna`,
 1 AS `latitud`,
 1 AS `longitud`,
 1 AS `totalRecursos`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_ruta_detalle`
--

DROP TABLE IF EXISTS `vw_ruta_detalle`;
/*!50001 DROP VIEW IF EXISTS `vw_ruta_detalle`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_ruta_detalle` AS SELECT 
 1 AS `idRuta`,
 1 AS `nombreRuta`,
 1 AS `ordenVisita`,
 1 AS `idObra`,
 1 AS `nombreObra`,
 1 AS `nombreLugar`,
 1 AS `direccion`,
 1 AS `nombreBarrio`,
 1 AS `numeroComuna`,
 1 AS `tiempoPermanencia`,
 1 AS `recomendacion`*/;
SET character_set_client = @saved_cs_client;

--
-- Final view structure for view `vw_detalle_obra`
--

/*!50001 DROP VIEW IF EXISTS `vw_detalle_obra`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_detalle_obra` AS select `o`.`idObra` AS `idObra`,`o`.`nombreObra` AS `nombreObra`,`o`.`fechaCreacion` AS `fechaCreacion`,`o`.`estadoConservacion` AS `estadoConservacion`,`a`.`nombreArtistico` AS `nombreArtistico`,`c`.`nombreCategoria` AS `nombreCategoria`,`u`.`nombreLugar` AS `nombreLugar`,`u`.`direccion` AS `direccion`,`b`.`nombreBarrio` AS `nombreBarrio`,`cm`.`numeroComuna` AS `numeroComuna`,`u`.`latitud` AS `latitud`,`u`.`longitud` AS `longitud`,(select count(0) from `recursos_multimedias` `r` where (`r`.`idObraFK` = `o`.`idObra`)) AS `totalRecursos` from (((((`obras` `o` join `artistas` `a` on((`a`.`idArtista` = `o`.`idArtistaFK`))) join `categorias` `c` on((`c`.`idCategoria` = `o`.`idCategoriaFK`))) join `ubicaciones` `u` on((`u`.`idUbicacion` = `o`.`idUbicacionFK`))) join `barrios` `b` on((`b`.`idBarrio` = `u`.`idBarrioFK`))) join `comunas` `cm` on((`cm`.`idComuna` = `b`.`idComunaFK`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_ruta_detalle`
--

/*!50001 DROP VIEW IF EXISTS `vw_ruta_detalle`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_ruta_detalle` AS select `r`.`idRuta` AS `idRuta`,`r`.`nombreRuta` AS `nombreRuta`,`ro`.`ordenVisita` AS `ordenVisita`,`o`.`idObra` AS `idObra`,`o`.`nombreObra` AS `nombreObra`,`u`.`nombreLugar` AS `nombreLugar`,`u`.`direccion` AS `direccion`,`b`.`nombreBarrio` AS `nombreBarrio`,`cm`.`numeroComuna` AS `numeroComuna`,`ro`.`tiempoPermanencia` AS `tiempoPermanencia`,`ro`.`recomendacion` AS `recomendacion` from (((((`rutas` `r` join `rutas_obras` `ro` on((`ro`.`idRutaFK` = `r`.`idRuta`))) join `obras` `o` on((`o`.`idObra` = `ro`.`idObraFK`))) join `ubicaciones` `u` on((`u`.`idUbicacion` = `o`.`idUbicacionFK`))) join `barrios` `b` on((`b`.`idBarrio` = `u`.`idBarrioFK`))) join `comunas` `cm` on((`cm`.`idComuna` = `b`.`idComunaFK`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-26 15:27:01
