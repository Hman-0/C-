using Microsoft.EntityFrameworkCore;
using StudentManager.Data;
using System.Windows;

namespace StudentManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize database
            using (var context = new StudentDbContext())
            {
                try
                {
                    // Ensure database is created
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}