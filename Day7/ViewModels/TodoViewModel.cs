using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Day7.Commands;
using Day7.Data;
using Day7.Models;
using Microsoft.EntityFrameworkCore;

namespace Day7.ViewModels;

public class TodoViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _context;
    private string _newTodoTitle = string.Empty;
    private DateTime _newTodoDeadline = DateTime.Today.AddDays(1);
    private bool _newTodoIsCompleted = false;
    private string _searchText = string.Empty;
    private string _selectedFilter = "Tất cả";
    private TodoItem? _selectedTodo;
    private ObservableCollection<TodoItem> _allTodos;
    private ObservableCollection<TodoItem> _filteredTodos;

    public TodoViewModel()
    {
        try
        {
            _context = new AppDbContext();
            _context.Database.EnsureCreated();
            
            _allTodos = new ObservableCollection<TodoItem>();
            _filteredTodos = new ObservableCollection<TodoItem>();
            
            // Initialize commands
            AddTodoCommand = new RelayCommand(_ => AddTodo(), _ => CanAddTodo());
            UpdateTodoCommand = new RelayCommand(_ => UpdateTodo(), _ => CanUpdateTodo());
            DeleteTodoCommand = new RelayCommand(_ => DeleteTodo(), _ => CanDeleteTodo());
            ToggleCompletedCommand = new RelayCommand<TodoItem>(ToggleCompleted);
            
            FilterOptions = new ObservableCollection<string> { "Tất cả", "Hoàn thành", "Chưa hoàn thành" };
            
            LoadTodos();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Lỗi khởi tạo database: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    #region Properties
    public string NewTodoTitle
    {
        get => _newTodoTitle;
        set
        {
            _newTodoTitle = value;
            OnPropertyChanged();
        }
    }

    public DateTime NewTodoDeadline
    {
        get => _newTodoDeadline;
        set
        {
            _newTodoDeadline = value;
            OnPropertyChanged();
        }
    }

    public bool NewTodoIsCompleted
    {
        get => _newTodoIsCompleted;
        set
        {
            _newTodoIsCompleted = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            _selectedFilter = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public TodoItem? SelectedTodo
    {
        get => _selectedTodo;
        set
        {
            _selectedTodo = value;
            OnPropertyChanged();
            if (_selectedTodo != null)
            {
                NewTodoTitle = _selectedTodo.Title;
                NewTodoDeadline = _selectedTodo.Deadline;
                NewTodoIsCompleted = _selectedTodo.IsCompleted;
            }
        }
    }

    public ObservableCollection<TodoItem> FilteredTodos
    {
        get => _filteredTodos;
        set
        {
            _filteredTodos = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> FilterOptions { get; }
    #endregion

    #region Commands
    public ICommand AddTodoCommand { get; }
    public ICommand UpdateTodoCommand { get; }
    public ICommand DeleteTodoCommand { get; }
    public ICommand ToggleCompletedCommand { get; }
    #endregion

    #region Methods
    private async void LoadTodos()
    {
        var todos = await _context.TodoItems.OrderBy(t => t.Deadline).ToListAsync();
        _allTodos.Clear();
        foreach (var todo in todos)
        {
            _allTodos.Add(todo);
        }
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = _allTodos.AsEnumerable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(t => t.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        // Apply status filter
        filtered = SelectedFilter switch
        {
            "Hoàn thành" => filtered.Where(t => t.IsCompleted),
            "Chưa hoàn thành" => filtered.Where(t => !t.IsCompleted),
            _ => filtered
        };

        FilteredTodos.Clear();
        foreach (var todo in filtered.OrderBy(t => t.Deadline))
        {
            FilteredTodos.Add(todo);
        }
    }

    private bool CanAddTodo()
    {
        return !string.IsNullOrWhiteSpace(NewTodoTitle);
    }

    private async void AddTodo()
    {
        var newTodo = new TodoItem
        {
            Title = NewTodoTitle.Trim(),
            Deadline = NewTodoDeadline,
            IsCompleted = NewTodoIsCompleted
        };

        _context.TodoItems.Add(newTodo);
        await _context.SaveChangesAsync();
        
        _allTodos.Add(newTodo);
        ApplyFilter();
        
        // Clear form
        NewTodoTitle = string.Empty;
        NewTodoDeadline = DateTime.Today.AddDays(1);
        NewTodoIsCompleted = false;
    }

    private bool CanUpdateTodo()
    {
        return SelectedTodo != null && !string.IsNullOrWhiteSpace(NewTodoTitle);
    }

    private async void UpdateTodo()
    {
        if (SelectedTodo == null) return;

        SelectedTodo.Title = NewTodoTitle.Trim();
        SelectedTodo.Deadline = NewTodoDeadline;
        SelectedTodo.IsCompleted = NewTodoIsCompleted;
        
        _context.TodoItems.Update(SelectedTodo);
        await _context.SaveChangesAsync();
        
        ApplyFilter();
        
        // Clear selection
        SelectedTodo = null;
        NewTodoTitle = string.Empty;
        NewTodoDeadline = DateTime.Today.AddDays(1);
        NewTodoIsCompleted = false;
    }

    private bool CanDeleteTodo()
    {
        return SelectedTodo != null;
    }

    private async void DeleteTodo()
    {
        if (SelectedTodo == null) return;

        _context.TodoItems.Remove(SelectedTodo);
        await _context.SaveChangesAsync();
        
        _allTodos.Remove(SelectedTodo);
        ApplyFilter();
        
        SelectedTodo = null;
        NewTodoTitle = string.Empty;
        NewTodoDeadline = DateTime.Today.AddDays(1);
    }

    private async void ToggleCompleted(TodoItem? todo)
    {
        if (todo == null) return;

        todo.IsCompleted = !todo.IsCompleted;
        _context.TodoItems.Update(todo);
        await _context.SaveChangesAsync();
        
        ApplyFilter();
    }
    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}