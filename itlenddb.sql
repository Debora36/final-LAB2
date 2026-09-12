-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: itlenddb
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `categoria`
--

DROP TABLE IF EXISTS `categoria`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categoria` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Descripcion` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nombre` (`Nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categoria`
--

LOCK TABLES `categoria` WRITE;
/*!40000 ALTER TABLE `categoria` DISABLE KEYS */;
INSERT INTO `categoria` VALUES (2,'teclado','perifericos'),(3,'monitor','de pc'),(4,'Televisor','para reuniones/presentaciones'),(6,'parlante','de pc');
/*!40000 ALTER TABLE `categoria` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empleado`
--

DROP TABLE IF EXISTS `empleado`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empleado` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Apellido` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DNI` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Telefono` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `UsuarioId` int DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `DNI` (`DNI`),
  UNIQUE KEY `UsuarioId` (`UsuarioId`),
  CONSTRAINT `empleado_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuario` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empleado`
--

LOCK TABLES `empleado` WRITE;
/*!40000 ALTER TABLE `empleado` DISABLE KEYS */;
INSERT INTO `empleado` VALUES (1,'Pablo','Martinez','30234234','+54233123120',3,1),(2,'Micaela','Glaria','382221576','+542662525250',4,0),(3,'Juan','Perez','33333333','+54266252525',NULL,0),(4,'maria','sosa','33666666','5492665036260',NULL,1);
/*!40000 ALTER TABLE `empleado` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `equipo`
--

DROP TABLE IF EXISTS `equipo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `equipo` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Modelo` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NumeroSerie` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Estado` enum('Disponible','Prestado','En Mantenimiento','Baja') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Disponible',
  `RutaArchivoGarantia` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CategoriaId` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `NumeroSerie` (`NumeroSerie`),
  KEY `CategoriaId` (`CategoriaId`),
  CONSTRAINT `equipo_ibfk_1` FOREIGN KEY (`CategoriaId`) REFERENCES `categoria` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `equipo`
--

LOCK TABLES `equipo` WRITE;
/*!40000 ALTER TABLE `equipo` DISABLE KEYS */;
INSERT INTO `equipo` VALUES (1,'mecanico','00001','Disponible','/Uploads/Equipos/a3940ef4-917e-4604-b544-4418c8c84057.pdf',2),(2,'mecanico','00002','Disponible','/Uploads/Equipos/a8a8ff09-74e5-4278-a2e8-e1b09d2042fd.pdf',2),(3,'lcd','0003','Prestado','/Uploads/Equipos/6273f882-3dbc-45fe-b046-7cdbe4f54bca.pdf',3),(4,'tactil','0004','Prestado',NULL,3),(5,'smart','5978','Disponible',NULL,4),(6,'curvo','6457','Disponible',NULL,3),(7,'noga','1212','Disponible',NULL,6),(8,'jbl','500','Disponible',NULL,6),(9,'inalambrico','02000','Prestado',NULL,2);
/*!40000 ALTER TABLE `equipo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prestamo`
--

DROP TABLE IF EXISTS `prestamo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prestamo` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EquipoId` int NOT NULL,
  `EmpleadoId` int NOT NULL,
  `FechaPrestamo` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaDevolucionEstimada` datetime DEFAULT NULL,
  `FechaDevolucionReal` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `EquipoId` (`EquipoId`),
  KEY `EmpleadoId` (`EmpleadoId`),
  CONSTRAINT `prestamo_ibfk_1` FOREIGN KEY (`EquipoId`) REFERENCES `equipo` (`Id`),
  CONSTRAINT `prestamo_ibfk_2` FOREIGN KEY (`EmpleadoId`) REFERENCES `empleado` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prestamo`
--

LOCK TABLES `prestamo` WRITE;
/*!40000 ALTER TABLE `prestamo` DISABLE KEYS */;
INSERT INTO `prestamo` VALUES (1,2,1,'2026-09-03 00:27:03','2026-09-05 00:00:00','2026-09-10 20:36:55'),(2,4,2,'2026-09-03 12:55:15','2026-09-05 00:00:00','2026-09-08 13:39:46'),(3,2,1,'2026-09-03 13:29:14','2026-10-04 00:00:00','2026-09-03 13:44:32'),(4,5,1,'2026-09-07 17:25:45','2026-09-07 00:00:00','2026-09-08 22:14:09'),(5,1,1,'2026-09-08 00:32:15','2026-09-12 00:00:00','2026-09-08 15:26:44'),(6,4,1,'2026-09-08 23:58:34','2026-09-15 00:00:00',NULL),(8,6,1,'2026-09-09 00:02:01','2026-09-24 00:00:00','2026-09-09 00:02:42'),(9,2,1,'2026-09-10 20:35:38','2026-09-13 00:00:00',NULL),(10,9,1,'2026-09-12 00:36:01','2026-09-15 00:00:00',NULL);
/*!40000 ALTER TABLE `prestamo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `solicitud`
--

DROP TABLE IF EXISTS `solicitud`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `solicitud` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EmpleadoId` int NOT NULL,
  `CategoriaId` int NOT NULL,
  `Motivo` varchar(250) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TiempoNecesario` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `FechaSolicitud` datetime DEFAULT CURRENT_TIMESTAMP,
  `Estado` enum('Pendiente','Aprobada','Rechazada') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pendiente',
  PRIMARY KEY (`Id`),
  KEY `EmpleadoId` (`EmpleadoId`),
  KEY `CategoriaId` (`CategoriaId`),
  CONSTRAINT `solicitud_ibfk_1` FOREIGN KEY (`EmpleadoId`) REFERENCES `empleado` (`Id`),
  CONSTRAINT `solicitud_ibfk_2` FOREIGN KEY (`CategoriaId`) REFERENCES `categoria` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `solicitud`
--

LOCK TABLES `solicitud` WRITE;
/*!40000 ALTER TABLE `solicitud` DISABLE KEYS */;
INSERT INTO `solicitud` VALUES (1,1,2,'nmose','3 meses','2026-08-29 00:42:32','Rechazada'),(5,2,3,'reunion','1 dia','2026-08-31 21:32:47','Aprobada'),(6,1,3,'reunion','1 dia','2026-09-01 00:19:35','Aprobada'),(7,1,2,'trabajo','30 dias','2026-09-03 13:27:17','Aprobada'),(8,1,4,'reunion','1 dia','2026-09-07 17:23:34','Aprobada'),(9,1,2,'dfwf','3 dias','2026-09-07 21:37:41','Aprobada'),(10,1,4,NULL,'30 dias','2026-09-08 00:39:51','Rechazada'),(11,1,3,'ydhy','1 dia','2026-09-08 00:44:26','Rechazada'),(12,1,3,NULL,'10 dias','2026-09-08 17:12:27','Aprobada'),(13,1,2,'hjklh','30 dias','2026-09-08 23:56:59','Rechazada'),(14,1,3,NULL,'30 dias','2026-09-09 00:00:54','Aprobada'),(15,1,4,NULL,'3 dias','2026-09-09 00:20:26','Pendiente'),(16,1,2,NULL,'3 dias','2026-09-10 17:22:15','Aprobada');
/*!40000 ALTER TABLE `solicitud` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Rol` enum('Admin','Tecnico','Empleado') COLLATE utf8mb4_unicode_ci NOT NULL,
  `AvatarUrl` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Username` (`Username`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'admin','$2a$12$e333xVJlvbX.Z3zw3CVbLOVMYSYIrlfZiOvLxoiwDu5dAKEb8Atwu','Admin','/Uploads/Avatars/ead8f678-0a96-40d6-9d78-2b4855d038f6.jpg','admin1@itlend.com',1),(2,'Tecnico','$2a$12$4OFMqcd6DzU4NHqVRtE7FenRHJOw1C1QCarH9L5ozqVbiumYo99Le','Tecnico','/Uploads/Avatars/0e8f4db3-6ccc-4eb9-b6d4-cc4f060fc3e1.png','tecnico1@gmail.com',1),(3,'Empleado123','$2a$12$amso.7cqOrax7t3SES5vQ.ua84VgY0DJfshei4CVovDzB3F7BkHcO','Empleado','/Uploads/Avatars/973db716-a0ca-409d-8bd6-57f2024fbc4d.png','Empleado1@itlend.com',1),(4,'UserEmpleado','$2a$12$YSRJbhtJpx5jXVg5ZcTgTukFqSyanjhfeJ0mVC9wGt5i7PAgfjALG','Empleado','/Uploads/Avatars/31cd0729-c731-42be-b70a-e7669f7cc173.jpg','user@gmail.com',0),(5,'empleao2','$2a$12$wPPXF1lRnSQmVaFfmezPKu4H8m0yREp3/SX1.t50yAWC2Jtsw/jRW','Tecnico','/Uploads/Avatars/2b1ab144-1132-4557-b046-4da3f3f1a61d.jpg','tt@gmail.com',1),(6,'prueba','$2a$12$CElW59X1td4WSZjica16beWA4bNlqc88viezu1c5M32WL2tWj0ZLS','Empleado',NULL,'prueba123!@gmail.com',1);
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'itlenddb'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-12 18:13:36
