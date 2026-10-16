using Bunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures
{
    public class DialogJsInterop
    {
        public static void Register(BunitContext context)
        {
            var module = context.JSInterop.SetupModule("./js/dialog.js");

            module.SetupVoid("show", _ => true)
                .SetVoidResult();

            module.SetupVoid("close", _ => true)
                .SetVoidResult();
        }
    }
}
