using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using aXon.Rover.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

using MongoDB.Driver.GridFS;

namespace aXon.Rover
{
    public class MongoDataService : IDataService
    {
        private MongoDatabase _db;
        private MongoServer _svr;

        public MongoDatabase DataBase
        {
            get { return _db ?? (_db = Server.GetDatabase("aXon")); }
        }

        public MongoServer Server
        {
            get { return _svr ?? (_svr = MongoServer.Create("mongodb://192.169.164.138/")); }
        }


        public ObservableCollection<T> GetCollectionQueryModel<T>()
        {
            MongoCollection<BsonDocument> collection = DataBase.GetCollection(typeof (T).Name);
            var r = new ObservableCollection<T>();
            MongoCursor<T> cursor = collection.FindAllAs<T>();
            foreach (T t in cursor)
            {
                r.Add(t);
            }
            return r;
        }

        public ObservableCollection<T> GetCollectionQueryModel<T>(string collectionName)
        {
            MongoCollection<BsonDocument> collection = DataBase.GetCollection(collectionName);
            var r = new ObservableCollection<T>();
            MongoCursor<T> cursor = collection.FindAllAs<T>();
            foreach (T t in cursor)
            {
                r.Add(t);
            }
            return r;
        }

        public ObservableCollection<T> GetCollectionQueryModel<T>(IMongoQuery query)
        {
            MongoCollection<BsonDocument> collection = DataBase.GetCollection(typeof(T).Name);
            var r = new ObservableCollection<T>();
            MongoCursor<T> cursor = collection.FindAs<T>(query);
            foreach (T t in cursor)
            {
                r.Add(t);
            }
            return r;
        }

        //public ObservableCollection<TMapType> GetPartialModelCollection<TMapType, TDbType>(QueryComplete query)
        //{
        //    MongoCollection<BsonDocument> collection = DataBase.GetCollection(typeof (TDbType).Name);
        //    var r = new ObservableCollection<TMapType>();
        //    MongoCursor<TMapType> cursor = collection.FindAs<TMapType>(query);

        //    Type type = typeof (TMapType);
        //    List<string> fields = type.GetProperties().Where(f => f.CanWrite).Select(f => f.Name).ToList();

        //    cursor.SetFields(fields.ToArray());
        //    foreach (TMapType t in cursor)
        //    {
        //        r.Add(t);
        //    }
        //    return r;
        //}

        public ObservableCollection<TMapType> GetPartialModelCollection<TMapType, TDbType>()
        {
            
            MongoCollection<BsonDocument> collection = DataBase.GetCollection(typeof (TDbType).Name);
            var r = new ObservableCollection<TMapType>();
            MongoCursor<TMapType> cursor = collection.FindAllAs<TMapType>();

            Type type = typeof (TMapType);
            List<string> fields = type.GetProperties().Where(f => f.CanWrite).Select(f => f.Name).ToList();

            cursor.SetFields(fields.ToArray());
            foreach (TMapType t in cursor)
            {
                r.Add(t);
            }
            return r;
        }

        //public ObservableCollection<T> GetCollectionQueryModel<T>(string collectionName, QueryComplete query)
        //{
        //    MongoCollection<BsonDocument> collection = DataBase.GetCollection(collectionName);
        //    var r = new ObservableCollection<T>();
        //    MongoCursor<T> cursor = collection.FindAs<T>(query);
        //    foreach (T t in cursor)
        //    {
        //        r.Add(t);
        //    }
        //    return r;
        //}

        public Stream OpenFileAsStream(Guid id, string databaseName = "")
        {
            var gridFs = new MongoGridFS(DataBase);

            MongoGridFSFileInfo find = gridFs.FindOneById(id);
            if (find == null)
                return new MemoryStream();
            return find.Open(FileMode.Open, FileAccess.Read);
        }

        public string GetFileName(Guid id, string databaseName = "")
        {
            var gridFs = new MongoGridFS(DataBase);
            return gridFs.FindOneById(id).Name;
        }

        public Guid SaveFile(Stream incoming, string fileExtension, string databaseName = "")
        {
            Guid id = Guid.NewGuid();

            var gridFs = new MongoGridFS(DataBase);

            MongoGridFSStream remote = gridFs.OpenWrite(string.Format("{0}.{1}", id, fileExtension),
                                                        new MongoGridFSCreateOptions
                                                            {
                                                                UploadDate = DateTime.Now,
                                                                Id = id
                                                            });
            if (incoming.CanSeek)
                incoming.Seek(0, SeekOrigin.Begin);
            incoming.CopyTo(remote);
            remote.Close();
            return id;
        }

        public Guid SaveFile(string path, Guid g)
        {
            var mgf = new MongoGridFS(DataBase);
            MongoGridFSStream s = mgf.OpenWrite(g.ToString(),
                                                new MongoGridFSCreateOptions {UploadDate = DateTime.Now, Id = g});
            FileStream f = File.OpenRead(path);
            var buf = new byte[f.Length];
            f.Read(buf, 0, Convert.ToInt32(f.Length));
            f.Close();
            s.Write(buf, 0, buf.Length);
            s.Close();
            return g;
        }

        public void RemoveFile(Guid g)
        {
            var mgf = new MongoGridFS(DataBase);

            mgf.Delete(g.ToString());
        }

        public void DropCollection()
        {
            MongoCollection<BsonDocument> collection = DataBase.GetCollection("Client");
            collection.Drop();
        }

        public Guid SaveFile(string path)
        {
            Guid g = Guid.NewGuid();
            var mgf = new MongoGridFS(DataBase);
            MongoGridFSStream s = mgf.OpenWrite(g.ToString(),
                                                new MongoGridFSCreateOptions {UploadDate = DateTime.Now, Id = g});
            FileStream f = File.OpenRead(path);
            var buf = new byte[f.Length];
            f.Read(buf, 0, Convert.ToInt32(f.Length));
            f.Close();
            s.Write(buf, 0, buf.Length);
            s.Close();
            return g;
        }

        public byte[] OpenFile(Guid id)
        {
            var mgf = new MongoGridFS(DataBase);
            MongoGridFSStream s = mgf.OpenRead(id.ToString());
            var buf = new byte[s.Length];
            s.Read(buf, 0, buf.Length);
            s.Close();
            return buf;
        }
    }
}