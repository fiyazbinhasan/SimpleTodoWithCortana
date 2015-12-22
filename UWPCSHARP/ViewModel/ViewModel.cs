using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using UWPCSHARP.Model;

namespace UWPCSHARP.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Todo> Todos { get; set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private Todo _selectedTodo;
        public Todo SelectedTodo
        {
            get { return _selectedTodo; }
            set
            {
                if (_selectedTodo == value)
                    return;
                _selectedTodo = value;
                OnPropertyChanged("SelectedTodo");
            }
        }

        public ViewModel()
        {
            Todos = new ObservableCollection<Todo>();
            Todos.CollectionChanged += Todos_CollectionChanged;
        }

        private void Todos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Todo item in e.OldItems)
                {
                    item.PropertyChanged -= TodoModelPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Todo item in e.NewItems)
                {
                    item.PropertyChanged += TodoModelPropertyChanged;
                }
            }
        }

        private async void TodoModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Utility.Utility.WriteTodosToLocalFolderAsync(Todos);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
