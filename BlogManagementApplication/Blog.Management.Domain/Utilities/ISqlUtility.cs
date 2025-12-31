namespace Blog.Management.Domain.Utilities
{
    public interface ISqlUtility
    {
        public Task<TReturn> ExecuteScalarAsync<TReturn>(string storedProcedureName, IDictionary<string, object> parameters = null);
        public IDictionary<string, object> ExecuteStoredProcedure(string storedProcedureName, IDictionary<string, object> parameters = null, IDictionary<string, Type> outParameters = null);
        public Task<IDictionary<string, object>> ExecuteStoredProcedureAsync(string storedProcedureName, IDictionary<string, object> parameters = null, IDictionary<string, Type> outParameters = null);
        public Task<(IList<TReturn> result, IDictionary<string, object> outValues)> QueryWithStoredProcedureAsync<TReturn>(string storedProcedureName, IDictionary<string, object> parameters = null, IDictionary<string, Type> outParameters = null) where TReturn : class, new();
    }
}
