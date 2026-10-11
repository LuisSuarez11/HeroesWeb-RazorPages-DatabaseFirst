USE HeroesClase1;
GO
PRINT '=== D1: nombre de ciudad repetido (UQ_Ciudades_Nombre) ===';
BEGIN TRY
    BEGIN TRANSACTION;
    INSERT INTO dbo.Ciudades (Nombre, Pais) VALUES (N'La Paz', N'Bolivia');
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    SELECT ERROR_MESSAGE() AS Resultado;
END CATCH;
GO
PRINT '=== D2: nivel de amenaza fuera de rango (CK_Villanos_NivelAmenaza) ===';
BEGIN TRY
    BEGIN TRANSACTION;
    UPDATE dbo.Villanos SET Nivel_Amenaza = 9
    WHERE Id = (SELECT MIN(Id) FROM dbo.Villanos);
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    SELECT ERROR_MESSAGE() AS Resultado;
END CATCH;
GO
PRINT '=== D3: borrar ciudad con villanos (FK_Villanos_Ciudades) ===';
BEGIN TRY
    BEGIN TRANSACTION;
    DELETE FROM dbo.Ciudades WHERE Nombre = N'La Paz';
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    SELECT ERROR_MESSAGE() AS Resultado;
END CATCH;
GO
PRINT '=== E4: placa repetida 2345-ABC (UQ_Vehiculos_Placa) ===';
BEGIN TRY
    BEGIN TRANSACTION;
    INSERT INTO dbo.Vehiculos (Nro_Placa, Modelo, CostoMantenimiento) VALUES (N'2345-ABC', N'Otro', 100);
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    SELECT ERROR_MESSAGE() AS Resultado;
END CATCH;
GO
PRINT '=== E5: capacidad 0 (CK_Cuarteles_Capacidad) ===';
BEGIN TRY
    BEGIN TRANSACTION;
    UPDATE dbo.Cuarteles SET Capacidad = 0 WHERE Nombre = N'Base Oriente';
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    SELECT ERROR_MESSAGE() AS Resultado;
END CATCH;
GO
PRINT '=== E6: borrar Base Illimani (SET NULL), luego ROLLBACK ===';
BEGIN TRANSACTION;
DECLARE @Id INT = (SELECT Id FROM dbo.Cuarteles WHERE Nombre = N'Base Illimani');
DELETE FROM dbo.Cuarteles WHERE Id = @Id;
SELECT Nro_Placa, Modelo, CuartelId FROM dbo.Vehiculos;
ROLLBACK TRANSACTION;
GO
PRINT '=== Verificacion: los datos siguen intactos ===';
SELECT (SELECT COUNT(*) FROM dbo.Ciudades) AS Ciudades, (SELECT COUNT(*) FROM dbo.Villanos) AS Villanos,
       (SELECT COUNT(*) FROM dbo.Cuarteles) AS Cuarteles, (SELECT COUNT(*) FROM dbo.Vehiculos WHERE CuartelId IS NOT NULL) AS VehiculosConCuartel;
GO
