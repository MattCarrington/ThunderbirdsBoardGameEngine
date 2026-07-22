using ThunderbirdsBoardGameEngine.Api.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Validators
{
    public static class CollectionMappingValidator
    {
        public static void ValidateStringCollection(IEnumerable<string>? list, string propertyName)
        {
            if (list is null)
            {
                throw new BadRequestException($"{propertyName} cannot be null.");
            }

            if (list.Any(string.IsNullOrWhiteSpace))
            {
                throw new BadRequestException($"{propertyName} cannot contain null or whitespace values.");
            }
        }
    }
}