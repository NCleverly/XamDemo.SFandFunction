using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using DatabaseSync.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace DatabaseSync.Repo
{
    public class DocumentDbRepository<T> where T : class
    {
        private string _databaseId;
        private string _collectionId;
        private DocumentClient _client;


        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document =
                    await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsBySqlAsync(string query)
        {
            Uri database = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId);
            IDocumentQuery<T> queryResult = _client.CreateDocumentQuery<T>(database, query, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
              .AsDocumentQuery();

            List<T> results = new List<T>();
            while (queryResult.HasMoreResults)
            {
                results.AddRange(await queryResult.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<Document> CreateItemAsync(T item)
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
        }

        public async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
        }

        public async Task DeleteItemAsync(string id, string category)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
        }

        public async Task<IEnumerable<T>> CallStoredProcedure(string procName, params dynamic[] parameters)
        {
            try
            {
                Uri uri = UriFactory.CreateStoredProcedureUri(_databaseId, _collectionId, procName);
                StoredProcedureResponse<string> response = await _client.ExecuteStoredProcedureAsync<string>(uri, parameters);
                if (response == null || response.Response == "No documents matching query were found.")
                    return new List<T>();
                else
                {
                    List<T> documents = JsonConvert.DeserializeObject<List<T>>(response.Response);
                    return documents;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Initialize(DocDbConfig config)
        {
            _client = new DocumentClient(new Uri(config.Endpoint), config.AuthKey);
            _databaseId = config.DatabaseId;
            _collectionId = config.CollectionId;
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = _databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_databaseId),
                        new DocumentCollection
                        {
                            Id = _collectionId
                        },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
