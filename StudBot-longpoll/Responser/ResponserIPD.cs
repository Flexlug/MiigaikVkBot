using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TimetableGetter;

using StudBot.Converters;
using StudBot.Utils;

using VkNet;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace StudBot.Responsers
{
    public class ResponserIPD : Responser
    {
        /// <summary>
        /// Ссылка на страницу с расписанием
        /// </summary>
        public readonly string GIDZ_IPD_SHEDULE_URL = "http://studydep.miigaik.ru/index.php?fak=ФПКиФ&kurs=3&grup=ГиДЗипд+III-1б";

        /// <summary>
        /// Расписание группы
        /// </summary>
        public Shedule IpdShedule;

        /// <summary>
        /// Конструктор клавиатуры
        /// </summary>
        public KeyboardBuilder keyboardBuilder = new KeyboardBuilder(false);

        public ResponserIPD(long _groupId, VkApi _vkApi, bool reverseWeek) : base(_groupId, _vkApi)
        {
            IpdShedule = new Shedule(reverseWeek)
            {
                URL = GIDZ_IPD_SHEDULE_URL
            };
            IpdShedule.UpdateTimetable();

            keyboardBuilder.AddButton("Сегодня", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Primary);
            keyboardBuilder.AddButton("Завтра", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Primary);
            keyboardBuilder.AddLine();
            keyboardBuilder.AddButton("Сейчас", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Negative);
            keyboardBuilder.AddLine();
            keyboardBuilder.AddButton("Понедельник", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Default);
            keyboardBuilder.AddButton("Вторник", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Default);
            keyboardBuilder.AddButton("Среда", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Default);
            keyboardBuilder.AddLine();
            keyboardBuilder.AddButton("Четверг", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Default);
            keyboardBuilder.AddButton("Пятница", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Default);
            keyboardBuilder.AddButton("Суббота", "", VkNet.Enums.SafetyEnums.KeyboardButtonColor.Default);
            keyboardBuilder.AddLine();

        }

        public override MessagesSendParams ConstructResponse(Message message)
        {
            string response = "";
            string msg = message.Body.ToLower();

            switch (msg)
            {
                case "сегодня":
                    if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                            response = Converters.SheduleFormat.GetSheduleOn(DateTime.Now, IpdShedule, false);
                    else
                        return new MessagesSendParams()
                        {
                            Message = "ВОСКРЕСЕНЬЕ:\n\nВыходной день. Пар не должно быть.",
                            RandomId = new DateTime().Millisecond,
                            UserId = message.UserId,
                            Keyboard = keyboardBuilder.Build()
                        };
                    break;
                case "завтра":
                    if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
                            response = Converters.SheduleFormat.GetSheduleOn(DateTime.Now.AddDays(1), IpdShedule, false);
                    else
                        return new MessagesSendParams()
                        {
                            Message = "ВОСКРЕСЕНЬЕ:\n\nВыходной день. Пар не должно быть.",
                            RandomId = new DateTime().Millisecond,
                            UserId = message.UserId,
                            Keyboard = keyboardBuilder.Build()
                        };
                    break;
                case "понедельник":
                case "вторник":
                case "среда":
                case "четверг":
                case "пятница":
                case "суббота":
                    DayOfWeek day = Converters.DayOfWeekConverter.FromStrToDOW(message.Body);
                    // на неделю вперёд
                    //response = Converters.SheduleFormat.GetSheduleOn((new DateTime(2019, 9, 2)).AddDays(day == DayOfWeek.Sunday ? 6 : (int)day - 1), IpdShedule, true);
                    response = Converters.SheduleFormat.GetSheduleOn((new DateTime(2019, 9, 2)).AddDays(day == DayOfWeek.Sunday ? 6 : (int)day - 1), IpdShedule, true);
                    //response = Converters.SheduleFormat.GetSheduleOn(DateTime.Now, IpdShedule, true);
                    break;
                case "воскресенье":
                    return new MessagesSendParams()
                    {
                        Message = "ВОСКРЕСЕНЬЕ:\n\nВыходной день. Пар не должно быть.",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                case "помощь":
                    return new MessagesSendParams()
                    {
                        Message = $"{Emoji.RedCircle()} SheduleBOT ALPHA v{VersionInfo.Ver} {Emoji.RedCircle()}\n\nПоследние изменения:\n{VersionInfo.LastUpdates}\nЭтот бот находится на стадии alpha-тестирования и может работать нестабильно. Обо всех найденных ошибках сообщать: http://vk.com/flexlug \n Для того, чтобы увидеть расписание, введите соответствующий день недели.\nТакже можно вывести расписание на сегодняшний или завтрашний день. Вводите \"сегодня\" или \"завтра\" соответственно.",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                case "force_update_00":
                    IpdShedule.UpdateTimetable();
                    return new MessagesSendParams()
                    {
                        Message = "Расписание успешно обновлено!",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                case "есть ли вебинары?":
                    return new MessagesSendParams()
                    {
                        Message = "Режим СДО выключен.",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                case "сейчас":
                    return new MessagesSendParams()
                    {
                        Message = CurrentSubjectGetter.GetSubject(IpdShedule),
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                default:
                    return new MessagesSendParams()
                    {
                        Message = InvMsgReactProv.ProcMsg(msg),
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
            }

            return new MessagesSendParams()
            {
                Message = response,
                RandomId = new DateTime().Millisecond,
                UserId = message.UserId,
                Keyboard = keyboardBuilder.Build()
            };
        }
    }
}