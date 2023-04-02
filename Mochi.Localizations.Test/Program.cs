// See https://aka.ms/new-console-template for more information

using Mochi.Localizations;
using Mochi.Utils;

var i18N = new I18n();
var i18N2 = new I18n("branch");
var i18N3 = new I18n("mutate", i18N2);

Logger.Info(i18N.Of("message.first.title"));
Logger.Info(i18N.Of("message.first.description"));
Logger.Info(i18N.Of("message.second.title"));
Logger.Info(i18N.Of("message.second.description"));
Logger.Verbose("----");
Logger.Info(i18N2.Of("message.first.title"));
Logger.Info(i18N2.Of("message.first.description"));
Logger.Info(i18N2.Of("message.second.title"));
Logger.Info(i18N2.Of("message.second.description"));
Logger.Verbose("----");
Logger.Info(i18N3.Of("message.first.title"));
Logger.Info(i18N3.Of("message.first.description"));
Logger.Info(i18N3.Of("message.second.title"));
Logger.Info(i18N3.Of("message.second.description"));