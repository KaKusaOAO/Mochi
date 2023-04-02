// See https://aka.ms/new-console-template for more information

using Mochi.Localizations;
using Mochi.Utils;

var i18n = new I18n();
var i18n2 = new I18n("branch");
var i18n3 = new I18n("mutate", i18n2);

Logger.Info(i18n.Format("message.first.title"));
Logger.Info(i18n.Format("message.first.description"));
Logger.Info(i18n.Format("message.second.title"));
Logger.Info(i18n.Format("message.second.description"));
Logger.Verbose("----");
Logger.Info(i18n2.Format("message.first.title"));
Logger.Info(i18n2.Format("message.first.description"));
Logger.Info(i18n2.Format("message.second.title"));
Logger.Info(i18n2.Format("message.second.description"));
Logger.Verbose("----");
Logger.Info(i18n3.Format("message.first.title"));
Logger.Info(i18n3.Format("message.first.description"));
Logger.Info(i18n3.Format("message.second.title"));
Logger.Info(i18n3.Format("message.second.description"));