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
using System.IO;
using System.Collections.Concurrent;

namespace StudBot.Responsers
{
    public class ResponserIPD : Responser
    {
        /// <summary>
        /// Ссылка на страницу с расписанием
        /// </summary>
        public readonly string GIDZ_IPD_SHEDULE_URL = "http://studydep.miigaik.ru/index.php?fak=ГФ&kurs=4&grup=ГиДЗипд+IV-1б";

        /// <summary>
        /// Расписание группы
        /// </summary>
        public Shedule IpdShedule;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        private DateTime LastUpdateTime;

        /// <summary>
        /// Конструктор клавиатуры
        /// </summary>
        public KeyboardBuilder keyboardBuilder = new KeyboardBuilder(false);

        /// <summary>
        /// Словарь с ссылками на пары
        /// </summary>
        public ConcurrentDictionary<string, string> URLs = new ConcurrentDictionary<string, string>();

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

            if (File.Exists("urls.json"))
            {
                try
                {
                    URLs = Newtonsoft.Json.JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(File.ReadAllText("urls.json")) ?? new();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Не удалось считать файл с ссылками");
                }
            }
            else
                File.Create("urls.json");
        }

        public override MessagesSendParams ConstructResponse(Message message)
        {
            if (DateTime.Now - LastUpdateTime > TimeSpan.FromHours(1))
                IpdShedule.UpdateTimetable();

            string response = "";
            string msg = message.Body.ToLower();

            if (msg.StartsWith("ссылка"))
            {
                msg = message.Body;
                var strings = msg.Split('\n').Select(x => x).Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (strings.Count == 3)
                {
                    string newValue = strings[2].Trim();

                    URLs.AddOrUpdate(strings[1].ToLower(), strings[2], (key, value) => strings[2]);

                    File.WriteAllText("urls.json", Newtonsoft.Json.JsonConvert.SerializeObject(URLs));
                    return new MessagesSendParams()
                    {
                        Message = "Успешно",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                }
                else
                {
                    return new MessagesSendParams()
                    {
                        Message = "Неверный формат",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                }
            }

            if (msg.StartsWith("удалить ссылку"))
            {
                var strings = msg.Split('\n').Select(x => x).Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (strings.Count == 2)
                {
                    string value = string.Empty;
                    if (URLs.TryGetValue(strings[1], out value))
                        URLs.Remove(strings[1], out _);

                    return new MessagesSendParams()
                    {
                        Message = "Успешно",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                }
                else
                {
                    return new MessagesSendParams()
                    {
                        Message = "Неверный формат",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                }
            }

            switch (msg)
            {
                case "сегодня":
                    if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                        response = Converters.SheduleFormat.GetSheduleOn(DateTime.Now, IpdShedule, false, URLs);
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
                            response = Converters.SheduleFormat.GetSheduleOn(DateTime.Now.AddDays(1), IpdShedule, false, URLs);
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
                    response = Converters.SheduleFormat.GetSheduleOn((new DateTime(2019, 9, 2)).AddDays(day == DayOfWeek.Sunday ? 6 : (int)day - 1), IpdShedule, true, URLs);
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
                case "начать":
                case "помощь":
                    return new MessagesSendParams()
                    {
                        Message = $"{Emoji.RedCircle()} SheduleBOT ALPHA v{VersionInfo.Ver} {Emoji.RedCircle()}\n\nПоследние изменения:\n{VersionInfo.LastUpdates}\nЭтот бот находится на стадии alpha-тестирования и может работать нестабильно. Обо всех найденных ошибках сообщать: http://vk.com/flexlug \n Для того, чтобы увидеть расписание, введите соответствующий день недели.\nТакже можно вывести расписание на сегодняшний или завтрашний день. Вводите \"сегодня\" или \"завтра\" соответственно.",
                        RandomId = new DateTime().Millisecond,
                        UserId = message.UserId,
                        Keyboard = keyboardBuilder.Build()
                    };
                case "обновить расписание":
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
                        Message = CurrentSubjectGetter.GetSubject(IpdShedule, URLs),
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