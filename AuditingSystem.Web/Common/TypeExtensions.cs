namespace AuditingSystem.Web.Common
{
    public static class TypeExtensions
    {
        public static string GetEntityType(this object repository)
        {
            var repositoryType = repository.GetType();
            var genericArguments = repositoryType.GenericTypeArguments;

            if (genericArguments.Length == 1)
            {
                var entityType = genericArguments[0].Name;
                return entityType;
            }
             
            throw new InvalidOperationException($"Unable to determine entity type for repository: {repositoryType.Name}");
        }
    }
}
