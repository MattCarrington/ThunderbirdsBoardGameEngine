using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using Xunit.Abstractions;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.ClassData
{
    public class CatalogDataAccessExceptionHandlerTestCase : IXunitSerializable
    {
        public CatalogDataAccessErrorCode ErrorCode { get; private set; }

        public int ExpectedStatus { get; private set; }

        public string ExpectedTitle { get; private set; } = string.Empty;

        public string ExpectedType { get; private set; } = string.Empty;

        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public CatalogDataAccessExceptionHandlerTestCase()
        {
        }

        public CatalogDataAccessExceptionHandlerTestCase(CatalogDataAccessErrorCode errorCode, int expectedStatus, string expectedTitle, string expectedType)
        {
            ErrorCode = errorCode;
            ExpectedStatus = expectedStatus;
            ExpectedTitle = expectedTitle;
            ExpectedType = expectedType;
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(ErrorCode), ErrorCode);
            info.AddValue(nameof(ExpectedStatus), ExpectedStatus);
            info.AddValue(nameof(ExpectedTitle), ExpectedTitle);
            info.AddValue(nameof(ExpectedType), ExpectedType);
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            ErrorCode = info.GetValue<CatalogDataAccessErrorCode>(nameof(ErrorCode));
            ExpectedStatus = info.GetValue<int>(nameof(ExpectedStatus));
            ExpectedTitle = info.GetValue<string>(nameof(ExpectedTitle))!;
            ExpectedType = info.GetValue<string>(nameof(ExpectedType))!;
        }
    }
}

