namespace ThunderbirdsBoardGameEngine.Rules.Client.Routing.V1
{
    internal static class ApiRoutes
    {
        public const string RescueTarget = "api/rules/rescue/{disasterCardCode}/target";

        public const string ValidateMovement = "api/rules/movement/{thunderbirdCode}/validate";

        public const string GetAccessibleLocations = "api/rules/movement/{thunderbirdCode}/accessible-locations";
    }
}
