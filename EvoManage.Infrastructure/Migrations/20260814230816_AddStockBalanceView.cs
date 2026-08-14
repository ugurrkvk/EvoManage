using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvoManage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockBalanceView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE VIEW dbo.StockBalanceView
                AS
                SELECT
                    sm.ProductId,
                    p.Code AS ProductCode,
                    p.Name AS ProductName,

                    sm.WarehouseId,
                    w.Code AS WarehouseCode,
                    w.Name AS WarehouseName,

                    sm.LocationId,
                    l.Code AS LocationCode,

                    SUM(
                        CASE sm.MovementType
                            WHEN 1 THEN sm.Quantity
                            WHEN 2 THEN -sm.Quantity
                            WHEN 3 THEN sm.Quantity
                            WHEN 4 THEN -sm.Quantity
                            ELSE 0
                        END
                    ) AS Quantity
                FROM dbo.StockMovements AS sm
                INNER JOIN dbo.Products AS p
                    ON p.Id = sm.ProductId
                INNER JOIN dbo.Warehouses AS w
                    ON w.Id = sm.WarehouseId
                INNER JOIN dbo.Locations AS l
                    ON l.Id = sm.LocationId
                GROUP BY
                    sm.ProductId,
                    p.Code,
                    p.Name,
                    sm.WarehouseId,
                    w.Code,
                    w.Name,
                    sm.LocationId,
                    l.Code;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP VIEW IF EXISTS dbo.StockBalanceView;
                """);
        }
    }
}
