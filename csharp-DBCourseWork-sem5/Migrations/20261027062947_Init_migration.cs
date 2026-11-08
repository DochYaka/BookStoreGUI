using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_DBCourseWork_sem5.Migrations
{
    /// <inheritdoc />
    public partial class Init_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:abbreviation", "ООО,АО,ОАО,ЗАО,ПАО,ИП")
                .Annotation("Npgsql:Enum:doc_type", "Приказ,Заявление,Распоряжение,Договор")
                .Annotation("Npgsql:Enum:event_type", "Приём,Увольнение,Перевод на другую работу")
                .Annotation("Npgsql:Enum:gender", "Мужской,Женский");

            migrationBuilder.CreateTable(
                name: "author",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("author_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bank",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    bik = table.Column<string>(type: "character(9)", fixedLength: true, maxLength: 9, nullable: false),
                    inn = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    number_of_correspondent = table.Column<string>(type: "character(20)", fixedLength: true, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bank_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "book",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    edition = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    year = table.Column<int>(type: "integer", nullable: false),
                    annotation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    purchase_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    sale_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("book_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("city_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("customer_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("department_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("position_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "profession",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("profession_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "qualification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("qualification_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "specialty",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("specialty_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "street",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("street_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "author_book",
                columns: table => new
                {
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    book_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("author_book_pkey", x => new { x.author_id, x.book_id });
                    table.ForeignKey(
                        name: "author_book_author_id_fkey",
                        column: x => x.author_id,
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "author_book_book_id_fkey",
                        column: x => x.book_id,
                        principalTable: "book",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "book_copy",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    book_id = table.Column<int>(type: "integer", nullable: false),
                    copy_purchase_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("book_copy_pkey", x => x.id);
                    table.ForeignKey(
                        name: "book_copy_book_id_fkey",
                        column: x => x.book_id,
                        principalTable: "book",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "publisher",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    city_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("publisher_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_publisher_city",
                        column: x => x.city_id,
                        principalTable: "city",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    date_birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    house_number = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    city_id = table.Column<int>(type: "integer", nullable: true),
                    street_id = table.Column<int>(type: "integer", nullable: true),
                    work_experience = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_city_id_fkey",
                        column: x => x.city_id,
                        principalTable: "city",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "employee_street_id_fkey",
                        column: x => x.street_id,
                        principalTable: "street",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "individual",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone_number = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: true),
                    passport_series = table.Column<string>(type: "character(4)", fixedLength: true, maxLength: 4, nullable: true),
                    passport_number = table.Column<string>(type: "character(6)", fixedLength: true, maxLength: 6, nullable: true),
                    date_issue = table.Column<DateOnly>(type: "date", nullable: true),
                    issued_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    house_number = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    city_id = table.Column<int>(type: "integer", nullable: true),
                    street_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("individual_pkey", x => x.id);
                    table.ForeignKey(
                        name: "individual_city_id_fkey",
                        column: x => x.city_id,
                        principalTable: "city",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "individual_id_fkey",
                        column: x => x.id,
                        principalTable: "customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "individual_street_id_fkey",
                        column: x => x.street_id,
                        principalTable: "street",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "legalentity",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "character(8)", fixedLength: true, maxLength: 8, nullable: false),
                    current_account_number = table.Column<string>(type: "character(20)", fixedLength: true, maxLength: 20, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone_number = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: false),
                    house_number = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    city_id = table.Column<int>(type: "integer", nullable: true),
                    street_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("legalentity_pkey", x => x.id);
                    table.ForeignKey(
                        name: "legalentity_city_id_fkey",
                        column: x => x.city_id,
                        principalTable: "city",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "legalentity_id_fkey",
                        column: x => x.id,
                        principalTable: "customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "legalentity_street_id_fkey",
                        column: x => x.street_id,
                        principalTable: "street",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "publisher_book",
                columns: table => new
                {
                    publishing_house_id = table.Column<int>(type: "integer", nullable: false),
                    book_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("publisher_book_pkey", x => new { x.publishing_house_id, x.book_id });
                    table.ForeignKey(
                        name: "publisher_book_book_id_fkey",
                        column: x => x.book_id,
                        principalTable: "book",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "publisher_book_publishing_house_id_fkey",
                        column: x => x.publishing_house_id,
                        principalTable: "publisher",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    order_date = table.Column<DateOnly>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    copy_of_the_book_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orders_pkey", x => x.id);
                    table.ForeignKey(
                        name: "orders_copy_of_the_book_id_fkey",
                        column: x => x.copy_of_the_book_id,
                        principalTable: "book_copy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "orders_customer_id_fkey",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "orders_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_book",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_of_employment = table.Column<DateOnly>(type: "date", nullable: false),
                    date_of_dismissal = table.Column<DateOnly>(type: "date", nullable: true),
                    date_of_transfer = table.Column<DateOnly>(type: "date", nullable: true),
                    basis_date = table.Column<DateOnly>(type: "date", nullable: false),
                    basis_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    grounds_for_termination = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    position_id = table.Column<int>(type: "integer", nullable: true),
                    profession_id = table.Column<int>(type: "integer", nullable: true),
                    structural_unit_id = table.Column<int>(type: "integer", nullable: true),
                    specialty_id = table.Column<int>(type: "integer", nullable: true),
                    qualification_id = table.Column<int>(type: "integer", nullable: true),
                    employee_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("work_book_pkey", x => x.id);
                    table.ForeignKey(
                        name: "work_book_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "work_book_position_id_fkey",
                        column: x => x.position_id,
                        principalTable: "position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "work_book_profession_id_fkey",
                        column: x => x.profession_id,
                        principalTable: "profession",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "work_book_qualification_id_fkey",
                        column: x => x.qualification_id,
                        principalTable: "qualification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "work_book_specialty_id_fkey",
                        column: x => x.specialty_id,
                        principalTable: "specialty",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "work_book_structural_unit_id_fkey",
                        column: x => x.structural_unit_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "supplier",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    legal_id = table.Column<int>(type: "integer", nullable: false),
                    bank_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("supplier_pkey", x => x.id);
                    table.ForeignKey(
                        name: "supplier_bank_id_fkey",
                        column: x => x.bank_id,
                        principalTable: "bank",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "supplier_legal_id_fkey",
                        column: x => x.legal_id,
                        principalTable: "legalentity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "supply",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    delivery_date = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    supplier_id = table.Column<int>(type: "integer", nullable: false),
                    copy_of_the_book_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("supply_pkey", x => x.id);
                    table.ForeignKey(
                        name: "supply_copy_of_the_book_id_fkey",
                        column: x => x.copy_of_the_book_id,
                        principalTable: "book_copy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "supply_supplier_id_fkey",
                        column: x => x.supplier_id,
                        principalTable: "supplier",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_author_book_book_id",
                table: "author_book",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "bank_bik_key",
                table: "bank",
                column: "bik",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "bank_inn_key",
                table: "bank",
                column: "inn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "bank_name_key",
                table: "bank",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_book_copy_book_id",
                table: "book_copy",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "city_name_key",
                table: "city",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_city_id",
                table: "employee",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_street_id",
                table: "employee",
                column: "street_id");

            migrationBuilder.CreateIndex(
                name: "uq_employee_fio_birthday",
                table: "employee",
                columns: new[] { "first_name", "last_name", "middle_name", "date_birthday" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "individual_passport_number_key",
                table: "individual",
                column: "passport_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "individual_passport_series_key",
                table: "individual",
                column: "passport_series",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_individual_city_id",
                table: "individual",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_individual_street_id",
                table: "individual",
                column: "street_id");

            migrationBuilder.CreateIndex(
                name: "IX_legalentity_city_id",
                table: "legalentity",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_legalentity_street_id",
                table: "legalentity",
                column: "street_id");

            migrationBuilder.CreateIndex(
                name: "legalentity_code_key",
                table: "legalentity",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_orders_customer",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "idx_orders_date",
                table: "orders",
                column: "order_date");

            migrationBuilder.CreateIndex(
                name: "IX_orders_copy_of_the_book_id",
                table: "orders",
                column: "copy_of_the_book_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_employee_id",
                table: "orders",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_publisher_city_id",
                table: "publisher",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_publisher_book_book_id",
                table: "publisher_book",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_bank_id",
                table: "supplier",
                column: "bank_id");

            migrationBuilder.CreateIndex(
                name: "supplier_legal_id_key",
                table: "supplier",
                column: "legal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supply_copy_of_the_book_id",
                table: "supply",
                column: "copy_of_the_book_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_supplier_id",
                table: "supply",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_book_employee_id",
                table: "work_book",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_book_position_id",
                table: "work_book",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_book_profession_id",
                table: "work_book",
                column: "profession_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_book_qualification_id",
                table: "work_book",
                column: "qualification_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_book_specialty_id",
                table: "work_book",
                column: "specialty_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_book_structural_unit_id",
                table: "work_book",
                column: "structural_unit_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "author_book");

            migrationBuilder.DropTable(
                name: "individual");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "publisher_book");

            migrationBuilder.DropTable(
                name: "supply");

            migrationBuilder.DropTable(
                name: "work_book");

            migrationBuilder.DropTable(
                name: "author");

            migrationBuilder.DropTable(
                name: "publisher");

            migrationBuilder.DropTable(
                name: "book_copy");

            migrationBuilder.DropTable(
                name: "supplier");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "position");

            migrationBuilder.DropTable(
                name: "profession");

            migrationBuilder.DropTable(
                name: "qualification");

            migrationBuilder.DropTable(
                name: "specialty");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "book");

            migrationBuilder.DropTable(
                name: "bank");

            migrationBuilder.DropTable(
                name: "legalentity");

            migrationBuilder.DropTable(
                name: "city");

            migrationBuilder.DropTable(
                name: "customer");

            migrationBuilder.DropTable(
                name: "street");
        }
    }
}
