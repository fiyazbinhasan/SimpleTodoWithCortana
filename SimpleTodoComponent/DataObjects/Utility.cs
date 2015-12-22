using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace SimpleTodoComponent.DataObjects
{
    public static class Utility
    {
        private const string Filename = "data.json";

        public static IAsyncOperation<IEnumerable<Todo>> ReadTodosFromLocalFolderOperationAsync()
        {
            return ReadTodosFromLocalFolderAsync().AsAsyncOperation();
        }

        internal static async Task<IEnumerable<Todo>> ReadTodosFromLocalFolderAsync()
        {
            IEnumerable<Todo> todos;

            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Todo>));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(Filename))
            {
                todos = (IEnumerable<Todo>)serializer.ReadObject(stream);
            }

            return todos;
        }

        public static IAsyncAction WriteTodosToLocalFolderActionAsync(IEnumerable<Todo> todos)
        {
            return WriteTodosToLocalFolderAsync(todos).AsAsyncAction();
        }

        internal static async Task WriteTodosToLocalFolderAsync(IEnumerable<Todo> todos)
        {
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Todo>));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(Filename,
                CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, todos);
            }
        }
    }
}
