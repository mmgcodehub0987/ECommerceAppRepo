using ECOMapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ECOMapplication.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        //constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        //DB sets that represent tables in the database.
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrdersItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cancellation> Cancellation { get; set; }
        public DbSet<Refund> Refunds { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        
        public DbSet<Status> Statuss { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region FluentAPI
            //Customer -> Address (1- *)
            modelBuilder.Entity<Address>()
            .HasOne(c => c.Customer)
            .WithMany(a => a.Adresses)
            .HasForeignKey(a=> a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

            //Customer -> Orders (1 - *)
            modelBuilder.Entity<Order>()
                .HasOne(c => c.Customer)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            //Customer -> Cart (1 - 1)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithOne(ca => ca.Carts)
                .HasForeignKey<Cart> (ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            //Customer -> Feedback ( 1 - *)
            modelBuilder.Entity<Feedback>()
                .HasOne(c => c.Customer)
                .WithMany( f => f.Feedbacks)
                .HasForeignKey( f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            //Order -> Shipping Address
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey( o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            //Order -> Billing Address
            modelBuilder.Entity<Order>()
                .HasOne( o=> o.BillingAddress)
                .WithMany()
                .HasForeignKey(o => o.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            //Order -> payment (1-0..1)
            modelBuilder.Entity<Payment>()
                .HasOne( p => p.Order)
                .WithOne( o=> o.Payment)
                .HasForeignKey<Payment> (p=> p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            //Order -> Cancellation (1-0..1)
            modelBuilder.Entity<Cancellation>()
                .HasOne( can => can.Order)
                .WithOne( o => o.Cancellation)
                .HasForeignKey<Cancellation> (can => can.OrderId)
                .OnDelete (DeleteBehavior.Restrict);

            //Order -> OrderItems (1 - *)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            //Cart -> cartItem (1-*)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany( ca => ca.CartItems)
                .HasForeignKey( ci => ci.CartId)
                .OnDelete(DeleteBehavior.Restrict);

            //Product -> cartItem ()
            modelBuilder.Entity<CartItem>()
                .HasOne( ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //Product -> Order Item (1- *)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(pr => pr.OrderItems)
                .HasForeignKey(oi => oi.ProdctId)
                .OnDelete(DeleteBehavior.Restrict);

            //category -> Product (1 -*)
            modelBuilder.Entity<Product>()
                .HasOne( pr => pr.Category)
                .WithMany(cat => cat.Products)
                .HasForeignKey(pr => pr.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //Product -> Feedback (1-*)
            modelBuilder.Entity<Feedback>()
                .HasOne(fb => fb.Product)
                .WithMany(pr => pr.Feedbacks)
                .HasForeignKey(fb => fb.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //Refund -> Payment (1-1)
            modelBuilder.Entity<Refund>()
                .HasOne(rf => rf.Payment)
                .WithOne(pm => pm.Refund)
                .HasForeignKey<Refund> (rf => rf.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            //refund - cancellation (1 - 0..1)
            modelBuilder.Entity<Refund>()
                .HasOne(rf => rf.Cancellation)
                .WithOne(can => can.Refund)
                .HasForeignKey<Refund>(rf => rf.CancellationId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region enumMappedAsString
            //enum mapped as string for readability in DB
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<Payment>()
                .Property(pm => pm.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<Cancellation>()
                .Property(can => can.CancellationStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<Refund>()
                .Property(rf => rf.RefundStatus)
                .HasConversion<string>()
                .HasMaxLength(50);
            #endregion

            #region Data Seeding
            //Product table
            modelBuilder.Entity<Product>().HasData(
                new Product { Id=1, ProductName= "Samsung Galaxy S24 Ultra", Description= "6.8 inch, LTPO AMOLED Screen | 200 MP Quad Rear Camera with OIS | Qualcomm Snapdragon 8 Gen3 Chipset",
                               CategoryId=1, IsAvailable= true, StockQuantity= 30, DiscountPercentage=8,Price=79999.0m, ImageURL= "https://example.com/images/smartphone.jpg"
                });

            //status
            modelBuilder.Entity<Status>().HasData(
                //Order Statuses
                new Status { Id = 1, Name = "Pending" }, //Can be used to with Order, Paymeny, Cancellation, and Refund
                new Status { Id = 2, Name = "Processing" },
                new Status { Id = 3, Name = "Shipped" },
                new Status { Id = 4, Name = "Delivered" },
                new Status { Id = 5, Name = "Canceled" },
                //Refund Status
                new Status { Id = 6, Name = "Completed" },
                new Status { Id = 7, Name = "Failed" },
                //Cancellation Statuses
                new Status { Id = 8, Name = "Approved" },
                new Status { Id = 9, Name = "Rejected" },
                //Payment Status
                new Status { Id = 10, Name = "Refunded" }
            );

            //categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id=1, CategoryName="Smart Phones", CategoryDescription="Smart Phones & mobile Devices", IsActive= true });
            #endregion


        }
    }
}
