using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class SeedDocumentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: ["Id", "DocumentType", "Fields", "HelpText", "HelpTextHeader", "Name", "Regex"],
                values: new object[,]
                {
                    { 1, "CC", null, "Número de cédula sin puntos ni espacios", "Ingrese su cédula", "Cédula de Ciudadanía", "^[0-9]{6,10}$" },
                    { 2, "CE", null, "Número de cédula de extranjería", "Ingrese su cédula de extranjería", "Cédula de Extranjería", "^[0-9]{6,12}$" },
                    { 3, "NIT", null, "Número de Identificación Tributario sin dígito de verificación", "Ingrese el NIT", "NIT", "^[0-9]{6,12}$" },
                    { 4, "PP", null, "Número de pasaporte", "Ingrese su pasaporte", "Pasaporte", "^[A-Za-z0-9]{6,20}$" },
                    { 5, "RC", null, "Número de registro civil", "Ingrese el registro civil", "Registro Civil", "^[0-9]{6,10}$" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DocumentTypes",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
