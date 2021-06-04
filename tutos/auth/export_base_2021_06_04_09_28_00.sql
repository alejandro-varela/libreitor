-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Versión del servidor:         10.5.10-MariaDB - mariadb.org binary distribution
-- SO del servidor:              Win64
-- HeidiSQL Versión:             11.2.0.6213
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Volcando estructura de base de datos para dbprueba
CREATE DATABASE IF NOT EXISTS `dbprueba` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `dbprueba`;

-- Volcando estructura para tabla dbprueba.auth_hash_algos
CREATE TABLE IF NOT EXISTS `auth_hash_algos` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `description` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `description` (`description`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- Volcando datos para la tabla dbprueba.auth_hash_algos: ~1 rows (aproximadamente)
DELETE FROM `auth_hash_algos`;
/*!40000 ALTER TABLE `auth_hash_algos` DISABLE KEYS */;
INSERT INTO `auth_hash_algos` (`id`, `description`) VALUES
	(1, 'SHA256');
/*!40000 ALTER TABLE `auth_hash_algos` ENABLE KEYS */;

-- Volcando estructura para tabla dbprueba.auth_paths
CREATE TABLE IF NOT EXISTS `auth_paths` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `description` varchar(2048) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `description` (`description`) USING HASH
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

-- Volcando datos para la tabla dbprueba.auth_paths: ~9 rows (aproximadamente)
DELETE FROM `auth_paths`;
/*!40000 ALTER TABLE `auth_paths` DISABLE KEYS */;
INSERT INTO `auth_paths` (`id`, `description`) VALUES
	(1, 'AlgunSistema/QrController/GetImage'),
	(2, 'AlgunSistema/QrController/ReadImage'),
	(3, 'AlgunSistema/PerchasController/Metodo123'),
	(4, 'AlgunSistema/GorgojosController/GetGorgojos'),
	(5, 'AlgunSistema/BeerFactory/GetMore'),
	(6, 'OtroSistema/Empleados/MetodoA'),
	(7, 'OtroSistema/Empleados/MetodoB'),
	(8, 'SigofReloaded/Coches/AvisarEntrada'),
	(9, 'SigofReloaded/Coches/MandarMensajeAPipo');
/*!40000 ALTER TABLE `auth_paths` ENABLE KEYS */;

-- Volcando estructura para tabla dbprueba.auth_roles
CREATE TABLE IF NOT EXISTS `auth_roles` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `description` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `description` (`description`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COMMENT='a table representing distinct roles for authenticacion and authorization';

-- Volcando datos para la tabla dbprueba.auth_roles: ~7 rows (aproximadamente)
DELETE FROM `auth_roles`;
/*!40000 ALTER TABLE `auth_roles` DISABLE KEYS */;
INSERT INTO `auth_roles` (`id`, `description`) VALUES
	(3, 'admin'),
	(1, 'god'),
	(6, 'it_help_desk'),
	(7, 'it_net_administrator'),
	(2, 'nop'),
	(5, 'payroll_asisstant'),
	(4, 'payroll_boss');
/*!40000 ALTER TABLE `auth_roles` ENABLE KEYS */;

-- Volcando estructura para tabla dbprueba.auth_role_paths
CREATE TABLE IF NOT EXISTS `auth_role_paths` (
  `roleId` int(10) unsigned NOT NULL,
  `pathId` int(10) unsigned NOT NULL,
  UNIQUE KEY `roleId_pathId` (`roleId`,`pathId`),
  KEY `roleId` (`roleId`),
  KEY `FK__auth_paths` (`pathId`),
  CONSTRAINT `FK__auth_paths` FOREIGN KEY (`pathId`) REFERENCES `auth_paths` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `FK__auth_roles` FOREIGN KEY (`roleId`) REFERENCES `auth_roles` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Volcando datos para la tabla dbprueba.auth_role_paths: ~20 rows (aproximadamente)
DELETE FROM `auth_role_paths`;
/*!40000 ALTER TABLE `auth_role_paths` DISABLE KEYS */;
INSERT INTO `auth_role_paths` (`roleId`, `pathId`) VALUES
	(3, 1),
	(3, 2),
	(3, 3),
	(3, 4),
	(3, 5),
	(3, 6),
	(3, 7),
	(3, 8),
	(3, 9),
	(4, 6),
	(4, 7),
	(5, 7),
	(6, 5),
	(6, 6),
	(6, 7),
	(6, 9),
	(7, 3),
	(7, 5),
	(7, 6),
	(7, 7);
/*!40000 ALTER TABLE `auth_role_paths` ENABLE KEYS */;

-- Volcando estructura para tabla dbprueba.auth_users
CREATE TABLE IF NOT EXISTS `auth_users` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `hashAlgoId` int(10) unsigned NOT NULL,
  `hashPwd` varchar(1024) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`),
  KEY `FK_auth_users_auth_hash_algos` (`hashAlgoId`),
  CONSTRAINT `FK_auth_users_auth_hash_algos` FOREIGN KEY (`hashAlgoId`) REFERENCES `auth_hash_algos` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- Volcando datos para la tabla dbprueba.auth_users: ~5 rows (aproximadamente)
DELETE FROM `auth_users`;
/*!40000 ALTER TABLE `auth_users` DISABLE KEYS */;
INSERT INTO `auth_users` (`id`, `name`, `hashAlgoId`, `hashPwd`) VALUES
	(1, 'brenda.ballester', 1, 'f9f68f1e4dad63da10cc68634defd868d49f1eeb6ff682c1cda44cfa290745db'),
	(2, 'julian.pagano', 1, '38c7ff55c030512920d00d5437c54ea5d6c0b3ca703960ea93e3095a9f2aa7ad'),
	(3, 'matias.alvarado', 1, 'ad3bfb82f32db0ca860445f804c31526ed7ec11234efe0c24569963872f12814'),
	(4, 'fernando.feldman', 1, 'db6121f918dee472cffbe6e34a5b5e4db9804a3e564024b11f86735645280a7b'),
	(5, 'alejandro.varela', 1, '14da30db908f525fd409a860be98fa4463bf346cd91a7b26d7527db66192f2d9');
/*!40000 ALTER TABLE `auth_users` ENABLE KEYS */;

-- Volcando estructura para tabla dbprueba.auth_user_roles
CREATE TABLE IF NOT EXISTS `auth_user_roles` (
  `userId` int(10) unsigned NOT NULL,
  `roleId` int(10) unsigned NOT NULL,
  UNIQUE KEY `userId_roleId` (`userId`,`roleId`),
  KEY `FK_auth_user_roles_auth_roles` (`roleId`),
  CONSTRAINT `FK_auth_user_roles_auth_roles` FOREIGN KEY (`roleId`) REFERENCES `auth_roles` (`id`) ON UPDATE CASCADE,
  CONSTRAINT `FK_auth_user_roles_auth_users` FOREIGN KEY (`userId`) REFERENCES `auth_users` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Volcando datos para la tabla dbprueba.auth_user_roles: ~6 rows (aproximadamente)
DELETE FROM `auth_user_roles`;
/*!40000 ALTER TABLE `auth_user_roles` DISABLE KEYS */;
INSERT INTO `auth_user_roles` (`userId`, `roleId`) VALUES
	(1, 3),
	(2, 3),
	(3, 7),
	(4, 4),
	(5, 5),
	(5, 6);
/*!40000 ALTER TABLE `auth_user_roles` ENABLE KEYS */;

-- Volcando estructura para vista dbprueba.vw_auth_users
-- Creando tabla temporal para superar errores de dependencia de VIEW
CREATE TABLE `vw_auth_users` (
	`id` INT(10) UNSIGNED NOT NULL,
	`name` VARCHAR(50) NOT NULL COLLATE 'utf8_general_ci',
	`hashAlgoId` INT(10) UNSIGNED NOT NULL,
	`hashAlgo` VARCHAR(50) NOT NULL COLLATE 'utf8_general_ci',
	`hash` VARCHAR(1024) NOT NULL COLLATE 'utf8_general_ci',
	`roleId` INT(10) UNSIGNED NOT NULL,
	`role` VARCHAR(50) NOT NULL COLLATE 'utf8_general_ci',
	`path` VARCHAR(2048) NOT NULL COLLATE 'utf8_general_ci'
) ENGINE=MyISAM;

-- Volcando estructura para vista dbprueba.vw_auth_users
-- Eliminando tabla temporal y crear estructura final de VIEW
DROP TABLE IF EXISTS `vw_auth_users`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vw_auth_users` AS SELECT      users.id          AS 'id',
            users.NAME        AS 'name',
            users.hashAlgoId  AS 'hashAlgoId',
            algos.description AS 'hashAlgo',
            users.hashpwd     AS 'hash',
            user_roles.roleId AS 'roleId',
            roles.description AS 'role',
            paths.description AS 'path'
FROM        auth_users      users
INNER JOIN  auth_hash_algos algos      ON users.hashAlgoId  = algos.id
INNER JOIN  auth_user_roles user_roles ON user_roles.userId = users.id
INNER JOIN  auth_roles      roles      ON roles.id          = user_roles.roleId 
INNER JOIN  auth_role_paths role_paths ON role_paths.roleId = roles.id
INNER JOIN  auth_paths      paths      ON paths.id          = role_paths.pathId ;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
