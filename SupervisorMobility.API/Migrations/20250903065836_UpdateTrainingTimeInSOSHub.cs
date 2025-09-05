using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupervisorMobility.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrainingTimeInSOSHub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Primero, reemplazar TrainingTime con solo el número si contiene 'cycle'
                UPDATE SOSHubs
                SET TrainingTime = 
                    CASE 
                        WHEN TrainingTime LIKE '%cycle%' THEN SUBSTRING(TrainingTime, 1, CHARINDEX(' ', TrainingTime)-1)
                        ELSE TrainingTime
                    END
            ");

            migrationBuilder.AlterColumn<int>(
                name: "TrainingTime",
                table: "SOSHubs",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1863));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1865));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1867));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1869));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1871));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1872));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1874));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1876));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1658));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1672));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1674));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1682));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1697));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1700));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1702));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 58, 35, 719, DateTimeKind.Local).AddTicks(1705));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SOSAnalysisSOSSynopticTableofControlPoints");

            migrationBuilder.DropTable(
                name: "SOSSequenceSOSSynopticTableofControlPoints");

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2477));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 3,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2480));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 4,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2481));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 5,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2483));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 6,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2490));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 7,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2492));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 8,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2494));

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: 9,
                column: "EntryDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2495));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2282));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2306));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2308));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2309));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2313));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2316));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2318));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 0, 52, 57, 735, DateTimeKind.Local).AddTicks(2321));


            migrationBuilder.AddColumn<string>(
                name: "TrainingTimeTemp",
                table: "SOSHubs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE SOSHubs
                SET TrainingTimeTemp = 
                    CASE 
                        WHEN TrainingTime IS NOT NULL THEN CAST(TrainingTime AS nvarchar) + ' cycles'
                        ELSE '0 cycles'
                    END
            ");

            migrationBuilder.DropColumn(
                name: "TrainingTime",
                table: "SOSHubs");

            migrationBuilder.RenameColumn(
                name: "TrainingTimeTemp",
                table: "SOSHubs",
                newName: "TrainingTime");
        }
    }
}
