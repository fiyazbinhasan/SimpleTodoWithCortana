using System.ComponentModel;

namespace UWPCSHARP.Model
{
    public class Todo : INotifyPropertyChanged
    {
        private string _title;
        private bool _isDone;

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

        public bool IsDone
        {
            get { return _isDone; }
            set
            {
                if (_isDone == value)
                    return;
                _isDone = value;
                OnPropertyChanged("IsDone");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
