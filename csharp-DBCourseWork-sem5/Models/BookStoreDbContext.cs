using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace csharp_DBCourseWork_sem5.Models;

public partial class BookStoreDbContext : DbContext
{
    public BookStoreDbContext()
    {
    }

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Copy> Copies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeInfo> EmployeeInfos { get; set; }

    public virtual DbSet<Individual> Individuals { get; set; }

    public virtual DbSet<Legalentity> Legalentities { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Profession> Professions { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Qualification> Qualifications { get; set; }

    public virtual DbSet<Rolepermission> Rolepermissions { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<Street> Streets { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Supply> Supplies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkBook> WorkBooks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=BookStore_db;Username=postgres;Password=your_secure_password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("abbreviation", new[] { "ООО", "АО", "ОАО", "ЗАО", "ПАО", "ИП" })
            .HasPostgresEnum("doc_type", new[] { "Приказ", "Заявление", "Распоряжение", "Договор" })
            .HasPostgresEnum("event_type", new[] { "Приём", "Увольнение", "Перевод на другую работу" })
            .HasPostgresEnum("gender", new[] { "Мужской", "Женский" });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("author_pkey");

            entity.ToTable("author", tb => tb.HasComment("Авторы"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");

            entity.HasMany(d => d.Books).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "AuthorBook",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("author_book_book_id_fkey"),
                    l => l.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("author_book_author_id_fkey"),
                    j =>
                    {
                        j.HasKey("AuthorId", "BookId").HasName("author_book_pkey");
                        j.ToTable("author_book", tb => tb.HasComment("Автор-Книга"));
                        j.IndexerProperty<int>("AuthorId").HasColumnName("author_id");
                        j.IndexerProperty<int>("BookId").HasColumnName("book_id");
                    });
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bank_pkey");

            entity.ToTable("bank", tb => tb.HasComment("Банки"));

            entity.HasIndex(e => e.Bik, "bank_bik_key").IsUnique();

            entity.HasIndex(e => e.Inn, "bank_inn_key").IsUnique();

            entity.HasIndex(e => e.Name, "bank_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bik)
                .HasMaxLength(9)
                .IsFixedLength()
                .HasColumnName("bik");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("inn");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.NumberOfCorrespondent)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("number_of_correspondent");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("book_pkey");

            entity.ToTable("book", tb => tb.HasComment("Книги"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Annotation)
                .HasMaxLength(150)
                .HasColumnName("annotation");
            entity.Property(e => e.Edition)
                .HasDefaultValue(1)
                .HasColumnName("edition");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("city_pkey");

            entity.ToTable("city", tb => tb.HasComment("Города"));

            entity.HasIndex(e => e.Name, "city_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Copy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("book_copy_pkey");

            entity.ToTable("copy", tb => tb.HasComment("Экземпляры книг"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('book_copy_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.PurchasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("purchase_price");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(0)
                .HasColumnName("quantity");
            entity.Property(e => e.SalePrice)
                .HasPrecision(10, 2)
                .HasColumnName("sale_price");

            entity.HasOne(d => d.Book).WithMany(p => p.Copies)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("book_copy_book_id_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_pkey");

            entity.ToTable("customer", tb => tb.HasComment("Клиенты"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("department_pkey");

            entity.ToTable("department", tb => tb.HasComment("Отделы"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employee", tb => tb.HasComment("Сотрудники"));

            entity.HasIndex(e => e.IndividualId, "employee_individual_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IndividualId).HasColumnName("individual_id");
            entity.Property(e => e.WorkExperience).HasColumnName("work_experience");

            entity.HasOne(d => d.Individual).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.IndividualId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("employee_individual_id_fkey");
        });

        modelBuilder.Entity<EmployeeInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("employee_info");

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("passport_number");
            entity.Property(e => e.PassportSeries)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("passport_series");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("phone_number");
            entity.Property(e => e.WorkExperience).HasColumnName("work_experience");
        });

        modelBuilder.Entity<Individual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("individual_pkey");

            entity.ToTable("individual", tb => tb.HasComment("Физические лица"));

            entity.HasIndex(e => e.PassportNumber, "individual_passport_number_key").IsUnique();

            entity.HasIndex(e => e.PassportSeries, "individual_passport_series_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.DateIssue).HasColumnName("date_issue");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.HouseNumber)
                .HasMaxLength(25)
                .HasColumnName("house_number");
            entity.Property(e => e.IssuedBy)
                .HasMaxLength(50)
                .HasColumnName("issued_by");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("passport_number");
            entity.Property(e => e.PassportSeries)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("passport_series");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("phone_number");
            entity.Property(e => e.StreetId).HasColumnName("street_id");

            entity.HasOne(d => d.City).WithMany(p => p.Individuals)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("individual_city_id_fkey");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Individual)
                .HasForeignKey<Individual>(d => d.Id)
                .HasConstraintName("individual_id_fkey");

            entity.HasOne(d => d.Street).WithMany(p => p.Individuals)
                .HasForeignKey(d => d.StreetId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("individual_street_id_fkey");
        });

        modelBuilder.Entity<Legalentity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("legalentity_pkey");

            entity.ToTable("legalentity", tb => tb.HasComment("Юридические лица"));

            entity.HasIndex(e => e.Code, "legalentity_code_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Code)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("code");
            entity.Property(e => e.CurrentAccountNumber)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("current_account_number");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.HouseNumber)
                .HasMaxLength(25)
                .HasColumnName("house_number");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("phone_number");
            entity.Property(e => e.StreetId).HasColumnName("street_id");

            entity.HasOne(d => d.City).WithMany(p => p.Legalentities)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("legalentity_city_id_fkey");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Legalentity)
                .HasForeignKey<Legalentity>(d => d.Id)
                .HasConstraintName("legalentity_id_fkey");

            entity.HasOne(d => d.Street).WithMany(p => p.Legalentities)
                .HasForeignKey(d => d.StreetId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("legalentity_street_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders", tb => tb.HasComment("Заказы"));

            entity.HasIndex(e => e.CustomerId, "idx_orders_customer");

            entity.HasIndex(e => e.OrderDate, "idx_orders_date");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CopyId).HasColumnName("copy_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(20)
                .HasColumnName("order_number");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Copy).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CopyId)
                .HasConstraintName("orders_copy_of_the_book_id_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("orders_customer_id_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("orders_employee_id_fkey");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("position_pkey");

            entity.ToTable("position", tb => tb.HasComment("Должности"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Profession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("profession_pkey");

            entity.ToTable("profession", tb => tb.HasComment("Профессии"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("publisher_pkey");

            entity.ToTable("publisher", tb => tb.HasComment("Издательства"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");

            entity.HasOne(d => d.City).WithMany(p => p.Publishers)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_publisher_city");

            entity.HasMany(d => d.Books).WithMany(p => p.PublishingHouses)
                .UsingEntity<Dictionary<string, object>>(
                    "PublisherBook",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("publisher_book_book_id_fkey"),
                    l => l.HasOne<Publisher>().WithMany()
                        .HasForeignKey("PublishingHouseId")
                        .HasConstraintName("publisher_book_publishing_house_id_fkey"),
                    j =>
                    {
                        j.HasKey("PublishingHouseId", "BookId").HasName("publisher_book_pkey");
                        j.ToTable("publisher_book", tb => tb.HasComment("Издательство-Книга"));
                        j.IndexerProperty<int>("PublishingHouseId").HasColumnName("publishing_house_id");
                        j.IndexerProperty<int>("BookId").HasColumnName("book_id");
                    });
        });

        modelBuilder.Entity<Qualification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("qualification_pkey");

            entity.ToTable("qualification", tb => tb.HasComment("Квалификации"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Rolepermission>(entity =>
        {
            entity.HasKey(e => new { e.Role, e.TableName }).HasName("rolepermissions_pkey");

            entity.ToTable("rolepermissions");

            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("table_name");
            entity.Property(e => e.FuncName)
                .HasMaxLength(25)
                .HasColumnName("func_name");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("specialty_pkey");

            entity.ToTable("specialty", tb => tb.HasComment("Специальности"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("street_pkey");

            entity.ToTable("street", tb => tb.HasComment("Улицы"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supplier_pkey");

            entity.ToTable("supplier", tb => tb.HasComment("Поставщики"));

            entity.HasIndex(e => e.LegalId, "supplier_legal_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankId).HasColumnName("bank_id");
            entity.Property(e => e.LegalId).HasColumnName("legal_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Bank).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.BankId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("supplier_bank_id_fkey");

            entity.HasOne(d => d.Legal).WithOne(p => p.Supplier)
                .HasForeignKey<Supplier>(d => d.LegalId)
                .HasConstraintName("supplier_legal_id_fkey");
        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supply_pkey");

            entity.ToTable("supply", tb => tb.HasComment("Поставки"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CopyId).HasColumnName("copy_id");
            entity.Property(e => e.DeliveryDate).HasColumnName("delivery_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

            entity.HasOne(d => d.Copy).WithMany(p => p.Supplies)
                .HasForeignKey(d => d.CopyId)
                .HasConstraintName("supply_copy_of_the_book_id_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Supplies)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("supply_supplier_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", tb => tb.HasComment("Пользователи"));

            entity.HasIndex(e => e.Login, "users_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.Login)
                .HasMaxLength(25)
                .HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Salt)
                .HasMaxLength(255)
                .HasColumnName("salt");
        });

        modelBuilder.Entity<WorkBook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("work_book_pkey");

            entity.ToTable("work_book", tb => tb.HasComment("Рабочие книги"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasisDate).HasColumnName("basis_date");
            entity.Property(e => e.BasisNumber)
                .HasMaxLength(20)
                .HasColumnName("basis_number");
            entity.Property(e => e.DateOfDismissal).HasColumnName("date_of_dismissal");
            entity.Property(e => e.DateOfEmployment).HasColumnName("date_of_employment");
            entity.Property(e => e.DateOfTransfer).HasColumnName("date_of_transfer");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.GroundsForTermination)
                .HasMaxLength(250)
                .HasColumnName("grounds_for_termination");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.ProfessionId).HasColumnName("profession_id");
            entity.Property(e => e.QualificationId).HasColumnName("qualification_id");
            entity.Property(e => e.SpecialtyId).HasColumnName("specialty_id");
            entity.Property(e => e.StructuralUnitId).HasColumnName("structural_unit_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.WorkBooks)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("work_book_employee_id_fkey");

            entity.HasOne(d => d.Position).WithMany(p => p.WorkBooks)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("work_book_position_id_fkey");

            entity.HasOne(d => d.Profession).WithMany(p => p.WorkBooks)
                .HasForeignKey(d => d.ProfessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("work_book_profession_id_fkey");

            entity.HasOne(d => d.Qualification).WithMany(p => p.WorkBooks)
                .HasForeignKey(d => d.QualificationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("work_book_qualification_id_fkey");

            entity.HasOne(d => d.Specialty).WithMany(p => p.WorkBooks)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("work_book_specialty_id_fkey");

            entity.HasOne(d => d.StructuralUnit).WithMany(p => p.WorkBooks)
                .HasForeignKey(d => d.StructuralUnitId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("work_book_structural_unit_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
