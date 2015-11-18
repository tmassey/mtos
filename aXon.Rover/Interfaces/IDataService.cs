using System;
using System.Collections.ObjectModel;
using System.IO;
using MongoDB.Driver;

namespace aXon.Rover.Interfaces
{
    public interface IDataService
    {
        MongoDatabase DataBase { get; }
        MongoServer Server { get; }
        ObservableCollection<T> GetCollectionQueryModel<T>();
        ObservableCollection<T> GetCollectionQueryModel<T>(string collectionName);
        ObservableCollection<T> GetCollectionQueryModel<T>(IMongoQuery query);
        ObservableCollection<TMapType> GetPartialModelCollection<TMapType, TDbType>();
        Stream OpenFileAsStream(Guid id, string databaseName = "");
        string GetFileName(Guid id, string databaseName = "");
        Guid SaveFile(Stream incoming, string fileExtension, string databaseName = "");
        Guid SaveFile(string path, Guid g);
        void RemoveFile(Guid g);
        void DropCollection();
        Guid SaveFile(string path);
        byte[] OpenFile(Guid id);
    }
}