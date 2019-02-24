namespace LinqToWikiTest1.Domain
{
    public struct SparqlQuery
    {
        string query;

        public SparqlQuery(string query) => this.query = query;

        public static implicit operator string(SparqlQuery query)
        {
            return query.query;
        }
    }
}
