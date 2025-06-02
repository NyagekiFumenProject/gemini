using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using Gemini.Framework.Languages;

namespace Gemini.Framework.Markup
{
    public class TranslateExtensionBase : Binding
    {
        public TranslateExtensionBase(string member, Func<string, CultureInfo, string> callback)
            : base(member)
        {
            Mode = BindingMode.OneWay;
            var language = default(ILanguageManager);
#if DEBUG
            if (View.InDesignMode)
                language = new DefaultLanguageManager();
#endif
            language = language ?? IoC.Get<ILanguageManager>();
            Source = language.GetTranslationSource(callback);
        }
    }
}
